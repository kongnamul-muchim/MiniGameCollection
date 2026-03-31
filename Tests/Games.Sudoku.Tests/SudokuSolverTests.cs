using Games.Sudoku.Models;
using Games.Sudoku.Logic;
using Xunit;

namespace Games.Sudoku.Tests;

public class SudokuSolverTests
{
    [Fact]
    public void Solver_ShouldReturnFalseForInvalidPuzzle()
    {
        var solver = new SudokuSolver();
        var board = new SudokuBoard();
        
        // Invalid: duplicate in row
        board.SetCell(0, 0, 5);
        board.SetCell(0, 1, 5);
        
        var result = solver.Solve(board);
        
        Assert.False(result);
    }
}