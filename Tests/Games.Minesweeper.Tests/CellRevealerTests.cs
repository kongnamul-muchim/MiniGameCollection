using Games.Minesweeper.Logic;
using Games.Minesweeper.Models;
using Xunit;

namespace Games.Minesweeper.Tests;

public class CellRevealerTests
{
    [Fact]
    public void Revealer_ShouldRevealCell()
    {
        var revealer = new CellRevealer();
        var board = new MinesweeperBoard(9, 9);
        
        revealer.Reveal(board, 0, 0);
        
        Assert.True(board.Cells[0, 0].IsRevealed);
    }

    [Fact]
    public void Revealer_ShouldNotRevealFlaggedCell()
    {
        var revealer = new CellRevealer();
        var board = new MinesweeperBoard(9, 9);
        
        board.Cells[0, 0].IsFlagged = true;
        revealer.Reveal(board, 0, 0);
        
        Assert.False(board.Cells[0, 0].IsRevealed);
    }

    [Fact]
    public void Revealer_ShouldExpandEmptyCells()
    {
        var board = new MinesweeperBoard(9, 9);
        // Set up: center cell has no adjacent mines
        board.Cells[4, 4].AdjacentMines = 0;
        
        var revealer = new CellRevealer();
        revealer.Reveal(board, 4, 4);
        
        // Center should be revealed
        Assert.True(board.Cells[4, 4].IsRevealed);
    }
}
