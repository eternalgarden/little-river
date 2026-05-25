using System;
using System.Reactive.Linq;
using Godot;
using Rzeka;

namespace LittleRiver;

public partial class Star : Node3D
{
	[Export]
	Area3D _starArea;

	[Export]
	AudioStreamPlayer3D _audioPlayer;

	CollectibleDisposable Q { get; set; }
	static IRzeka rzeka => LittleSource.Rzeka;

	public override void _EnterTree()
	{
		Q = new();

		RegisterSpells();
	}

	public override void _Ready() { }

	public override void _Process(double delta) { }

	public override void _ExitTree()
	{
		Q.Dispose();
	}

	void RegisterSpells()
	{
		Q += _starArea
			.OnBodyEntered()
			.Where(h => h is Player)
			.Take(1)
			.Subscribe(_ => CollectStar());
	}

	void CollectStar()
	{
		rzeka.Pluck(this, new StarCollected(Name));
		Visible = false;
		// _starArea.SetDeferred(Area3D.PropertyName.Monitorable, false);
		_starArea.Monitorable = false;
		_audioPlayer.Play();
		_audioPlayer.OnFinished().Take(1).Subscribe(_ => QueueFree());
	}
}
