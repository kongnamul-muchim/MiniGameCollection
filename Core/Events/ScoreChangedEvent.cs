namespace Core.Events;

public class ScoreChangedEvent : GameEvent
{
    public int NewScore { get; }
    public int Delta { get; }

    public ScoreChangedEvent(int newScore, int delta) : base("ScoreChanged")
    {
        NewScore = newScore;
        Delta = delta;
    }
}