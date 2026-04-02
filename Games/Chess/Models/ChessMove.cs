namespace Games.Chess.Models;

public class ChessMove
{
    public Position From { get; }
    public Position To { get; }
    public PieceType? Promotion { get; }
    public bool IsCastling { get; }
    public bool IsEnPassant { get; }
    
    public ChessMove(Position from, Position to, PieceType? promotion = null, bool isCastling = false, bool isEnPassant = false)
    {
        From = from;
        To = to;
        Promotion = promotion;
        IsCastling = isCastling;
        IsEnPassant = isEnPassant;
    }
}