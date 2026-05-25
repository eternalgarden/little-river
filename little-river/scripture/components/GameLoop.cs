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
    static IRzeka rzeka => LittleSource.Rzeka;

    bool _isGameOn = false;
    readonly GameTimer _gameTimer = new();

    public override void _EnterTree()
    {
        Q = new();
        RegisterGameStartSpells();
        RegisterGameLoopSpells();
    }

    public override void _Ready()
    {
        rzeka.Pluck(this, new GameTimerState(_gameTimer));
    }

    public override void _Process(double delta)
    {
        if (_isGameOn)
            _gameTimer.Tick(delta);
    }

    public override void _ExitTree()
    {
        Q.Dispose();
    }

    void RegisterGameStartSpells()
    {
        Q += rzeka.Loom<StartGameRequested, GameReadyToLoad>(
            this,
            spell =>
                spell.SelectMany(startGame =>
                    rzeka
                        .Ask<ScreenFadeRequest, ScreenFadeResponse>(
                            this,
                            new ScreenFadeRequest(
                                ScreenFadeRequest.ScreenFadeEnum.FadeOut,
                                0.5f
                            ).WithCircumstances(startGame)
                        )
                        .Where(fadeRes => fadeRes.WasSuccessful)
                        .Select(fadeRes =>
                            new GameReadyToLoad().WithCircumstances(startGame, fadeRes)
                        )
                )
        );

        // Q += rzeka.Loom<GameReadyToLoad, GameTimerState>(
        //     this,
        //     spell => spell.Take(1).Select(_ => new GameTimerState(_gameTimer))
        // );

        Q += rzeka.Loom<GameReadyToLoad, GameLoaded>(
            this,
            spell =>
                spell.SelectMany(gameReady =>
                    rzeka
                        .Ask<LoadSceneRequest, LoadSceneResponse>(
                            this,
                            new LoadSceneRequest(_gameScene.ResourcePath).WithCircumstances(
                                gameReady
                            )
                        )
                        .Where(res => res.WasSuccessful)
                        .SelectMany(async r =>
                        {
                            var scene =
                                r.PackedScene.Instantiate()
                                ?? throw new InvalidOperationException(
                                    $"Instantiate returned null for {_gameScene.ResourcePath}"
                                );

                            _gameSceneParent.CallDeferred(Node.MethodName.AddChild, scene);
                            await scene.ToSignal(scene, Node.SignalName.Ready);
                            return new GameLoaded().WithCircumstances(gameReady, r);
                        })
                        .ObserveOn(rzeka.MainThread)
                )
        );

        Q += rzeka.Loom<GameLoaded, GameStarted>(
            this,
            spell =>
                spell.SelectMany(e =>
                    rzeka
                        .Ask<ScreenFadeRequest, ScreenFadeResponse>(
                            this,
                            new ScreenFadeRequest(
                                ScreenFadeRequest.ScreenFadeEnum.FadeIn,
                                0.5f
                            ).WithCircumstances(e)
                        )
                        .Where(r => r.WasSuccessful)
                        .Select(_ => new GameStarted())
                )
        );
    }

    void RegisterGameLoopSpells()
    {
        Q += rzeka.Weave<GameStarted>(
            this,
            spell =>
                spell.Subscribe(_ =>
                {
                    _gameTimer.Reset();
                    _isGameOn = true;
                })
        );

        Q += rzeka.Loom<PlayerScoreState, PlayerScoreState>(
            this,
            state =>
                Observable.Merge(
                    rzeka
                        .Scry<StarCollected>()
                        .WithLatestFrom(
                            state,
                            (col, s) => new PlayerScoreState(s.Score + 1).WithCircumstances(col)
                        ),
                    rzeka
                        .Scry<GameReadyToLoad>()
                        .Select(e => new PlayerScoreState(0).WithCircumstances(e))
                )
        );

        Q += rzeka.Weave<GameWon>(this, spell => spell.Subscribe(_ => _isGameOn = false));

        Q += rzeka.Loom<PlayerScoreState, GameWon>(
            this,
            spell =>
                spell
                    .Where(scoreState => scoreState.Score == 1)
                    .Select(_ => new GameWon(_gameTimer.Elapsed))
        );
    }
}
