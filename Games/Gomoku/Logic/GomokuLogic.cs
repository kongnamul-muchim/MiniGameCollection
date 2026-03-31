using Games.Gomoku.Models;

namespace Games.Gomoku.Logic;

public class GomokuLogic
{
    private readonly IGomokuValidator _validator;
    private readonly GomokuState _state;
    
    public GomokuBoard Board { get; }
    public int CurrentPlayer => _state.CurrentPlayer;
    public bool IsGameOver => _state.IsGameOver;
    public int? Winner => _state.Winner;

    public GomokuLogic(IGomokuValidator validator)
    {
        _validator = validator;
        _state = new GomokuState();
        Board = new GomokuBoard();
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

    public GomokuMove? GetAIMove()
    {
        // Simple AI: find first empty cell near existing stones
        // For a real implementation, use MinimaxAI with GomokuEvaluator
        
        var candidates = new List<(int, int)>();
        
        // Find cells near existing stones
        for (int r = 0; r < Board.Size; r++)
        {
            for (int c = 0; c < Board.Size; c++)
            {
                if (Board.IsEmpty(r, c) && HasNeighbor(r, c))
                {
                    candidates.Add((r, c));
                }
            }
        }
        
        if (candidates.Count == 0)
        {
            // First move - play center
            return new GomokuMove(7, 7);
        }
        
        // Random selection from candidates
        var random = new Random();
        var (row, col) = candidates[random.Next(candidates.Count)];
        return new GomokuMove(row, col);
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
}