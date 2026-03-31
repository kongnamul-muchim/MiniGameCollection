using Games.Tetris.Models;

namespace Games.Tetris.Logic;

public interface ICollisionChecker
{
    bool CanPlace(TetrisBoard board, Tetromino tetromino, int row, int col);
}