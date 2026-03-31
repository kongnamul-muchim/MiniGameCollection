namespace Games.Tetris.Models;

public class Tetromino
{
    public int[,] Shape { get; set; }
    public int Color { get; }
    public TetrominoType Type { get; }
    public int Row { get; set; }
    public int Column { get; set; }
    
    private static readonly Dictionary<TetrominoType, int[,]> Shapes = new()
    {
        { TetrominoType.I, new int[,] {
            {0,0,0,0},
            {1,1,1,1},
            {0,0,0,0},
            {0,0,0,0} }},
        { TetrominoType.O, new int[,] {
            {1,1},
            {1,1} }},
        { TetrominoType.T, new int[,] {
            {0,1,0},
            {1,1,1},
            {0,0,0} }},
        { TetrominoType.S, new int[,] {
            {0,1,1},
            {1,1,0},
            {0,0,0} }},
        { TetrominoType.Z, new int[,] {
            {1,1,0},
            {0,1,1},
            {0,0,0} }},
        { TetrominoType.J, new int[,] {
            {1,0,0},
            {1,1,1},
            {0,0,0} }},
        { TetrominoType.L, new int[,] {
            {0,0,1},
            {1,1,1},
            {0,0,0} }}
    };
    
    private static readonly Dictionary<TetrominoType, int> Colors = new()
    {
        { TetrominoType.I, 1 }, { TetrominoType.O, 2 }, { TetrominoType.T, 3 },
        { TetrominoType.S, 4 }, { TetrominoType.Z, 5 }, { TetrominoType.J, 6 },
        { TetrominoType.L, 7 }
    };
    
    public Tetromino(TetrominoType type)
    {
        Type = type;
        Shape = (int[,])Shapes[type].Clone();
        Color = Colors[type];
        Row = 0;
        Column = 3;
    }
    
    public Tetromino RotateClockwise()
    {
        var rotated = new Tetromino(Type);
        int n = Shape.GetLength(0);
        rotated.Shape = new int[n, n];
        
        for (int r = 0; r < n; r++)
            for (int c = 0; c < n; c++)
                rotated.Shape[c, n - 1 - r] = Shape[r, c];
        
        rotated.Row = Row;
        rotated.Column = Column;
        return rotated;
    }
    
    public Tetromino RotateCounterClockwise()
    {
        var rotated = new Tetromino(Type);
        int n = Shape.GetLength(0);
        rotated.Shape = new int[n, n];
        
        for (int r = 0; r < n; r++)
            for (int c = 0; c < n; c++)
                rotated.Shape[n - 1 - c, r] = Shape[r, c];
        
        rotated.Row = Row;
        rotated.Column = Column;
        return rotated;
    }
}