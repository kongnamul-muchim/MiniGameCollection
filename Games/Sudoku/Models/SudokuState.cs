using Core.Interfaces;

namespace Games.Sudoku.Models;

public class SudokuState : IGameState
{
    public int Rows => 9;
    public int Columns => 9;
    public int CurrentPlayer => 1;
    public bool IsGameOver => IsVictory;
    public int? Winner => IsVictory ? 1 : null;
    
    public bool IsVictory { get; internal set; }
    public int Mistakes { get; set; }
    public int Difficulty { get; set; } = 1;
    
    public void SetVictory() => IsVictory = true;
    public void IncrementMistakes() => Mistakes++;
}