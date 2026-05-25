using System.Reactive.Linq;
using Godot;
using Rzeka;

namespace LittleRiver;

public partial class Main : Node
{
    static IRzeka rzeka => LittleSource.Rzeka;
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
            spell => spell.Take(1).Select(_ => new MainMenuRequested(skipFadeOut: true))
        );

        Q += rzeka.Loom<MainMenuLoaded, GameReady>(
            this,
            spell => spell.Take(1).Select(_ => new GameReady())
        );
    }
}
