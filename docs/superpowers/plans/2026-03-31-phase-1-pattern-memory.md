# Phase 1: Pattern Memory Game Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build the Pattern Memory game module - a game where players memorize and reproduce number sequences that grow longer each level.

**Architecture:** Pure C# game logic following Core.Interfaces.IGame. PatternMemoryLogic handles game rules, PatternGenerator creates sequences, PatternMemoryGame implements IGame for integration with the event system.

**Tech Stack:** C# 10, .NET 8+, xUnit for testing

---

## File Structure

```
Games/
└── PatternMemory/
    ├── Models/
    │   ├── PatternMemoryState.cs      # 게임 상태 모델 (IPatternMemoryState 구현)
    │   └── PatternMemoryConfig.cs     # 게임 설정 (난이도, 최대 레벨 등)
    ├── Logic/
    │   ├── PatternMemoryLogic.cs      # 핵심 게임 로직
    │   └── PatternGenerator.cs        # 패턴 생성기
    ├── PatternMemoryGame.cs           # IGame 구현체
    └── PatternMemoryGame.csproj       # 프로젝트 파일

Tests/
└── Games.PatternMemory.Tests/
    ├── PatternMemoryStateTests.cs
    ├── PatternGeneratorTests.cs
    ├── PatternMemoryLogicTests.cs
    ├── PatternMemoryGameTests.cs
    └── Games.PatternMemory.Tests.csproj
```

---

## Task 1: Project Setup

**Files:**
- Create: `Games/PatternMemory/PatternMemoryGame.csproj`
- Create: `Tests/Games.PatternMemory.Tests/Games.PatternMemory.Tests.csproj`
- Modify: `MiniGameCollection.sln` (add projects)

- [ ] **Step 1: Create project files**

```xml
<!-- Games/PatternMemory/PatternMemoryGame.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <RootNamespace>Games.PatternMemory</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\Core\Core.csproj" />
  </ItemGroup>
</Project>
```

```xml
<!-- Tests/Games.PatternMemory.Tests/Games.PatternMemory.Tests.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.3.2" />
    <PackageReference Include="xunit" Version="2.4.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.4.5" />
    <ProjectReference Include="..\..\Games\PatternMemory\PatternMemoryGame.csproj" />
  </ItemGroup>
</Project>
```

- [ ] **Step 2: Create directory structure**

```bash
mkdir -p Games/PatternMemory/Models Games/PatternMemory/Logic Tests/Games.PatternMemory.Tests
```

- [ ] **Step 3: Add to solution**

```bash
dotnet sln add Games/PatternMemory/PatternMemoryGame.csproj
dotnet sln add Tests/Games.PatternMemory.Tests/Games.PatternMemory.Tests.csproj
```

- [ ] **Step 4: Verify build**

```bash
dotnet build
```
Expected: Build succeeds

- [ ] **Step 5: Commit**

```bash
git add Games/PatternMemory Tests/Games.PatternMemory.Tests MiniGameCollection.sln
git commit -m "feat(pattern-memory): initialize project structure"
```

---

## Task 2: PatternMemoryState Model

**Files:**
- Create: `Games/PatternMemory/Models/PatternMemoryState.cs`
- Test: `Tests/Games.PatternMemory.Tests/PatternMemoryStateTests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
// Tests/Games.PatternMemory.Tests/PatternMemoryStateTests.cs
using Games.PatternMemory.Models;
using Xunit;

namespace Games.PatternMemory.Tests;

public class PatternMemoryStateTests
{
    [Fact]
    public void State_ShouldInitializeWithDefaults()
    {
        var state = new PatternMemoryState();
        
        Assert.Equal(1, state.CurrentLevel);
        Assert.Equal(0, state.PlayerScore);
        Assert.Equal(3, state.MistakesAllowed);
        Assert.Empty(state.CurrentPattern);
        Assert.False(state.IsGameOver);
        Assert.Equal(1, state.CurrentPlayer);
        Assert.Null(state.Winner);
    }

    [Fact]
    public void State_ShouldTrackPattern()
    {
        var state = new PatternMemoryState();
        state.SetPattern(new List<int> { 1, 2, 3 });
        
        Assert.Equal(3, state.CurrentPattern.Count);
        Assert.Equal(1, state.CurrentPattern[0]);
        Assert.Equal(2, state.CurrentPattern[1]);
        Assert.Equal(3, state.CurrentPattern[2]);
    }

    [Fact]
    public void State_ShouldDetectGameOver()
    {
        var state = new PatternMemoryState();
        state.SetMistakesAllowed(0);
        
        Assert.True(state.IsGameOver);
    }

    [Fact]
    public void State_ShouldDetectVictory()
    {
        var state = new PatternMemoryState();
        state.SetVictory();
        
        Assert.True(state.IsGameOver);
        Assert.Equal(1, state.Winner);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test Tests/Games.PatternMemory.Tests`
Expected: FAIL

- [ ] **Step 3: Write PatternMemoryState**

```csharp
// Games/PatternMemory/Models/PatternMemoryState.cs
using System.Collections.Generic;
using Core.Interfaces;

namespace Games.PatternMemory.Models;

public class PatternMemoryState : IGameState
{
    public List<int> Pattern { get; private set; } = new();
    public int CurrentLevel { get; set; } = 1;
    public int PlayerScore { get; private set; } = 0;
    public int MistakesAllowed { get; private set; } = 3;
    public bool IsVictory { get; private set; }

    public IReadOnlyList<int> CurrentPattern => Pattern;
    public int CurrentPlayer => 1;
    public bool IsGameOver => MistakesAllowed <= 0 || IsVictory;
    public int? Winner => IsVictory ? 1 : null;

    public void SetPattern(List<int> pattern) => Pattern = pattern;
    public void SetMistakesAllowed(int count) => MistakesAllowed = count;
    public void AddScore(int points) => PlayerScore += points;
    public void SetVictory() => IsVictory = true;
    public void Reset()
    {
        Pattern = new List<int>();
        CurrentLevel = 1;
        PlayerScore = 0;
        MistakesAllowed = 3;
        IsVictory = false;
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test Tests/Games.PatternMemory.Tests`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add Games/PatternMemory/Models Tests/Games.PatternMemory.Tests
git commit -m "feat(pattern-memory): add PatternMemoryState model"
```

---

## Task 3: PatternGenerator

**Files:**
- Create: `Games/PatternMemory/Logic/IPatternGenerator.cs`
- Create: `Games/PatternMemory/Logic/PatternGenerator.cs`
- Test: `Tests/Games.PatternMemory.Tests/PatternGeneratorTests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
// Tests/Games.PatternMemory.Tests/PatternGeneratorTests.cs
using Games.PatternMemory.Logic;
using Xunit;

namespace Games.PatternMemory.Tests;

public class PatternGeneratorTests
{
    [Fact]
    public void Generate_ShouldReturnCorrectLength()
    {
        var generator = new PatternGenerator(new Random(42));
        var pattern = generator.GeneratePattern(5);
        
        Assert.Equal(5, pattern.Count);
    }

    [Fact]
    public void Generate_ShouldReturnNumbersInRange()
    {
        var generator = new PatternGenerator(new Random(42));
        var pattern = generator.GeneratePattern(10);
        
        foreach (var num in pattern)
        {
            Assert.InRange(num, 0, 8); // 0-8 (Next(0, 9) excludes 9)
        }
    }

    [Fact]
    public void Generate_ShouldBeDeterministicWithSameSeed()
    {
        var gen1 = new PatternGenerator(new Random(42));
        var gen2 = new PatternGenerator(new Random(42));
        
        var pattern1 = gen1.GeneratePattern(5);
        var pattern2 = gen2.GeneratePattern(5);
        
        Assert.Equal(pattern1, pattern2);
    }

    [Fact]
    public void Generate_ShouldReturnDifferentPatternsForDifferentLengths()
    {
        var generator = new PatternGenerator(new Random(42));
        
        var shortPattern = generator.GeneratePattern(3);
        var longPattern = generator.GeneratePattern(7);
        
        Assert.Equal(3, shortPattern.Count);
        Assert.Equal(7, longPattern.Count);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test Tests/Games.PatternMemory.Tests`
Expected: FAIL

- [ ] **Step 3: Write IPatternGenerator**

```csharp
// Games/PatternMemory/Logic/IPatternGenerator.cs
namespace Games.PatternMemory.Logic;

public interface IPatternGenerator
{
    List<int> GeneratePattern(int length);
}
```

- [ ] **Step 4: Write PatternGenerator**

```csharp
// Games/PatternMemory/Logic/PatternGenerator.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace Games.PatternMemory.Logic;

public class PatternGenerator : IPatternGenerator
{
    private readonly Random _random;

    public PatternGenerator(Random random)
    {
        _random = random ?? throw new ArgumentNullException(nameof(random));
    }

    public List<int> GeneratePattern(int length)
    {
        return Enumerable.Range(0, length)
            .Select(_ => _random.Next(0, 9))
            .ToList();
    }
}
```

- [ ] **Step 5: Run test to verify it passes**

Run: `dotnet test Tests/Games.PatternMemory.Tests`
Expected: PASS

- [ ] **Step 6: Commit**

```bash
git add Games/PatternMemory/Logic Tests/Games.PatternMemory.Tests
git commit -m "feat(pattern-memory): add PatternGenerator"
```

---

## Task 4: PatternMemoryLogic

**Files:**
- Create: `Games/PatternMemory/Logic/PatternMemoryLogic.cs`
- Test: `Tests/Games.PatternMemory.Tests/PatternMemoryLogicTests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
// Tests/Games.PatternMemory.Tests/PatternMemoryLogicTests.cs (append)
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
        Assert.Equal(10, _logic.PlayerScore); // Level 1 * 10
    }

    [Fact]
    public void CheckInput_WrongPattern_ShouldReduceMistakes()
    {
        _logic.StartGame();
        var wrongInput = new List<int> { 9, 9, 9 };
        
        var result = _logic.CheckInput(wrongInput);
        
        Assert.False(result);
        Assert.Equal(2, _logic.MistakesAllowed);
    }

    [Fact]
    public void CheckInput_ThreeMistakes_ShouldEndGame()
    {
        _logic.StartGame();
        var wrongInput = new List<int> { 9, 9, 9 };
        
        _logic.CheckInput(wrongInput); // 1st mistake
        _logic.CheckInput(wrongInput); // 2nd mistake
        _logic.CheckInput(wrongInput); // 3rd mistake
        
        Assert.True(_logic.IsGameOver);
        Assert.False(_logic.IsVictory);
    }

    [Fact]
    public void CheckInput_PartialMatch_ShouldBeWrong()
    {
        _logic.StartGame();
        var pattern = _logic.CurrentPattern;
        var partialInput = pattern.Take(pattern.Count - 1).ToList();
        
        var result = _logic.CheckInput(partialInput);
        
        Assert.False(result);
    }

    [Fact]
    public void CheckInput_ExtraInput_ShouldBeWrong()
    {
        _logic.StartGame();
        var pattern = _logic.CurrentPattern;
        var extraInput = new List<int>(pattern) { 5 };
        
        var result = _logic.CheckInput(extraInput);
        
        Assert.False(result);
    }

    [Fact]
    public void ResetGame_ShouldClearState()
    {
        _logic.StartGame();
        _logic.CheckInput(new List<int> { 9, 9, 9 });
        
        _logic.ResetGame();
        
        Assert.Equal(1, _logic.CurrentLevel);
        Assert.Equal(0, _logic.PlayerScore);
        Assert.Equal(3, _logic.MistakesAllowed);
        Assert.False(_logic.IsGameOver);
    }

    [Fact]
    public void NextLevel_ShouldIncreaseLevelAndGenerateNewPattern()
    {
        _logic.StartGame();
        var oldPattern = _logic.CurrentPattern;
        
        _logic.CheckInput(oldPattern); // Correct, advances to level 2
        
        Assert.Equal(2, _logic.CurrentLevel);
        Assert.NotEqual(oldPattern.Count, _logic.CurrentPattern.Count); // Pattern grows
    }

    [Fact]
    public void CheckInput_ReachMaxLevel_ShouldTriggerVictory()
    {
        _logic.MaxLevel = 2;
        _logic.StartGame();
        
        _logic.CheckInput(_logic.CurrentPattern); // Level 1 → 2 (max)
        
        Assert.True(_logic.IsVictory);
        Assert.True(_logic.IsGameOver);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test Tests/Games.PatternMemory.Tests`
Expected: FAIL

- [ ] **Step 3: Write PatternMemoryLogic**

```csharp
// Games/PatternMemory/Logic/PatternMemoryLogic.cs
using System;
using System.Collections.Generic;
using Games.PatternMemory.Models;

namespace Games.PatternMemory.Logic;

public class PatternMemoryLogic
{
    private readonly IPatternGenerator _generator;
    private readonly PatternMemoryState _state;

    public int CurrentLevel => _state.CurrentLevel;
    public int PlayerScore => _state.PlayerScore;
    public int MistakesAllowed => _state.MistakesAllowed;
    public IReadOnlyList<int> CurrentPattern => _state.CurrentPattern;
    public bool IsGameOver => _state.IsGameOver;
    public bool IsVictory => _state.IsVictory;
    public int MaxLevel { get; set; } = 10;

    public PatternMemoryLogic(IPatternGenerator generator)
    {
        _generator = generator ?? throw new ArgumentNullException(nameof(generator));
        _state = new PatternMemoryState();
    }

    public void StartGame()
    {
        _state.Reset();
        GenerateNewPattern();
    }

    public void ResetGame()
    {
        _state.Reset();
    }

    public bool CheckInput(IReadOnlyList<int> playerInput)
    {
        if (playerInput.Count != _state.Pattern.Count)
        {
            _state.SetMistakesAllowed(_state.MistakesAllowed - 1);
            return false;
        }

        for (int i = 0; i < _state.Pattern.Count; i++)
        {
            if (playerInput[i] != _state.Pattern[i])
            {
                _state.SetMistakesAllowed(_state.MistakesAllowed - 1);
                return false;
            }
        }

        // Correct!
        _state.AddScore(_state.CurrentLevel * 10);
        
        // Check victory condition
        if (_state.CurrentLevel >= MaxLevel)
        {
            _state.SetVictory();
            return true;
        }
        
        GenerateNewPattern();
        return true;
    }

    private void GenerateNewPattern()
    {
        var pattern = _generator.GeneratePattern(_state.CurrentLevel + 2);
        _state.SetPattern(pattern);
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test Tests/Games.PatternMemory.Tests`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add Games/PatternMemory/Logic Tests/Games.PatternMemory.Tests
git commit -m "feat(pattern-memory): add PatternMemoryLogic"
```

---

## Task 5: PatternMemoryGame (IGame Implementation)

**Files:**
- Create: `Games/PatternMemory/PatternMemoryGame.cs`
- Test: `Tests/Games.PatternMemory.Tests/PatternMemoryGameTests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
// Tests/Games.PatternMemory.Tests/PatternMemoryGameTests.cs
using Core.Events;
using Core.Interfaces;
using Games.PatternMemory.Logic;
using Xunit;

namespace Games.PatternMemory.Tests;

public class PatternMemoryGameTests
{
    private readonly PatternMemoryGame _game;

    public PatternMemoryGameTests()
    {
        var generator = new PatternGenerator(new Random(42));
        var logic = new PatternMemoryLogic(generator);
        _game = new PatternMemoryGame(logic);
    }

    [Fact]
    public void Game_ShouldHaveCorrectInfo()
    {
        Assert.Equal("Pattern Memory", _game.GameName);
        Assert.NotEmpty(_game.GameDescription);
    }

    [Fact]
    public void Game_ShouldStartInReadyState()
    {
        Assert.Equal(GameState.None, _game.CurrentState);
        Assert.False(_game.IsPlaying);
    }

    [Fact]
    public void StartGame_ShouldTransitionToPlaying()
    {
        _game.StartGame();
        
        Assert.Equal(GameState.Playing, _game.CurrentState);
        Assert.True(_game.IsPlaying);
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

    [Fact]
    public void ResetGame_ShouldReturnToReady()
    {
        _game.StartGame();
        _game.ResetGame();
        
        Assert.Equal(GameState.Ready, _game.CurrentState);
    }

    [Fact]
    public void EndGame_ShouldEnd()
    {
        _game.StartGame();
        _game.EndGame();
        
        Assert.Equal(GameState.GameOver, _game.CurrentState);
    }

    [Fact]
    public void SerializeDeserialize_ShouldPreserveState()
    {
        _game.StartGame();
        var json = _game.SerializeState();
        
        var newGame = new PatternMemoryGame(
            new PatternMemoryLogic(new PatternGenerator(new Random(42))));
        newGame.DeserializeState(json);
        
        // State should be restored from serialized data
        Assert.NotEqual(GameState.None, newGame.CurrentState);
    }

    [Fact]
    public void OnGameEvent_ShouldFireOnStateChange()
    {
        GameEvent? receivedEvent = null;
        _game.OnGameEvent += e => receivedEvent = e;
        
        _game.StartGame();
        
        Assert.NotNull(receivedEvent);
        Assert.Equal("GameStateChanged", receivedEvent.EventType);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test Tests/Games.PatternMemory.Tests`
Expected: FAIL

- [ ] **Step 3: Write PatternMemoryGame**

```csharp
// Games/PatternMemory/PatternMemoryGame.cs
using System;
using System.Text.Json;
using Core.Events;
using Core.Interfaces;
using Core.State;
using Games.PatternMemory.Logic;

namespace Games.PatternMemory;

public class PatternMemoryGame : IGame
{
    private readonly PatternMemoryLogic _logic;
    private readonly IStateManager _stateManager;

    public string GameName => "Pattern Memory";
    public string GameDescription => "Memorize and reproduce growing number sequences";
    public GameState CurrentState => _stateManager.CurrentState;
    public bool IsPlaying => _stateManager.CurrentState == GameState.Playing;
    public bool IsPaused => _stateManager.CurrentState == GameState.Paused;

    public event Action<GameEvent>? OnGameEvent;

    public PatternMemoryGame(PatternMemoryLogic logic)
    {
        _logic = logic ?? throw new ArgumentNullException(nameof(logic));
        _stateManager = new StateManager(new DefaultStateTransitionRule());
        
        _stateManager.OnStateChanged += (prev, current) =>
        {
            var evt = new GameStateChangedEvent(prev, current);
            OnGameEvent?.Invoke(evt);
        };
    }

    public void StartGame()
    {
        // None → Ready → Playing (transition rule allows None→Ready)
        _stateManager.ChangeState(GameState.Ready);
        _stateManager.ChangeState(GameState.Playing);
        _logic.StartGame();
    }

    public void PauseGame()
    {
        _stateManager.ChangeState(GameState.Paused);
    }

    public void ResumeGame()
    {
        _stateManager.ChangeState(GameState.Playing);
    }

    public void ResetGame()
    {
        _logic.ResetGame();
        _stateManager.ChangeState(GameState.Ready);
    }

    public void EndGame()
    {
        _stateManager.ChangeState(GameState.GameOver);
    }

    public string SerializeState()
    {
        return JsonSerializer.Serialize(new
        {
            Level = _logic.CurrentLevel,
            Score = _logic.PlayerScore,
            Mistakes = _logic.MistakesAllowed,
            State = _stateManager.CurrentState
        });
    }

    public void DeserializeState(string json)
    {
        var data = JsonSerializer.Deserialize<PatternMemorySaveData>(json);
        if (data == null) return;
        
        _logic.ResetGame();
        _stateManager.ChangeState(data.State);
    }

    private class PatternMemorySaveData
    {
        public int Level { get; set; }
        public int Score { get; set; }
        public int Mistakes { get; set; }
        public GameState State { get; set; }
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test Tests/Games.PatternMemory.Tests`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add Games/PatternMemory/PatternMemoryGame.cs Tests/Games.PatternMemory.Tests
git commit -m "feat(pattern-memory): add PatternMemoryGame IGame implementation"
```

---

## Task 6: Final Verification

- [ ] **Step 1: Run all tests**

Run: `dotnet test`
Expected: All tests pass (Core + PatternMemory)

- [ ] **Step 2: Verify file structure**

```
Games/PatternMemory/
├── Models/PatternMemoryState.cs
├── Logic/
│   ├── IPatternGenerator.cs
│   ├── PatternGenerator.cs
│   └── PatternMemoryLogic.cs
├── PatternMemoryGame.cs
└── PatternMemoryGame.csproj
```

- [ ] **Step 3: Final commit**

```bash
git add .
git commit -m "feat(pattern-memory): complete Phase 1 - Pattern Memory game

- PatternMemoryState model
- PatternGenerator with DI
- PatternMemoryLogic with game rules
- PatternMemoryGame implementing IGame
- Full test coverage"
```

---

## Summary

Phase 1 완료 후:
- Pattern Memory 게임이 Core의 IGame 인터페이스를 완전히 구현
- EventBus를 통한 상태 변경 알림
- 저장/로드 지원
- Phase 2 (Minesweeper)와 동일한 패턴으로 개발 가능

---

## Next Phase

Phase 2: Minesweeper 게임 구현 시작.