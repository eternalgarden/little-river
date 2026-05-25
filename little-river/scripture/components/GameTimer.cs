using System;
using System.Reactive.Subjects;

namespace LittleRiver;

public class GameTimer
{
    readonly Subject<double> _time = new();
    public IObservable<double> Time => _time;
    public double Elapsed { get; private set; }

    public void Tick(double delta)
    {
        Elapsed += delta;
        _time.OnNext(Elapsed);
    }

    public void Reset()
    {
        Elapsed = 0;
    }
}
