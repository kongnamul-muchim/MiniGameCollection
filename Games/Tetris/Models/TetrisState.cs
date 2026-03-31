using Core.Interfaces;

namespace Games.Tetris.Models;

public class TetrisState : IGameState
{
    public int Rows => 20;
    public int Columns => 10;
    public int CurrentPlayer => 1;
    public bool IsGameOver => IsVictory || GameOver;
    public int? Winner => IsVictory ? 1 : null;
    
    public bool IsVictory { get; internal set; }
    public bool GameOver { get; internal set; }
    public int Score { get; set; }
    public int LinesCleared { get; set; }
    public int Level { get; set; } = 1;
    
    public void SetGameOver() => GameOver = true;
    public void SetVictory() => IsVictory = true;
    public void AddScore(int points) => Score += points;
    public void IncrementLines() => LinesCleared++;
    public void IncrementLevel() => Level++;
}