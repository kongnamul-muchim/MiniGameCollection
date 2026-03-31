namespace Core.AI;

public class AIPlayerFactory : IAIPlayerFactory
{
    private readonly Dictionary<string, int> _difficultyDepths = new()
    {
        { "Easy", 1 },
        { "Normal", 3 },
        { "Hard", 5 }
    };

    public IAIPlayer<TMove> Create<TMove>(string difficulty)
    {
        int depth = _difficultyDepths.TryGetValue(difficulty, out var d) ? d : 3;
        var ai = new MinimaxAI<TMove>(depth);
        ai.Difficulty = difficulty;
        return ai;
    }
}