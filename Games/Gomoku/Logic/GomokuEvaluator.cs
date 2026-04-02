using Core.AI;
using Core.Interfaces;
using Games.Gomoku.Models;

namespace Games.Gomoku.Logic;

/// <summary>
/// Evaluator for Gomoku AI using pattern-based scoring.
/// Implements IGameStateEvaluator for use with MinimaxAI.
/// </summary>
public class GomokuEvaluator : IGameStateEvaluator<GomokuMove>
{
    private readonly IGomokuValidator _validator;
    private const int WIN_SCORE = 100000;
    private const int FOUR_SCORE = 10000;
    private const int THREE_SCORE = 1000;
    private const int TWO_SCORE = 100;
    
    // Direction vectors for checking lines
    private static readonly (int, int)[] Directions = 
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
        var gomokuState = (GomokuAIState)state;
        
        // If game is over, return win/loss score
        if (gomokuState.IsGameOver)
        {
            if (gomokuState.Winner == 1) // Black (AI maximizes for player 1)
                return WIN_SCORE;
            else if (gomokuState.Winner == 2) // White
                return -WIN_SCORE;
            return 0; // Draw
        }
        
        return EvaluateBoard(gomokuState.Board, gomokuState.Size);
    }
    
    private int EvaluateBoard(int[,] board, int size)
    {
        int score = 0;
        
        // Evaluate for both players
        for (int player = 1; player <= 2; player++)
        {
            int playerScore = CountPatterns(board, size, player);
            if (player == 1)
                score += playerScore;
            else
                score -= playerScore;
        }
        
        // Add center preference
        int center = size / 2;
        for (int r = center - 2; r <= center + 2; r++)
        {
            for (int c = center - 2; c <= center + 2; c++)
            {
                if (r >= 0 && r < size && c >= 0 && c < size)
                {
                    if (board[r, c] == 1) score += 5;
                    else if (board[r, c] == 2) score -= 5;
                }
            }
        }
        
        return score;
    }
    
    private int CountPatterns(int[,] board, int size, int player)
    {
        int total = 0;
        
        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                foreach (var (dr, dc) in Directions)
                {
                    int count = CountLine(board, size, r, c, dr, dc, player);
                    total += PatternScore(count);
                }
            }
        }
        
        return total;
    }
    
    private int CountLine(int[,] board, int size, int row, int col, int dr, int dc, int player)
    {
        int count = 0;
        int openEnds = 0;
        
        // Check backward
        int br = row - dr, bc = col - dc;
        if (br < 0 || br >= size || bc < 0 || bc >= size || board[br, bc] == 0)
            openEnds++;
        else if (board[br, bc] == player)
            return 0; // Part of longer line counted elsewhere
        
        // Count forward
        for (int i = 0; i < 5; i++)
        {
            int r = row + i * dr, c = col + i * dc;
            if (r < 0 || r >= size || c < 0 || c >= size)
                break;
            if (board[r, c] == player)
                count++;
            else if (board[r, c] != 0)
                break; // Blocked by opponent
        }
        
        // Check forward end
        int fr = row + count * dr, fc = col + count * dc;
        if (fr >= 0 && fr < size && fc >= 0 && fc < size && board[fr, fc] == 0)
            openEnds++;
        
        // Score based on count and open ends
        if (count >= 5) return WIN_SCORE;
        if (count == 4 && openEnds >= 1) return FOUR_SCORE * openEnds;
        if (count == 3 && openEnds >= 1) return THREE_SCORE * openEnds;
        if (count == 2 && openEnds == 2) return TWO_SCORE;
        
        return count;
    }
    
    private int PatternScore(int count)
    {
        return count switch
        {
            >= 5 => WIN_SCORE,
            4 => FOUR_SCORE,
            3 => THREE_SCORE,
            2 => TWO_SCORE,
            _ => 0
        };
    }
    
    public IEnumerable<GomokuMove> GetValidMoves(IGameState state)
    {
        var gomokuState = (GomokuAIState)state;
        
        // Limit search to cells near existing stones for efficiency
        var candidates = new List<(GomokuMove Move, int Score)>();
        int searchRange = 2;
        
        for (int r = 0; r < gomokuState.Size; r++)
        {
            for (int c = 0; c < gomokuState.Size; c++)
            {
                if (gomokuState.Board[r, c] == 0 && HasNeighbor(gomokuState.Board, gomokuState.Size, r, c, searchRange))
                {
                    int score = EvaluatePosition(gomokuState.Board, gomokuState.Size, r, c);
                    candidates.Add((new GomokuMove(r, c), score));
                }
            }
        }
        
        // If no candidates (empty board), return center
        if (candidates.Count == 0)
        {
            int center = gomokuState.Size / 2;
            return new[] { new GomokuMove(center, center) };
        }
        
        // Sort by potential and take top candidates (reduce branching factor)
        return candidates
            .OrderByDescending(x => x.Score)
            .Take(10) // Limit to 10 best moves for performance
            .Select(x => x.Move);
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
    
    private int EvaluatePosition(int[,] board, int size, int row, int col)
    {
        int score = 0;
        int center = size / 2;
        
        // Center preference
        score += 10 - Math.Abs(row - center) - Math.Abs(col - center);
        
        // Check potential for each direction
        foreach (var (dr, dc) in Directions)
        {
            int count1 = 0, count2 = 0;
            for (int i = -4; i <= 4; i++)
            {
                int r = row + i * dr, c = col + i * dc;
                if (r >= 0 && r < size && c >= 0 && c < size)
                {
                    if (board[r, c] == 1) count1++;
                    else if (board[r, c] == 2) count2++;
                }
            }
            score += count1 * 2 + count2 * 2;
        }
        
        return score;
    }
    
    public IGameState ApplyMove(IGameState state, GomokuMove move)
    {
        var gomokuState = (GomokuAIState)state;
        var newBoard = (int[,])gomokuState.Board.Clone();
        
        newBoard[move.Row, move.Column] = gomokuState.CurrentPlayer;
        
        var newState = new GomokuAIState(
            newBoard,
            gomokuState.Size,
            gomokuState.CurrentPlayer == 1 ? 2 : 1
        );
        
        // Check for win
        if (_validator.CheckWin(new GomokuBoardAdapter(newBoard, gomokuState.Size), 
            move.Row, move.Column, gomokuState.CurrentPlayer))
        {
            newState.SetWinner(gomokuState.CurrentPlayer);
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
    public bool IsGameOver { get; private set; }
    public int? Winner { get; private set; }
    
    public GomokuAIState(int[,] board, int size, int currentPlayer)
    {
        Board = board;
        Size = size;
        CurrentPlayer = currentPlayer;
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
/// </summary>
public class GomokuBoardAdapter : GomokuBoard
{
    private readonly int[,] _board;
    private readonly int _size;
    
    public GomokuBoardAdapter(int[,] board, int size) : base(size)
    {
        _board = board;
        _size = size;
    }
    
    public override int GetCell(int row, int col)
    {
        if (row < 0 || row >= _size || col < 0 || col >= _size)
            return 0;
        return _board[row, col];
    }
    
    public override void SetCell(int row, int col, int value)
    {
        if (row >= 0 && row < _size && col >= 0 && col < _size)
            _board[row, col] = value;
    }
    
    public override bool IsEmpty(int row, int col)
    {
        return GetCell(row, col) == 0;
    }
    
    public override void Clear()
    {
        for (int r = 0; r < _size; r++)
            for (int c = 0; c < _size; c++)
                _board[r, c] = 0;
    }
}