namespace Games.Chess.Models;

public class ChessMove
{
    public Position From { get; }
    public Position To { get; }
    public PieceType? Promotion { get; }
    
    public ChessMove(Position from, Position to, PieceType? promotion = null)
    {
        From = from;
        To = to;
        Promotion = promotion;
    }
}