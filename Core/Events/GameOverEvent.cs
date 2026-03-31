namespace Core.Events;

public class GameOverEvent : GameEvent
{
    public bool IsVictory { get; }
    public int FinalScore { get; }

    public GameOverEvent(bool isVictory, int finalScore) : base("GameOver")
    {
        IsVictory = isVictory;
        FinalScore = finalScore;
    }
}