using Godot;
using Rzeka;
using System.Text.Json.Serialization;

namespace LittleRiver;
public class ScreenFadeRequest : Request
{
    public enum ScreenFadeEnum { FadeIn, FadeOut };

    public ScreenFadeEnum ScreenFade { get; }
    public float ScreenFadeLength { get; }

    public ScreenFadeRequest(ScreenFadeEnum screenFade, float screenFadeLength)
    {
        ScreenFade = screenFade;
        ScreenFadeLength = screenFadeLength;
    }
}

public class ScreenFadeResponse : Response<ScreenFadeRequest>
{
    public ScreenFadeResponse(ScreenFadeRequest request, bool wasSuccessful) 
        : base(request, wasSuccessful)
    { 
    }
}
