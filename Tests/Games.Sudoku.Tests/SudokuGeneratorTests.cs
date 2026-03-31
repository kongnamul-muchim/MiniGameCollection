using Games.Sudoku.Models;
using Games.Sudoku.Logic;
using Xunit;

namespace Games.Sudoku.Tests;

public class SudokuGeneratorTests
{
    [Fact]
    public void Generator_ShouldCreateValidPuzzle()
    {
        var generator = new SudokuGenerator(new Random(42));
        
        var puzzle = generator.GeneratePuzzle(1);
        
        var validator = new SudokuValidator();
        var board = new SudokuBoard();
        for (int r = 0; r < 9; r++)
            for (int c = 0; c < 9; c++)
                if (puzzle[r, c] != 0)
                    board.SetCell(r, c, puzzle[r, c], isFixed: true);
        
        // Should have valid rows
        for (int r = 0; r < 9; r++)
        {
            var seen = new HashSet<int>();
            for (int c = 0; c < 9; c++)
            {
                if (puzzle[r, c] != 0)
                {
                    Assert.True(seen.Add(puzzle[r, c]), $"Duplicate {puzzle[r, c]} in row {r}");
                }
            }
        }
    }

    [Fact]
    public void Generator_ShouldRemoveCorrectNumberOfCells()
    {
        var generator = new SudokuGenerator(new Random(42));
        
        var puzzle = generator.GeneratePuzzle(1);
        
        int givenCount = 0;
        for (int r = 0; r < 9; r++)
            for (int c = 0; c < 9; c++)
                if (puzzle[r, c] != 0) givenCount++;
        
        // Easy should have around 36-46 given numbers
        Assert.True(givenCount >= 30 && givenCount <= 50);
    }
}