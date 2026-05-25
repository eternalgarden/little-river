using Rzeka;

namespace LittleRiver;

public class SceneEnteredTree : Matter
{
    public enum SceneEnum
    {
        MainMenu,
        Game,
    }

    public SceneEnum Scene { get; }

    public SceneEnteredTree(SceneEnum scene)
    {
        Scene = scene;
    }
}
