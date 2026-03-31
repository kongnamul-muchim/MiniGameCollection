using Games.Tetris.Models;

namespace Games.Tetris.Logic;

public class TetrisLogic
{
    private readonly ITetrominoGenerator _generator;
    private readonly ICollisionChecker _checker;
    private readonly ILineClearer _clearer;
    private readonly TetrisState _state;
    
    public TetrisBoard? Board { get; private set; }
    public Tetromino? CurrentPiece { get; private set; }
    public Tetromino? NextPiece { get; private set; }
    public bool IsGameOver => _state.IsGameOver;

    public TetrisLogic(ITetrominoGenerator generator, ICollisionChecker checker, ILineClearer clearer)
    {
        _generator = generator;
        _checker = checker;
        _clearer = clearer;
        _state = new TetrisState();
    }

    public void StartGame()
    {
        _state.Score = 0;
        _state.LinesCleared = 0;
        _state.Level = 1;
        _state.GameOver = false;
        _state.IsVictory = false;
        
        Board = new TetrisBoard();
        SpawnPiece();
    }

    private void SpawnPiece()
    {
        CurrentPiece = _generator.Generate();
        NextPiece = _generator.GenerateNext();
        
        if (!_checker.CanPlace(Board!, CurrentPiece, CurrentPiece.Row, CurrentPiece.Column))
            _state.SetGameOver();
    }

    public bool MoveDown()
    {
        if (CurrentPiece == null || Board == null) return false;
        
        if (_checker.CanPlace(Board, CurrentPiece, CurrentPiece.Row + 1, CurrentPiece.Column))
        {
            CurrentPiece.Row++;
            return true;
        }
        
        // Lock piece
        LockPiece();
        return false;
    }

    public bool MoveLeft()
    {
        if (CurrentPiece == null || Board == null) return false;
        
        if (_checker.CanPlace(Board, CurrentPiece, CurrentPiece.Row, CurrentPiece.Column - 1))
        {
            CurrentPiece.Column--;
            return true;
        }
        return false;
    }

    public bool MoveRight()
    {
        if (CurrentPiece == null || Board == null) return false;
        
        if (_checker.CanPlace(Board, CurrentPiece, CurrentPiece.Row, CurrentPiece.Column + 1))
        {
            CurrentPiece.Column++;
            return true;
        }
        return false;
    }

    public bool Rotate()
    {
        if (CurrentPiece == null || Board == null) return false;
        
        var rotated = CurrentPiece.RotateClockwise();
        if (_checker.CanPlace(Board, rotated, rotated.Row, rotated.Column))
        {
            CurrentPiece = rotated;
            return true;
        }
        return false;
    }

    private void LockPiece()
    {
        if (CurrentPiece == null || Board == null) return;
        
        // Copy piece to board
        for (int r = 0; r < CurrentPiece.Shape.GetLength(0); r++)
        {
            for (int c = 0; c < CurrentPiece.Shape.GetLength(1); c++)
            {
                if (CurrentPiece.Shape[r, c] == 1)
                {
                    int boardRow = CurrentPiece.Row + r;
                    int boardCol = CurrentPiece.Column + c;
                    if (boardRow >= 0 && boardRow < Board.Rows && boardCol >= 0 && boardCol < Board.Columns)
                        Board.SetCell(boardRow, boardCol, CurrentPiece.Color);
                }
            }
        }
        
        // Clear lines
        int lines = _clearer.ClearLines(Board);
        if (lines > 0)
        {
            _state.AddScore(lines * 100 * _state.Level);
            _state.IncrementLines();
            
            if (_state.LinesCleared % 10 == 0)
                _state.IncrementLevel();
        }
        
        // Spawn next piece
        SpawnPiece();
    }

    public int GetScore() => _state.Score;
    public int GetLevel() => _state.Level;
    public int GetLinesCleared() => _state.LinesCleared;
}