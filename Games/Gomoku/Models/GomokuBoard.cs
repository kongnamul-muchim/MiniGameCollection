namespace Games.Gomoku.Models;

public class GomokuBoard
{
    public int[,] Cells { get; }
    public int Size => 15;
    
    public GomokuBoard()
    {
        Cells = new int[15, 15];
    }
    
    public GomokuBoard(int size)
    {
        Cells = new int[size, size];
    }
    
    public virtual void SetCell(int row, int col, int value)
    {
        if (row >= 0 && row < Size && col >= 0 && col < Size)
            Cells[row, col] = value;
    }
    
    public virtual int GetCell(int row, int col)
    {
        if (row < 0 || row >= Size || col < 0 || col >= Size)
            return 0;
        return Cells[row, col];
    }
    
    public virtual void Clear() => Array.Clear(Cells, 0, Cells.Length);
    
    public virtual bool IsEmpty(int row, int col) => GetCell(row, col) == 0;
    
    public GomokuBoard Clone()
    {
        var clone = new GomokuBoard();
        Array.Copy(Cells, clone.Cells, Cells.Length);
        return clone;
    }
}