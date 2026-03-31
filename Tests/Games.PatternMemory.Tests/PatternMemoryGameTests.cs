using Core.Interfaces;
using Core.Events;
using Games.PatternMemory.Logic;
using Xunit;

namespace Games.PatternMemory.Tests;

public class PatternMemoryGameTests
{
    private readonly PatternMemoryGame _game;

    public PatternMemoryGameTests()
    {
        var generator = new PatternGenerator(new Random(42));
        var logic = new PatternMemoryLogic(generator);
        _game = new PatternMemoryGame(logic);
    }

    [Fact]
    public void Game_ShouldHaveCorrectInfo()
    {
        Assert.Equal("Pattern Memory", _game.GameName);
        Assert.NotEmpty(_game.GameDescription);
    }

    [Fact]
    public void Game_ShouldStartInReadyState()
    {
        Assert.Equal(GameState.None, _game.CurrentState);
    }

    [Fact]
    public void StartGame_ShouldTransitionToPlaying()
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
    public void ResetGame_ShouldReturnToReady()
    {
        _game.StartGame();
        _game.ResetGame();
        
        Assert.Equal(GameState.Ready, _game.CurrentState);
    }

    [Fact]
    public void OnGameEvent_ShouldFireOnStateChange()
    {
        GameEvent? receivedEvent = null;
        _game.OnGameEvent += e => receivedEvent = e;
        
        _game.StartGame();
        
        Assert.NotNull(receivedEvent);
    }
}
