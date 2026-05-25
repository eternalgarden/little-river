using Rzeka;

namespace LittleRiver;

public class StarCollected : Matter
{
    public string StarName { get; }

    public StarCollected(string starName) => StarName = starName;
}
