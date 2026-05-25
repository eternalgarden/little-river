using Rzeka;

namespace LittleRiver;

public class MainMenuRequested : Matter
{
    public bool SkipFadeOut { get; }

    public MainMenuRequested(bool skipFadeOut = false)
    {
        SkipFadeOut = skipFadeOut;
    }
}
