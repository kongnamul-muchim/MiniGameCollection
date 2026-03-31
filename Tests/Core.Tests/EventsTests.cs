using Core.Events;
using Core.Interfaces;
using Xunit;

namespace Core.Tests;

public class GameEventTests
{
    [Fact]
    public void GameEvent_ShouldHaveEventTypeAndTimestamp()
    {
        var evt = new TestGameEvent("test");
        Assert.Equal("test", evt.EventType);
        Assert.True(evt.Timestamp > 0);
    }

    [Fact]
    public void GameEvent_ShouldHaveDataDictionary()
    {
        var evt = new TestGameEvent("test");
        evt.Data["key"] = "value";
        Assert.Equal("value", evt.Data["key"]);
    }

    private class TestGameEvent : GameEvent
    {
        public TestGameEvent(string type) : base(type) { }
    }
}

public class ScoreChangedEventTests
{
    [Fact]
    public void ScoreChangedEvent_ShouldContainScoreData()
    {
        var evt = new ScoreChangedEvent(100, 10);
        Assert.Equal("ScoreChanged", evt.EventType);
        Assert.Equal(100, evt.NewScore);
        Assert.Equal(10, evt.Delta);
    }
}

public class GameOverEventTests
{
    [Fact]
    public void GameOverEvent_ShouldContainVictoryAndScore()
    {
        var evt = new GameOverEvent(true, 500);
        Assert.Equal("GameOver", evt.EventType);
        Assert.True(evt.IsVictory);
        Assert.Equal(500, evt.FinalScore);
    }
}

public class GameStateChangedEventTests
{
    [Fact]
    public void GameStateChangedEvent_ShouldContainStateChange()
    {
        var evt = new GameStateChangedEvent(GameState.Ready, GameState.Playing);
        Assert.Equal("GameStateChanged", evt.EventType);
        Assert.Equal(GameState.Ready, evt.PreviousState);
        Assert.Equal(GameState.Playing, evt.NewState);
    }
}