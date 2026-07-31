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
    PackedScene _mainMenuScene;

    [Export]
    Node3D _gameSceneParent;

    static IRzeka rzeka => LittleSource.Rzeka;
    CollectibleDisposable Q { get; set; }

    bool _isGameOn = false;
    readonly GameTimer _gameTimer = new();

    const int HOW_MANY_STARS = 10;

    public override void _EnterTree()
    {
        Q = new();

        RegisterGameStartSpells();
        RegisterGameLoopSpells();
        RegisterReturnToMenuSpells();
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

    void RegisterReturnToMenuSpells()
    {
        Q += rzeka.Loom<MainMenuRequested, MainMenuReadyToLoad>(
            this,
            spell => spell.Where(req => req.SkipFadeOut).Select(_ => new MainMenuReadyToLoad())
        );

        Q += rzeka.Loom<MainMenuRequested, MainMenuReadyToLoad>(
            this,
            spell =>
                spell
                    .Where(req => !req.SkipFadeOut)
                    .SelectMany(startGame =>
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
                                new MainMenuReadyToLoad().WithCircumstances(startGame, fadeRes)
                            )
                    )
        );

        Q += rzeka.Loom<MainMenuReadyToLoad, MainMenuLoaded>(
            this,
            spell =>
                spell.SelectMany(menuLoadReady =>
                    rzeka
                        .Ask<LoadSceneRequest, LoadSceneResponse>(
                            this,
                            new LoadSceneRequest(_mainMenuScene.ResourcePath).WithCircumstances(
                                menuLoadReady
                            )
                        )
                        .Where(res => res.WasSuccessful)
                        .SelectMany(async r =>
                        {
                            var scene =
                                r.PackedScene.Instantiate()
                                ?? throw new InvalidOperationException(
                                    $"Instantiate returned null for {_mainMenuScene.ResourcePath}"
                                );

                            _gameSceneParent.CallDeferred(Node.MethodName.AddChild, scene);
                            await scene.ToSignal(scene, Node.SignalName.Ready);
                            return new MainMenuLoaded().WithCircumstances(menuLoadReady, r);
                        })
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
                    .Where(scoreState => scoreState.Score == HOW_MANY_STARS)
                    .Select(_ => new GameWon(_gameTimer.Elapsed))
        );
    }
}
