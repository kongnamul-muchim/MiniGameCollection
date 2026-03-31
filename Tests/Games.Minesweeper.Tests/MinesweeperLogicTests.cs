using Games.Minesweeper.Logic;
using Games.Minesweeper.Models;
using Xunit;

namespace Games.Minesweeper.Tests;

public class MinesweeperLogicTests
{
    [Fact]
    public void MinesweeperLogic_ShouldInitialize()
    {
        var generator = new BoardGenerator(new Random(42));
        var revealer = new CellRevealer();
        var logic = new MinesweeperLogic(generator, revealer);
        
        logic.Initialize(9, 9, 10);
        
        Assert.NotNull(logic.Board);
    }

    [Fact]
    public void MinesweeperLogic_ShouldRevealCell()
    {
        var generator = new BoardGenerator(new Random(42));
        var revealer = new CellRevealer();
        var logic = new MinesweeperLogic(generator, revealer);
        
        logic.Initialize(9, 9, 10);
        logic.RevealCell(0, 0);
        
        Assert.True(logic.Board!.Cells[0, 0].IsRevealed);
    }

    [Fact]
    public void MinesweeperLogic_ShouldToggleFlag()
    {
        var generator = new BoardGenerator(new Random(42));
        var revealer = new CellRevealer();
        var logic = new MinesweeperLogic(generator, revealer);
        
        logic.Initialize(9, 9, 10);
        logic.ToggleFlag(0, 0);
        
        Assert.True(logic.Board!.Cells[0, 0].IsFlagged);
        
        logic.ToggleFlag(0, 0);
        Assert.False(logic.Board.Cells[0, 0].IsFlagged);
    }

    [Fact]
    public void MinesweeperLogic_ShouldDetectGameOver()
    {
        var generator = new BoardGenerator(new Random(42));
        var revealer = new CellRevealer();
        var logic = new MinesweeperLogic(generator, revealer);
        
        logic.Initialize(9, 9, 10);
        
        // Find a mine cell
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                if (logic.Board!.Cells[r, c].IsMine)
                {
                    logic.RevealCell(r, c);
                    Assert.True(logic.IsGameOver);
                    return;
                }
            }
        }
    }
}
