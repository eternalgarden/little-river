using System.Reactive.Linq;
using Godot;
using Rzeka;

namespace LittleRiver;

public partial class Main : Node
{
    [Export]
    Node3D _mainMenuParent;

    [Export]
    PackedScene _mainMenuScene;

    IRzeka rzeka => LittleSource.Rzeka;
    CollectibleDisposable Q { get; set; }

    public override void _EnterTree()
    {
        Q = new();

        RegisterStartupSpells();
    }

    public override void _Ready()
    {
        rzeka.Pluck(this, new GameOpened());
    }

    public override void _ExitTree()
    {
        Q.Dispose();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent)
        {
            if (keyEvent.Pressed && keyEvent.Keycode == Key.Escape)
            {
                GetTree().Quit(0);
            }
        }
    }

    void RegisterStartupSpells()
    {
        Q += rzeka.Loom<GameOpened, MainMenuRequested>(
            this,
            spell => spell.Take(1).Select(_ => new MainMenuRequested())
        );

        Q += rzeka.Loom<MainMenuRequested, MainMenuLoaded>(
            this,
            spell =>
                spell.SelectMany(gameStarted =>
                    rzeka
                        .Ask<LoadSceneRequest, LoadSceneResponse>(
                            this,
                            new LoadSceneRequest(_mainMenuScene.ResourcePath).WithCircumstances(
                                gameStarted
                            )
                        )
                        .Take(1)
                        .Where(r => r.WasSuccessful)
                        .Reacting(r =>
                        {
                            var activeScene = r.PackedScene.Instantiate();
                            _mainMenuParent.CallDeferred(Node.MethodName.AddChild, activeScene);
                        })
                        .Select(r => new MainMenuLoaded().WithCircumstances(gameStarted, r))
                )
        );

        Q += rzeka.Loom<MainMenuLoaded, ScreenFadeRequest>(
            this,
            spell =>
                spell.Select(_ => new ScreenFadeRequest(
                    ScreenFadeRequest.ScreenFadeEnum.FadeIn,
                    1f
                ))
        );

        Q += rzeka.Loom<MainMenuLoaded, GameReady>(
            this,
            spell => spell.Take(1).Select(_ => new GameReady())
        );
    }
}
