using Games.Sudoku.Models;

namespace Games.Sudoku.Logic;

public class SudokuValidator : ISudokuValidator
{
    public bool IsValidMove(SudokuBoard board, int row, int col, int value)
    {
        if (value < 1 || value > 9) return false;
        if (board.IsFixed(row, col)) return false;
        
        return !IsInRow(board, row, col, value) &&
               !IsInColumn(board, row, col, value) &&
               !IsInBox(board, row, col, value);
    }
    
    public bool IsSolved(SudokuBoard board)
    {
        for (int r = 0; r < 9; r++)
            for (int c = 0; c < 9; c++)
                if (board.Cells[r, c] == 0) return false;
        
        // Check all constraints
        for (int r = 0; r < 9; r++)
            for (int c = 0; c < 9; c++)
            {
                int value = board.Cells[r, c];
                if (HasDuplicateInRow(board, r, c, value) ||
                    HasDuplicateInColumn(board, r, c, value) ||
                    HasDuplicateInBox(board, r, c, value))
                    return false;
            }
        
        return true;
    }
    
    private bool HasDuplicateInRow(SudokuBoard board, int row, int col, int value)
    {
        for (int c = 0; c < 9; c++)
            if (c != col && board.Cells[row, c] == value) return true;
        return false;
    }
    
    private bool HasDuplicateInColumn(SudokuBoard board, int row, int col, int value)
    {
        for (int r = 0; r < 9; r++)
            if (r != row && board.Cells[r, col] == value) return true;
        return false;
    }
    
    private bool HasDuplicateInBox(SudokuBoard board, int row, int col, int value)
    {
        int boxRow = (row / 3) * 3;
        int boxCol = (col / 3) * 3;
        
        for (int r = boxRow; r < boxRow + 3; r++)
            for (int c = boxCol; c < boxCol + 3; c++)
                if (r != row && c != col && board.Cells[r, c] == value) return true;
        return false;
    }
    
    private bool IsInRow(SudokuBoard board, int row, int col, int value)
    {
        for (int c = 0; c < 9; c++)
            if (c != col && board.Cells[row, c] == value) return true;
        return false;
    }
    
    private bool IsInColumn(SudokuBoard board, int row, int col, int value)
    {
        for (int r = 0; r < 9; r++)
            if (r != row && board.Cells[r, col] == value) return true;
        return false;
    }
    
    private bool IsInBox(SudokuBoard board, int row, int col, int value)
    {
        int boxRow = (row / 3) * 3;
        int boxCol = (col / 3) * 3;
        
        for (int r = boxRow; r < boxRow + 3; r++)
            for (int c = boxCol; c < boxCol + 3; c++)
                if (r != row && c != col && board.Cells[r, c] == value) return true;
        return false;
    }
}