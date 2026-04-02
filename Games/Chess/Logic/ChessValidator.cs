using Games.Chess.Models;

namespace Games.Chess.Logic;

public class ChessValidator
{
    public bool IsValidMove(ChessBoard board, ChessMove move, PieceColor color, ChessState? state = null)
    {
        var piece = board.GetPiece(move.From.Row, move.From.Column);
        if (piece == null || piece.Color != color) return false;
        
        var target = board.GetPiece(move.To.Row, move.To.Column);
        if (target != null && target.Color == color) return false;
        
        // Check castling
        if (piece.Type == PieceType.King && move.IsCastling)
        {
            return IsValidCastling(board, move, color, state);
        }
        
        // Check en passant
        if (piece.Type == PieceType.Pawn && move.IsEnPassant)
        {
            return IsValidEnPassant(board, move, color, state);
        }
        
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
    
    private bool IsValidCastling(ChessBoard board, ChessMove move, PieceColor color, ChessState? state)
    {
        if (state == null) return false;
        
        int kingRow = color == PieceColor.White ? 7 : 0;
        if (move.From.Row != kingRow || move.From.Column != 4) return false;
        
        // King must not be in check
        if (IsInCheck(board, color)) return false;
        
        bool kingSide = move.To.Column == 6;
        bool queenSide = move.To.Column == 2;
        
        if (!kingSide && !queenSide) return false;
        
        // Check castling rights
        if (color == PieceColor.White)
        {
            if (kingSide && !state.WhiteKingSideCastle) return false;
            if (queenSide && !state.WhiteQueenSideCastle) return false;
        }
        else
        {
            if (kingSide && !state.BlackKingSideCastle) return false;
            if (queenSide && !state.BlackQueenSideCastle) return false;
        }
        
        // Check path is clear
        if (kingSide)
        {
            if (board.GetPiece(kingRow, 5) != null || board.GetPiece(kingRow, 6) != null) return false;
            // King must not pass through check
            if (IsSquareAttacked(board, kingRow, 5, color)) return false;
            if (IsSquareAttacked(board, kingRow, 6, color)) return false;
        }
        else // queenSide
        {
            if (board.GetPiece(kingRow, 1) != null || board.GetPiece(kingRow, 2) != null || board.GetPiece(kingRow, 3) != null) return false;
            if (IsSquareAttacked(board, kingRow, 3, color)) return false;
            if (IsSquareAttacked(board, kingRow, 2, color)) return false;
        }
        
        return true;
    }
    
    private bool IsValidEnPassant(ChessBoard board, ChessMove move, PieceColor color, ChessState? state)
    {
        if (state == null) return false;
        if (state.EnPassantTarget == null) return false;
        
        var target = state.EnPassantTarget.Value;
        return move.To.Row == target.Row && move.To.Column == target.Column;
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
        Position? kingPos = null;
        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
                if (board.GetPiece(r, c) is { Type: PieceType.King, Color: var c1 } && c1 == color)
                    kingPos = new Position(r, c);
        
        if (kingPos == null) return false;
        
        return IsSquareAttacked(board, kingPos.Value.Row, kingPos.Value.Column, color);
    }
    
    public bool IsSquareAttacked(ChessBoard board, int row, int col, PieceColor defendingColor)
    {
        var attackingColor = defendingColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
        
        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                var piece = board.GetPiece(r, c);
                if (piece == null || piece.Color != attackingColor) continue;
                
                var move = new ChessMove(new Position(r, c), new Position(row, col));
                
                bool canAttack = piece.Type switch
                {
                    PieceType.Pawn => IsValidPawnCapture(board, move, attackingColor),
                    PieceType.Knight => IsValidKnightMove(move),
                    PieceType.Bishop => IsValidBishopMove(board, move),
                    PieceType.Rook => IsValidRookMove(board, move),
                    PieceType.Queen => IsValidQueenMove(board, move),
                    PieceType.King => IsValidKingMove(move),
                    _ => false
                };
                
                if (canAttack) return true;
            }
        }
        
        return false;
    }
    
    private bool IsValidPawnCapture(ChessBoard board, ChessMove move, PieceColor color)
    {
        int direction = color == PieceColor.White ? -1 : 1;
        int fromRow = move.From.Row, toRow = move.To.Row;
        int fromCol = move.From.Column, toCol = move.To.Column;
        
        return toRow == fromRow + direction && Math.Abs(toCol - fromCol) == 1;
    }
    
    public bool IsCheckmate(ChessBoard board, PieceColor color, ChessState state)
    {
        if (!IsInCheck(board, color)) return false;
        return !HasAnyLegalMove(board, color, state);
    }
    
    public bool IsStalemate(ChessBoard board, PieceColor color, ChessState state)
    {
        if (IsInCheck(board, color)) return false;
        return !HasAnyLegalMove(board, color, state);
    }
    
    private bool HasAnyLegalMove(ChessBoard board, PieceColor color, ChessState state)
    {
        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                var piece = board.GetPiece(r, c);
                if (piece == null || piece.Color != color) continue;
                
                // Normal moves
                for (int tr = 0; tr < 8; tr++)
                {
                    for (int tc = 0; tc < 8; tc++)
                    {
                        var move = new ChessMove(new Position(r, c), new Position(tr, tc));
                        if (IsValidMove(board, move, color, state) && !WouldBeInCheck(board, move, color, state))
                            return true;
                    }
                }
                
                // Castling moves
                if (piece.Type == PieceType.King)
                {
                    // King-side
                    var ksMove = new ChessMove(new Position(r, c), new Position(r, 6), isCastling: true);
                    if (IsValidMove(board, ksMove, color, state) && !WouldBeInCheck(board, ksMove, color, state))
                        return true;
                    
                    // Queen-side
                    var qsMove = new ChessMove(new Position(r, c), new Position(r, 2), isCastling: true);
                    if (IsValidMove(board, qsMove, color, state) && !WouldBeInCheck(board, qsMove, color, state))
                        return true;
                }
                
                // En passant
                if (piece.Type == PieceType.Pawn && state.EnPassantTarget.HasValue)
                {
                    var epMove = new ChessMove(new Position(r, c), state.EnPassantTarget.Value, isEnPassant: true);
                    if (IsValidMove(board, epMove, color, state) && !WouldBeInCheck(board, epMove, color, state))
                        return true;
                }
            }
        }
        return false;
    }
    
    private bool WouldBeInCheck(ChessBoard board, ChessMove move, PieceColor color, ChessState state)
    {
        var piece = board.GetPiece(move.From.Row, move.From.Column);
        var captured = board.GetPiece(move.To.Row, move.To.Column);
        
        // Handle en passant capture
        ChessPiece? epCaptured = null;
        Position? epPos = null;
        if (move.IsEnPassant)
        {
            epPos = new Position(move.From.Row, move.To.Column);
            epCaptured = board.GetPiece(epPos.Value.Row, epPos.Value.Column);
            board.Cells[epPos.Value.Row, epPos.Value.Column] = null;
        }
        
        board.Cells[move.To.Row, move.To.Column] = piece;
        board.Cells[move.From.Row, move.From.Column] = null;
        
        bool inCheck = IsInCheck(board, color);
        
        board.Cells[move.From.Row, move.From.Column] = piece;
        board.Cells[move.To.Row, move.To.Column] = captured;
        
        if (epPos.HasValue)
            board.Cells[epPos.Value.Row, epPos.Value.Column] = epCaptured;
        
        return inCheck;
    }
}
