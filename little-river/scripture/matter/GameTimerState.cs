using System.Text.Json.Serialization;
using Rzeka;

namespace LittleRiver;

[HasState]
public class GameTimerState : Matter
{
    [JsonIgnore]
    public GameTimer GameTimer { get; }

    public GameTimerState(GameTimer timer)
    {
        GameTimer = timer;
    }
}
