using System;
using System.Reactive.Linq;
using Godot;
using Rzeka;

namespace LittleRiver;

public partial class MainMenu : Node
{
    [Export]
    Button _startGameButton;

    [Export]
    Control _welcomingScreenControl;

    IRzeka rzeka => LittleSource.Rzeka;

    CollectibleDisposable Q { get; set; }

    public override void _EnterTree()
    {
        Q = new();

        // _startGameButton.Disabled = true;
        Input.MouseMode = Input.MouseModeEnum.Confined;

        RegisterSpells();

        rzeka.Pluck(
            this,
            new WorldEnvironmentRequested(WorldEnvironmentFairy.EnvironmentEnum.MainMenu)
        );
    }

    public override void _Ready() { }

    public override void _ExitTree()
    {
        Q.Dispose();
    }

    void RegisterSpells()
    {
        Q += rzeka.Strand(
            this,
            _startGameButton.OnPressed()
                .Do(_ => GD.Print("meowl"))
                .Take(1)
                .Select(_ => new StartGameRequested())
        );

        Q += rzeka.Weave<StartGameRequested>(
            this,
            spell =>
                spell
                    .Take(1)
                    .Subscribe(_ =>
                    {
                        _welcomingScreenControl.Visible = false;
                        QueueFree();
                    })
        );
    }
}
