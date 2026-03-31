using Core.Interfaces;
using Games.Sudoku.Logic;
using Xunit;

namespace Games.Sudoku.Tests;

public class SudokuGameTests
{
    private readonly SudokuGame _game;

    public SudokuGameTests()
    {
        var generator = new SudokuGenerator(new Random(42));
        var validator = new SudokuValidator();
        var logic = new SudokuLogic(generator, validator);
        _game = new SudokuGame(logic);
    }

    [Fact]
    public void Game_ShouldHaveCorrectInfo()
    {
        Assert.Equal("Sudoku", _game.GameName);
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