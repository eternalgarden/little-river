using System;
using System.Reactive.Linq;
using Godot;
using Rzeka;

namespace LittleRiver;

public partial class GameLevel : Node3D
{
	CollectibleDisposable Q { get; set; }
	IRzeka rzeka => LittleSource.Rzeka;

	public override void _EnterTree()
	{
		Q = new();
	}

	public override void _Ready() { }

	public override void _Process(double delta) { }

	public override void _ExitTree()
	{
		Q.Dispose();
	}
}
