using Core.Events;

namespace Core.Interfaces;

public interface IGameEventEmitter
{
    event Action<GameEvent>? OnGameEvent;
}