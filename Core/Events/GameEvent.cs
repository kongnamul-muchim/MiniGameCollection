using System.Collections.Generic;

namespace Core.Events;

public abstract class GameEvent
{
    public string EventType { get; protected set; }
    public long Timestamp { get; }
    public Dictionary<string, object> Data { get; }

    protected GameEvent(string type)
    {
        EventType = type;
        Timestamp = DateTime.UtcNow.Ticks;
        Data = new Dictionary<string, object>();
    }
}