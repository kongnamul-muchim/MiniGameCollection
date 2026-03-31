namespace Games.Tetris.Models;

public class TetrisBoard
{
    public int[,] Cells { get; }
    public int Rows => 20;
    public int Columns => 10;
    
    public TetrisBoard()
    {
        Cells = new int[20, 10];
    }
    
    public void SetCell(int row, int col, int value)
    {
        if (row >= 0 && row < Rows && col >= 0 && col < Columns)
            Cells[row, col] = value;
    }
    
    public int GetCell(int row, int col) => Cells[row, col];
    
    public void Clear() => Array.Clear(Cells, 0, Cells.Length);
    
    public bool IsFullLine(int row)
    {
        for (int c = 0; c < Columns; c++)
            if (Cells[row, c] == 0) return false;
        return true;
    }
    
    public void ClearLine(int row)
    {
        for (int c = 0; c < Columns; c++)
            Cells[row, c] = 0;
    }
    
    public void CopyRow(int fromRow, int toRow)
    {
        for (int c = 0; c < Columns; c++)
            Cells[toRow, c] = Cells[fromRow, c];
    }
}