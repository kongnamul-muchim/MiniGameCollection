using Games.Sudoku.Models;

namespace Games.Sudoku.Logic;

public interface ISudokuValidator
{
    bool IsValidMove(SudokuBoard board, int row, int col, int value);
    bool IsSolved(SudokuBoard board);
}