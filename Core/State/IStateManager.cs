using Core.Interfaces;

namespace Core.State;

public interface IStateManager
{
    GameState CurrentState { get; }
    event Action<GameState, GameState>? OnStateChanged;
    void ChangeState(GameState newState);
    bool CanTransitionTo(GameState target);
}