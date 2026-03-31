using Core.Interfaces;

namespace Core.AI;

public interface IAIPlayer<TMove>
{
    string Difficulty { get; set; }
    TMove GetBestMove(IGameState state, IGameStateEvaluator<TMove> evaluator);
    event Action<string>? OnThinking;
}