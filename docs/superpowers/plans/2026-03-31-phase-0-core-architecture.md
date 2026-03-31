# Phase 0: Core Architecture Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build the foundational Core module with all shared interfaces, state management, event system, save system, DI container, and AI module - all Unity/Blazor independent.

**Architecture:** Pure C# library with SOLID principles and dependency injection. All components are interface-driven, allowing any frontend (Unity, Blazor, etc.) to consume without modification.

**Tech Stack:** C# 10, .NET 6+, System.Text.Json, xUnit for testing

---

## File Structure

```
Core/
├── Interfaces/
│   ├── IGameInfo.cs           # 게임 정보 인터페이스
│   ├── IGameControllable.cs   # 게임 제어 인터페이스
│   ├── IGameSerializable.cs   # 게임 저장/로드 인터페이스
│   ├── IGameEventEmitter.cs   # 이벤트 발행 인터페이스
│   ├── IGame.cs               # 통합 게임 인터페이스
│   ├── IGameState.cs          # AI용 게임 상태 인터페이스
│   └── GameState.cs           # 게임 상태 enum
│
├── State/
│   ├── IStateManager.cs       # 상태 관리자 인터페이스
│   ├── StateManager.cs        # 상태 관리자 구현
│   ├── IStateTransitionRule.cs # 상태 전환 규칙 인터페이스
│   └── DefaultStateTransitionRule.cs # 기본 전환 규칙
│
├── Events/
│   ├── GameEvent.cs           # 이벤트 기본 클래스
│   ├── IEventBus.cs           # 이벤트 버스 인터페이스
│   ├── EventBus.cs            # 이벤트 버스 구현
│   ├── ScoreChangedEvent.cs   # 점수 변경 이벤트
│   ├── GameOverEvent.cs       # 게임 종료 이벤트
│   └── GameStateChangedEvent.cs # 상태 변경 이벤트
│
├── Data/
│   ├── ISerializer.cs         # 직렬화 인터페이스
│   ├── ISaveStorage.cs        # 저장소 인터페이스 (async)
│   ├── ISaveSystem.cs         # 저장 시스템 인터페이스
│   ├── JsonSerializer.cs      # JSON 직렬화 구현
│   ├── SaveSystem.cs          # 저장 시스템 구현
│   └── GameSettings.cs        # 게임 설정 모델
│
├── DI/
│   ├── IServiceContainer.cs   # DI 컨테이너 인터페이스
│   └── ServiceContainer.cs    # 간단한 DI 컨테이너 구현
│
├── AI/
│   ├── IAIPlayer.cs           # AI 플레이어 인터페이스
│   ├── IGameStateEvaluator.cs # 게임 상태 평가 인터페이스
│   ├── MinimaxAI.cs           # 미니맥스 AI 구현
│   ├── IAIPlayerFactory.cs    # AI 팩토리 인터페이스
│   └── AIPlayerFactory.cs     # AI 팩토리 구현
│
└── Core.csproj                # 프로젝트 파일

Tests/
└── Core.Tests/
    ├── InterfacesTests.cs
    ├── StateTests.cs
    ├── EventsTests.cs
    ├── DataTests.cs
    ├── AITests.cs
    └── Core.Tests.csproj
```

---

## Task 1: Project Setup

**Files:**
- Create: `Core/Core.csproj`
- Create: `Tests/Core.Tests/Core.Tests.csproj`

- [ ] **Step 1: Create Core project**

```bash
mkdir -p Core Tests/Core.Tests
```

- [ ] **Step 2: Create Core.csproj**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <RootNamespace>Core</RootNamespace>
  </PropertyGroup>
</Project>
```

- [ ] **Step 3: Create Core.Tests.csproj**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.3.2" />
    <PackageReference Include="xunit" Version="2.4.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.4.5" />
    <ProjectReference Include="..\..\Core\Core.csproj" />
  </ItemGroup>
</Project>
```

- [ ] **Step 4: Create directory structure**

```bash
mkdir -p Core/Interfaces Core/State Core/Events Core/Data Core/DI Core/AI
```

- [ ] **Step 5: Commit**

```bash
git add Core Tests
git commit -m "feat(core): initialize Core project structure"
```

---

## Task 2: GameState Enum

**Files:**
- Create: `Core/Interfaces/GameState.cs`
- Test: `Tests/Core.Tests/InterfacesTests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
// Tests/Core.Tests/InterfacesTests.cs
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
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test Tests/Core.Tests`
Expected: FAIL with "GameState does not exist"

- [ ] **Step 3: Write minimal implementation**

```csharp
// Core/Interfaces/GameState.cs
namespace Core.Interfaces;

public enum GameState
{
    None,
    Ready,
    Playing,
    Paused,
    GameOver,
    Victory
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test Tests/Core.Tests`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add Core/Interfaces/GameState.cs Tests/Core.Tests/InterfacesTests.cs
git commit -m "feat(core): add GameState enum"
```

---

## Task 3: ISP-Separated Interfaces

**Files:**
- Create: `Core/Interfaces/IGameInfo.cs`
- Create: `Core/Interfaces/IGameControllable.cs`
- Create: `Core/Interfaces/IGameSerializable.cs`
- Create: `Core/Interfaces/IGameEventEmitter.cs`
- Create: `Core/Interfaces/IGame.cs`
- Test: `Tests/Core.Tests/InterfacesTests.cs`

- [ ] **Step 1: Write the failing test for IGameInfo**

```csharp
// Tests/Core.Tests/InterfacesTests.cs (append)
public class GameInfoTests
{
    [Fact]
    public void IGameInfo_ShouldHaveRequiredProperties()
    {
        // Interface contract test - verify properties exist
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
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test Tests/Core.Tests`
Expected: FAIL with "IGameInfo does not exist"

- [ ] **Step 3: Write IGameInfo implementation**

```csharp
// Core/Interfaces/IGameInfo.cs
namespace Core.Interfaces;

public interface IGameInfo
{
    string GameName { get; }
    string GameDescription { get; }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test Tests/Core.Tests`
Expected: PASS

- [ ] **Step 5: Write the failing test for IGameControllable**

```csharp
// Tests/Core.Tests/InterfacesTests.cs (append)
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
```

- [ ] **Step 6: Run test to verify it fails**

Run: `dotnet test Tests/Core.Tests`
Expected: FAIL with "IGameControllable does not exist"

- [ ] **Step 7: Write IGameControllable implementation**

```csharp
// Core/Interfaces/IGameControllable.cs
namespace Core.Interfaces;

public interface IGameControllable
{
    GameState CurrentState { get; }
    bool IsPlaying { get; }
    bool IsPaused { get; }
    
    void StartGame();
    void PauseGame();
    void ResumeGame();
    void ResetGame();
    void EndGame();
}
```

- [ ] **Step 8: Run test to verify it passes**

Run: `dotnet test Tests/Core.Tests`
Expected: PASS

- [ ] **Step 9: Write IGameSerializable**

```csharp
// Core/Interfaces/IGameSerializable.cs
namespace Core.Interfaces;

public interface IGameSerializable
{
    string SerializeState();
    void DeserializeState(string json);
}
```

- [ ] **Step 10: Write IGameEventEmitter**

```csharp
// Core/Interfaces/IGameEventEmitter.cs
namespace Core.Interfaces;

public interface IGameEventEmitter
{
    event Action<GameEvent>? OnGameEvent;
}
```

- [ ] **Step 11: Write IGame (composite interface)**

```csharp
// Core/Interfaces/IGame.cs
namespace Core.Interfaces;

public interface IGame : IGameInfo, IGameControllable, IGameSerializable, IGameEventEmitter
{
}
```

- [ ] **Step 12: Commit**

```bash
git add Core/Interfaces Tests/Core.Tests
git commit -m "feat(core): add ISP-separated game interfaces"
```

---

## Task 4: IGameState Interface (AI)

**Files:**
- Create: `Core/Interfaces/IGameState.cs`
- Test: `Tests/Core.Tests/InterfacesTests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
// Tests/Core.Tests/InterfacesTests.cs (append)
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
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test Tests/Core.Tests`
Expected: FAIL with "IGameState does not exist"

- [ ] **Step 3: Write minimal implementation**

```csharp
// Core/Interfaces/IGameState.cs
namespace Core.Interfaces;

public interface IGameState
{
    int CurrentPlayer { get; }
    bool IsGameOver { get; }
    int? Winner { get; }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test Tests/Core.Tests`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add Core/Interfaces/IGameState.cs Tests/Core.Tests
git commit -m "feat(core): add IGameState interface for AI"
```

---

## Task 5: GameEvent Base Class

**Files:**
- Create: `Core/Events/GameEvent.cs`
- Test: `Tests/Core.Tests/EventsTests.cs`

- [ ] **Step 1: Create EventsTests.cs**

```csharp
// Tests/Core.Tests/EventsTests.cs
using Core.Events;
using Xunit;

namespace Core.Tests;

public class GameEventTests
{
    [Fact]
    public void GameEvent_ShouldHaveEventTypeAndTimestamp()
    {
        var event = new TestGameEvent("test");
        Assert.Equal("test", event.EventType);
        Assert.True(event.Timestamp > 0);
    }

    [Fact]
    public void GameEvent_ShouldHaveDataDictionary()
    {
        var event = new TestGameEvent("test");
        event.Data["key"] = "value";
        Assert.Equal("value", event.Data["key"]);
    }

    private class TestGameEvent : GameEvent
    {
        public TestGameEvent(string type) : base(type) { }
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test Tests/Core.Tests`
Expected: FAIL with "GameEvent does not exist"

- [ ] **Step 3: Write minimal implementation**

```csharp
// Core/Events/GameEvent.cs
using System.Collections.Generic;

namespace Core.Events;

public abstract class GameEvent
{
    public string EventType { get; protected set; }
    public long Timestamp { get; }
    public Dictionary<string, object> Data { get; }

    protected GameEvent(string type)
    {
        EventType = type;
        Timestamp = DateTime.UtcNow.Ticks;
        Data = new Dictionary<string, object>();
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test Tests/Core.Tests`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add Core/Events/GameEvent.cs Tests/Core.Tests/EventsTests.cs
git commit -m "feat(core): add GameEvent base class"
```

---

## Task 6: Concrete Event Classes

**Files:**
- Create: `Core/Events/ScoreChangedEvent.cs`
- Create: `Core/Events/GameOverEvent.cs`
- Create: `Core/Events/GameStateChangedEvent.cs`
- Test: `Tests/Core.Tests/EventsTests.cs`

- [ ] **Step 1: Write the failing test for ScoreChangedEvent**

```csharp
// Tests/Core.Tests/EventsTests.cs (append)
public class ScoreChangedEventTests
{
    [Fact]
    public void ScoreChangedEvent_ShouldContainScoreData()
    {
        var event = new ScoreChangedEvent(100, 10);
        Assert.Equal("ScoreChanged", event.EventType);
        Assert.Equal(100, event.NewScore);
        Assert.Equal(10, event.Delta);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test Tests/Core.Tests`
Expected: FAIL with "ScoreChangedEvent does not exist"

- [ ] **Step 3: Write ScoreChangedEvent**

```csharp
// Core/Events/ScoreChangedEvent.cs
namespace Core.Events;

public class ScoreChangedEvent : GameEvent
{
    public int NewScore { get; }
    public int Delta { get; }

    public ScoreChangedEvent(int newScore, int delta) : base("ScoreChanged")
    {
        NewScore = newScore;
        Delta = delta;
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test Tests/Core.Tests`
Expected: PASS

- [ ] **Step 5: Write the failing test for GameOverEvent**

```csharp
// Tests/Core.Tests/EventsTests.cs (append)
public class GameOverEventTests
{
    [Fact]
    public void GameOverEvent_ShouldContainVictoryAndScore()
    {
        var event = new GameOverEvent(true, 500);
        Assert.Equal("GameOver", event.EventType);
        Assert.True(event.IsVictory);
        Assert.Equal(500, event.FinalScore);
    }
}
```

- [ ] **Step 6: Run test to verify it fails**

Run: `dotnet test Tests/Core.Tests`
Expected: FAIL with "GameOverEvent does not exist"

- [ ] **Step 7: Write GameOverEvent**

```csharp
// Core/Events/GameOverEvent.cs
namespace Core.Events;

public class GameOverEvent : GameEvent
{
    public bool IsVictory { get; }
    public int FinalScore { get; }

    public GameOverEvent(bool isVictory, int finalScore) : base("GameOver")
    {
        IsVictory = isVictory;
        FinalScore = finalScore;
    }
}
```

- [ ] **Step 8: Run test to verify it passes**

Run: `dotnet test Tests/Core.Tests`
Expected: PASS

- [ ] **Step 9: Write GameStateChangedEvent**

```csharp
// Core/Events/GameStateChangedEvent.cs
using Core.Interfaces;

namespace Core.Events;

public class GameStateChangedEvent : GameEvent
{
    public GameState PreviousState { get; }
    public GameState NewState { get; }

    public GameStateChangedEvent(GameState previous, GameState newState) 
        : base("GameStateChanged")
    {
        PreviousState = previous;
        NewState = newState;
    }
}
```

- [ ] **Step 10: Commit**

```bash
git add Core/Events Tests/Core.Tests
git commit -m "feat(core): add concrete event classes"
```

---

## Task 7: EventBus

**Files:**
- Create: `Core/Events/IEventBus.cs`
- Create: `Core/Events/EventBus.cs`
- Test: `Tests/Core.Tests/EventsTests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
// Tests/Core.Tests/EventsTests.cs (append)
public class EventBusTests
{
    [Fact]
    public void EventBus_ShouldPublishAndSubscribe()
    {
        var bus = new EventBus();
        var received = false;
        int? receivedScore = null;
        
        bus.Subscribe<ScoreChangedEvent>(e => 
        {
            received = true;
            receivedScore = e.NewScore;
        });
        
        bus.Publish(new ScoreChangedEvent(100, 10));
        
        Assert.True(received);
        Assert.Equal(100, receivedScore);
    }

    [Fact]
    public void EventBus_ShouldUnsubscribe()
    {
        var bus = new EventBus();
        var count = 0;
        
        Action<ScoreChangedEvent> handler = e => count++;
        
        bus.Subscribe(handler);
        bus.Publish(new ScoreChangedEvent(100, 10));
        Assert.Equal(1, count);
        
        bus.Unsubscribe(handler);
        bus.Publish(new ScoreChangedEvent(100, 10));
        Assert.Equal(1, count);
    }

    [Fact]
    public void EventBus_ShouldNotThrowWhenNoSubscribers()
    {
        var bus = new EventBus();
        bus.Publish(new ScoreChangedEvent(100, 10));
        // No exception should be thrown
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test Tests/Core.Tests`
Expected: FAIL with "IEventBus/EventBus does not exist"

- [ ] **Step 3: Write IEventBus interface**

```csharp
// Core/Events/IEventBus.cs
namespace Core.Events;

public interface IEventBus
{
    void Subscribe<T>(Action<T> handler) where T : GameEvent;
    void Unsubscribe<T>(Action<T> handler) where T : GameEvent;
    void Publish<T>(T gameEvent) where T : GameEvent;
}
```

- [ ] **Step 4: Write EventBus implementation**

```csharp
// Core/Events/EventBus.cs
using System;
using System.Collections.Generic;

namespace Core.Events;

public class EventBus : IEventBus
{
    private readonly Dictionary<Type, List<Delegate>> _handlers 
        = new Dictionary<Type, List<Delegate>>();

    public void Subscribe<T>(Action<T> handler) where T : GameEvent
    {
        var type = typeof(T);
        if (!_handlers.ContainsKey(type))
            _handlers[type] = new List<Delegate>();
        _handlers[type].Add(handler);
    }

    public void Unsubscribe<T>(Action<T> handler) where T : GameEvent
    {
        var type = typeof(T);
        if (_handlers.TryGetValue(type, out var handlers))
            handlers.Remove(handler);
    }

    public void Publish<T>(T gameEvent) where T : GameEvent
    {
        var type = typeof(T);
        if (_handlers.TryGetValue(type, out var handlers))
        {
            foreach (var handler in handlers.ToList())
            {
                try
                {
                    (handler as Action<T>)?.Invoke(gameEvent);
                }
                catch (Exception ex)
                {
                    // Log error but don't crash
                    Console.WriteLine($"Event handler error: {ex.Message}");
                }
            }
        }
    }
}
```

- [ ] **Step 5: Run test to verify it passes**

Run: `dotnet test Tests/Core.Tests`
Expected: PASS

- [ ] **Step 6: Commit**

```bash
git add Core/Events Tests/Core.Tests
git commit -m "feat(core): add EventBus with subscribe/publish pattern"
```

---

## Task 8: State Management

**Files:**
- Create: `Core/State/IStateManager.cs`
- Create: `Core/State/StateManager.cs`
- Create: `Core/State/IStateTransitionRule.cs`
- Create: `Core/State/DefaultStateTransitionRule.cs`
- Test: `Tests/Core.Tests/StateTests.cs`

- [ ] **Step 1: Create StateTests.cs**

```csharp
// Tests/Core.Tests/StateTests.cs
using Core.Interfaces;
using Core.State;
using Xunit;

namespace Core.Tests;

public class StateManagerTests
{
    [Fact]
    public void StateManager_ShouldInitializeWithNone()
    {
        var rule = new DefaultStateTransitionRule();
        var manager = new StateManager(rule);
        Assert.Equal(GameState.None, manager.CurrentState);
    }

    [Fact]
    public void StateManager_ShouldTransitionState()
    {
        var rule = new DefaultStateTransitionRule();
        var manager = new StateManager(rule);
        
        manager.ChangeState(GameState.Ready);
        Assert.Equal(GameState.Ready, manager.CurrentState);
        
        manager.ChangeState(GameState.Playing);
        Assert.Equal(GameState.Playing, manager.CurrentState);
    }

    [Fact]
    public void StateManager_ShouldFireOnStateChanged()
    {
        var rule = new DefaultStateTransitionRule();
        var manager = new StateManager(rule);
        
        GameState? previousState = null;
        GameState? newState = null;
        
        manager.OnStateChanged += (prev, current) =>
        {
            previousState = prev;
            newState = current;
        };
        
        manager.ChangeState(GameState.Ready);
        
        Assert.Equal(GameState.None, previousState);
        Assert.Equal(GameState.Ready, newState);
    }

    [Fact]
    public void StateManager_ShouldNotAllowInvalidTransition()
    {
        var rule = new DefaultStateTransitionRule();
        var manager = new StateManager(rule);
        
        // Cannot go directly from None to Playing
        manager.ChangeState(GameState.Playing);
        Assert.Equal(GameState.None, manager.CurrentState);
    }

    [Fact]
    public void StateManager_ShouldRequireTransitionRule()
    {
        Assert.Throws<ArgumentNullException>(() => new StateManager(null!));
    }
}

public class StateTransitionRuleTests
{
    [Fact]
    public void DefaultRule_ShouldAllowValidTransitions()
    {
        var rule = new DefaultStateTransitionRule();
        
        Assert.True(rule.CanTransition(GameState.Ready, GameState.Playing));
        Assert.True(rule.CanTransition(GameState.Playing, GameState.Paused));
        Assert.True(rule.CanTransition(GameState.Paused, GameState.Playing));
        Assert.True(rule.CanTransition(GameState.Playing, GameState.GameOver));
        Assert.True(rule.CanTransition(GameState.GameOver, GameState.Ready));
    }

    [Fact]
    public void DefaultRule_ShouldDenyInvalidTransitions()
    {
        var rule = new DefaultStateTransitionRule();
        
        Assert.False(rule.CanTransition(GameState.None, GameState.Playing));
        Assert.False(rule.CanTransition(GameState.Paused, GameState.GameOver));
        Assert.False(rule.CanTransition(GameState.GameOver, GameState.Playing));
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test Tests/Core.Tests`
Expected: FAIL with "StateManager/IStateManager does not exist"

- [ ] **Step 3: Write IStateTransitionRule**

```csharp
// Core/State/IStateTransitionRule.cs
using Core.Interfaces;

namespace Core.State;

public interface IStateTransitionRule
{
    bool CanTransition(GameState from, GameState to);
}
```

- [ ] **Step 4: Write DefaultStateTransitionRule**

```csharp
// Core/State/DefaultStateTransitionRule.cs
using Core.Interfaces;

namespace Core.State;

public class DefaultStateTransitionRule : IStateTransitionRule
{
    public bool CanTransition(GameState from, GameState to)
    {
        return (from, to) switch
        {
            (GameState.None, GameState.Ready) => true,
            (GameState.Ready, GameState.Playing) => true,
            (GameState.Playing, GameState.Paused) => true,
            (GameState.Playing, GameState.GameOver) => true,
            (GameState.Playing, GameState.Victory) => true,
            (GameState.Paused, GameState.Playing) => true,
            (GameState.Paused, GameState.Ready) => true,
            (GameState.GameOver, GameState.Ready) => true,
            (GameState.Victory, GameState.Ready) => true,
            _ => false
        };
    }
}
```

- [ ] **Step 5: Write IStateManager**

```csharp
// Core/State/IStateManager.cs
using Core.Interfaces;

namespace Core.State;

public interface IStateManager
{
    GameState CurrentState { get; }
    event Action<GameState, GameState>? OnStateChanged;
    void ChangeState(GameState newState);
    bool CanTransitionTo(GameState target);
}
```

- [ ] **Step 6: Write StateManager**

```csharp
// Core/State/StateManager.cs
using System;
using Core.Interfaces;

namespace Core.State;

public class StateManager : IStateManager
{
    private GameState _currentState = GameState.None;
    private readonly IStateTransitionRule _transitionRule;

    public StateManager(IStateTransitionRule transitionRule)
    {
        _transitionRule = transitionRule 
            ?? throw new ArgumentNullException(nameof(transitionRule));
    }

    public GameState CurrentState => _currentState;

    public event Action<GameState, GameState>? OnStateChanged;

    public void ChangeState(GameState newState)
    {
        if (!CanTransitionTo(newState)) return;

        var previous = _currentState;
        _currentState = newState;
        OnStateChanged?.Invoke(previous, _currentState);
    }

    public bool CanTransitionTo(GameState target)
    {
        return _transitionRule.CanTransition(_currentState, target);
    }
}
```

- [ ] **Step 7: Run test to verify it passes**

Run: `dotnet test Tests/Core.Tests`
Expected: PASS

- [ ] **Step 8: Commit**

```bash
git add Core/State Tests/Core.Tests
git commit -m "feat(core): add state management with transition rules"
```

---

## Task 9: Save System Interfaces

**Files:**
- Create: `Core/Data/ISerializer.cs`
- Create: `Core/Data/ISaveStorage.cs` (async)
- Create: `Core/Data/ISaveSystem.cs`
- Create: `Core/Data/GameSettings.cs`
- Test: `Tests/Core.Tests/DataTests.cs`

- [ ] **Step 1: Create DataTests.cs**

```csharp
// Tests/Core.Tests/DataTests.cs
using Core.Data;
using Xunit;

namespace Core.Tests;

public class GameSettingsTests
{
    [Fact]
    public void GameSettings_ShouldInitializeWithDefaults()
    {
        var settings = new GameSettings();
        Assert.NotNull(settings.HighScores);
        Assert.NotNull(settings.GameProgress);
        Assert.Equal(0, settings.TotalPlayTime);
    }

    [Fact]
    public void GameSettings_ShouldAllowHighScoreUpdates()
    {
        var settings = new GameSettings();
        settings.HighScores["PatternMemory"] = 500;
        Assert.Equal(500, settings.HighScores["PatternMemory"]);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test Tests/Core.Tests`
Expected: FAIL with "GameSettings does not exist"

- [ ] **Step 3: Write ISerializer**

```csharp
// Core/Data/ISerializer.cs
namespace Core.Data;

public interface ISerializer
{
    string Serialize<T>(T data);
    T Deserialize<T>(string json);
}
```

- [ ] **Step 4: Write ISaveStorage (async)**

```csharp
// Core/Data/ISaveStorage.cs
using System.Threading.Tasks;

namespace Core.Data;

public interface ISaveStorage
{
    Task SaveAsync(string key, string data);
    Task<string?> LoadAsync(string key);
    Task<bool> ExistsAsync(string key);
    Task DeleteAsync(string key);
}
```

- [ ] **Step 5: Write ISaveSystem**

```csharp
// Core/Data/ISaveSystem.cs
using System.Threading.Tasks;

namespace Core.Data;

public interface ISaveSystem
{
    Task SaveGameAsync<T>(string gameId, T gameState);
    Task<T?> LoadGameAsync<T>(string gameId);
    Task<bool> HasSavedGameAsync(string gameId);
    Task DeleteSavedGameAsync(string gameId);
    Task SaveSettingsAsync(GameSettings settings);
    Task<GameSettings> LoadSettingsAsync();
}
```

- [ ] **Step 6: Write GameSettings**

```csharp
// Core/Data/GameSettings.cs
using System.Collections.Generic;

namespace Core.Data;

public class GameSettings
{
    public Dictionary<string, int> HighScores { get; set; } = new();
    public Dictionary<string, bool> GameProgress { get; set; } = new();
    public int TotalPlayTime { get; set; }
}
```

- [ ] **Step 7: Run test to verify it passes**

Run: `dotnet test Tests/Core.Tests`
Expected: PASS

- [ ] **Step 8: Commit**

```bash
git add Core/Data Tests/Core.Tests
git commit -m "feat(core): add save system interfaces (async)"
```

---

## Task 10: JsonSerializer Implementation

**Files:**
- Create: `Core/Data/JsonSerializer.cs`
- Test: `Tests/Core.Tests/DataTests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
// Tests/Core.Tests/DataTests.cs (append)
public class JsonSerializerTests
{
    [Fact]
    public void JsonSerializer_ShouldSerializeObject()
    {
        var serializer = new JsonSerializer();
        var settings = new GameSettings { TotalPlayTime = 100 };
        
        var json = serializer.Serialize(settings);
        
        Assert.Contains("TotalPlayTime", json);
        Assert.Contains("100", json);
    }

    [Fact]
    public void JsonSerializer_ShouldDeserializeObject()
    {
        var serializer = new JsonSerializer();
        var original = new GameSettings 
        { 
            TotalPlayTime = 200,
            HighScores = new Dictionary<string, int> { { "Test", 500 } }
        };
        
        var json = serializer.Serialize(original);
        var deserialized = serializer.Deserialize<GameSettings>(json);
        
        Assert.Equal(200, deserialized.TotalPlayTime);
        Assert.Equal(500, deserialized.HighScores["Test"]);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test Tests/Core.Tests`
Expected: FAIL with "JsonSerializer does not exist"

- [ ] **Step 3: Write JsonSerializer**

```csharp
// Core/Data/JsonSerializer.cs
using System.Text.Json;

namespace Core.Data;

public class JsonSerializer : ISerializer
{
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        WriteIndented = true
    };

    public string Serialize<T>(T data)
    {
        return System.Text.Json.JsonSerializer.Serialize(data, _options);
    }

    public T Deserialize<T>(string json)
    {
        return System.Text.Json.JsonSerializer.Deserialize<T>(json, _options)!;
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test Tests/Core.Tests`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add Core/Data/JsonSerializer.cs Tests/Core.Tests
git commit -m "feat(core): add JSON serializer implementation"
```

---

## Task 11: SaveSystem Implementation

**Files:**
- Create: `Core/Data/SaveSystem.cs`
- Test: `Tests/Core.Tests/DataTests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
// Tests/Core.Tests/DataTests.cs (append)
public class SaveSystemTests
{
    private class MockStorage : ISaveStorage
    {
        private readonly Dictionary<string, string> _data = new();

        public Task SaveAsync(string key, string data)
        {
            _data[key] = data;
            return Task.CompletedTask;
        }

        public Task<string?> LoadAsync(string key)
        {
            return Task.FromResult(_data.TryGetValue(key, out var value) ? value : null);
        }

        public Task<bool> ExistsAsync(string key)
        {
            return Task.FromResult(_data.ContainsKey(key));
        }

        public Task DeleteAsync(string key)
        {
            _data.Remove(key);
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task SaveSystem_ShouldSaveAndLoadGame()
    {
        var storage = new MockStorage();
        var serializer = new JsonSerializer();
        var saveSystem = new SaveSystem(serializer, storage);
        
        var state = new GameSettings { TotalPlayTime = 300 };
        await saveSystem.SaveGameAsync("test", state);
        
        var loaded = await saveSystem.LoadGameAsync<GameSettings>("test");
        Assert.NotNull(loaded);
        Assert.Equal(300, loaded.TotalPlayTime);
    }

    [Fact]
    public async Task SaveSystem_ShouldReturnNullWhenNoSavedGame()
    {
        var storage = new MockStorage();
        var serializer = new JsonSerializer();
        var saveSystem = new SaveSystem(serializer, storage);
        
        var loaded = await saveSystem.LoadGameAsync<GameSettings>("nonexistent");
        Assert.Null(loaded);
    }

    [Fact]
    public async Task SaveSystem_ShouldDetectSavedGame()
    {
        var storage = new MockStorage();
        var serializer = new JsonSerializer();
        var saveSystem = new SaveSystem(serializer, storage);
        
        Assert.False(await saveSystem.HasSavedGameAsync("test"));
        
        await saveSystem.SaveGameAsync("test", new GameSettings());
        Assert.True(await saveSystem.HasSavedGameAsync("test"));
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test Tests/Core.Tests`
Expected: FAIL with "SaveSystem does not exist"

- [ ] **Step 3: Write SaveSystem**

```csharp
// Core/Data/SaveSystem.cs
using System.Threading.Tasks;

namespace Core.Data;

public class SaveSystem : ISaveSystem
{
    private readonly ISerializer _serializer;
    private readonly ISaveStorage _storage;

    public SaveSystem(ISerializer serializer, ISaveStorage storage)
    {
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    public async Task SaveGameAsync<T>(string gameId, T gameState)
    {
        var json = _serializer.Serialize(gameState);
        await _storage.SaveAsync($"game_{gameId}", json);
    }

    public async Task<T?> LoadGameAsync<T>(string gameId)
    {
        var json = await _storage.LoadAsync($"game_{gameId}");
        return json != null ? _serializer.Deserialize<T>(json) : default;
    }

    public async Task<bool> HasSavedGameAsync(string gameId)
    {
        return await _storage.ExistsAsync($"game_{gameId}");
    }

    public async Task DeleteSavedGameAsync(string gameId)
    {
        await _storage.DeleteAsync($"game_{gameId}");
    }

    public async Task SaveSettingsAsync(GameSettings settings)
    {
        var json = _serializer.Serialize(settings);
        await _storage.SaveAsync("settings", json);
    }

    public async Task<GameSettings> LoadSettingsAsync()
    {
        var json = await _storage.LoadAsync("settings");
        return json != null 
            ? _serializer.Deserialize<GameSettings>(json) 
            : new GameSettings();
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test Tests/Core.Tests`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add Core/Data/SaveSystem.cs Tests/Core.Tests
git commit -m "feat(core): add SaveSystem implementation"
```

---

## Task 12: DI Container

**Files:**
- Create: `Core/DI/IServiceContainer.cs`
- Create: `Core/DI/ServiceContainer.cs`
- Test: `Tests/Core.Tests/DITests.cs`

- [ ] **Step 1: Create DITests.cs**

```csharp
// Tests/Core.Tests/DITests.cs
using Core.DI;
using Xunit;

namespace Core.Tests;

public class ServiceContainerTests
{
    [Fact]
    public void Container_ShouldRegisterSingleton()
    {
        var container = new ServiceContainer();
        container.RegisterSingleton<ITestService, TestService>();
        
        var instance1 = container.Resolve<ITestService>();
        var instance2 = container.Resolve<ITestService>();
        
        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void Container_ShouldRegisterTransient()
    {
        var container = new ServiceContainer();
        container.RegisterTransient<ITestService, TestService>();
        
        var instance1 = container.Resolve<ITestService>();
        var instance2 = container.Resolve<ITestService>();
        
        Assert.NotSame(instance1, instance2);
    }

    [Fact]
    public void Container_ShouldThrowWhenNotRegistered()
    {
        var container = new ServiceContainer();
        Assert.Throws<InvalidOperationException>(() => container.Resolve<ITestService>());
    }

    private interface ITestService { string Value { get; } }
    private class TestService : ITestService { public string Value => "test"; }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test Tests/Core.Tests`
Expected: FAIL with "IServiceContainer/ServiceContainer does not exist"

- [ ] **Step 3: Write IServiceContainer**

```csharp
// Core/DI/IServiceContainer.cs
namespace Core.DI;

public interface IServiceContainer
{
    void RegisterSingleton<TInterface, TImplementation>() 
        where TImplementation : class, TInterface;
    void RegisterTransient<TInterface, TImplementation>() 
        where TImplementation : class, TInterface;
    TInterface Resolve<TInterface>();
}
```

- [ ] **Step 4: Write ServiceContainer**

```csharp
// Core/DI/ServiceContainer.cs
using System;
using System.Collections.Generic;

namespace Core.DI;

public class ServiceContainer : IServiceContainer
{
    private readonly Dictionary<Type, Func<object>> _registrations 
        = new Dictionary<Type, Func<object>>();

    private readonly Dictionary<Type, object> _singletons 
        = new Dictionary<Type, object>();

    public void RegisterSingleton<TInterface, TImplementation>() 
        where TImplementation : class, TInterface
    {
        _registrations[typeof(TInterface)] = () =>
        {
            if (!_singletons.TryGetValue(typeof(TInterface), out var instance))
            {
                instance = Activator.CreateInstance<TImplementation>();
                _singletons[typeof(TInterface)] = instance;
            }
            return instance;
        };
    }

    public void RegisterTransient<TInterface, TImplementation>() 
        where TImplementation : class, TInterface
    {
        _registrations[typeof(TInterface)] = () => 
            Activator.CreateInstance<TImplementation>();
    }

    public TInterface Resolve<TInterface>()
    {
        if (!_registrations.TryGetValue(typeof(TInterface), out var factory))
            throw new InvalidOperationException(
                $"Service {typeof(TInterface).Name} is not registered");

        return (TInterface)factory();
    }
}
```

- [ ] **Step 5: Run test to verify it passes**

Run: `dotnet test Tests/Core.Tests`
Expected: PASS

- [ ] **Step 6: Commit**

```bash
git add Core/DI Tests/Core.Tests
git commit -m "feat(core): add simple DI container"
```

---

## Task 13: AI Interfaces

**Files:**
- Create: `Core/AI/IAIPlayer.cs`
- Create: `Core/AI/IGameStateEvaluator.cs`
- Test: `Tests/Core.Tests/AITests.cs`

- [ ] **Step 1: Create AITests.cs**

```csharp
// Tests/Core.Tests/AITests.cs
using Core.AI;
using Core.Interfaces;
using Xunit;

namespace Core.Tests;

public class AIInterfaceTests
{
    [Fact]
    public void IAIPlayer_ShouldHaveDifficultyProperty()
    {
        var ai = new MockAIPlayer();
        ai.Difficulty = "Hard";
        Assert.Equal("Hard", ai.Difficulty);
    }

    [Fact]
    public void IGameStateEvaluator_ShouldEvaluateState()
    {
        var evaluator = new MockEvaluator();
        var state = new MockState();
        
        var score = evaluator.Evaluate(state);
        Assert.Equal(0, score);
    }

    private class MockAIPlayer : IAIPlayer<int>
    {
        public string Difficulty { get; set; } = "Normal";
        public event Action<string>? OnThinking;
        
        public int GetBestMove(IGameState state, IGameStateEvaluator<int> evaluator)
        {
            OnThinking?.Invoke("Thinking...");
            return 0;
        }
    }

    private class MockEvaluator : IGameStateEvaluator<int>
    {
        public int Evaluate(IGameState state) => 0;
        public IEnumerable<int> GetValidMoves(IGameState state) => new[] { 0, 1, 2 };
        public IGameState ApplyMove(IGameState state, int move) => state;
    }

    private class MockState : IGameState
    {
        public int CurrentPlayer => 1;
        public bool IsGameOver => false;
        public int? Winner => null;
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test Tests/Core.Tests`
Expected: FAIL with "IAIPlayer/IGameStateEvaluator does not exist"

- [ ] **Step 3: Write IAIPlayer**

```csharp
// Core/AI/IAIPlayer.cs
using System;
using Core.Interfaces;

namespace Core.AI;

public interface IAIPlayer<TMove>
{
    string Difficulty { get; set; }
    TMove GetBestMove(IGameState state, IGameStateEvaluator<TMove> evaluator);
    event Action<string>? OnThinking;
}
```

- [ ] **Step 4: Write IGameStateEvaluator**

```csharp
// Core/AI/IGameStateEvaluator.cs
using System.Collections.Generic;
using Core.Interfaces;

namespace Core.AI;

public interface IGameStateEvaluator<TMove>
{
    int Evaluate(IGameState state);
    IEnumerable<TMove> GetValidMoves(IGameState state);
    IGameState ApplyMove(IGameState state, TMove move);
}
```

- [ ] **Step 5: Run test to verify it passes**

Run: `dotnet test Tests/Core.Tests`
Expected: PASS

- [ ] **Step 6: Commit**

```bash
git add Core/AI Tests/Core.Tests
git commit -m "feat(core): add AI interfaces"
```

---

## Task 14: MinimaxAI Implementation

**Files:**
- Create: `Core/AI/MinimaxAI.cs`
- Test: `Tests/Core.Tests/AITests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
// Tests/Core.Tests/AITests.cs (append)
public class MinimaxAITests
{
    [Fact]
    public void MinimaxAI_ShouldReturnBestMove()
    {
        // Simple game: pick the highest value move
        var evaluator = new SimpleEvaluator();
        var ai = new MinimaxAI<int>(2);
        
        var state = new SimpleState(false);
        var move = ai.GetBestMove(state, evaluator);
        
        // AI should pick move 2 (highest value)
        Assert.Equal(2, move);
    }

    [Fact]
    public void MinimaxAI_ShouldRespectDifficulty()
    {
        var aiEasy = new MinimaxAI<int>(1);
        var aiHard = new MinimaxAI<int>(5);
        
        Assert.Equal("Normal", aiEasy.Difficulty);
        Assert.Equal("Normal", aiHard.Difficulty);
        
        aiEasy.Difficulty = "Easy";
        aiHard.Difficulty = "Hard";
        
        Assert.Equal("Easy", aiEasy.Difficulty);
        Assert.Equal("Hard", aiHard.Difficulty);
    }

    private class SimpleEvaluator : IGameStateEvaluator<int>
    {
        public int Evaluate(IGameState state) => 0;
        
        public IEnumerable<int> GetValidMoves(IGameState state) => new[] { 0, 1, 2 };
        
        public IGameState ApplyMove(IGameState state, int move)
        {
            return new SimpleState(move == 2);  // Move 2 wins
        }
    }

    private class SimpleState : IGameState
    {
        private readonly bool _isWin;
        
        public SimpleState(bool isWin) { _isWin = isWin; }
        
        public int CurrentPlayer => 1;
        public bool IsGameOver => _isWin;
        public int? Winner => _isWin ? 1 : null;
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test Tests/Core.Tests`
Expected: FAIL with "MinimaxAI does not exist"

- [ ] **Step 3: Write MinimaxAI**

```csharp
// Core/AI/MinimaxAI.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Core.Interfaces;

namespace Core.AI;

public class MinimaxAI<TMove> : IAIPlayer<TMove>
{
    private readonly int _maxDepth;
    private readonly Random _random;

    public string Difficulty { get; set; } = "Normal";
    public event Action<string>? OnThinking;

    public MinimaxAI(int maxDepth = 3)
    {
        _maxDepth = maxDepth;
        _random = new Random();
    }

    public TMove GetBestMove(IGameState state, IGameStateEvaluator<TMove> evaluator)
    {
        OnThinking?.Invoke("AI thinking...");

        var validMoves = evaluator.GetValidMoves(state).ToList();
        
        if (validMoves.Count == 0)
            throw new InvalidOperationException("No valid moves available");
        
        if (validMoves.Count == 1)
            return validMoves[0];

        TMove bestMove = validMoves[0];
        int bestScore = int.MinValue;

        foreach (var move in validMoves)
        {
            var newState = evaluator.ApplyMove(state, move);
            int score = Minimax(newState, _maxDepth - 1, 
                                int.MinValue, int.MaxValue, 
                                false, evaluator);

            if (score > bestScore || 
                (score == bestScore && _random.Next(2) == 0))
            {
                bestScore = score;
                bestMove = move;
            }
        }

        return bestMove;
    }

    private int Minimax(IGameState state, int depth, 
                        int alpha, int beta, bool isMaximizing,
                        IGameStateEvaluator<TMove> evaluator)
    {
        if (depth == 0 || state.IsGameOver)
            return evaluator.Evaluate(state);

        var validMoves = evaluator.GetValidMoves(state);

        if (isMaximizing)
        {
            int maxScore = int.MinValue;
            foreach (var move in validMoves)
            {
                var newState = evaluator.ApplyMove(state, move);
                int score = Minimax(newState, depth - 1, 
                                    alpha, beta, false, evaluator);
                maxScore = Math.Max(maxScore, score);
                alpha = Math.Max(alpha, score);
                if (beta <= alpha) break;
            }
            return maxScore;
        }
        else
        {
            int minScore = int.MaxValue;
            foreach (var move in validMoves)
            {
                var newState = evaluator.ApplyMove(state, move);
                int score = Minimax(newState, depth - 1, 
                                    alpha, beta, true, evaluator);
                minScore = Math.Min(minScore, score);
                beta = Math.Min(beta, score);
                if (beta <= alpha) break;
            }
            return minScore;
        }
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test Tests/Core.Tests`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add Core/AI/MinimaxAI.cs Tests/Core.Tests
git commit -m "feat(core): add MinimaxAI with alpha-beta pruning"
```

---

## Task 15: AI Factory

**Files:**
- Create: `Core/AI/IAIPlayerFactory.cs`
- Create: `Core/AI/AIPlayerFactory.cs`
- Test: `Tests/Core.Tests/AITests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
// Tests/Core.Tests/AITests.cs (append)
public class AIPlayerFactoryTests
{
    [Fact]
    public void Factory_ShouldCreateAIWithCorrectDifficulty()
    {
        var factory = new AIPlayerFactory();
        
        var easyAI = factory.Create<int>("Easy");
        var normalAI = factory.Create<int>("Normal");
        var hardAI = factory.Create<int>("Hard");
        
        Assert.NotNull(easyAI);
        Assert.NotNull(normalAI);
        Assert.NotNull(hardAI);
    }

    [Fact]
    public void Factory_ShouldUseDefaultForUnknownDifficulty()
    {
        var factory = new AIPlayerFactory();
        var unknownAI = factory.Create<int>("Unknown");
        
        Assert.NotNull(unknownAI);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test Tests/Core.Tests`
Expected: FAIL with "AIPlayerFactory does not exist"

- [ ] **Step 3: Write IAIPlayerFactory**

```csharp
// Core/AI/IAIPlayerFactory.cs
namespace Core.AI;

public interface IAIPlayerFactory
{
    IAIPlayer<TMove> Create<TMove>(string difficulty);
}
```

- [ ] **Step 4: Write AIPlayerFactory**

```csharp
// Core/AI/AIPlayerFactory.cs
using System.Collections.Generic;

namespace Core.AI;

public class AIPlayerFactory : IAIPlayerFactory
{
    private readonly Dictionary<string, int> _difficultyDepths = new()
    {
        { "Easy", 1 },
        { "Normal", 3 },
        { "Hard", 5 }
    };

    public IAIPlayer<TMove> Create<TMove>(string difficulty)
    {
        int depth = _difficultyDepths.TryGetValue(difficulty, out var d) ? d : 3;
        var ai = new MinimaxAI<TMove>(depth);
        ai.Difficulty = difficulty;
        return ai;
    }
}
```

- [ ] **Step 5: Run test to verify it passes**

Run: `dotnet test Tests/Core.Tests`
Expected: PASS

- [ ] **Step 6: Commit**

```bash
git add Core/AI Tests/Core.Tests
git commit -m "feat(core): add AI player factory"
```

---

## Task 16: Final Verification & Summary

- [ ] **Step 1: Run all tests**

Run: `dotnet test Tests/Core.Tests`
Expected: All PASS

- [ ] **Step 2: Verify file structure**

Run: `find Core -type f -name "*.cs"`
Expected: All files created as specified

- [ ] **Step 3: Create solution file (optional)**

```bash
dotnet new sln -n MiniGameCollection
dotnet sln add Core/Core.csproj
dotnet sln add Tests/Core.Tests/Core.Tests.csproj
```

- [ ] **Step 4: Final commit**

```bash
git add .
git commit -m "feat(core): complete Phase 0 - Core Architecture

- All interfaces with ISP separation
- State management with transition rules
- Event system with EventBus
- Save system with async storage
- DI container
- AI module with Minimax and Factory
- Full test coverage"
```

---

## Summary

Phase 0 완료 후:
- **Core.dll** - 모든 게임이 사용하는 기반 라이브러리
- **Core.Tests.dll** - 테스트 커버리지
- Phase 1~6의 게임이 Core를 참조해서 구현 가능
- Phase 7에서 Blazor가 Core를 참조해서 UI 구현

---

## Next Phase

Phase 1: Pattern Memory 게임 구현 시작.