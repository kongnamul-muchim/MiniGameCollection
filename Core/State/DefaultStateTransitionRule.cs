using Core.Interfaces;

namespace Core.State;

public class DefaultStateTransitionRule : IStateTransitionRule
{
    private static readonly HashSet<(GameState From, GameState To)> ValidTransitions = new()
    {
        (GameState.None, GameState.Ready),
        (GameState.Ready, GameState.Playing),
        (GameState.Playing, GameState.Paused),
        (GameState.Playing, GameState.GameOver),
        (GameState.Playing, GameState.Victory),
        (GameState.Playing, GameState.Ready),
        (GameState.Paused, GameState.Playing),
        (GameState.Paused, GameState.Ready),
        (GameState.GameOver, GameState.Ready),
        (GameState.Victory, GameState.Ready)
    };

    public bool CanTransition(GameState from, GameState to)
    {
        return ValidTransitions.Contains((from, to));
    }
}