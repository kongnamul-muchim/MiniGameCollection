namespace Games.Chess.Models;

public class ChessBoard : IChessBoard
{
    public ChessPiece?[,] Cells { get; }
    public int Size => 8;
    
    public ChessBoard()
    {
        Cells = new ChessPiece?[8, 8];
        SetupInitialPosition();
    }
    
    private void SetupInitialPosition()
    {
        // White pieces (bottom)
        Cells[7, 0] = new ChessPiece(PieceType.Rook, PieceColor.White);
        Cells[7, 1] = new ChessPiece(PieceType.Knight, PieceColor.White);
        Cells[7, 2] = new ChessPiece(PieceType.Bishop, PieceColor.White);
        Cells[7, 3] = new ChessPiece(PieceType.Queen, PieceColor.White);
        Cells[7, 4] = new ChessPiece(PieceType.King, PieceColor.White);
        Cells[7, 5] = new ChessPiece(PieceType.Bishop, PieceColor.White);
        Cells[7, 6] = new ChessPiece(PieceType.Knight, PieceColor.White);
        Cells[7, 7] = new ChessPiece(PieceType.Rook, PieceColor.White);
        for (int c = 0; c < 8; c++)
            Cells[6, c] = new ChessPiece(PieceType.Pawn, PieceColor.White);
        
        // Black pieces (top)
        Cells[0, 0] = new ChessPiece(PieceType.Rook, PieceColor.Black);
        Cells[0, 1] = new ChessPiece(PieceType.Knight, PieceColor.Black);
        Cells[0, 2] = new ChessPiece(PieceType.Bishop, PieceColor.Black);
        Cells[0, 3] = new ChessPiece(PieceType.Queen, PieceColor.Black);
        Cells[0, 4] = new ChessPiece(PieceType.King, PieceColor.Black);
        Cells[0, 5] = new ChessPiece(PieceType.Bishop, PieceColor.Black);
        Cells[0, 6] = new ChessPiece(PieceType.Knight, PieceColor.Black);
        Cells[0, 7] = new ChessPiece(PieceType.Rook, PieceColor.Black);
        for (int c = 0; c < 8; c++)
            Cells[1, c] = new ChessPiece(PieceType.Pawn, PieceColor.Black);
    }
    
    public virtual ChessPiece? GetPiece(int row, int col) => Cells[row, col];
    
    public virtual void SetPiece(int row, int col, ChessPiece? piece)
    {
        if (row >= 0 && row < 8 && col >= 0 && col < 8)
            Cells[row, col] = piece;
    }
    
    public void MovePiece(int fromRow, int fromCol, int toRow, int toCol)
    {
        Cells[toRow, toCol] = Cells[fromRow, fromCol];
        Cells[fromRow, fromCol] = null;
    }
    
    public void Clear() => Array.Clear(Cells, 0, Cells.Length);
}