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

	IRzeka rzeka => LittleSource.Rzeka;
	CollectibleDisposable Q { get; set; }

	public override void _EnterTree()
	{
		Q = new();

		Input.MouseMode = Input.MouseModeEnum.Confined;

		RegisterSpells();

		rzeka.Pluck(this, new SceneEnteredTree("main_menu"));
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
					.Where(e => e.SceneName == "main_menu")
					.Take(1)
					.Select(_ => new WorldEnvironmentRequested(
						WorldEnvironmentFairy.EnvironmentEnum.MainMenu
					))
		);

		Q += rzeka.Strand(
			this,
			_startGameButton.OnPressed().Take(1).Select(_ => new StartGameRequested())
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
