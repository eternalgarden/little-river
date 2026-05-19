using Godot;
using System;
using System.Reactive;
using System.Reactive.Linq;

namespace LittleRiver;

public static class GodotRxExtensions
{
    public static IObservable<Unit> OnPressed(this BaseButton button) =>
        Observable.FromEvent(
            h => button.Pressed += h,
            h => button.Pressed -= h);
}
