using Core.Interfaces;

namespace Games.Chess.Models;

public class ChessState : IGameState
{
    public int CurrentPlayer { get; set; } = 1; // 1 = White, 2 = Black
    public bool IsGameOver { get; set; }
    public int? Winner { get; set; }
    
    // Castling rights
    public bool WhiteKingSideCastle { get; set; } = true;
    public bool WhiteQueenSideCastle { get; set; } = true;
    public bool BlackKingSideCastle { get; set; } = true;
    public bool BlackQueenSideCastle { get; set; } = true;
    
    // En passant target square
    public Position? EnPassantTarget { get; set; }
    
    public PieceColor CurrentColor => CurrentPlayer == 1 ? PieceColor.White : PieceColor.Black;
    
    public void SetWinner(int player)
    {
        Winner = player;
        IsGameOver = true;
    }
    
    public void SwitchPlayer()
    {
        CurrentPlayer = CurrentPlayer == 1 ? 2 : 1;
    }
}