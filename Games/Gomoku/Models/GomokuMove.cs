namespace Games.Gomoku.Models;

public class GomokuMove
{
    public int Row { get; }
    public int Column { get; }
    
    public GomokuMove(int row, int column)
    {
        Row = row;
        Column = column;
    }
}