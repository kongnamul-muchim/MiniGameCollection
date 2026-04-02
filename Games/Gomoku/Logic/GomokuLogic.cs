using Core.AI;
using Games.Gomoku.Models;

namespace Games.Gomoku.Logic;

public class GomokuLogic
{
    private readonly IGomokuValidator _validator;
    private readonly GomokuState _state;
    private readonly MinimaxAI<GomokuMove>? _ai;
    private readonly GomokuEvaluator? _evaluator;
    
    public GomokuBoard Board { get; }
    public int CurrentPlayer => _state.CurrentPlayer;
    public bool IsGameOver => _state.IsGameOver;
    public int? Winner => _state.Winner;
    public bool HasAI => _ai != null;
    public string? AIDifficulty => _ai?.Difficulty;
    public bool AIIsBlack { get; private set; } = true; // AI starts as Black by default
    
    public GomokuLogic(IGomokuValidator validator, bool useAI = false, int aiDepth = 2)
    {
        _validator = validator;
        _state = new GomokuState();
        Board = new GomokuBoard();
        
        if (useAI)
        {
            _evaluator = new GomokuEvaluator(validator);
            _ai = new MinimaxAI<GomokuMove>(aiDepth);
            _ai.Difficulty = aiDepth switch
            {
                1 => "Easy",
                2 => "Normal",
                3 => "Hard",
                _ => "Normal"
            };
        }
    }

    public void StartGame()
    {
        Board.Clear();
        _state.CurrentPlayer = 1;
        _state.IsGameOver = false;
        _state.Winner = null;
        _state.MovesPlayed = 0;
        
        // Random first player for AI games
        if (HasAI)
        {
            AIIsBlack = new Random().Next(2) == 0;
            // AI is Black (Player 1) = AI goes first
            // AI is White (Player 2) = Human goes first
        }
    }

    public bool PlaceStone(int row, int col)
    {
        if (_state.IsGameOver) return false;
        if (!_validator.IsValidMove(Board, row, col)) return false;
        
        Board.SetCell(row, col, _state.CurrentPlayer);
        _state.IncrementMoves();
        
        if (_validator.CheckWin(Board, row, col, _state.CurrentPlayer))
        {
            _state.SetWinner(_state.CurrentPlayer);
            return true;
        }
        
        _state.SwitchPlayer();
        return true;
    }
    
    /// <summary>
    /// Get AI move using Minimax with depth 2.
    /// Falls back to heuristic if AI not available.
    /// </summary>
    public GomokuMove? GetAIMove()
    {
        if (_state.IsGameOver)
            return null;
        
        // Try Minimax AI if available
        if (_ai != null && _evaluator != null)
        {
            try
            {
                var aiState = CreateAIState();
                return _ai.GetBestMove(aiState, _evaluator);
            }
            catch
            {
                // Fallback to heuristic
            }
        }
        
        // Fallback: play center or first empty near stones
        return GetFallbackMove();
    }
    
    /// <summary>
    /// Check if it's currently AI's turn.
    /// </summary>
    public bool IsAITurn()
    {
        if (!HasAI || _state.IsGameOver) return false;
        // AI is Black (1) or White (2)
        int aiPlayer = AIIsBlack ? 1 : 2;
        return _state.CurrentPlayer == aiPlayer;
    }
    
    private GomokuMove? GetFallbackMove()
    {
        int center = Board.Size / 2;
        if (Board.IsEmpty(center, center))
            return new GomokuMove(center, center);
        
        for (int r = 0; r < Board.Size; r++)
            for (int c = 0; c < Board.Size; c++)
                if (Board.IsEmpty(r, c) && HasNeighbor(r, c))
                    return new GomokuMove(r, c);
        
        return null;
    }
    
    private bool HasNeighbor(int row, int col)
    {
        for (int dr = -2; dr <= 2; dr++)
            for (int dc = -2; dc <= 2; dc++)
            {
                if (dr == 0 && dc == 0) continue;
                int r = row + dr, c = col + dc;
                if (r >= 0 && r < Board.Size && c >= 0 && c < Board.Size && !Board.IsEmpty(r, c))
                    return true;
            }
        return false;
    }
    
    private GomokuAIState CreateAIState()
    {
        var boardArray = new int[Board.Size, Board.Size];
        for (int r = 0; r < Board.Size; r++)
            for (int c = 0; c < Board.Size; c++)
                boardArray[r, c] = Board.GetCell(r, c);
        
        int aiColor = AIIsBlack ? 1 : 2;
        return new GomokuAIState(boardArray, Board.Size, CurrentPlayer, aiColor);
    }
    
    public void SetAIDifficulty(string difficulty)
    {
        if (_ai == null) return;
        _ai.Difficulty = difficulty;
    }
}
