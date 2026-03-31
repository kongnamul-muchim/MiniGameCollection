using Core.Events;
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