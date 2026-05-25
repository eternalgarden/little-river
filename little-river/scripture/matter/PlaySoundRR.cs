using Godot;
using Rzeka;

namespace LittleRiver;

public class PlaySoundRequest : Request
{
    public AudioStream Sound { get; }
    public float VolumeDb { get; }

    public PlaySoundRequest(AudioStream sound, float volumeDb = 0f)
    {
        Sound = sound;
        VolumeDb = volumeDb;
    }
}

public class PlaySoundResponse : Response<PlaySoundRequest>
{
    public PlaySoundResponse(PlaySoundRequest request, bool wasSuccessful)
        : base(request, wasSuccessful) { }
}
