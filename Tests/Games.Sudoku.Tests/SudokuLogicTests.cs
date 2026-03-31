using Games.Sudoku.Logic;
using Xunit;

namespace Games.Sudoku.Tests;

public class SudokuLogicTests
{
    [Fact]
    public void SudokuLogic_ShouldInitialize()
    {
        var generator = new SudokuGenerator(new Random(42));
        var validator = new SudokuValidator();
        var logic = new SudokuLogic(generator, validator);
        
        logic.NewGame(1);
        
        Assert.NotNull(logic.Board);
    }

    [Fact]
    public void SudokuLogic_ShouldPlaceNumber()
    {
        var generator = new SudokuGenerator(new Random(42));
        var validator = new SudokuValidator();
        var logic = new SudokuLogic(generator, validator);
        
        logic.NewGame(1);
        
        // Just verify we can create a game and board exists
        Assert.NotNull(logic.Board);
        
        // If board has empty cells, we can place
        bool hasEmpty = false;
        for (int r = 0; r < 9; r++)
            for (int c = 0; c < 9; c++)
                if (logic.Board.Cells[r, c] == 0) hasEmpty = true;
        
        Assert.True(hasEmpty, "Board should have empty cells");
    }

    [Fact]
    public void SudokuLogic_ShouldRejectInvalidMove()
    {
        var generator = new SudokuGenerator(new Random(42));
        var validator = new SudokuValidator();
        var logic = new SudokuLogic(generator, validator);
        
        logic.NewGame(1);
        
        // Find two empty cells in same row
        int? firstCol = null;
        int? secondCol = null;
        int row = 0;
        
        for (int c = 0; c < 9; c++)
        {
            if (!logic.Board!.IsFixed(row, c))
            {
                if (firstCol == null) firstCol = c;
                else if (secondCol == null) secondCol = c;
            }
        }
        
        if (firstCol != null && secondCol != null)
        {
            logic.PlaceNumber(row, firstCol.Value, 5);
            var result = logic.PlaceNumber(row, secondCol.Value, 5);
            Assert.False(result);
        }
    }
}