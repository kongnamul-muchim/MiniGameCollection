using Games.Tetris.Models;

namespace Games.Tetris.Logic;

public class CollisionChecker : ICollisionChecker
{
    public bool CanPlace(TetrisBoard board, Tetromino tetromino, int row, int col)
    {
        for (int r = 0; r < tetromino.Shape.GetLength(0); r++)
        {
            for (int c = 0; c < tetromino.Shape.GetLength(1); c++)
            {
                if (tetromino.Shape[r, c] == 1)
                {
                    int boardRow = row + r;
                    int boardCol = col + c;
                    
                    // Out of bounds
                    if (boardRow < 0 || boardRow >= board.Rows || 
                        boardCol < 0 || boardCol >= board.Columns)
                        return false;
                    
                    // Already filled
                    if (board.GetCell(boardRow, boardCol) != 0)
                        return false;
                }
            }
        }
        return true;
    }
}