using Core.Interfaces;

namespace Core.AI;

public interface IGameStateEvaluator<TMove>
{
    int Evaluate(IGameState state);
    IEnumerable<TMove> GetValidMoves(IGameState state);
    IGameState ApplyMove(IGameState state, TMove move);
}