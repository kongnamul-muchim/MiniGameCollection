using Core.Interfaces;

namespace Games.Gomoku.Models;

public class GomokuState : IGameState
{
    public int Size => 15;
    public int CurrentPlayer { get; set; } = 1;
    public bool IsGameOver { get; set; }
    public int? Winner { get; set; }
    
    public int MovesPlayed { get; set; }
    
    public void SetWinner(int player)
    {
        Winner = player;
        IsGameOver = true;
    }
    
    public void SwitchPlayer()
    {
        CurrentPlayer = CurrentPlayer == 1 ? 2 : 1;
    }
    
    public void IncrementMoves() => MovesPlayed++;
}