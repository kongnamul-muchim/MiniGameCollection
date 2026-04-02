using Games.Chess.Models;

namespace Games.Chess.Logic;

public class ChessLogic
{
    private readonly ChessValidator _validator;
    private readonly ChessState _state;
    
    public ChessBoard Board { get; }
    public PieceColor CurrentColor => _state.CurrentColor;
    public bool IsGameOver => _state.IsGameOver;
    public int? Winner => _state.Winner;

    public ChessLogic()
    {
        _validator = new ChessValidator();
        _state = new ChessState();
        Board = new ChessBoard();
    }

    public void StartGame()
    {
        // Create a fresh board with initial position
        var newBoard = new ChessBoard();
        for(int r=0;r<8;r++)for(int c=0;c<8;c++)Board.Cells[r,c]=newBoard.Cells[r,c];
        _state.CurrentPlayer = 1;
        _state.IsGameOver = false;
        _state.Winner = null;
    }

    public bool MakeMove(ChessMove move)
    {
        if (_state.IsGameOver) return false;
        if (!_validator.IsValidMove(Board, move, _state.CurrentColor)) return false;
        
        Board.MovePiece(move.From.Row, move.From.Column, move.To.Row, move.To.Column);
        
        // Check for checkmate (simplified)
        var enemyColor = _state.CurrentColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
        if (_validator.IsInCheck(Board, enemyColor))
        {
            // Simplified: if in check and no moves, game over
            // Real implementation would check for all possible moves
        }
        
        _state.SwitchPlayer();
        return true;
    }

    public ChessMove? GetAIMove()
    {
        // Simple AI: random valid move
        var validMoves = new List<ChessMove>();
        
        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                if (Board.GetPiece(r, c) is { Color: var color } && color == _state.CurrentColor)
                {
                    for (int tr = 0; tr < 8; tr++)
                    {
                        for (int tc = 0; tc < 8; tc++)
                        {
                            var move = new ChessMove(new Position(r, c), new Position(tr, tc));
                            if (_validator.IsValidMove(Board, move, _state.CurrentColor))
                                validMoves.Add(move);
                        }
                    }
                }
            }
        }
        
        if (validMoves.Count == 0) return null;
        return validMoves[new Random().Next(validMoves.Count)];
    }
}