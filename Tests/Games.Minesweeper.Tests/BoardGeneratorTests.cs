using Games.Minesweeper.Logic;
using Games.Minesweeper.Models;
using Xunit;

namespace Games.Minesweeper.Tests;

public class BoardGeneratorTests
{
    [Fact]
    public void BoardGenerator_ShouldPlaceMinesRandomly()
    {
        var generator = new BoardGenerator(new Random(42));
        var board = new MinesweeperBoard(9, 9);
        
        generator.GenerateBoard(board, 10);
        
        int mineCount = 0;
        for (int r = 0; r < 9; r++)
            for (int c = 0; c < 9; c++)
                if (board.Cells[r, c].IsMine) mineCount++;
        
        Assert.Equal(10, mineCount);
    }

    [Fact]
    public void BoardGenerator_ShouldCalculateAdjacentMines()
    {
        var generator = new BoardGenerator(new Random(42));
        var board = new MinesweeperBoard(9, 9);
        
        // Place a mine at (4,4)
        board.Cells[4, 4].IsMine = true;
        
        generator.GenerateBoard(board, 1); // Already has 1 mine
        
        // Cell (3,3) should have 1 adjacent mine
        Assert.Equal(1, board.Cells[3, 3].AdjacentMines);
    }
}
