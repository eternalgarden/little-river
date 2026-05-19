using Godot;
using Rzeka;
using Rzeka.Dev;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Threading;

namespace LittleRiver;
public partial class LittleSource : Node
{
	public static IRzeka Rzeka { get; private set; }
	public static IScheduler MainThread { get; private set; }

    CollectibleDisposable Q { get; set; } = new();

	public override void _EnterTree()
	{
		SynchronizationContext.SetSynchronizationContext(new GodotMainThreadContext());
		MainThread = new SynchronizationContextScheduler(SynchronizationContext.Current);

        Spring spring = new();
        Q += spring.EnableDevServer();
		Rzeka = spring
            .Create("little-river");
			
		GD.Print("🌊 Rzeka is operational!");
	}

	public override void _ExitTree()
	{
		Q.Dispose();
        Rzeka.Dispose();
	}

	// Posts callbacks to Godot's main thread via CallDeferred - backs the MainThread scheduler above.
	private sealed class GodotMainThreadContext : SynchronizationContext
	{
		public override void Post(SendOrPostCallback d, object state) =>
			Callable.From(() => d(state)).CallDeferred();
	}
}
