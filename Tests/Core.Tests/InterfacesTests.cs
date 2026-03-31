using Core.Interfaces;
using Xunit;

namespace Core.Tests;

public class GameStateTests
{
    [Fact]
    public void GameState_ShouldHaveExpectedValues()
    {
        Assert.Equal(0, (int)GameState.None);
        Assert.Equal(1, (int)GameState.Ready);
        Assert.Equal(2, (int)GameState.Playing);
        Assert.Equal(3, (int)GameState.Paused);
        Assert.Equal(4, (int)GameState.GameOver);
        Assert.Equal(5, (int)GameState.Victory);
    }
}