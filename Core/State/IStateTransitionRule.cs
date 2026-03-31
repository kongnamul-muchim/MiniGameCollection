using Core.Interfaces;

namespace Core.State;

public interface IStateTransitionRule
{
    bool CanTransition(GameState from, GameState to);
}