namespace Core.Data;

public class GameSettings
{
    public Dictionary<string, int> HighScores { get; set; } = new();
    public Dictionary<string, bool> GameProgress { get; set; } = new();
    public int TotalPlayTime { get; set; }
}