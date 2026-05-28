using System;
using System.Reactive.Linq;
using Godot;
using Rzeka;

namespace LittleRiver;

public partial class MainMenu : Node3D
{
    [Export]
    Button _startGameButton;

    [Export]
    Control _welcomingScreenControl;

    static IRzeka rzeka => LittleSource.Rzeka;
    CollectibleDisposable Q { get; set; }

    public override void _EnterTree()
    {
        Q = new();

        Input.MouseMode = Input.MouseModeEnum.Confined;

        RegisterSpells();

        rzeka.Whisper("Oiiii, is it the main menu? :o", RzekaMessageType.Hunch);

        rzeka.Pluck(this, new SceneEnteredTree(SceneEnteredTree.SceneEnum.MainMenu));
    }

    public override void _Ready() { }

    public override void _ExitTree()
    {
        Q.Dispose();
    }

    void RegisterSpells()
    {
        Q += rzeka.Loom<SceneEnteredTree, WorldEnvironmentRequested>(
            this,
            spell =>
                spell
                    .Where(e => e.Scene == SceneEnteredTree.SceneEnum.MainMenu)
                    .Select(_ => new WorldEnvironmentRequested(
                        WorldEnvironmentFairy.EnvironmentEnum.MainMenu
                    ))
        );

        Q += rzeka.Strand(
            this,
            _startGameButton.OnPressed().Select(_ => new StartGameButtonPressed())
        );

        Q += rzeka.Loom<StartGameButtonPressed, StartGameRequested>(
            this,
            presses => presses.Take(1).Select(_ => new StartGameRequested())
        );

        Q += rzeka.Loom<StartGameButtonPressed, UIButtonPressed>(
            this,
            presses => presses.Select(_ => new UIButtonPressed())
        );

        Q += rzeka.Weave<GameReadyToLoad>(
            this,
            spell =>
                spell
                    .Take(1)
                    .Subscribe(_ =>
                    {
                        _welcomingScreenControl.Visible = false;
                        Visible = false;
                        QueueFree();
                    })
        );
    }
}
