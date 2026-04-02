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
    
    public GomokuLogic(IGomokuValidator validator, bool useAI = false, int aiDepth = 2)
    {
        _validator = validator;
        _state = new GomokuState();
        Board = new GomokuBoard();
        
        if (useAI)
        {
            _evaluator = new GomokuEvaluator(validator);
            // Limit depth for performance (Gomoku has large board)
            _ai = new MinimaxAI<GomokuMove>(Math.Min(aiDepth, 3));
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
    /// Get AI move - uses Minimax or falls back to heuristic.
    /// </summary>
    public GomokuMove? GetAIMove()
    {
        if (_state.IsGameOver)
            return null;
        
        // Always use heuristic AI for now (Minimax is too slow for Gomoku)
        // Minimax works better for smaller games like Tic-Tac-Toe
        return GetHeuristicAIMove();
    }
    
    /// <summary>
    /// Fast heuristic-based AI move selection.
    /// </summary>
    private GomokuMove? GetHeuristicAIMove()
    {
        var candidates = new List<(int row, int col, int score)>();
        
        // Find cells near existing stones
        for (int r = 0; r < Board.Size; r++)
        {
            for (int c = 0; c < Board.Size; c++)
            {
                if (Board.IsEmpty(r, c) && HasNeighbor(r, c))
                {
                    int score = EvaluatePositionForAI(r, c);
                    candidates.Add((r, c, score));
                }
            }
        }
        
        if (candidates.Count == 0)
        {
            // First move - play center
            return new GomokuMove(7, 7);
        }
        
        // Return best position
        var best = candidates.OrderByDescending(x => x.score).First();
        return new GomokuMove(best.row, best.col);
    }
    
    /// <summary>
    /// Evaluate a position for AI move selection.
    /// </summary>
    private int EvaluatePositionForAI(int row, int col)
    {
        int score = 0;
        int opponent = CurrentPlayer == 1 ? 2 : 1;
        
        // Check all directions
        int[] dr = { 0, 1, 1, 1 };
        int[] dc = { 1, 0, 1, -1 };
        
        for (int d = 0; d < 4; d++)
        {
            int ownCount = 0, oppCount = 0;
            int ownOpenEnds = 0, oppOpenEnds = 0;
            
            // Forward direction
            for (int i = 1; i <= 4; i++)
            {
                int r = row + i * dr[d], c = col + i * dc[d];
                if (r < 0 || r >= Board.Size || c < 0 || c >= Board.Size)
                {
                    ownOpenEnds--;
                    oppOpenEnds--;
                    break;
                }
                int cell = Board.GetCell(r, c);
                if (cell == CurrentPlayer) ownCount++;
                else if (cell == opponent) { oppCount++; break; }
                else { ownOpenEnds++; break; }
            }
            
            // Backward direction
            for (int i = 1; i <= 4; i++)
            {
                int r = row - i * dr[d], c = col - i * dc[d];
                if (r < 0 || r >= Board.Size || c < 0 || c >= Board.Size)
                {
                    ownOpenEnds--;
                    oppOpenEnds--;
                    break;
                }
                int cell = Board.GetCell(r, c);
                if (cell == CurrentPlayer) ownCount++;
                else if (cell == opponent) { oppCount++; break; }
                else { ownOpenEnds++; break; }
            }
            
            // Score based on patterns
            // Own patterns (attack)
            if (ownCount >= 4) score += 1000000; // Win!
            else if (ownCount == 3 && ownOpenEnds >= 2) score += 100000; // Open 4 guaranteed win
            else if (ownCount == 3 && ownOpenEnds >= 1) score += 10000; // Can make 4
            else if (ownCount == 2 && ownOpenEnds >= 2) score += 1000; // Open 3
            else if (ownCount == 2 && ownOpenEnds >= 1) score += 100;
            else if (ownCount == 1 && ownOpenEnds >= 2) score += 50;
            
            // Opponent patterns (defense - prioritize blocking!)
            if (oppCount >= 4) score += 900000; // Must block! (slightly less than win)
            else if (oppCount == 3 && oppOpenEnds >= 2) score += 800000; // Must block open 3!
            else if (oppCount == 3 && oppOpenEnds >= 1) score += 50000; // Block 4 threat
            else if (oppCount == 2 && oppOpenEnds >= 2) score += 5000; // Block open 3
            else if (oppCount == 2 && oppOpenEnds >= 1) score += 500;
        }
        
        // Center preference
        int center = Board.Size / 2;
        score += Math.Max(0, 15 - Math.Abs(row - center) - Math.Abs(col - center));
        
        return score;
    }
    
    private bool HasNeighbor(int row, int col)
    {
        for (int dr = -2; dr <= 2; dr++)
        {
            for (int dc = -2; dc <= 2; dc++)
            {
                if (dr == 0 && dc == 0) continue;
                int r = row + dr, c = col + dc;
                if (r >= 0 && r < Board.Size && c >= 0 && c < Board.Size)
                    if (!Board.IsEmpty(r, c)) return true;
            }
        }
        return false;
    }
    
    private GomokuAIState CreateAIState()
    {
        // Clone board to int array
        var boardArray = new int[Board.Size, Board.Size];
        for (int r = 0; r < Board.Size; r++)
            for (int c = 0; c < Board.Size; c++)
                boardArray[r, c] = Board.GetCell(r, c);
        
        return new GomokuAIState(boardArray, Board.Size, CurrentPlayer);
    }
    
    /// <summary>
    /// Set AI difficulty level.
    /// </summary>
    public void SetAIDifficulty(string difficulty)
    {
        if (_ai == null) return;
        
        _ai.Difficulty = difficulty;
    }
}