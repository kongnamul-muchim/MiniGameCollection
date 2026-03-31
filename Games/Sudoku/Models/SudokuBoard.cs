namespace Games.Sudoku.Models;

public class SudokuBoard
{
    public int[,] Cells { get; }
    public bool[,] FixedCells { get; }
    public int Rows => 9;
    public int Columns => 9;
    
    public SudokuBoard()
    {
        Cells = new int[9, 9];
        FixedCells = new bool[9, 9];
    }
    
    public void SetCell(int row, int col, int value, bool isFixed = false)
    {
        Cells[row, col] = value;
        FixedCells[row, col] = isFixed;
    }
    
    public bool IsFixed(int row, int col) => FixedCells[row, col];
    
    public void Clear() => Array.Clear(Cells, 0, Cells.Length);
    
    public SudokuBoard Clone()
    {
        var clone = new SudokuBoard();
        Array.Copy(Cells, clone.Cells, Cells.Length);
        Array.Copy(FixedCells, clone.FixedCells, FixedCells.Length);
        return clone;
    }
}