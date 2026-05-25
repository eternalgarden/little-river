using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Godot;
using Rzeka;

namespace LittleRiver;

// For such simple scenes the async SceneLoader.cs is an overkill
// But it serves as an example for how would you deal with async load situation
// Which you will definitely do for more complex scenes
public partial class SceneLoader : Node
{
    static IRzeka rzeka => LittleSource.Rzeka;
    CollectibleDisposable Q { get; set; }

    public override void _EnterTree()
    {
        Q = new();

        Q += rzeka.Shuttle<LoadSceneRequest, LoadSceneResponse>(
            this,
            reqs =>
                reqs.SelectMany(req =>
                    LoadSceneThreaded(req.ScenePath)
                        .Select(scene => new LoadSceneResponse(req, scene, true))
                        .Catch<LoadSceneResponse, Exception>(ex =>
                        {
                            rzeka.Whisper(ex);
                            return Observable.Return(new LoadSceneResponse(req, null, false));
                        })
                )
        );
    }

    public override void _ExitTree()
    {
        Q.Dispose();
    }

    static IObservable<PackedScene> LoadSceneThreaded(string scenePath)
    {
        return Observable.Create<PackedScene>(observer =>
        {
            GD.Print(scenePath);
            Error error = ResourceLoader.LoadThreadedRequest(scenePath);
            if (error != Error.Ok)
            {
                observer.OnError(
                    new Exception(
                        $"Scene Load request for scene path: {scenePath} failed due to err: {error}"
                    )
                );
                return Disposable.Empty;
            }

            return IntervalResourceLoadObservable(scenePath, observer);
        });
    }

    static IDisposable IntervalResourceLoadObservable(
        string scenePath,
        IObserver<PackedScene> observer
    )
    {
        return Observable
            // 🐖✨ LoadSceneResponse will be on the main thread thanks to this so no need for .ObserveOn later, neat
            .Interval(TimeSpan.FromMilliseconds(33), rzeka.MainThread)
            .Subscribe(_ =>
            {
                switch (ResourceLoader.LoadThreadedGetStatus(scenePath))
                {
                    case ResourceLoader.ThreadLoadStatus.Loaded:
                        observer.OnNext((PackedScene)ResourceLoader.LoadThreadedGet(scenePath));
                        observer.OnCompleted();
                        break;
                    case ResourceLoader.ThreadLoadStatus.Failed:
                    case ResourceLoader.ThreadLoadStatus.InvalidResource:
                        observer.OnError(new Exception($"Load failed at path: {scenePath}"));
                        break;
                }
            });
    }
}
