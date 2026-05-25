using System;
using System.Reactive;
using System.Reactive.Linq;
using Godot;

namespace LittleRiver;

public static class GodotRxExtensions
{
    public static IObservable<Unit> OnPressed(this BaseButton button) =>
        Observable.FromEvent(h => button.Pressed += h, h => button.Pressed -= h);

    public static IObservable<Node3D> OnBodyEntered(this Area3D area) =>
        Observable.FromEvent<Area3D.BodyEnteredEventHandler, Node3D>(
            h => new Area3D.BodyEnteredEventHandler(h),
            h => area.BodyEntered += h,
            h => area.BodyEntered -= h
        );

    public static IObservable<Node3D> OnBodyExited(this Area3D area) =>
        Observable.FromEvent<Area3D.BodyExitedEventHandler, Node3D>(
            h => new Area3D.BodyExitedEventHandler(h),
            h => area.BodyExited += h,
            h => area.BodyExited -= h
        );

    public static IObservable<Unit> OnFinished(this AudioStreamPlayer player) =>
        Observable.FromEvent(h => player.Finished += h, h => player.Finished -= h);

    public static IObservable<Unit> OnFinished(this AudioStreamPlayer3D player) =>
        Observable.FromEvent(h => player.Finished += h, h => player.Finished -= h);
}
