namespace Core.Events;

public interface IEventBus
{
    void Subscribe<T>(Action<T> handler) where T : GameEvent;
    void Unsubscribe<T>(Action<T> handler) where T : GameEvent;
    void Publish<T>(T gameEvent) where T : GameEvent;
}