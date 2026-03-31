namespace Games.Gomoku.Models;

public class GomokuBoard
{
    public int[,] Cells { get; }
    public int Size => 15;
    
    public GomokuBoard()
    {
        Cells = new int[15, 15];
    }
    
    public void SetCell(int row, int col, int value)
    {
        if (row >= 0 && row < Size && col >= 0 && col < Size)
            Cells[row, col] = value;
    }
    
    public int GetCell(int row, int col) => Cells[row, col];
    
    public void Clear() => Array.Clear(Cells, 0, Cells.Length);
    
    public bool IsEmpty(int row, int col) => Cells[row, col] == 0;
    
    public GomokuBoard Clone()
    {
        var clone = new GomokuBoard();
        Array.Copy(Cells, clone.Cells, Cells.Length);
        return clone;
    }
}