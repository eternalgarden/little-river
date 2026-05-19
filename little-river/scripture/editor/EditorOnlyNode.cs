using Godot;
using System;
using System.Reactive.Linq;
using Rzeka;

namespace LittleRiver;
// Just to be able to have a world environment while doing the level design
// But ingame a central world environment is used
public partial class EditorOnlyNode : Node
{
	public override void _Ready()
	{
		QueueFree();
	}
}
