using Core.Interfaces;
using Games.Minesweeper.Logic;
using Xunit;

namespace Games.Minesweeper.Tests;

public class MinesweeperGameTests
{
    private readonly MinesweeperGame _game;

    public MinesweeperGameTests()
    {
        var generator = new BoardGenerator(new Random(42));
        var revealer = new CellRevealer();
        var logic = new MinesweeperLogic(generator, revealer);
        _game = new MinesweeperGame(logic);
    }

    [Fact]
    public void Game_ShouldHaveCorrectInfo()
    {
        Assert.Equal("Minesweeper", _game.GameName);
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
