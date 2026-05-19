using Godot;
using System;
using System.Reactive.Linq;
using Rzeka;

namespace LittleRiver;
public partial class WelcomingScreen : Node
{
	[Export] Button _startGameButton;
	[Export] Control _welcomingScreenControl;

	static IRzeka rzeka => LittleSource.Rzeka;

	CollectibleDisposable Q { get; set; }

	public override void _EnterTree()
	{
		Q = new();

		_startGameButton.Disabled = true;

		Q += rzeka.Weave<GameReady>(
			this,
			spell => spell
				.Take(1)
				.Subscribe(_ => _startGameButton.Disabled = false));

		Q += rzeka.Strand(
			this,
			_startGameButton.OnPressed()
				.Take(1)
				.Select(_ => new StartGameRequested()));

		Q += rzeka.Weave<StartGameRequested>(
			this,
			spell => spell
				.Take(1)
				.Subscribe(_ => {
					_welcomingScreenControl.Visible = false;
					QueueFree();
				}));
		
		// Q += rzeka.Loom<GameReady, >(
		//     this,
		//     spell => spell
		//     _startGameButton.Disa
	}

	public override void _ExitTree()
	{
		Q.Dispose();
	}
}
