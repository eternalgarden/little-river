using System.Reactive.Linq;
using Godot;
using Rzeka;

namespace LittleRiver;

public partial class Main : Node
{
	[Export]
	Node3D _activeSceneParent;

	[Export]
	PackedScene _mainMenuScene;

	[Export]
	PackedScene _levelOneScene;

	CollectibleDisposable Q { get; set; }
	IRzeka rzeka => LittleSource.Rzeka;

	Node _activeScene;

	public override void _EnterTree()
	{
		Q = new();

		Initialise();
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

	void Initialise()
	{
		Q += rzeka.Loom<GameOpened, MainMenuLoaded>(
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
							_activeScene = r.PackedScene.Instantiate();
							_activeSceneParent.CallDeferred(Node.MethodName.AddChild, _activeScene);
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
