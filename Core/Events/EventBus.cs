using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Core.Events;

public class EventBus : IEventBus
{
    private readonly ConcurrentDictionary<System.Type, List<System.Delegate>> _handlers = new();

    public void Subscribe<T>(Action<T> handler) where T : GameEvent
    {
        if (handler == null)
            return;

        var type = typeof(T);
        if (!_handlers.TryGetValue(type, out var list))
        {
            list = new List<System.Delegate>();
            _handlers.TryAdd(type, list);
        }
        lock (list)
        {
            list.Add(handler);
        }
    }

    public void Unsubscribe<T>(Action<T> handler) where T : GameEvent
    {
        if (handler == null)
            return;

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
                try
                {
                    (handler as Action<T>)?.Invoke(gameEvent);
                }
                catch
                {
                }
            }
        }
    }
}