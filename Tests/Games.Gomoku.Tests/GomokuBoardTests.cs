using Games.Gomoku.Models;
using Xunit;

namespace Games.Gomoku.Tests;

public class GomokuBoardTests
{
    [Fact]
    public void Board_ShouldHaveCorrectSize()
    {
        var board = new GomokuBoard();
        
        Assert.Equal(15, board.Size);
        Assert.Equal(225, board.Cells.Length);
    }

    [Fact]
    public void Board_ShouldTrackStones()
    {
        var board = new GomokuBoard();
        
        board.SetCell(7, 7, 1);
        board.SetCell(7, 8, 2);
        
        Assert.Equal(1, board.GetCell(7, 7));
        Assert.Equal(2, board.GetCell(7, 8));
    }

    [Fact]
    public void Board_ShouldDetectEmptyCell()
    {
        var board = new GomokuBoard();
        
        Assert.True(board.IsEmpty(0, 0));
        
        board.SetCell(0, 0, 1);
        
        Assert.False(board.IsEmpty(0, 0));
    }

    [Fact]
    public void Board_ShouldClone()
    {
        var board = new GomokuBoard();
        board.SetCell(7, 7, 1);
        
        var clone = board.Clone();
        
        Assert.Equal(1, clone.GetCell(7, 7));
        
        // Modify original should not affect clone
        board.SetCell(7, 7, 2);
        
        Assert.Equal(1, clone.GetCell(7, 7));
    }
}

public class GomokuStateTests
{
    [Fact]
    public void State_ShouldStartWithPlayer1()
    {
        var state = new GomokuState();
        
        Assert.Equal(1, state.CurrentPlayer);
    }

    [Fact]
    public void State_ShouldSwitchPlayer()
    {
        var state = new GomokuState();
        
        state.SwitchPlayer();
        
        Assert.Equal(2, state.CurrentPlayer);
        
        state.SwitchPlayer();
        
        Assert.Equal(1, state.CurrentPlayer);
    }
}