using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Core.Events;

public class EventBus : IEventBus
{
    private readonly ConcurrentDictionary<System.Type, List<System.Delegate>> _handlers = new();

    public void Subscribe<T>(Action<T> handler) where T : GameEvent
    {
        var type = typeof(T);
        var list = _handlers.GetOrAdd(type, _ => new List<System.Delegate>());
        lock (list)
        {
            list.Add(handler);
        }
    }

    public void Unsubscribe<T>(Action<T> handler) where T : GameEvent
    {
        var type = typeof(T);
        if (_handlers.TryGetValue(type, out var list))
        {
            lock (list)
            {
                list.Remove(handler);
            }
        }
    }

    public void Publish<T>(T gameEvent) where T : GameEvent
    {
        var type = typeof(T);
        if (_handlers.TryGetValue(type, out var list))
        {
            System.Delegate[] handlersCopy;
            lock (list)
            {
                handlersCopy = list.ToArray();
            }
            foreach (var handler in handlersCopy)
            {
                (handler as Action<T>)?.Invoke(gameEvent);
            }
        }
    }
}