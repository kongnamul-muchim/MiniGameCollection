using System.Collections.Generic;
using Core.Interfaces;

namespace Games.PatternMemory.Models;

public class PatternMemoryState : IGameState
{
    public List<int> Pattern { get; private set; } = new();
    public int CurrentLevel { get; set; } = 1;
    public int PlayerScore { get; private set; } = 0;
    public int MistakesAllowed { get; private set; } = 3;
    public bool IsVictory { get; private set; }

    public IReadOnlyList<int> CurrentPattern => Pattern;
    public int CurrentPlayer => 1;
    public bool IsGameOver => MistakesAllowed <= 0 || IsVictory;
    public int? Winner => IsVictory ? 1 : null;

    public void SetPattern(List<int> pattern) => Pattern = pattern;
    public void SetMistakesAllowed(int count) => MistakesAllowed = count;
    public void AddScore(int points) => PlayerScore += points;
    public void SetVictory() => IsVictory = true;
    public void Reset()
    {
        Pattern = new List<int>();
        CurrentLevel = 1;
        PlayerScore = 0;
        MistakesAllowed = 3;
        IsVictory = false;
    }
}