using Core.AI;
using Core.Interfaces;
using Xunit;

namespace Core.Tests;

public class AITests
{
    private class TestMove
    {
        public int Position { get; set; }
    }

    private class TestGameState : IGameState
    {
        public int CurrentPlayer { get; set; } = 1;
        public bool IsGameOver { get; set; }
        public int? Winner { get; set; }
        public int[] Board { get; set; } = new int[9];
    }

    private class TestEvaluator : IGameStateEvaluator<TestMove>
    {
        public int Evaluate(IGameState state)
        {
            if (state is not TestGameState testState)
                return 0;

            if (testState.Winner == 1) return 100;
            if (testState.Winner == 2) return -100;
            return 0;
        }

        public IEnumerable<TestMove> GetValidMoves(IGameState state)
        {
            if (state is not TestGameState testState)
                return Enumerable.Empty<TestMove>();

            var moves = new List<TestMove>();
            for (int i = 0; i < testState.Board.Length; i++)
            {
                if (testState.Board[i] == 0)
                    moves.Add(new TestMove { Position = i });
            }
            return moves;
        }

        public IGameState ApplyMove(IGameState state, TestMove move)
        {
            if (state is not TestGameState testState)
                return state;

            var newState = new TestGameState
            {
                CurrentPlayer = testState.CurrentPlayer == 1 ? 2 : 1,
                Board = (int[])testState.Board.Clone()
            };
            newState.Board[move.Position] = testState.CurrentPlayer;

            if (CheckWin(newState.Board, testState.CurrentPlayer))
            {
                newState.IsGameOver = true;
                newState.Winner = testState.CurrentPlayer;
            }
            else if (!newState.Board.Contains(0))
            {
                newState.IsGameOver = true;
            }

            return newState;
        }

        private bool CheckWin(int[] board, int player)
        {
            int[][] winPatterns = new[]
            {
                new[] { 0, 1, 2 }, new[] { 3, 4, 5 }, new[] { 6, 7, 8 },
                new[] { 0, 3, 6 }, new[] { 1, 4, 7 }, new[] { 2, 5, 8 },
                new[] { 0, 4, 8 }, new[] { 2, 4, 6 }
            };

            return winPatterns.Any(pattern => pattern.All(pos => board[pos] == player));
        }
    }

    [Fact]
    public void MinimaxAI_ShouldReturnBestMove()
    {
        var ai = new MinimaxAI<TestMove>(3);
        var state = new TestGameState { CurrentPlayer = 1 };
        state.Board[0] = 1;
        state.Board[1] = 1;
        state.Board[3] = 2;
        state.Board[4] = 2;

        var evaluator = new TestEvaluator();
        var move = ai.GetBestMove(state, evaluator);

        Assert.Equal(2, move.Position);
    }

    [Fact]
    public void MinimaxAI_ShouldRespectDifficulty()
    {
        var aiEasy = new MinimaxAI<TestMove>(1) { Difficulty = "Easy" };
        var aiHard = new MinimaxAI<TestMove>(5) { Difficulty = "Hard" };

        Assert.Equal("Easy", aiEasy.Difficulty);
        Assert.Equal("Hard", aiHard.Difficulty);
    }

    [Fact]
    public void MinimaxAI_ShouldFireOnThinkingEvent()
    {
        var ai = new MinimaxAI<TestMove>(3);
        var state = new TestGameState { CurrentPlayer = 1 };
        var evaluator = new TestEvaluator();

        var thinkingMessages = new List<string>();
        ai.OnThinking += msg => thinkingMessages.Add(msg);

        ai.GetBestMove(state, evaluator);

        Assert.NotEmpty(thinkingMessages);
        Assert.Contains(thinkingMessages, m => m.Contains("Analyzing"));
    }

    [Fact]
    public void MinimaxAI_ShouldThrowWhenNoValidMoves()
    {
        var ai = new MinimaxAI<TestMove>(3);
        var state = new TestGameState { CurrentPlayer = 1, IsGameOver = true };
        state.Board = new int[] { 1, 2, 1, 2, 1, 2, 1, 2, 1 };
        var evaluator = new TestEvaluator();

        Assert.Throws<InvalidOperationException>(() => ai.GetBestMove(state, evaluator));
    }

    [Fact]
    public void Factory_ShouldCreateAIWithCorrectDifficulty()
    {
        var factory = new AIPlayerFactory();

        var easyAI = factory.Create<TestMove>("Easy");
        var normalAI = factory.Create<TestMove>("Normal");
        var hardAI = factory.Create<TestMove>("Hard");

        Assert.Equal("Easy", easyAI.Difficulty);
        Assert.Equal("Normal", normalAI.Difficulty);
        Assert.Equal("Hard", hardAI.Difficulty);
    }

    [Fact]
    public void Factory_ShouldUseDefaultForUnknownDifficulty()
    {
        var factory = new AIPlayerFactory();

        var ai = factory.Create<TestMove>("Unknown");

        Assert.Equal("Unknown", ai.Difficulty);
    }

    [Fact]
    public void Factory_ShouldCreateMinimaxAI()
    {
        var factory = new AIPlayerFactory();
        var ai = factory.Create<TestMove>("Normal");

        Assert.IsType<MinimaxAI<TestMove>>(ai);
    }
}