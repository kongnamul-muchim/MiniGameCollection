// Games/Minesweeper/Models/MinesweeperState.cs
using Core.Interfaces;

namespace Games.Minesweeper.Models;

public class MinesweeperState : IGameState
{
    public int Rows { get; set; } = 9;
    public int Columns { get; set; } = 9;
    public int TotalMines { get; set; } = 10;
    public int RevealedCount { get; set; }
    public int FlaggedCount { get; set; }
    public bool IsVictory { get; private set; }
    
    public int CurrentPlayer => 1;
    public bool IsGameOver => IsVictory || HitMine;
    public int? Winner => IsVictory ? 1 : null;
    
    public bool HitMine { get; private set; }
    
    public void SetVictory() => IsVictory = true;
    public void SetHitMine() => HitMine = true;
    public void IncrementRevealed() => RevealedCount++;
    public void IncrementFlagged() => FlaggedCount++;
    public void DecrementFlagged() => FlaggedCount--;
}
