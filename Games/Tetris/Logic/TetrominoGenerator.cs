using Games.Tetris.Models;

namespace Games.Tetris.Logic;

public class TetrominoGenerator : ITetrominoGenerator
{
    private readonly Random _random;
    private Tetromino? _nextPiece;
    
    public TetrominoGenerator(Random random)
    {
        _random = random;
    }
    
    public Tetromino Generate()
    {
        if (_nextPiece == null)
            _nextPiece = GenerateRandom();
        
        var piece = _nextPiece;
        _nextPiece = GenerateRandom();
        return piece;
    }
    
    public Tetromino GenerateNext()
    {
        _nextPiece ??= GenerateRandom();
        return _nextPiece;
    }
    
    private Tetromino GenerateRandom()
    {
        var types = Enum.GetValues<TetrominoType>();
        return new Tetromino(types[_random.Next(types.Length)]);
    }
}