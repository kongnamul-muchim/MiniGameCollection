using Core.Interfaces;

namespace Core.Events;

public class GameStateChangedEvent : GameEvent
{
    public GameState PreviousState { get; }
    public GameState NewState { get; }

    public GameStateChangedEvent(GameState previousState, GameState newState) : base("GameStateChanged")
    {
        PreviousState = previousState;
        NewState = newState;
    }
}