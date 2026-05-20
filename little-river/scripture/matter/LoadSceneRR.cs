using Godot;
using Rzeka;
using System.Text.Json.Serialization;

namespace LittleRiver;
public class LoadSceneRequest : Request
{
	public string ScenePath { get; }

	public LoadSceneRequest(string scenePath)
	{
		ScenePath = scenePath;
	}
}

public class LoadSceneResponse : Response<LoadSceneRequest>
{
	[JsonIgnore] public PackedScene PackedScene { get; }

	public LoadSceneResponse(LoadSceneRequest request, PackedScene packedScene, bool wasSuccessful) 
		: base(request, wasSuccessful)
	{ 
		PackedScene = packedScene;
	}
}
