using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Godot;
using Rzeka;

namespace LittleRiver;

public partial class GameLevel : Node3D
{
	static IRzeka rzeka => LittleSource.Rzeka;
	CollectibleDisposable Q { get; set; }

	readonly Subject<int> _enterPressed = new();
	bool _gameWon;

	public override void _EnterTree()
	{
		Q = new();

		Input.MouseMode = Input.MouseModeEnum.Captured;

		RegisterSpells();

		rzeka.Pluck(this, new SceneEnteredTree(SceneEnteredTree.SceneEnum.Game));
	}

	public override void _Ready() { }

	public override void _Process(double delta) { }

	public override void _UnhandledKeyInput(InputEvent @event)
	{
		if (!_gameWon)
			return;

		if (@event is InputEventKey keyEvent)
		{
			if (keyEvent.Pressed && keyEvent.Keycode == Key.Enter)
			{
				_enterPressed.OnNext(0);
			}
		}
	}

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
					.Where(e => e.Scene == SceneEnteredTree.SceneEnum.Game)
					.Select(_ => new WorldEnvironmentRequested(
						WorldEnvironmentFairy.EnvironmentEnum.Game
					))
		);

		Q += rzeka.Weave<GameWon>(this, spell => spell.Subscribe(_ => _gameWon = true));

		Q += rzeka.Loom<GameWon, MainMenuRequested>(
			this,
			spell => spell.SelectMany(_enterPressed).Take(1).Select(_ => new MainMenuRequested())
		);

		Q += rzeka.Weave<MainMenuReadyToLoad>(
			this,
			spell =>
				spell
					.Take(1)
					.Subscribe(_ =>
					{
						Visible = false;
						QueueFree();
					})
		);
	}
}
