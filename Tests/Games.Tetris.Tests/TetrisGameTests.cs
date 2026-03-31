using Core.Interfaces;
using Games.Tetris.Logic;
using Games.Tetris.Models;
using Xunit;

namespace Games.Tetris.Tests;

public class TetrisGameTests
{
    private readonly TetrisGame _game;

    public TetrisGameTests()
    {
        var generator = new TetrominoGenerator(new Random(42));
        var checker = new CollisionChecker();
        var clearer = new LineClearer();
        var logic = new TetrisLogic(generator, checker, clearer);
        _game = new TetrisGame(logic);
    }

    [Fact]
    public void Game_ShouldHaveCorrectInfo()
    {
        Assert.Equal("Tetris", _game.GameName);
        Assert.NotEmpty(_game.GameDescription);
    }

    [Fact]
    public void StartGame_ShouldInitialize()
    {
        _game.StartGame();
        
        Assert.Equal(GameState.Playing, _game.CurrentState);
    }

    [Fact]
    public void PauseGame_ShouldPause()
    {
        _game.StartGame();
        _game.PauseGame();
        
        Assert.True(_game.IsPaused);
    }

    [Fact]
    public void ResumeGame_ShouldResume()
    {
        _game.StartGame();
        _game.PauseGame();
        _game.ResumeGame();
        
        Assert.True(_game.IsPlaying);
    }
}