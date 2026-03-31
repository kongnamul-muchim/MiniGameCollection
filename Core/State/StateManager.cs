using Core.Interfaces;

namespace Core.State;

public class StateManager : IStateManager
{
    private readonly IStateTransitionRule _transitionRule;
    private GameState _currentState = GameState.None;

    public GameState CurrentState => _currentState;
    
    public event Action<GameState, GameState>? OnStateChanged;

    public StateManager(IStateTransitionRule transitionRule)
    {
        _transitionRule = transitionRule ?? throw new ArgumentNullException(nameof(transitionRule));
    }

    public bool CanTransitionTo(GameState target)
    {
        return _transitionRule.CanTransition(_currentState, target);
    }

    public void ChangeState(GameState newState)
    {
        if (!CanTransitionTo(newState))
            return;

        var previousState = _currentState;
        _currentState = newState;
        OnStateChanged?.Invoke(previousState, newState);
    }
}