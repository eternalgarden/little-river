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

	GameTimer _gameTimer;
	IDisposable _timerSubscription;

	public override void _EnterTree()
	{
		Q = new();
		Visible = false;

		Q += rzeka.Weave<PlayerScoreState>(
			this,
			state => state.Subscribe(s => _scoreLabel.Text = $"Stars: {s.Score}/10")
		);

		Q += rzeka.Weave<GameTimerState>(
			this,
			spell => spell.Subscribe(state => _gameTimer = state.GameTimer)
		);

		Q += rzeka.Weave<GameStarted>(
			this,
			spell =>
				spell.Subscribe(_ =>
				{
					Visible = true;
					_timerSubscription?.Dispose();
					_timerSubscription = _gameTimer.Time.Subscribe(t =>
						_timeLabel.Text = $"{t:F2}s"
					);
				})
		);

		Q += rzeka.Weave<GameWon>(
			this,
			spell =>
				spell.Subscribe(_ =>
				{
					_timerSubscription?.Dispose();
					_timerSubscription = null;
					Visible = false;
				})
		);
	}

	public override void _ExitTree()
	{
		_timerSubscription?.Dispose();
		Q.Dispose();
	}
}
