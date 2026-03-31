using Games.Sudoku.Models;
using Xunit;

namespace Games.Sudoku.Tests;

public class SudokuBoardTests
{
    [Fact]
    public void Board_ShouldInitializeWithEmptyGrid()
    {
        var board = new SudokuBoard();
        
        Assert.Equal(9, board.Rows);
        Assert.Equal(9, board.Columns);
        Assert.Equal(81, board.Cells.Length);
        
        for (int r = 0; r < 9; r++)
            for (int c = 0; c < 9; c++)
                Assert.Equal(0, board.Cells[r, c]);
    }

    [Fact]
    public void Board_ShouldAllowSettingCells()
    {
        var board = new SudokuBoard();
        
        board.SetCell(0, 0, 5);
        
        Assert.Equal(5, board.Cells[0, 0]);
    }

    [Fact]
    public void Board_ShouldTrackFixedCells()
    {
        var board = new SudokuBoard();
        
        board.SetCell(0, 0, 5, isFixed: true);
        
        Assert.Equal(5, board.Cells[0, 0]);
        Assert.True(board.IsFixed(0, 0));
    }

    [Fact]
    public void Board_ShouldClone()
    {
        var board = new SudokuBoard();
        board.SetCell(0, 0, 5, isFixed: true);
        
        var clone = board.Clone();
        
        Assert.Equal(5, clone.Cells[0, 0]);
        Assert.True(clone.IsFixed(0, 0));
    }
}