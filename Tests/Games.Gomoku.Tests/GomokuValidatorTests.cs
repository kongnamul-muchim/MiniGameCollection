using Games.Gomoku.Logic;
using Games.Gomoku.Models;
using Xunit;

namespace Games.Gomoku.Tests;

public class GomokuValidatorTests
{
    private readonly GomokuValidator _validator;

    public GomokuValidatorTests()
    {
        _validator = new GomokuValidator();
    }

    [Fact]
    public void Validator_ShouldAcceptValidMove()
    {
        var board = new GomokuBoard();
        
        var result = _validator.IsValidMove(board, 7, 7);
        
        Assert.True(result);
    }

    [Fact]
    public void Validator_ShouldRejectOccupiedCell()
    {
        var board = new GomokuBoard();
        board.SetCell(7, 7, 1);
        
        var result = _validator.IsValidMove(board, 7, 7);
        
        Assert.False(result);
    }

    [Fact]
    public void Validator_ShouldRejectOutOfBounds()
    {
        var board = new GomokuBoard();
        
        Assert.False(_validator.IsValidMove(board, -1, 0));
        Assert.False(_validator.IsValidMove(board, 0, -1));
        Assert.False(_validator.IsValidMove(board, 15, 0));
        Assert.False(_validator.IsValidMove(board, 0, 15));
    }

    [Fact]
    public void Validator_ShouldDetectHorizontalWin()
    {
        var board = new GomokuBoard();
        
        // Place 5 stones horizontally
        for (int c = 5; c < 10; c++)
            board.SetCell(7, c, 1);
        
        var result = _validator.CheckWin(board, 7, 7, 1);
        
        Assert.True(result);
    }

    [Fact]
    public void Validator_ShouldDetectVerticalWin()
    {
        var board = new GomokuBoard();
        
        // Place 5 stones vertically
        for (int r = 5; r < 10; r++)
            board.SetCell(r, 7, 1);
        
        var result = _validator.CheckWin(board, 7, 7, 1);
        
        Assert.True(result);
    }

    [Fact]
    public void Validator_ShouldDetectDiagonalWin()
    {
        var board = new GomokuBoard();
        
        // Place 5 stones diagonally
        for (int i = 0; i < 5; i++)
            board.SetCell(5 + i, 5 + i, 1);
        
        var result = _validator.CheckWin(board, 7, 7, 1);
        
        Assert.True(result);
    }

    [Fact]
    public void Validator_ShouldNotDetectWinWithFourStones()
    {
        var board = new GomokuBoard();
        
        // Place 4 stones
        for (int c = 5; c < 9; c++)
            board.SetCell(7, c, 1);
        
        var result = _validator.CheckWin(board, 7, 7, 1);
        
        Assert.False(result);
    }
}