using Games.Tetris.Models;

namespace Games.Tetris.Logic;

public class LineClearer : ILineClearer
{
    public int ClearLines(TetrisBoard board)
    {
        int linesCleared = 0;
        
        for (int r = board.Rows - 1; r >= 0; r--)
        {
            if (board.IsFullLine(r))
            {
                ClearAndShift(board, r);
                linesCleared++;
                r++; // Check same row again
            }
        }
        
        return linesCleared;
    }
    
    private void ClearAndShift(TetrisBoard board, int lineRow)
    {
        // Shift all rows above down
        for (int r = lineRow; r > 0; r--)
        {
            for (int c = 0; c < board.Columns; c++)
                board.Cells[r, c] = board.Cells[r - 1, c];
        }
        
        // Clear top row
        board.ClearLine(0);
    }
}