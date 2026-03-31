using Games.Tetris.Logic;
using Games.Tetris.Models;
using Xunit;

namespace Games.Tetris.Tests;

public class TetrisLogicTests
{
    [Fact]
    public void TetrisLogic_ShouldInitialize()
    {
        var generator = new TetrominoGenerator(new Random(42));
        var checker = new CollisionChecker();
        var clearer = new LineClearer();
        var logic = new TetrisLogic(generator, checker, clearer);
        
        logic.StartGame();
        
        Assert.NotNull(logic.Board);
        Assert.NotNull(logic.CurrentPiece);
    }

    [Fact]
    public void TetrisLogic_ShouldMovePieceDown()
    {
        var generator = new TetrominoGenerator(new Random(42));
        var checker = new CollisionChecker();
        var clearer = new LineClearer();
        var logic = new TetrisLogic(generator, checker, clearer);
        
        logic.StartGame();
        var initialRow = logic.CurrentPiece!.Row;
        
        logic.MoveDown();
        
        Assert.True(logic.CurrentPiece.Row > initialRow);
    }

    [Fact]
    public void TetrisLogic_ShouldMovePieceLeft()
    {
        var generator = new TetrominoGenerator(new Random(42));
        var checker = new CollisionChecker();
        var clearer = new LineClearer();
        var logic = new TetrisLogic(generator, checker, clearer);
        
        logic.StartGame();
        var initialCol = logic.CurrentPiece!.Column;
        
        logic.MoveLeft();
        
        Assert.True(logic.CurrentPiece.Column < initialCol);
    }

    [Fact]
    public void TetrisLogic_ShouldRotatePiece()
    {
        var generator = new TetrominoGenerator(new Random(42));
        var checker = new CollisionChecker();
        var clearer = new LineClearer();
        var logic = new TetrisLogic(generator, checker, clearer);
        
        logic.StartGame();
        
        // Store initial shape
        var initialShape = logic.CurrentPiece!.Shape;
        
        logic.Rotate();
        
        // Shape should be different after rotation
        bool same = true;
        for (int r = 0; r < 4 && same; r++)
            for (int c = 0; c < 4 && same; c++)
                if (logic.CurrentPiece.Shape[r, c] != initialShape[r, c])
                    same = false;
        
        Assert.False(same);
    }
}