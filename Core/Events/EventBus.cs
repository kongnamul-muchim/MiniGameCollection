using System.Collections.Generic;

namespace Core.Events;

public class EventBus : IEventBus
{
    private readonly Dictionary<System.Type, object> _handlers = new();

    public void Subscribe<T>(Action<T> handler) where T : GameEvent
    {
        var type = typeof(T);
        if (!_handlers.TryGetValue(type, out var list))
        {
            list = new List<Action<T>>();
            _handlers[type] = list;
        }
        ((List<Action<T>>)list).Add(handler);
    }

    public void Unsubscribe<T>(Action<T> handler) where T : GameEvent
    {
        var type = typeof(T);
        if (_handlers.TryGetValue(type, out var list))
        {
            ((List<Action<T>>)list).Remove(handler);
        }
    }

    public void Publish<T>(T gameEvent) where T : GameEvent
    {
        var type = typeof(T);
        if (!_handlers.TryGetValue(type, out var list))
        {
            return;
        }
        
        var handlers = ((List<Action<T>>)list).ToArray();
        foreach (var handler in handlers)
        {
            handler(gameEvent);
        }
    }
}