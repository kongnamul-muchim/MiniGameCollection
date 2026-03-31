using Games.Tetris.Models;
using Xunit;

namespace Games.Tetris.Tests;

public class TetrominoTests
{
    [Fact]
    public void Tetromino_ShouldHaveCorrectShape()
    {
        var tetromino = new Tetromino(TetrominoType.I);
        
        Assert.Equal(4, tetromino.Shape.GetLength(0));
        Assert.Equal(4, tetromino.Shape.GetLength(1));
    }

    [Fact]
    public void Tetromino_ShouldRotateClockwise()
    {
        var tetromino = new Tetromino(TetrominoType.I);
        var rotated = tetromino.RotateClockwise();
        
        Assert.NotNull(rotated.Shape);
    }

    [Fact]
    public void Tetromino_ShouldHaveCorrectColor()
    {
        var tetromino = new Tetromino(TetrominoType.O);
        
        Assert.Equal(2, tetromino.Color);
    }
}

public class TetrisBoardTests
{
    [Fact]
    public void Board_ShouldHaveCorrectDimensions()
    {
        var board = new TetrisBoard();
        
        Assert.Equal(20, board.Rows);
        Assert.Equal(10, board.Columns);
    }

    [Fact]
    public void Board_ShouldTrackFilledCells()
    {
        var board = new TetrisBoard();
        
        board.SetCell(19, 0, 1);
        
        Assert.Equal(1, board.Cells[19, 0]);
    }

    [Fact]
    public void Board_ShouldDetectFullLine()
    {
        var board = new TetrisBoard();
        
        for (int c = 0; c < 10; c++)
            board.SetCell(19, c, 1);
        
        Assert.True(board.IsFullLine(19));
    }
}