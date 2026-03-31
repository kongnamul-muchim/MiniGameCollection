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

public class EventBusTests
{
    [Fact]
    public void EventBus_ShouldPublishAndSubscribe()
    {
        var bus = new EventBus();
        var received = false;
        int? receivedScore = null;
        
        bus.Subscribe<ScoreChangedEvent>(e => 
        {
            received = true;
            receivedScore = e.NewScore;
        });
        
        bus.Publish(new ScoreChangedEvent(100, 10));
        
        Assert.True(received);
        Assert.Equal(100, receivedScore);
    }

    [Fact]
    public void EventBus_ShouldUnsubscribe()
    {
        var bus = new EventBus();
        var count = 0;
        
        Action<ScoreChangedEvent> handler = e => count++;
        
        bus.Subscribe(handler);
        bus.Publish(new ScoreChangedEvent(100, 10));
        Assert.Equal(1, count);
        
        bus.Unsubscribe(handler);
        bus.Publish(new ScoreChangedEvent(100, 10));
        Assert.Equal(1, count);
    }

    [Fact]
    public void EventBus_ShouldNotThrowWhenNoSubscribers()
    {
        var bus = new EventBus();
        bus.Publish(new ScoreChangedEvent(100, 10));
    }
}