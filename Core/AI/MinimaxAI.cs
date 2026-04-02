using Core.Interfaces;

namespace Core.AI;

public class MinimaxAI<TMove> : IAIPlayer<TMove>
{
    private readonly int _maxDepth;
    private readonly Random _random;

    public string Difficulty { get; set; } = "Normal";
    public event Action<string>? OnThinking;

    public MinimaxAI(int maxDepth = 3, Random? random = null)
    {
        _maxDepth = maxDepth;
        _random = random ?? Random.Shared;
    }

    public TMove GetBestMove(IGameState state, IGameStateEvaluator<TMove> evaluator)
    {
        var validMoves = evaluator.GetValidMoves(state).ToList();
        if (validMoves.Count == 0)
            throw new InvalidOperationException("No valid moves available");

        OnThinking?.Invoke($"Analyzing {validMoves.Count} possible moves...");

        var bestMoves = new List<TMove>();
        var bestScore = int.MinValue;
        var alpha = int.MinValue;
        var beta = int.MaxValue;

        foreach (var move in validMoves)
        {
            var newState = evaluator.ApplyMove(state, move);
            var score = Minimax(newState, evaluator, _maxDepth - 1, alpha, beta, false);

            if (score > bestScore)
            {
                bestScore = score;
                bestMoves.Clear();
                bestMoves.Add(move);
            }
            else if (score == bestScore)
            {
                bestMoves.Add(move);
            }

            alpha = Math.Max(alpha, score);
        }

        OnThinking?.Invoke($"Best move score: {bestScore}");

        return bestMoves.Count == 1 ? bestMoves[0] : bestMoves[_random.Next(bestMoves.Count)];
    }

    private int Minimax(IGameState state, IGameStateEvaluator<TMove> evaluator, 
        int depth, int alpha, int beta, bool isMaximizing)
    {
        if (depth == 0 || state.IsGameOver)
            return evaluator.Evaluate(state);

        var validMoves = evaluator.GetValidMoves(state).ToList();
        if (validMoves.Count == 0)
            return evaluator.Evaluate(state);

        if (isMaximizing)
        {
            var maxEval = int.MinValue;
            foreach (var move in validMoves)
            {
                var newState = evaluator.ApplyMove(state, move);
                var eval = Minimax(newState, evaluator, depth - 1, alpha, beta, false);
                maxEval = Math.Max(maxEval, eval);
                alpha = Math.Max(alpha, eval);
                if (beta <= alpha)
                    break;
            }
            return maxEval;
        }
        else
        {
            var minEval = int.MaxValue;
            foreach (var move in validMoves)
            {
                var newState = evaluator.ApplyMove(state, move);
                var eval = Minimax(newState, evaluator, depth - 1, alpha, beta, true);
                minEval = Math.Min(minEval, eval);
                beta = Math.Min(beta, eval);
                if (beta <= alpha)
                    break;
            }
            return minEval;
        }
    }
}