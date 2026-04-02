namespace Games.Chess.Models;

/// <summary>
/// Interface for Chess board operations.
/// Enables composition-based adapters instead of inheritance (LSP fix).
/// </summary>
public interface IChessBoard
{
    int Size { get; }
    ChessPiece? GetPiece(int row, int col);
    void SetPiece(int row, int col, ChessPiece? piece);
}
