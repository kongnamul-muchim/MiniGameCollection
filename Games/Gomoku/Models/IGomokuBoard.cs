namespace Games.Gomoku.Models;

/// <summary>
/// Interface for Gomoku board operations.
/// Enables composition-based adapters instead of inheritance.
/// </summary>
public interface IGomokuBoard
{
    int Size { get; }
    int GetCell(int row, int col);
    void SetCell(int row, int col, int value);
    bool IsEmpty(int row, int col);
    void Clear();
}
