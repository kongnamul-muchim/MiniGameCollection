using Core.Events;
using Core.Interfaces;
using Xunit;

namespace Core.Tests;

public class GameStateTests
{
    [Fact]
    public void GameState_ShouldHaveExpectedValues()
    {
        Assert.Equal(0, (int)GameState.None);
        Assert.Equal(1, (int)GameState.Ready);
        Assert.Equal(2, (int)GameState.Playing);
        Assert.Equal(3, (int)GameState.Paused);
        Assert.Equal(4, (int)GameState.GameOver);
        Assert.Equal(5, (int)GameState.Victory);
    }
}

public class GameInfoTests
{
    [Fact]
    public void IGameInfo_ShouldHaveRequiredProperties()
    {
        var gameInfo = new MockGameInfo();
        Assert.NotNull(gameInfo.GameName);
        Assert.NotNull(gameInfo.GameDescription);
    }

    private class MockGameInfo : IGameInfo
    {
        public string GameName => "Test";
        public string GameDescription => "Test Description";
    }
}

public class GameControllableTests
{
    [Fact]
    public void IGameControllable_ShouldHaveRequiredMembers()
    {
        var game = new MockGameControllable();
        Assert.Equal(GameState.None, game.CurrentState);
        Assert.False(game.IsPlaying);
        Assert.False(game.IsPaused);
        
        game.StartGame();
        Assert.Equal(GameState.Playing, game.CurrentState);
    }

    private class MockGameControllable : IGameControllable
    {
        public GameState CurrentState { get; private set; } = GameState.None;
        public bool IsPlaying => CurrentState == GameState.Playing;
        public bool IsPaused => CurrentState == GameState.Paused;
        
        public void StartGame() => CurrentState = GameState.Playing;
        public void PauseGame() => CurrentState = GameState.Paused;
        public void ResumeGame() => CurrentState = GameState.Playing;
        public void ResetGame() => CurrentState = GameState.Ready;
        public void EndGame() => CurrentState = GameState.GameOver;
    }
}

public class GameSerializableTests
{
    [Fact]
    public void IGameSerializable_ShouldHaveRequiredMethods()
    {
        var serializable = new MockGameSerializable();
        var json = serializable.SerializeState();
        Assert.NotNull(json);
        serializable.DeserializeState(json);
    }

    private class MockGameSerializable : IGameSerializable
    {
        public string SerializeState() => "{}";
        public void DeserializeState(string json) { }
    }
}

public class GameEventEmitterTests
{
    [Fact]
    public void IGameEventEmitter_ShouldHaveEvent()
    {
        var emitter = new MockGameEventEmitter();
        var eventRaised = false;
        emitter.OnGameEvent += (e) => eventRaised = true;
        emitter.RaiseEvent();
        Assert.True(eventRaised);
    }

    private class TestEvent : GameEvent
    {
        public TestEvent(string type) : base(type) { }
    }

    private class MockGameEventEmitter : IGameEventEmitter
    {
        public event Action<GameEvent>? OnGameEvent;
        public void RaiseEvent() => OnGameEvent?.Invoke(new TestEvent("test"));
    }
}

public class GameTests
{
    [Fact]
    public void IGame_ShouldCombineAllInterfaces()
    {
        var game = new MockGame();
        Assert.NotNull(game.GameName);
        Assert.Equal(GameState.None, game.CurrentState);
        Assert.NotNull(game.SerializeState());
    }

    private class MockGame : IGame
    {
        public string GameName => "Test Game";
        public string GameDescription => "A test game";
        public GameState CurrentState { get; private set; } = GameState.None;
        public bool IsPlaying => CurrentState == GameState.Playing;
        public bool IsPaused => CurrentState == GameState.Paused;
        
        public event Action<GameEvent>? OnGameEvent;
        
        public void StartGame() => CurrentState = GameState.Playing;
        public void PauseGame() => CurrentState = GameState.Paused;
        public void ResumeGame() => CurrentState = GameState.Playing;
        public void ResetGame() => CurrentState = GameState.Ready;
        public void EndGame() => CurrentState = GameState.GameOver;
        public string SerializeState() => "{}";
        public void DeserializeState(string json) { }
    }
}

public class GameStateInterfaceTests
{
    [Fact]
    public void IGameState_ShouldHaveRequiredProperties()
    {
        var state = new MockGameState();
        Assert.Equal(1, state.CurrentPlayer);
        Assert.False(state.IsGameOver);
        Assert.Null(state.Winner);
    }

    private class MockGameState : IGameState
    {
        public int CurrentPlayer => 1;
        public bool IsGameOver => false;
        public int? Winner => null;
    }
}