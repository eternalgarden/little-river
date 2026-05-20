using System;
using System.Reactive.Linq;
using Godot;
using Rzeka;

namespace LittleRiver;

public partial class GameLoop : Node3D
{
    [Export]
    PackedScene _gameScene;

    [Export]
    Node3D _gameSceneParent;

    CollectibleDisposable Q { get; set; }
    IRzeka rzeka => LittleSource.Rzeka;

    public override void _EnterTree()
    {
        Q = new();
        RegisterGameLoopSpells();
    }

    public override void _Ready() { }

    public override void _Process(double delta) { }

    public override void _ExitTree()
    {
        Q.Dispose();
    }

    void RegisterGameLoopSpells()
    {
        Q += rzeka.Loom<StartGameRequested, GameReadyToLoad>(
            this,
            spell =>
                spell.SelectMany(req =>
                    rzeka
                        .Ask<ScreenFadeRequest, ScreenFadeResponse>(
                            this,
                            new ScreenFadeRequest(
                                ScreenFadeRequest.ScreenFadeEnum.FadeOut,
                                0.5f
                            ).WithCircumstances(req)
                        )
                        .Where(res => res.WasSuccessful)
                        .Select(_ => new GameReadyToLoad())
                )
        );

        Q += rzeka.Loom<GameReadyToLoad, GameStarted>(
            this,
            spell =>
                spell.SelectMany(req =>
                    rzeka
                        .Ask<LoadSceneRequest, LoadSceneResponse>(
                            this,
                            new LoadSceneRequest(_gameScene.ResourcePath).WithCircumstances(req)
                        )
                        .Where(res => res.WasSuccessful)
                        .Reacting(r =>
                        {
                            var scene = r.PackedScene.Instantiate();
                            if (_gameSceneParent is null)
                            {
                                rzeka.Whisper("_gameSceneParent is null", RzekaMessageType.Horror);
                                return;
                            }
                            if (scene is null)
                            {
                                rzeka.Whisper("scene is null", RzekaMessageType.Horror);
                                return;
                            }
                            _gameSceneParent.CallDeferred(Node.MethodName.AddChild, scene);
                        })
                        .Select(res => new GameStarted().WithCircumstances(req, res))
                )
        );

		Q += rzeka.Loom<GameReadyToLoad, WorldEnvironmentRequested>(
			this,
			spell =>
				spell
					.Take(1)
					.Select(_ => new WorldEnvironmentRequested(
						WorldEnvironmentFairy.EnvironmentEnum.Game
					))
		);

        Q += rzeka.Loom<GameStarted, ScreenFadeRequest>(
            this,
            spell =>
                spell.Select(_ => new ScreenFadeRequest(
                    ScreenFadeRequest.ScreenFadeEnum.FadeIn,
                    0.5f
                ))
        );
    }
}
