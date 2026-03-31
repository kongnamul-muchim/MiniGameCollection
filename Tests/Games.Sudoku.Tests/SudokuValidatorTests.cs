using Games.Sudoku.Models;
using Games.Sudoku.Logic;
using Xunit;

namespace Games.Sudoku.Tests;

public class SudokuValidatorTests
{
    private readonly SudokuValidator _validator;

    public SudokuValidatorTests()
    {
        _validator = new SudokuValidator();
    }

    [Fact]
    public void Validator_ShouldAcceptValidMove()
    {
        var board = new SudokuBoard();
        
        var result = _validator.IsValidMove(board, 0, 0, 5);
        
        Assert.True(result);
    }

    [Fact]
    public void Validator_ShouldRejectDuplicateInRow()
    {
        var board = new SudokuBoard();
        board.SetCell(0, 0, 5);
        
        var result = _validator.IsValidMove(board, 0, 1, 5);
        
        Assert.False(result);
    }

    [Fact]
    public void Validator_ShouldRejectDuplicateInColumn()
    {
        var board = new SudokuBoard();
        board.SetCell(0, 0, 5);
        
        var result = _validator.IsValidMove(board, 1, 0, 5);
        
        Assert.False(result);
    }

    [Fact]
    public void Validator_ShouldRejectDuplicateInBox()
    {
        var board = new SudokuBoard();
        board.SetCell(0, 0, 5);
        
        var result = _validator.IsValidMove(board, 1, 1, 5);
        
        Assert.False(result);
    }

    [Fact]
    public void Validator_ShouldDetectSolvedBoard()
    {
        var board = new SudokuBoard();
        var solvedGrid = new int[,]
        {
            {5,3,4,6,7,8,9,1,2},
            {6,7,2,1,9,5,3,4,8},
            {1,9,8,3,4,2,5,6,7},
            {8,5,9,7,6,1,4,2,3},
            {4,2,6,8,5,3,7,9,1},
            {7,1,3,9,2,4,8,5,6},
            {9,6,1,5,3,7,2,8,4},
            {2,8,7,4,1,9,6,3,5},
            {3,4,5,2,8,6,1,7,9}
        };
        
        for (int r = 0; r < 9; r++)
            for (int c = 0; c < 9; c++)
                board.SetCell(r, c, solvedGrid[r, c]);
        
        var result = _validator.IsSolved(board);
        
        Assert.True(result);
    }

    [Fact]
    public void Validator_ShouldRejectInvalidNumber()
    {
        var board = new SudokuBoard();
        
        var result = _validator.IsValidMove(board, 0, 0, 0);
        
        Assert.False(result);

        result = _validator.IsValidMove(board, 0, 0, 10);
        
        Assert.False(result);
    }
}