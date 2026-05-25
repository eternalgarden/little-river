using Rzeka;

namespace LittleRiver;

public class FinalGameTimeCaptured : Matter
{
    public double GameTime { get; }

    public FinalGameTimeCaptured(double gameTime)
    {
        GameTime = gameTime;
    }
}
