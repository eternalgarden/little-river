using Rzeka;

namespace LittleRiver;

public class WorldEnvironmentRequested : Matter
{
    public WorldEnvironmentFairy.EnvironmentEnum Environment { get; }

    public WorldEnvironmentRequested(WorldEnvironmentFairy.EnvironmentEnum environment)
    {
        Environment = environment;
    }
}
