using Rzeka;

namespace LittleRiver;

public class GameWon : Matter
{
    public double ElapsedTime { get; }

    public GameWon(double time)
    {
        ElapsedTime = time;
    }
}
