using Games.Gomoku.Logic;
using Games.Gomoku.Models;
using Xunit;

namespace Games.Gomoku.Tests;

public class GomokuLogicTests
{
    [Fact]
    public void GomokuLogic_ShouldInitialize()
    {
        var validator = new GomokuValidator();
        var logic = new GomokuLogic(validator);
        
        logic.StartGame();
        
        Assert.Equal(1, logic.CurrentPlayer);
        Assert.False(logic.IsGameOver);
    }

    [Fact]
    public void GomokuLogic_ShouldPlaceStone()
    {
        var validator = new GomokuValidator();
        var logic = new GomokuLogic(validator);
        
        logic.StartGame();
        var result = logic.PlaceStone(7, 7);
        
        Assert.True(result);
        Assert.Equal(2, logic.CurrentPlayer);
    }

    [Fact]
    public void GomokuLogic_ShouldRejectInvalidMove()
    {
        var validator = new GomokuValidator();
        var logic = new GomokuLogic(validator);
        
        logic.StartGame();
        logic.PlaceStone(7, 7);
        var result = logic.PlaceStone(7, 7);
        
        Assert.False(result);
    }

    [Fact]
    public void GomokuLogic_ShouldDetectWin()
    {
        var validator = new GomokuValidator();
        var logic = new GomokuLogic(validator);
        
        logic.StartGame();
        
        // Player 1: 5 stones
        logic.PlaceStone(7, 5);
        logic.PlaceStone(0, 0);
        logic.PlaceStone(7, 6);
        logic.PlaceStone(0, 1);
        logic.PlaceStone(7, 7);
        logic.PlaceStone(0, 2);
        logic.PlaceStone(7, 8);
        logic.PlaceStone(0, 3);
        logic.PlaceStone(7, 9);
        
        Assert.True(logic.IsGameOver);
        Assert.Equal(1, logic.Winner);
    }

    [Fact]
    public void GomokuLogic_ShouldGetAIMove()
    {
        var validator = new GomokuValidator();
        var logic = new GomokuLogic(validator);
        
        logic.StartGame();
        
        var move = logic.GetAIMove();
        
        Assert.NotNull(move);
        Assert.True(move.Row >= 0 && move.Row < 15);
        Assert.True(move.Column >= 0 && move.Column < 15);
    }
}