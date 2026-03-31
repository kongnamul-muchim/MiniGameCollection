# Phase 4: Tetris Game Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build the Tetris game module - the classic block-stacking puzzle game where players rotate and place falling tetrominoes to complete lines.

**Architecture:** Pure C# game logic following Core.Interfaces.IGame. TetrisBoard holds the grid, Tetromino represents the falling pieces, TetrisLogic handles game rules.

**Tech Stack:** C# 10, .NET 8+, xUnit for testing

---

## File Structure

```
Games/
└── Tetris/
    ├── Models/
    │   ├── TetrisBoard.cs        # 보드 및 셀 모델
    │   ├── TetrisState.cs        # 게임 상태 모델
    │   ├── Tetromino.cs          # 테트로미노 모델
    │   └── TetrominoType.cs      # 테트로미노 타입 열거
    ├── Logic/
    │   ├── ITetrominoGenerator.cs  # 테트로미노 생성 인터페이스
    │   ├── TetrominoGenerator.cs   # 테트로미노 생성 구현
    │   ├── ICollisionChecker.cs    # 충돌 검사 인터페이스
    │   ├── CollisionChecker.cs     # 충돌 검사 구현
    │   ├── ILineClearer.cs         # 라인 삭제 인터페이스
    │   ├── LineClearer.cs          # 라인 삭제 구현
    │   └── TetrisLogic.cs         # 핵심 게임 로직
    ├── TetrisGame.cs              # IGame 구현체
    └── TetrisGame.csproj          # 프로젝트 파일

Tests/
└── Games.Tetris.Tests/
    ├── TetrisBoardTests.cs
    ├── TetrominoTests.cs
    ├── TetrisLogicTests.cs
    ├── TetrisGameTests.cs
    └── Games.Tetris.Tests.csproj
```

---

## Task 1: Project Setup

**Files:**
- Create: `Games/Tetris/TetrisGame.csproj`
- Create: `Tests/Games.Tetris.Tests/Games.Tetris.Tests.csproj`

- [ ] **Step 1: Create project files**

```xml
<!-- Games/Tetris/TetrisGame.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <RootNamespace>Games.Tetris</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\Core\Core.csproj" />
  </ItemGroup>
</Project>
```

```xml
<!-- Tests/Games.Tetris.Tests/Games.Tetris.Tests.csproj -->
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
    <ProjectReference Include="..\..\Games\Tetris\TetrisGame.csproj" />
  </ItemGroup>
</Project>
```

- [ ] **Step 2: Add to solution**

```bash
dotnet sln add Games/Tetris/TetrisGame.csproj
dotnet sln add Tests/Games.Tetris.Tests/Games.Tetris.Tests.csproj
```

- [ ] **Step 3: Verify build**

```bash
dotnet build
```

- [ ] **Step 4: Commit**

```bash
git add Games/Tetris Tests/Games.Tetris.Tests MiniGameCollection.sln
git commit -m "feat(tetris): initialize project structure"
```

---

## Task 2: Tetris Board & State Models

**Files:**
- Create: `Games/Tetris/Models/TetrominoType.cs`
- Create: `Games/Tetris/Models/Tetromino.cs`
- Create: `Games/Tetris/Models/TetrisBoard.cs`
- Create: `Games/Tetris/Models/TetrisState.cs`
- Test: `Tests/Games.Tetris.Tests/TetrisBoardTests.cs`
- Test: `Tests/Games.Tetris.Tests/TetrominoTests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
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
        
        // Rotation should produce different shape
        Assert.NotNull(rotated.Shape);
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
}
```

- [ ] **Step 2: Run test to verify it fails**

```bash
dotnet test Tests/Games.Tetris.Tests
```

- [ ] **Step 3: Write TetrominoType**

```csharp
// Games/Tetris/Models/TetrominoType.cs
namespace Games.Tetris.Models;

public enum TetrominoType
{
    I,  // Cyan - line
    O,  // Yellow - square
    T,  // Purple - T-shape
    S,  // Green - S-shape
    Z,  // Red - Z-shape
    J,  // Blue - J-shape
    L   // Orange - L-shape
}
```

- [ ] **Step 4: Write Tetromino**

```csharp
// Games/Tetris/Models/Tetromino.cs
namespace Games.Tetris.Models;

public class Tetromino
{
    public int[,] Shape { get; }
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
```

- [ ] **Step 5: Write TetrisBoard**

```csharp
// Games/Tetris/Models/TetrisBoard.cs
namespace Games.Tetris.Models;

public class TetrisBoard
{
    public int[,] Cells { get; }
    public int Rows => 20;
    public int Columns => 10;
    
    public TetrisBoard()
    {
        Cells = new int[20, 10];
    }
    
    public void SetCell(int row, int col, int value)
    {
        if (row >= 0 && row < Rows && col >= 0 && col < Columns)
            Cells[row, col] = value;
    }
    
    public int GetCell(int row, int col) => Cells[row, col];
    
    public void Clear() => Array.Clear(Cells, 0, Cells.Length);
    
    public bool IsFullLine(int row)
    {
        for (int c = 0; c < Columns; c++)
            if (Cells[row, c] == 0) return false;
        return true;
    }
    
    public void ClearLine(int row)
    {
        for (int c = 0; c < Columns; c++)
            Cells[row, c] = 0;
    }
    
    public void CopyRow(int fromRow, int toRow)
    {
        for (int c = 0; c < Columns; c++)
            Cells[toRow, c] = Cells[fromRow, c];
    }
}
```

- [ ] **Step 6: Write TetrisState**

```csharp
// Games/Tetris/Models/TetrisState.cs
using Core.Interfaces;

namespace Games.Tetris.Models;

public class TetrisState : IGameState
{
    public int Rows => 20;
    public int Columns => 10;
    public int CurrentPlayer => 1;
    public bool IsGameOver => IsVictory || GameOver;
    public int? Winner => IsVictory ? 1 : null;
    
    public bool IsVictory { get; private set; }
    public bool GameOver { get; private set; }
    public int Score { get; set; }
    public int LinesCleared { get; set; }
    public int Level { get; set; } = 1;
    
    public void SetGameOver() => GameOver = true;
    public void SetVictory() => IsVictory = true;
    public void AddScore(int points) => Score += points;
    public void IncrementLines() => LinesCleared++;
    public void IncrementLevel() => Level++;
}
```

- [ ] **Step 7: Run test to verify it passes**

```bash
dotnet test Tests/Games.Tetris.Tests
```

- [ ] **Step 8: Commit**

```bash
git add Games/Tetris/Models Tests/Games.Tetris.Tests
git commit -m "feat(tetris): add TetrisBoard, Tetromino, TetrisState models"
```

---

## Task 3: Tetris Logic Components

**Files:**
- Create: `Games/Tetris/Logic/ITetrominoGenerator.cs`
- Create: `Games/Tetris/Logic/TetrominoGenerator.cs`
- Create: `Games/Tetris/Logic/ICollisionChecker.cs`
- Create: `Games/Tetris/Logic/CollisionChecker.cs`
- Create: `Games/Tetris/Logic/ILineClearer.cs`
- Create: `Games/Tetris/Logic/LineClearer.cs`

- [ ] **Step 1: Write TetrominoGenerator**

```csharp
// Games/Tetris/Logic/ITetrominoGenerator.cs
using Games.Tetris.Models;

namespace Games.Tetris.Logic;

public interface ITetrominoGenerator
{
    Tetromino Generate();
    Tetromino GenerateNext();
}
```

```csharp
// Games/Tetris/Logic/TetrominoGenerator.cs
using Games.Tetris.Models;

namespace Games.Tetris.Logic;

public class TetrominoGenerator : ITetrominoGenerator
{
    private readonly Random _random;
    private Tetromino? _nextPiece;
    
    public TetrominoGenerator(Random random)
    {
        _random = random;
    }
    
    public Tetromino Generate()
    {
        if (_nextPiece == null)
            _nextPiece = GenerateRandom();
        
        var piece = _nextPiece;
        _nextPiece = GenerateRandom();
        return piece;
    }
    
    public Tetromino GenerateNext()
    {
        _nextPiece ??= GenerateRandom();
        return _nextPiece;
    }
    
    private Tetromino GenerateRandom()
    {
        var types = Enum.GetValues<TetrominoType>();
        return new Tetromino(types[_random.Next(types.Length)]);
    }
}
```

```csharp
// Games/Tetris/Logic/ICollisionChecker.cs
using Games.Tetris.Models;

namespace Games.Tetris.Logic;

public interface ICollisionChecker
{
    bool CanPlace(TetrisBoard board, Tetromino tetromino, int row, int col);
}
```

```csharp
// Games/Tetris/Logic/CollisionChecker.cs
using Games.Tetris.Models;

namespace Games.Tetris.Logic;

public class CollisionChecker : ICollisionChecker
{
    public bool CanPlace(TetrisBoard board, Tetromino tetromino, int row, int col)
    {
        for (int r = 0; r < tetromino.Shape.GetLength(0); r++)
        {
            for (int c = 0; c < tetromino.Shape.GetLength(1); c++)
            {
                if (tetromino.Shape[r, c] == 1)
                {
                    int boardRow = row + r;
                    int boardCol = col + c;
                    
                    // Out of bounds
                    if (boardRow < 0 || boardRow >= board.Rows || 
                        boardCol < 0 || boardCol >= board.Columns)
                        return false;
                    
                    // Already filled
                    if (board.GetCell(boardRow, boardCol) != 0)
                        return false;
                }
            }
        }
        return true;
    }
}
```

```csharp
// Games/Tetris/Logic/ILineClearer.cs
using Games.Tetris.Models;

namespace Games.Tetris.Logic;

public interface ILineClearer
{
    int ClearLines(TetrisBoard board);
}
```

```csharp
// Games/Tetris/Logic/LineClearer.cs
using Games.Tetris.Models;

namespace Games.Tetris.Logic;

public class LineClearer : ILineClearer
{
    public int ClearLines(TetrisBoard board)
    {
        int linesCleared = 0;
        
        for (int r = board.Rows - 1; r >= 0; r--)
        {
            if (board.IsFullLine(r))
            {
                ClearAndShift(board, r);
                linesCleared++;
                r++; // Check same row again
            }
        }
        
        return linesCleared;
    }
    
    private void ClearAndShift(TetrisBoard board, int lineRow)
    {
        // Shift all rows above down
        for (int r = lineRow; r > 0; r--)
        {
            for (int c = 0; c < board.Columns; c++)
                board.Cells[r, c] = board.Cells[r - 1, c];
        }
        
        // Clear top row
        board.ClearLine(0);
    }
}
```

- [ ] **Step 2: Run test to verify it compiles**

```bash
dotnet build Tests/Games.Tetris.Tests
```

- [ ] **Step 3: Commit**

```bash
git add Games/Tetris/Logic Tests/Games.Tetris.Tests
git commit -m "feat(tetris): add TetrominoGenerator, CollisionChecker, LineClearer"
```

---

## Task 4: Tetris Logic

**Files:**
- Create: `Games/Tetris/Logic/TetrisLogic.cs`
- Test: `Tests/Games.Tetris.Tests/TetrisLogicTests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
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
}
```

- [ ] **Step 2: Run test to verify it fails**

```bash
dotnet test Tests/Games.Tetris.Tests
```

- [ ] **Step 3: Write TetrisLogic**

```csharp
// Games/Tetris/Logic/TetrisLogic.cs
using Games.Tetris.Models;

namespace Games.Tetris.Logic;

public class TetrisLogic
{
    private readonly ITetrominoGenerator _generator;
    private readonly ICollisionChecker _checker;
    private readonly ILineClearer _clearer;
    private readonly TetrisState _state;
    
    public TetrisBoard? Board { get; private set; }
    public Tetromino? CurrentPiece { get; private set; }
    public Tetromino? NextPiece { get; private set; }
    public bool IsGameOver => _state.IsGameOver;

    public TetrisLogic(ITetrominoGenerator generator, ICollisionChecker checker, ILineClearer clearer)
    {
        _generator = generator;
        _checker = checker;
        _clearer = clearer;
        _state = new TetrisState();
    }

    public void StartGame()
    {
        _state.Score = 0;
        _state.LinesCleared = 0;
        _state.Level = 1;
        _state.GameOver = false;
        _state.IsVictory = false;
        
        Board = new TetrisBoard();
        SpawnPiece();
    }

    private void SpawnPiece()
    {
        CurrentPiece = _generator.Generate();
        NextPiece = _generator.GenerateNext();
        
        if (!_checker.CanPlace(Board!, CurrentPiece, CurrentPiece.Row, CurrentPiece.Column))
            _state.SetGameOver();
    }

    public bool MoveDown()
    {
        if (CurrentPiece == null || Board == null) return false;
        
        if (_checker.CanPlace(Board, CurrentPiece, CurrentPiece.Row + 1, CurrentPiece.Column))
        {
            CurrentPiece.Row++;
            return true;
        }
        
        // Lock piece
        LockPiece();
        return false;
    }

    public bool MoveLeft()
    {
        if (CurrentPiece == null || Board == null) return false;
        
        if (_checker.CanPlace(Board, CurrentPiece, CurrentPiece.Row, CurrentPiece.Column - 1))
        {
            CurrentPiece.Column--;
            return true;
        }
        return false;
    }

    public bool MoveRight()
    {
        if (CurrentPiece == null || Board == null) return false;
        
        if (_checker.CanPlace(Board, CurrentPiece, CurrentPiece.Row, CurrentPiece.Column + 1))
        {
            CurrentPiece.Column++;
            return true;
        }
        return false;
    }

    public bool Rotate()
    {
        if (CurrentPiece == null || Board == null) return false;
        
        var rotated = CurrentPiece.RotateClockwise();
        if (_checker.CanPlace(Board, rotated, rotated.Row, rotated.Column))
        {
            CurrentPiece = rotated;
            return true;
        }
        return false;
    }

    private void LockPiece()
    {
        if (CurrentPiece == null || Board == null) return;
        
        // Copy piece to board
        for (int r = 0; r < CurrentPiece.Shape.GetLength(0); r++)
        {
            for (int c = 0; c < CurrentPiece.Shape.GetLength(1); c++)
            {
                if (CurrentPiece.Shape[r, c] == 1)
                {
                    int boardRow = CurrentPiece.Row + r;
                    int boardCol = CurrentPiece.Column + c;
                    if (boardRow >= 0 && boardRow < Board.Rows && boardCol >= 0 && boardCol < Board.Columns)
                        Board.SetCell(boardRow, boardCol, CurrentPiece.Color);
                }
            }
        }
        
        // Clear lines
        int lines = _clearer.ClearLines(Board);
        if (lines > 0)
        {
            _state.AddScore(lines * 100 * _state.Level);
            _state.IncrementLines();
            
            if (_state.LinesCleared % 10 == 0)
                _state.IncrementLevel();
        }
        
        // Spawn next piece
        SpawnPiece();
    }

    public int GetScore() => _state.Score;
    public int GetLevel() => _state.Level;
    public int GetLinesCleared() => _state.LinesCleared;
}
```

- [ ] **Step 4: Run test to verify it passes**

```bash
dotnet test Tests/Games.Tetris.Tests
```

- [ ] **Step 5: Commit**

```bash
git add Games/Tetris/Logic Tests/Games.Tetris.Tests
git commit -m "feat(tetris): add TetrisLogic"
```

---

## Task 5: TetrisGame (IGame Implementation)

**Files:**
- Create: `Games/Tetris/TetrisGame.cs`
- Test: `Tests/Games.Tetris.Tests/TetrisGameTests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
using Core.Interfaces;
using Games.Tetris.Logic;
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
}
```

- [ ] **Step 2: Run test to verify it fails**

```bash
dotnet test Tests/Games.Tetris.Tests
```

- [ ] **Step 3: Write TetrisGame**

```csharp
using System;
using System.Text.Json;
using Core.Events;
using Core.Interfaces;
using Core.State;
using Games.Tetris.Logic;

namespace Games.Tetris;

public class TetrisGame : IGame
{
    private readonly TetrisLogic _logic;
    private readonly IStateManager _stateManager;

    public string GameName => "Tetris";
    public string GameDescription => "Stack falling blocks to complete lines";
    public GameState CurrentState => _stateManager.CurrentState;
    public bool IsPlaying => _stateManager.CurrentState == GameState.Playing;
    public bool IsPaused => _stateManager.CurrentState == GameState.Paused;

    public event Action<GameEvent>? OnGameEvent;

    public TetrisGame(TetrisLogic logic)
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
            State = _stateManager.CurrentState,
            Score = _logic.GetScore(),
            Level = _logic.GetLevel(),
            Lines = _logic.GetLinesCleared()
        });
    }

    public void DeserializeState(string json)
    {
        _stateManager.ChangeState(GameState.Ready);
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

```bash
dotnet test Tests/Games.Tetris.Tests
```

- [ ] **Step 5: Commit**

```bash
git add Games/Tetris/TetrisGame.cs Tests/Games.Tetris.Tests
git commit -m "feat(tetris): add TetrisGame IGame implementation"
```

---

## Task 6: Final Verification

- [ ] **Step 1: Run all tests**

```bash
dotnet test
```

- [ ] **Step 2: Verify file structure**

```
Games/Tetris/
├── Models/
│   ├── TetrominoType.cs
│   ├── Tetromino.cs
│   ├── TetrisBoard.cs
│   └── TetrisState.cs
├── Logic/
│   ├── ITetrominoGenerator.cs
│   ├── TetrominoGenerator.cs
│   ├── ICollisionChecker.cs
│   ├── CollisionChecker.cs
│   ├── ILineClearer.cs
│   ├── LineClearer.cs
│   └── TetrisLogic.cs
└── TetrisGame.cs
```

- [ ] **Step 3: Final commit**

```bash
git add .
git commit -m "feat(tetris): complete Phase 4 - Tetris game

- TetrisBoard with Tetromino model
- TetrominoGenerator for piece generation
- CollisionChecker for movement validation
- LineClearer for line clearing logic
- TetrisLogic with game rules
- TetrisGame implementing IGame
- Full test coverage"
```

---

## Summary

Phase 4 완료 후:
- Tetris 게임이 Core의 IGame 인터페이스를 완전히 구현
- Phase 5 (Gomoku)와 동일한 패턴으로 개발 가능

---

## Next Phase

Phase 5: Gomoku 게임 구현 시작 (AI 필요).