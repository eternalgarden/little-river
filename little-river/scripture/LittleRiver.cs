using Godot;
using Rzeka;
using System.Reactive.Concurrency;
using System.Threading;

public partial class LittleRiver : Node
{
	public static IRzeka Rzeka { get; private set; }
	public static IScheduler MainThread { get; private set; }

	public override void _Ready()
	{
		SynchronizationContext.SetSynchronizationContext(new GodotMainThreadContext());
		MainThread = new SynchronizationContextScheduler(SynchronizationContext.Current);

		Rzeka = new Spring()
			.Create("little-river");
			
		GD.Print("🌊 Rzeka is operational!");
	}

	// Posts callbacks to Godot's main thread via CallDeferred - backs the MainThread scheduler above.
	private sealed class GodotMainThreadContext : SynchronizationContext
	{
		public override void Post(SendOrPostCallback d, object state) =>
			Callable.From(() => d(state)).CallDeferred();
	}
}
