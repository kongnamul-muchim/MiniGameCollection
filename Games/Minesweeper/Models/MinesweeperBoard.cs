// Games/Minesweeper/Models/MinesweeperBoard.cs
namespace Games.Minesweeper.Models;

public class Cell
{
    public int Row { get; }
    public int Column { get; }
    public bool IsMine { get; set; }
    public bool IsRevealed { get; set; }
    public bool IsFlagged { get; set; }
    public int AdjacentMines { get; set; }
    
    public Cell(int row, int column)
    {
        Row = row;
        Column = column;
    }
}

public class MinesweeperBoard
{
    public Cell[,] Cells { get; }
    public int Rows { get; }
    public int Columns { get; }
    
    public MinesweeperBoard(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        Cells = new Cell[rows, columns];
        InitializeCells();
    }
    
    private void InitializeCells()
    {
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Columns; c++)
                Cells[r, c] = new Cell(r, c);
    }
}
