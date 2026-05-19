using Godot;
using System;
using System.Reactive.Linq;
using Rzeka;

namespace LittleRiver;
public partial class WorldEnvironmentFairy : Node
{
	public enum EnvironmentEnum { Startup, MainMenu, Game }
	
	[Export] WorldEnvironment WorldEnvironment { get; set; }
	[Export] Godot.Environment StartupEnvironment { get; set; }
	[Export] Godot.Environment MainMenuEnvironment { get; set; }
	[Export] Godot.Environment GameEnvironment { get; set; }
	
	CollectibleDisposable Q { get; set; }
	IRzeka rzeka => LittleSource.Rzeka;

	public override void _EnterTree()
	{
		Q = new();

		WorldEnvironment.Environment = StartupEnvironment;

		RegisterSpells();
	}

	public override void _Ready()
	{
	}

	public override void _ExitTree()
	{
		Q.Dispose();
	}

	void RegisterSpells()
	{
		Q += rzeka.Weave<WorldEnvironmentRequested>(
			this,
			spell => spell
				.Subscribe(e => WorldEnvironment.Environment = GetEnvironment(e.Environment)));
	}

	Godot.Environment GetEnvironment(EnvironmentEnum env)
	{
		return env switch
		{
			EnvironmentEnum.Startup => StartupEnvironment,
			EnvironmentEnum.MainMenu => StartupEnvironment,
			EnvironmentEnum.Game => StartupEnvironment,
			_ => StartupEnvironment
		};
	}
}
