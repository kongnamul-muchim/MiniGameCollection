using Games.Minesweeper.Models;
using Xunit;

namespace Games.Minesweeper.Tests;

public class MinesweeperBoardTests
{
    [Fact]
    public void Board_ShouldInitializeWithCorrectSize()
    {
        var board = new MinesweeperBoard(9, 9);
        
        Assert.Equal(9, board.Rows);
        Assert.Equal(9, board.Columns);
        Assert.Equal(81, board.Cells.Length);
    }

    [Fact]
    public void Cell_ShouldHaveCorrectPosition()
    {
        var board = new MinesweeperBoard(9, 9);
        var cell = board.Cells[3, 5];
        
        Assert.Equal(3, cell.Row);
        Assert.Equal(5, cell.Column);
        Assert.False(cell.IsMine);
        Assert.False(cell.IsRevealed);
        Assert.False(cell.IsFlagged);
        Assert.Equal(0, cell.AdjacentMines);
    }
}
