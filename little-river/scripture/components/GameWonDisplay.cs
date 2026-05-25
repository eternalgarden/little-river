using System;
using System.Reactive.Linq;
using Godot;
using Rzeka;

namespace LittleRiver;

public partial class GameWonDisplay : Control
{
	CollectibleDisposable Q { get; set; }
	static IRzeka rzeka => LittleSource.Rzeka;

	[Export]
	RichTextLabel _timeLabel;

	public override void _EnterTree()
	{
		Q = new();
		Visible = false;

		Q += rzeka.Weave<GameWon>(
			this,
			spell =>
				spell.Subscribe(won =>
				{
					_timeLabel.Text = $"It took you: {won.ElapsedTime:F2}s! ✨";
					Visible = true;
				})
		);
	}

	public override void _ExitTree()
	{
		Q.Dispose();
	}
}
