using Core.AI;
using Core.Interfaces;
using Games.Gomoku.Models;

namespace Games.Gomoku.Logic;

/// <summary>
/// Strong pattern-based evaluator for Gomoku AI.
/// Evaluates all lines on the board for patterns like open-4, closed-4, open-3, etc.
/// </summary>
public class GomokuEvaluator : IGameStateEvaluator<GomokuMove>
{
    private readonly IGomokuValidator _validator;
    
    // Pattern scores - carefully balanced
    private const int WIN = 10000000;
    private const int OPEN_FOUR = 1000000;    // Four with both ends open (guaranteed win)
    private const int CLOSED_FOUR = 500000;   // Four with one end open (must block!)
    private const int OPEN_THREE = 100000;    // Three with both ends open (dangerous)
    private const int CLOSED_THREE = 10000;   // Three with one end open
    private const int OPEN_TWO = 5000;        // Two with both ends open
    private const int CLOSED_TWO = 500;       // Two with one end open
    private const int SINGLE = 50;            // Single stone
    
    private static readonly (int dr, int dc)[] Directions = 
    {
        (0, 1),   // Horizontal
        (1, 0),   // Vertical
        (1, 1),   // Diagonal \
        (1, -1),  // Diagonal /
    };
    
    public GomokuEvaluator(IGomokuValidator validator)
    {
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }
    
    public int Evaluate(IGameState state)
    {
        var gs = (GomokuAIState)state;
        
        if (gs.IsGameOver)
        {
            if (gs.Winner == gs.AIColor) return WIN;
            if (gs.Winner.HasValue) return -WIN;
            return 0;
        }
        
        // Always evaluate from AI's perspective
        int aiScore = EvaluateForPlayer(gs.Board, gs.Size, gs.AIColor);
        int oppColor = gs.AIColor == 1 ? 2 : 1;
        int oppScore = EvaluateForPlayer(gs.Board, gs.Size, oppColor);
        
        return aiScore - oppScore;
    }
    
    /// <summary>
    /// Evaluate the board for a specific player.
    /// Scans all lines and counts patterns.
    /// </summary>
    private int EvaluateForPlayer(int[,] board, int size, int player)
    {
        int totalScore = 0;
        
        // Scan every cell as potential start of a line
        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                foreach (var (dr, dc) in Directions)
                {
                    // Only count lines that start at this cell (avoid double counting)
                    // Check that the cell before this is NOT the player's stone
                    int pr = r - dr, pc = c - dc;
                    if (pr >= 0 && pr < size && pc >= 0 && pc < size && board[pr, pc] == player)
                        continue; // This line will be counted from the actual start
                    
                    var pattern = AnalyzeLine(board, size, r, c, dr, dc, player);
                    totalScore += PatternToScore(pattern);
                }
            }
        }
        
        return totalScore;
    }
    
    /// <summary>
    /// Analyze a line starting at (r,c) in direction (dr,dc) for the given player.
    /// Returns (count, openEnds) where openEnds is 0, 1, or 2.
    /// </summary>
    private (int count, int openEnds) AnalyzeLine(int[,] board, int size, int r, int c, int dr, int dc, int player)
    {
        int count = 0;
        int openEnds = 0;
        
        // Check if the start is open
        int sr = r - dr, sc = c - dc;
        bool startOpen = (sr < 0 || sr >= size || sc < 0 || sc >= size || board[sr, sc] == 0);
        
        // Count consecutive stones
        int cr = r, cc = c;
        while (cr >= 0 && cr < size && cc >= 0 && cc < size && board[cr, cc] == player)
        {
            count++;
            cr += dr;
            cc += dc;
        }
        
        // Check if the end is open
        bool endOpen = (cr < 0 || cr >= size || cc < 0 || cc >= size || board[cr, cc] == 0);
        
        if (startOpen) openEnds++;
        if (endOpen) openEnds++;
        
        return (count, openEnds);
    }
    
    /// <summary>
    /// Convert a pattern (count, openEnds) to a score.
    /// </summary>
    private int PatternToScore((int count, int openEnds) pattern)
    {
        var (count, openEnds) = pattern;
        
        if (count >= 5) return WIN;
        
        if (count == 4)
        {
            if (openEnds >= 2) return OPEN_FOUR;   // Unstoppable
            if (openEnds == 1) return CLOSED_FOUR;  // Must block
            return 0; // Blocked on both sides
        }
        
        if (count == 3)
        {
            if (openEnds >= 2) return OPEN_THREE;   // Very dangerous
            if (openEnds == 1) return CLOSED_THREE;
            return 0;
        }
        
        if (count == 2)
        {
            if (openEnds >= 2) return OPEN_TWO;
            if (openEnds == 1) return CLOSED_TWO;
            return 0;
        }
        
        if (count == 1)
        {
            if (openEnds >= 2) return SINGLE;
            if (openEnds == 1) return SINGLE / 2;
            return 0;
        }
        
        return 0;
    }
    
    public IEnumerable<GomokuMove> GetValidMoves(IGameState state)
    {
        var gs = (GomokuAIState)state;
        var candidates = new List<(GomokuMove Move, int Score)>();
        
        // Search range of 2 around existing stones
        for (int r = 0; r < gs.Size; r++)
        {
            for (int c = 0; c < gs.Size; c++)
            {
                if (gs.Board[r, c] == 0 && HasNeighbor(gs.Board, gs.Size, r, c, 2))
                {
                    // Quick heuristic score for move ordering
                    int score = QuickEvaluate(gs.Board, gs.Size, r, c, gs.CurrentPlayer);
                    candidates.Add((new GomokuMove(r, c), score));
                }
            }
        }
        
        if (candidates.Count == 0)
        {
            int center = gs.Size / 2;
            return new[] { new GomokuMove(center, center) };
        }
        
        // Sort by heuristic score and take top candidates
        // For depth 2, we can afford more candidates
        return candidates
            .OrderByDescending(x => x.Score)
            .Take(15)
            .Select(x => x.Move);
    }
    
    /// <summary>
    /// Quick heuristic evaluation of placing a stone at (r,c).
    /// Used for move ordering in GetValidMoves.
    /// </summary>
    private int QuickEvaluate(int[,] board, int size, int r, int c, int player)
    {
        int score = 0;
        int opponent = player == 1 ? 2 : 1;
        
        foreach (var (dr, dc) in Directions)
        {
            // Count own stones in this direction
            int ownCount = 0, ownOpen = 0;
            int oppCount = 0, oppOpen = 0;
            
            // Forward
            for (int i = 1; i <= 4; i++)
            {
                int nr = r + i * dr, nc = c + i * dc;
                if (nr < 0 || nr >= size || nc < 0 || nc >= size) break;
                if (board[nr, nc] == player) ownCount++;
                else if (board[nr, nc] == opponent) break;
                else { ownOpen++; break; }
            }
            
            // Backward
            for (int i = 1; i <= 4; i++)
            {
                int nr = r - i * dr, nc = c - i * dc;
                if (nr < 0 || nr >= size || nc < 0 || nc >= size) break;
                if (board[nr, nc] == player) ownCount++;
                else if (board[nr, nc] == opponent) break;
                else { ownOpen++; break; }
            }
            
            // Score own patterns
            if (ownCount >= 4) score += 1000000;
            else if (ownCount == 3 && ownOpen >= 2) score += 100000;
            else if (ownCount == 3 && ownOpen >= 1) score += 10000;
            else if (ownCount == 2 && ownOpen >= 2) score += 5000;
            else if (ownCount == 2 && ownOpen >= 1) score += 500;
            
            // Count opponent stones (for blocking)
            int oppForward = 0, oppBackward = 0;
            for (int i = 1; i <= 4; i++)
            {
                int nr = r + i * dr, nc = c + i * dc;
                if (nr < 0 || nr >= size || nc < 0 || nc >= size) break;
                if (board[nr, nc] == opponent) oppForward++;
                else break;
            }
            for (int i = 1; i <= 4; i++)
            {
                int nr = r - i * dr, nc = c - i * dc;
                if (nr < 0 || nr >= size || nc < 0 || nc >= size) break;
                if (board[nr, nc] == opponent) oppBackward++;
                else break;
            }
            int totalOpp = oppForward + oppBackward;
            int oppOpenEnds = 0;
            if (r - dr >= 0 && r - dr < size && c - dc >= 0 && c - dc < size && board[r - dr, c - dc] == 0) oppOpenEnds++;
            if (r + (oppForward + 1) * dr >= 0 && r + (oppForward + 1) * dr < size && 
                c + (oppForward + 1) * dc >= 0 && c + (oppForward + 1) * dc < size && 
                board[r + (oppForward + 1) * dr, c + (oppForward + 1) * dc] == 0) oppOpenEnds++;
            
            // Score blocking opponent
            if (totalOpp >= 4) score += 900000;
            else if (totalOpp == 3 && oppOpenEnds >= 2) score += 800000;
            else if (totalOpp == 3 && oppOpenEnds >= 1) score += 50000;
            else if (totalOpp == 2 && oppOpenEnds >= 2) score += 5000;
            else if (totalOpp == 2 && oppOpenEnds >= 1) score += 500;
        }
        
        // Center preference
        int center = size / 2;
        score += Math.Max(0, 10 - Math.Abs(r - center) - Math.Abs(c - center));
        
        return score;
    }
    
    private bool HasNeighbor(int[,] board, int size, int row, int col, int range)
    {
        for (int dr = -range; dr <= range; dr++)
        {
            for (int dc = -range; dc <= range; dc++)
            {
                if (dr == 0 && dc == 0) continue;
                int r = row + dr, c = col + dc;
                if (r >= 0 && r < size && c >= 0 && c < size && board[r, c] != 0)
                    return true;
            }
        }
        return false;
    }
    
    public IGameState ApplyMove(IGameState state, GomokuMove move)
    {
        var gs = (GomokuAIState)state;
        var newBoard = (int[,])gs.Board.Clone();
        
        newBoard[move.Row, move.Column] = gs.CurrentPlayer;
        
        var newState = new GomokuAIState(
            newBoard,
            gs.Size,
            gs.CurrentPlayer == 1 ? 2 : 1,
            gs.AIColor
        );
        
        // Check for win
        if (_validator.CheckWin(new GomokuBoardAdapter(newBoard, gs.Size),
            move.Row, move.Column, gs.CurrentPlayer))
        {
            newState.SetWinner(gs.CurrentPlayer);
        }
        
        return newState;
    }
}

/// <summary>
/// AI-specific state class that includes board data.
/// </summary>
public class GomokuAIState : IGameState
{
    public int[,] Board { get; }
    public int Size { get; }
    public int CurrentPlayer { get; }
    public int AIColor { get; }
    public bool IsGameOver { get; private set; }
    public int? Winner { get; private set; }
    
    public GomokuAIState(int[,] board, int size, int currentPlayer, int aiColor)
    {
        Board = board;
        Size = size;
        CurrentPlayer = currentPlayer;
        AIColor = aiColor;
        IsGameOver = false;
        Winner = null;
    }
    
    public void SetWinner(int player)
    {
        Winner = player;
        IsGameOver = true;
    }
}

/// <summary>
/// Adapter to use int[,] board with IGomokuValidator.
/// Uses composition instead of inheritance (LSP fix).
/// </summary>
public class GomokuBoardAdapter : IGomokuBoard
{
    private readonly int[,] _board;
    private readonly int _size;
    
    public int Size => _size;
    
    public GomokuBoardAdapter(int[,] board, int size)
    {
        _board = board;
        _size = size;
    }
    
    public int GetCell(int row, int col)
    {
        if (row < 0 || row >= _size || col < 0 || col >= _size)
            return 0;
        return _board[row, col];
    }
    
    public void SetCell(int row, int col, int value)
    {
        if (row >= 0 && row < _size && col >= 0 && col < _size)
            _board[row, col] = value;
    }
    
    public bool IsEmpty(int row, int col)
    {
        return GetCell(row, col) == 0;
    }
    
    public void Clear()
    {
        for (int r = 0; r < _size; r++)
            for (int c = 0; c < _size; c++)
                _board[r, c] = 0;
    }
}
