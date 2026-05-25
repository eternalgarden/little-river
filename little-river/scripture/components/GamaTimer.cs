using System;
using System.Reactive.Subjects;

namespace LittleRiver;

public class GameTimer
{
    readonly Subject<double> _tick = new();
    public IObservable<double> Tick => _tick;
    public double Elapsed { get; private set; }

    public void Advance(double delta)
    {
        Elapsed += delta;
        _tick.OnNext(Elapsed);
    }

    public void Reset()
    {
        Elapsed = 0;
    }
}
