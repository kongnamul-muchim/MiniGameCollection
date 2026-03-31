using Games.PatternMemory.Models;
using Core.Interfaces;
using Xunit;

namespace Games.PatternMemory.Tests;

public class PatternMemoryStateTests
{
    [Fact]
    public void State_ShouldInitializeWithDefaults()
    {
        var state = new PatternMemoryState();
        
        Assert.Equal(1, state.CurrentLevel);
        Assert.Equal(0, state.PlayerScore);
        Assert.Equal(3, state.MistakesAllowed);
        Assert.Empty(state.CurrentPattern);
        Assert.False(state.IsGameOver);
        Assert.Equal(1, state.CurrentPlayer);
        Assert.Null(state.Winner);
    }

    [Fact]
    public void State_ShouldTrackPattern()
    {
        var state = new PatternMemoryState();
        state.SetPattern(new List<int> { 1, 2, 3 });
        
        Assert.Equal(3, state.CurrentPattern.Count);
        Assert.Equal(1, state.CurrentPattern[0]);
    }

    [Fact]
    public void State_ShouldDetectGameOver()
    {
        var state = new PatternMemoryState();
        state.SetMistakesAllowed(0);
        
        Assert.True(state.IsGameOver);
    }

    [Fact]
    public void State_ShouldDetectVictory()
    {
        var state = new PatternMemoryState();
        state.SetVictory();
        
        Assert.True(state.IsGameOver);
        Assert.Equal(1, state.Winner);
    }
}