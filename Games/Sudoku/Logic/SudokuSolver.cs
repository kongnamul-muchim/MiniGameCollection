using Games.Sudoku.Models;

namespace Games.Sudoku.Logic;

public class SudokuSolver : ISudokuSolver
{
    private readonly SudokuValidator _validator;
    
    public SudokuSolver()
    {
        _validator = new SudokuValidator();
    }
    
    public bool Solve(SudokuBoard board)
    {
        // First check if the board has any duplicates (invalid puzzle)
        if (!IsValidBoard(board))
            return false;
        
        return SolveBacktrack(board);
    }
    
    private bool IsValidBoard(SudokuBoard board)
    {
        // Check for duplicates in rows
        for (int r = 0; r < 9; r++)
        {
            var seen = new HashSet<int>();
            for (int c = 0; c < 9; c++)
            {
                int val = board.Cells[r, c];
                if (val != 0 && !seen.Add(val))
                    return false;
            }
        }
        
        // Check for duplicates in columns
        for (int c = 0; c < 9; c++)
        {
            var seen = new HashSet<int>();
            for (int r = 0; r < 9; r++)
            {
                int val = board.Cells[r, c];
                if (val != 0 && !seen.Add(val))
                    return false;
            }
        }
        
        // Check for duplicates in 3x3 boxes
        for (int boxRow = 0; boxRow < 3; boxRow++)
        {
            for (int boxCol = 0; boxCol < 3; boxCol++)
            {
                var seen = new HashSet<int>();
                for (int r = boxRow * 3; r < boxRow * 3 + 3; r++)
                {
                    for (int c = boxCol * 3; c < boxCol * 3 + 3; c++)
                    {
                        int val = board.Cells[r, c];
                        if (val != 0 && !seen.Add(val))
                            return false;
                    }
                }
            }
        }
        
        return true;
    }
    
    private bool SolveBacktrack(SudokuBoard board)
    {
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                if (board.Cells[r, c] == 0)
                {
                    // Try numbers 1-9 (skip fixed cells - they already have values)
                    for (int num = 1; num <= 9; num++)
                    {
                        if (CanPlace(board, r, c, num))
                        {
                            board.SetCell(r, c, num);
                            
                            if (SolveBacktrack(board))
                                return true;
                            
                            board.SetCell(r, c, 0); // Backtrack
                        }
                    }
                    return false; // No valid number found
                }
            }
        }
        return true; // Solved!
    }
    
    private bool CanPlace(SudokuBoard board, int row, int col, int value)
    {
        // Check row
        for (int c = 0; c < 9; c++)
            if (c != col && board.Cells[row, c] == value) return false;
        
        // Check column
        for (int r = 0; r < 9; r++)
            if (r != row && board.Cells[r, col] == value) return false;
        
        // Check 3x3 box
        int boxRow = (row / 3) * 3;
        int boxCol = (col / 3) * 3;
        for (int r = boxRow; r < boxRow + 3; r++)
            for (int c = boxCol; c < boxCol + 3; c++)
                if (r != row && c != col && board.Cells[r, c] == value) return false;
        
        return true;
    }
}