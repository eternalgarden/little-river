using System;
using System.Reactive.Linq;
using Godot;
using Rzeka;

namespace LittleRiver;

public partial class ScoreCounter : Control
{
    CollectibleDisposable Q { get; set; }
    static IRzeka rzeka => LittleSource.Rzeka;

    [Export]
    RichTextLabel _scoreLabel;

    [Export]
    RichTextLabel _timeLabel;

    double _elapsed;
    bool _isRunning;

    public override void _EnterTree()
    {
        Q = new();

        Q += rzeka.Weave<PlayerScoreState>(
            this,
            state => state.Subscribe(s => _scoreLabel.Text = $"Stars: {s.Score}/10")
        );

        Q += rzeka.Weave<GameStarted>(
            this,
            spell =>
                spell.Subscribe(_ =>
                {
                    _elapsed = 0;
                    _isRunning = true;
                })
        );

        Q += rzeka.Loom<GameWon, FinalGameTimeCaptured>(
            this,
            spell =>
                spell
                    .Take(1)
                    .Reacting(_ => _isRunning = false)
                    .Select(_ => new FinalGameTimeCaptured(_elapsed))
        );
    }

    public override void _Process(double delta)
    {
        if (!_isRunning)
            return;
        _elapsed += delta;
        _timeLabel.Text = $"{_elapsed:F2}s";
    }

    public override void _ExitTree()
    {
        Q.Dispose();
    }
}
