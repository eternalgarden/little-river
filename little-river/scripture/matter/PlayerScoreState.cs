using Rzeka;

namespace LittleRiver;

[HasState]
public class PlayerScoreState : Matter
{
	public int Score { get; }

	public PlayerScoreState(int score)
	{
		Score = score;
	}
}
