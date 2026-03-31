using Games.Tetris.Models;

namespace Games.Tetris.Logic;

public interface ITetrominoGenerator
{
    Tetromino Generate();
    Tetromino GenerateNext();
}