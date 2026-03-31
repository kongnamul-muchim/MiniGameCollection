namespace Games.Chess.Models;

public enum PieceType { None, Pawn, Knight, Bishop, Rook, Queen, King }
public enum PieceColor { White, Black }

public class ChessPiece
{
    public PieceType Type { get; }
    public PieceColor Color { get; }
    
    public ChessPiece(PieceType type, PieceColor color)
    {
        Type = type;
        Color = color;
    }
}

public struct Position
{
    public int Row { get; }
    public int Column { get; }
    
    public Position(int row, int column)
    {
        Row = row;
        Column = column;
    }
}