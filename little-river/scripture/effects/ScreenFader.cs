using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Godot;
using Rzeka;

namespace LittleRiver;

public partial class ScreenFader : Node
{
    CollectibleDisposable Q { get; set; }
    IRzeka rzeka => LittleSource.Rzeka;

    IObservable<Unit> FadeOut(float duration) => Fade(1f, duration);

    IObservable<Unit> FadeIn(float duration) => Fade(0f, duration);

    ColorRect _overlay;
    CanvasLayer _canvasLayer;

    public override void _EnterTree()
    {
        Q = new();

        RegisterSpells();
    }

    public override void _Ready()
    {
        _canvasLayer = new CanvasLayer { Layer = 100 };
        AddChild(_canvasLayer);

        // Don't forget this mouse fileter setting, by default it will block all mouse input derp
        _overlay = new ColorRect
        {
            Color = Colors.Black,
            MouseFilter = Control.MouseFilterEnum.Ignore,
        };
        _overlay.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        _canvasLayer.AddChild(_overlay);

        _overlay.Modulate = Colors.Black;
    }

    void RegisterSpells()
    {
        Q += rzeka.Shuttle<ScreenFadeRequest, ScreenFadeResponse>(
            this,
            spell =>
                spell.SelectMany(req =>
                    (
                        req.ScreenFade == ScreenFadeRequest.ScreenFadeEnum.FadeIn
                            ? FadeIn(req.ScreenFadeLength)
                            : FadeOut(req.ScreenFadeLength)
                    ).Select(_ => new ScreenFadeResponse(req, true))
                )
        );
    }

    IObservable<Unit> Fade(float targetAlpha, float duration) =>
        Observable.Create<Unit>(observer =>
        {
            var tween = CreateTween();
            tween.TweenProperty(_overlay, "modulate:a", targetAlpha, duration);
            tween.TweenCallback(
                Callable.From(() =>
                {
                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();
                })
            );
            return Disposable.Create(() => tween.Kill());
        });
}
