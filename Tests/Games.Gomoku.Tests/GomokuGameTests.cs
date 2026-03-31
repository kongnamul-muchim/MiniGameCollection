using Core.Interfaces;
using Games.Gomoku.Logic;
using Xunit;

namespace Games.Gomoku.Tests;

public class GomokuGameTests
{
    private readonly GomokuGame _game;

    public GomokuGameTests()
    {
        var validator = new GomokuValidator();
        var logic = new GomokuLogic(validator);
        _game = new GomokuGame(logic);
    }

    [Fact]
    public void Game_ShouldHaveCorrectInfo()
    {
        Assert.Equal("Gomoku", _game.GameName);
        Assert.NotEmpty(_game.GameDescription);
    }

    [Fact]
    public void StartGame_ShouldInitialize()
    {
        _game.StartGame();
        
        Assert.Equal(GameState.Playing, _game.CurrentState);
    }

    [Fact]
    public void PauseGame_ShouldPause()
    {
        _game.StartGame();
        _game.PauseGame();
        
        Assert.True(_game.IsPaused);
    }

    [Fact]
    public void ResumeGame_ShouldResume()
    {
        _game.StartGame();
        _game.PauseGame();
        _game.ResumeGame();
        
        Assert.True(_game.IsPlaying);
    }
}