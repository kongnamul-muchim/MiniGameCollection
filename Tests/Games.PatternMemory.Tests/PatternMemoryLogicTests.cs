using Games.PatternMemory.Logic;
using Xunit;

namespace Games.PatternMemory.Tests;

public class PatternMemoryLogicTests
{
    private readonly PatternGenerator _generator;
    private readonly PatternMemoryLogic _logic;

    public PatternMemoryLogicTests()
    {
        _generator = new PatternGenerator(new Random(42));
        _logic = new PatternMemoryLogic(_generator);
    }

    [Fact]
    public void StartGame_ShouldInitializeState()
    {
        _logic.StartGame();
        
        Assert.Equal(1, _logic.CurrentLevel);
        Assert.Equal(0, _logic.PlayerScore);
        Assert.Equal(3, _logic.MistakesAllowed);
        Assert.NotEmpty(_logic.CurrentPattern);
    }

    [Fact]
    public void CheckInput_CorrectPattern_ShouldAdvanceLevel()
    {
        _logic.StartGame();
        var pattern = _logic.CurrentPattern;
        
        var result = _logic.CheckInput(pattern);
        
        Assert.True(result);
        Assert.Equal(2, _logic.CurrentLevel);
    }

    [Fact]
    public void CheckInput_WrongPattern_ShouldReduceMistakes()
    {
        _logic.StartGame();
        
        var result = _logic.CheckInput(new List<int> { 9, 9, 9 });
        
        Assert.False(result);
        Assert.Equal(2, _logic.MistakesAllowed);
    }

    [Fact]
    public void CheckInput_ThreeMistakes_ShouldEndGame()
    {
        _logic.StartGame();
        
        _logic.CheckInput(new List<int> { 9, 9, 9 });
        _logic.CheckInput(new List<int> { 9, 9, 9 });
        _logic.CheckInput(new List<int> { 9, 9, 9 });
        
        Assert.True(_logic.IsGameOver);
    }

    [Fact]
    public void ResetGame_ShouldClearState()
    {
        _logic.StartGame();
        _logic.CheckInput(new List<int> { 9, 9, 9 });
        
        _logic.ResetGame();
        
        Assert.Equal(1, _logic.CurrentLevel);
        Assert.Equal(3, _logic.MistakesAllowed);
    }
}