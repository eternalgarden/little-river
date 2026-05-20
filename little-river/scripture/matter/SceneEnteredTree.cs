using Rzeka;

namespace LittleRiver;
public class SceneEnteredTree : Matter 
{
	public string SceneName { get; }
	
	public SceneEnteredTree(string sceneName)
	{
		SceneName = sceneName;
	}
}
