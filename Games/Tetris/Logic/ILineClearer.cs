using Games.Tetris.Models;

namespace Games.Tetris.Logic;

public interface ILineClearer
{
    int ClearLines(TetrisBoard board);
}