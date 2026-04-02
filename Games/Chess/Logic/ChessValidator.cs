using Games.Chess.Models;

namespace Games.Chess.Logic;

public class ChessValidator
{
    public bool IsValidMove(ChessBoard board, ChessMove move, PieceColor color)
    {
        var piece = board.GetPiece(move.From.Row, move.From.Column);
        if (piece == null || piece.Color != color) return false;
        
        var target = board.GetPiece(move.To.Row, move.To.Column);
        if (target != null && target.Color == color) return false;
        
        return piece.Type switch
        {
            PieceType.Pawn => IsValidPawnMove(board, move, color),
            PieceType.Knight => IsValidKnightMove(move),
            PieceType.Bishop => IsValidBishopMove(board, move),
            PieceType.Rook => IsValidRookMove(board, move),
            PieceType.Queen => IsValidQueenMove(board, move),
            PieceType.King => IsValidKingMove(move),
            _ => false
        };
    }
    
    private bool IsValidPawnMove(ChessBoard board, ChessMove move, PieceColor color)
    {
        int direction = color == PieceColor.White ? -1 : 1;
        int fromRow = move.From.Row, toRow = move.To.Row;
        int fromCol = move.From.Column, toCol = move.To.Column;
        
        // Forward move
        if (fromCol == toCol)
        {
            if (toRow == fromRow + direction && board.GetPiece(toRow, toCol) == null)
                return true;
            
            // Initial double move
            bool isInitial = (color == PieceColor.White && fromRow == 6) || (color == PieceColor.Black && fromRow == 1);
            if (isInitial && toRow == fromRow + 2 * direction && 
                board.GetPiece(toRow, toCol) == null && board.GetPiece(fromRow + direction, toCol) == null)
                return true;
        }
        
        // Capture
        if (toRow == fromRow + direction && Math.Abs(toCol - fromCol) == 1)
            return board.GetPiece(toRow, toCol) != null;
        
        return false;
    }
    
    private bool IsValidKnightMove(ChessMove move)
    {
        int dr = Math.Abs(move.To.Row - move.From.Row);
        int dc = Math.Abs(move.To.Column - move.From.Column);
        return (dr == 2 && dc == 1) || (dr == 1 && dc == 2);
    }
    
    private bool IsValidBishopMove(ChessBoard board, ChessMove move)
    {
        int dr = move.To.Row - move.From.Row;
        int dc = move.To.Column - move.From.Column;
        if (Math.Abs(dr) != Math.Abs(dc)) return false;
        return IsPathClear(board, move.From, move.To, dr > 0 ? 1 : -1, dc > 0 ? 1 : -1);
    }
    
    private bool IsValidRookMove(ChessBoard board, ChessMove move)
    {
        int dr = move.To.Row - move.From.Row;
        int dc = move.To.Column - move.From.Column;
        if (dr != 0 && dc != 0) return false;
        return IsPathClear(board, move.From, move.To, dr == 0 ? 0 : (dr > 0 ? 1 : -1), dc == 0 ? 0 : (dc > 0 ? 1 : -1));
    }
    
    private bool IsValidQueenMove(ChessBoard board, ChessMove move)
    {
        return IsValidBishopMove(board, move) || IsValidRookMove(board, move);
    }
    
    private bool IsValidKingMove(ChessMove move)
    {
        int dr = Math.Abs(move.To.Row - move.From.Row);
        int dc = Math.Abs(move.To.Column - move.From.Column);
        return dr <= 1 && dc <= 1;
    }
    
    private bool IsPathClear(ChessBoard board, Position from, Position to, int dr, int dc)
    {
        int r = from.Row + dr, c = from.Column + dc;
        while (r != to.Row || c != to.Column)
        {
            if (board.GetPiece(r, c) != null) return false;
            r += dr;
            c += dc;
        }
        return true;
    }
    
    public bool IsInCheck(ChessBoard board, PieceColor color)
    {
        // Find king position
        Position? kingPos = null;
        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
                if (board.GetPiece(r, c) is { Type: PieceType.King, Color: var c1 } && c1 == color)
                    kingPos = new Position(r, c);
        
        if (kingPos == null) return false;
        
        // Check if any enemy piece can capture king
        var enemyColor = color == PieceColor.White ? PieceColor.Black : PieceColor.White;
        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
                if (board.GetPiece(r, c) is { Color: var c2 } piece && c2 == enemyColor)
                    if (IsValidMove(board, new ChessMove(new Position(r, c), kingPos.Value), enemyColor))
                        return true;
        
        return false;
    }
    
    public bool IsCheckmate(ChessBoard board, PieceColor color)
    {
        if (!IsInCheck(board, color)) return false;
        return !HasAnyLegalMove(board, color);
    }
    
    public bool IsStalemate(ChessBoard board, PieceColor color)
    {
        if (IsInCheck(board, color)) return false;
        return !HasAnyLegalMove(board, color);
    }
    
    private bool HasAnyLegalMove(ChessBoard board, PieceColor color)
    {
        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                if (board.GetPiece(r, c) is { Color: var pc } && pc == color)
                {
                    for (int tr = 0; tr < 8; tr++)
                    {
                        for (int tc = 0; tc < 8; tc++)
                        {
                            var move = new ChessMove(new Position(r, c), new Position(tr, tc));
                            if (IsValidMove(board, move, color) && !WouldBeInCheck(board, move, color))
                                return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    
    private bool WouldBeInCheck(ChessBoard board, ChessMove move, PieceColor color)
    {
        var piece = board.GetPiece(move.From.Row, move.From.Column);
        var captured = board.GetPiece(move.To.Row, move.To.Column);
        
        board.Cells[move.To.Row, move.To.Column] = piece;
        board.Cells[move.From.Row, move.From.Column] = null;
        
        bool inCheck = IsInCheck(board, color);
        
        board.Cells[move.From.Row, move.From.Column] = piece;
        board.Cells[move.To.Row, move.To.Column] = captured;
        
        return inCheck;
    }
}