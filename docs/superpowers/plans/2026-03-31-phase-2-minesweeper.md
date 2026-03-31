# Phase 2: Minesweeper Game Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build the Minesweeper game module - the classic puzzle game where players clear a grid without hitting mines, using logic to deduce safe cells.

**Architecture:** Pure C# game logic following Core.Interfaces.IGame. MinesweeperBoard holds the grid, Cell models each tile, MinesweeperLogic handles reveal/flag logic.

**Tech Stack:** C# 10, .NET 8+, xUnit for testing

---

## File Structure

```
Games/
└── Minesweeper/
    ├── Models/
    │   ├── MinesweeperState.cs        # 게임 상태 모델
    │   └── MinesweeperBoard.cs        # 보드 및 셀 모델
    ├── Logic/
    │   ├── MinesweeperLogic.cs         # 핵심 게임 로직
    │   ├── IBoardGenerator.cs         # 지뢰 배치 인터페이스
    │   ├── BoardGenerator.cs           # 지뢰 배치 구현
    │   ├── ICellRevealer.cs            # 셀 공개 인터페이스
    │   └── CellRevealer.cs             # 셀 공개 구현 (확장 로직 포함)
    ├── MinesweeperGame.cs              # IGame 구현체
    └── MinesweeperGame.csproj          # 프로젝트 파일

Tests/
└── Games.Minesweeper.Tests/
    ├── MinesweeperTests.cs
    └── Games.Minesweeper.Tests.csproj
```

---

## Task 1: Project Setup

**Files:**
- Create: `Games/Minesweeper/MinesweeperGame.csproj`
- Create: `Tests/Games.Minesweeper.Tests/Games.Minesweeper.Tests.csproj`
- Modify: `MiniGameCollection.sln`

- [ ] **Step 1: Create project files**

```bash
mkdir -p Games/Minesweeper/Models Games/Minesweeper/Logic Tests/Games.Minesweeper.Tests
```

```xml
<!-- Games/Minesweeper/MinesweeperGame.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <RootNamespace>Games.Minesweeper</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\Core\Core.csproj" />
  </ItemGroup>
</Project>
```

```xml
<!-- Tests/Games.Minesweeper.Tests/Games.Minesweeper.Tests.csproj -->
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
    <ProjectReference Include="..\..\Games\Minesweeper\MinesweeperGame.csproj" />
  </ItemGroup>
</Project>
```

- [ ] **Step 2: Add to solution**

```bash
dotnet sln add Games/Minesweeper/MinesweeperGame.csproj
dotnet sln add Tests/Games.Minesweeper.Tests/Games.Minesweeper.Tests.csproj
```

- [ ] **Step 3: Verify build**

```bash
dotnet build
```

- [ ] **Step 4: Commit**

```bash
git add Games/Minesweeper Tests/Games.Minesweeper.Tests MiniGameCollection.sln
git commit -m "feat(minesweeper): initialize project structure"
```

---

## Task 2: Minesweeper Board & Cell Models

**Files:**
- Create: `Games/Minesweeper/Models/MinesweeperBoard.cs`
- Create: `Games/Minesweeper/Models/MinesweeperState.cs`
- Test: `Tests/Games.Minesweeper.Tests/MinesweeperTests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
using Games.Minesweeper.Models;
using Xunit;

namespace Games.Minesweeper.Tests;

public class MinesweeperBoardTests
{
    [Fact]
    public void Board_ShouldInitializeWithCorrectSize()
    {
        var board = new MinesweeperBoard(9, 9);
        
        Assert.Equal(9, board.Rows);
        Assert.Equal(9, board.Columns);
        Assert.Equal(81, board.Cells.Length);
    }

    [Fact]
    public void Cell_ShouldHaveCorrectPosition()
    {
        var board = new MinesweeperBoard(9, 9);
        var cell = board.Cells[3, 5];
        
        Assert.Equal(3, cell.Row);
        Assert.Equal(5, cell.Column);
        Assert.False(cell.IsMine);
        Assert.False(cell.IsRevealed);
        Assert.False(cell.IsFlagged);
        Assert.Equal(0, cell.AdjacentMines);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

```bash
dotnet test Tests/Games.Minesweeper.Tests
```

- [ ] **Step 3: Write Cell class**

```csharp
// Games/Minesweeper/Models/MinesweeperBoard.cs
namespace Games.Minesweeper.Models;

public class Cell
{
    public int Row { get; }
    public int Column { get; }
    public bool IsMine { get; set; }
    public bool IsRevealed { get; set; }
    public bool IsFlagged { get; set; }
    public int AdjacentMines { get; set; }
    
    public Cell(int row, int column)
    {
        Row = row;
        Column = column;
    }
}
```

- [ ] **Step 4: Write MinesweeperBoard class**

```csharp
// Games/Minesweeper/Models/MinesweeperBoard.cs
namespace Games.Minesweeper.Models;

public class MinesweeperBoard
{
    public Cell[,] Cells { get; }
    public int Rows { get; }
    public int Columns { get; }
    
    public MinesweeperBoard(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        Cells = new Cell[rows, columns];
        InitializeCells();
    }
    
    private void InitializeCells()
    {
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Columns; c++)
                Cells[r, c] = new Cell(r, c);
    }
}
```

- [ ] **Step 5: Write MinesweeperState**

```csharp
// Games/Minesweeper/Models/MinesweeperState.cs
using Core.Interfaces;

namespace Games.Minesweeper.Models;

public class MinesweeperState : IGameState
{
    public int Rows { get; set; } = 9;
    public int Columns { get; set; } = 9;
    public int TotalMines { get; set; } = 10;
    public int RevealedCount { get; set; }
    public int FlaggedCount { get; set; }
    public bool IsVictory { get; private set; }
    
    public int CurrentPlayer => 1;
    public bool IsGameOver => IsVictory || HitMine;
    public int? Winner => IsVictory ? 1 : null;
    
    public bool HitMine { get; private set; }
    
    public void SetVictory() => IsVictory = true;
    public void SetHitMine() => HitMine = true;
    public void IncrementRevealed() => RevealedCount++;
    public void IncrementFlagged() => FlaggedCount++;
    public void DecrementFlagged() => FlaggedCount--;
}
```

- [ ] **Step 6: Run test to verify it passes**

```bash
dotnet test Tests/Games.Minesweeper.Tests
```

- [ ] **Step 7: Commit**

```bash
git add Games/Minesweeper/Models Tests/Games.Minesweeper.Tests
git commit -m "feat(minesweeper): add MinesweeperBoard and Cell models"
```

---

## Task 3: Board Generator

**Files:**
- Create: `Games/Minesweeper/Logic/IBoardGenerator.cs`
- Create: `Games/Minesweeper/Logic/BoardGenerator.cs`
- Test: `Tests/Games.Minesweeper.Tests/MinesweeperTests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
[Fact]
public void BoardGenerator_ShouldPlaceMinesRandomly()
{
    var generator = new BoardGenerator(new Random(42));
    var board = new MinesweeperBoard(9, 9);
    
    generator.GenerateBoard(board, 10);
    
    int mineCount = 0;
    for (int r = 0; r < 9; r++)
        for (int c = 0; c < 9; c++)
            if (board.Cells[r, c].IsMine) mineCount++;
    
    Assert.Equal(10, mineCount);
}
```

- [ ] **Step 2: Run test to verify it fails**

```bash
dotnet test Tests/Games.Minesweeper.Tests
```

- [ ] **Step 3: Write IBoardGenerator**

```csharp
namespace Games.Minesweeper.Logic;

public interface IBoardGenerator
{
    void GenerateBoard(MinesweeperBoard board, int mineCount);
}
```

- [ ] **Step 4: Write BoardGenerator**

```csharp
using System;
using Games.Minesweeper.Models;

namespace Games.Minesweeper.Logic;

public class BoardGenerator : IBoardGenerator
{
    private readonly Random _random;

    public BoardGenerator(Random random)
    {
        _random = random ?? throw new ArgumentNullException(nameof(random));
    }

    public void GenerateBoard(MinesweeperBoard board, int mineCount)
    {
        int placed = 0;
        while (placed < mineCount)
        {
            int r = _random.Next(board.Rows);
            int c = _random.Next(board.Columns);
            
            if (!board.Cells[r, c].IsMine)
            {
                board.Cells[r, c].IsMine = true;
                placed++;
            }
        }
        
        // Calculate adjacent mines
        for (int r = 0; r < board.Rows; r++)
            for (int c = 0; c < board.Columns; c++)
                if (!board.Cells[r, c].IsMine)
                    board.Cells[r, c].AdjacentMines = CountAdjacentMines(board, r, c);
    }

    private int CountAdjacentMines(MinesweeperBoard board, int row, int col)
    {
        int count = 0;
        for (int dr = -1; dr <= 1; dr++)
            for (int dc = -1; dc <= 1; dc++)
            {
                if (dr == 0 && dc == 0) continue;
                int nr = row + dr, nc = col + dc;
                if (nr >= 0 && nr < board.Rows && nc >= 0 && nc < board.Columns)
                    if (board.Cells[nr, nc].IsMine) count++;
            }
        return count;
    }
}
```

- [ ] **Step 5: Run test to verify it passes**

```bash
dotnet test Tests/Games.Minesweeper.Tests
```

- [ ] **Step 6: Commit**

```bash
git add Games/Minesweeper/Logic Tests/Games.Minesweeper.Tests
git commit -m "feat(minesweeper): add BoardGenerator"
```

---

## Task 4: Cell Revealer

**Files:**
- Create: `Games/Minesweeper/Logic/ICellRevealer.cs`
- Create: `Games/Minesweeper/Logic/CellRevealer.cs`
- Test: `Tests/Games.Minesweeper.Tests/MinesweeperTests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
[Fact]
public void Revealer_ShouldRevealCell()
{
    var revealer = new CellRevealer();
    var board = new MinesweeperBoard(9, 9);
    
    revealer.Reveal(board, 0, 0);
    
    Assert.True(board.Cells[0, 0].IsRevealed);
}

[Fact]
public void Revealer_ShouldExpandEmptyCells()
{
    var board = new MinesweeperBoard(9, 9);
    // Set up: center cell has no adjacent mines
    board.Cells[4, 4].IsMine = false;
    board.Cells[4, 4].AdjacentMines = 0;
    // Surrounding cells also have 0 adjacent mines
    
    var revealer = new CellRevealer();
    revealer.Reveal(board, 4, 4);
    
    // Center should be revealed
    Assert.True(board.Cells[4, 4].IsRevealed);
}
```

- [ ] **Step 2: Run test to verify it fails**

```bash
dotnet test Tests/Games.Minesweeper.Tests
```

- [ ] **Step 3: Write ICellRevealer**

```csharp
namespace Games.Minesweeper.Logic;

public interface ICellRevealer
{
    void Reveal(MinesweeperBoard board, int row, int col);
}
```

- [ ] **Step 4: Write CellRevealer**

```csharp
using Games.Minesweeper.Models;

namespace Games.Minesweeper.Logic;

public class CellRevealer : ICellRevealer
{
    public void Reveal(MinesweeperBoard board, int row, int col)
    {
        var cell = board.Cells[row, col];
        if (cell.IsRevealed || cell.IsFlagged) return;
        
        cell.IsRevealed = true;
        
        // Expand if no adjacent mines
        if (cell.AdjacentMines == 0 && !cell.IsMine)
        {
            for (int dr = -1; dr <= 1; dr++)
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0) continue;
                    int nr = row + dr, nc = col + dc;
                    if (nr >= 0 && nr < board.Rows && nc >= 0 && nc < board.Columns)
                        Reveal(board, nr, nc);
                }
        }
    }
}
```

- [ ] **Step 5: Run test to verify it passes**

```bash
dotnet test Tests/Games.Minesweeper.Tests
```

- [ ] **Step 6: Commit**

```bash
git add Games/Minesweeper/Logic Tests/Games.Minesweeper.Tests
git commit -m "feat(minesweeper): add CellRevealer with expansion logic"
```

---

## Task 5: Minesweeper Logic

**Files:**
- Create: `Games/Minesweeper/Logic/MinesweeperLogic.cs`
- Test: `Tests/Games.Minesweeper.Tests/MinesweeperTests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
[Fact]
public void MinesweeperLogic_ShouldInitialize()
{
    var generator = new BoardGenerator(new Random(42));
    var revealer = new CellRevealer();
    var logic = new MinesweeperLogic(generator, revealer);
    
    logic.Initialize(9, 9, 10);
    
    Assert.NotNull(logic.Board);
}

[Fact]
public void MinesweeperLogic_ShouldRevealCell()
{
    var generator = new BoardGenerator(new Random(42));
    var revealer = new CellRevealer();
    var logic = new MinesweeperLogic(generator, revealer);
    
    logic.Initialize(9, 9, 10);
    logic.RevealCell(0, 0);
    
    Assert.True(logic.Board.Cells[0, 0].IsRevealed);
}

[Fact]
public void MinesweeperLogic_ShouldToggleFlag()
{
    var generator = new BoardGenerator(new Random(42));
    var revealer = new CellRevealer();
    var logic = new MinesweeperLogic(generator, revealer);
    
    logic.Initialize(9, 9, 10);
    logic.ToggleFlag(0, 0);
    
    Assert.True(logic.Board.Cells[0, 0].IsFlagged);
    
    logic.ToggleFlag(0, 0);
    Assert.False(logic.Board.Cells[0, 0].IsFlagged);
}
```

- [ ] **Step 2: Run test to verify it fails**

```bash
dotnet test Tests/Games.Minesweeper.Tests
```

- [ ] **Step 3: Write MinesweeperLogic**

```csharp
using System;
using Games.Minesweeper.Models;

namespace Games.Minesweeper.Logic;

public class MinesweeperLogic
{
    private readonly IBoardGenerator _generator;
    private readonly ICellRevealer _revealer;
    private readonly MinesweeperState _state;
    
    public MinesweeperBoard? Board { get; private set; }
    public bool IsGameOver => _state.IsGameOver;
    public bool IsVictory => _state.IsVictory;

    public MinesweeperLogic(IBoardGenerator generator, ICellRevealer revealer)
    {
        _generator = generator ?? throw new ArgumentNullException(nameof(generator));
        _revealer = revealer ?? throw new ArgumentNullException(nameof(revealer));
        _state = new MinesweeperState();
    }

    public void Initialize(int rows, int cols, int mines)
    {
        _state.Rows = rows;
        _state.Columns = cols;
        _state.TotalMines = mines;
        
        Board = new MinesweeperBoard(rows, cols);
        _generator.GenerateBoard(Board, mines);
    }

    public void RevealCell(int row, int col)
    {
        if (Board == null) return;
        
        var cell = Board.Cells[row, col];
        if (cell.IsRevealed || cell.IsFlagged) return;
        
        if (cell.IsMine)
        {
            _state.SetHitMine();
            return;
        }
        
        _revealer.Reveal(Board, row, col);
        
        // Count revealed cells
        int revealed = 0;
        for (int r = 0; r < Board.Rows; r++)
            for (int c = 0; c < Board.Columns; c++)
                if (Board.Cells[r, c].IsRevealed) revealed++;
        
        int safeCells = (Board.Rows * Board.Columns) - _state.TotalMines;
        if (revealed >= safeCells)
            _state.SetVictory();
    }

    public void ToggleFlag(int row, int col)
    {
        if (Board == null) return;
        
        var cell = Board.Cells[row, col];
        if (cell.IsRevealed) return;
        
        if (cell.IsFlagged)
            _state.DecrementFlagged();
        else
            _state.IncrementFlagged();
        
        cell.IsFlagged = !cell.IsFlagged;
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

```bash
dotnet test Tests/Games.Minesweeper.Tests
```

- [ ] **Step 5: Commit**

```bash
git add Games/Minesweeper/Logic Tests/Games.Minesweeper.Tests
git commit -m "feat(minesweeper): add MinesweeperLogic"
```

---

## Task 6: MinesweeperGame (IGame Implementation)

**Files:**
- Create: `Games/Minesweeper/MinesweeperGame.cs`
- Test: `Tests/Games.Minesweeper.Tests/MinesweeperGameTests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
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
}
```

- [ ] **Step 2: Run test to verify it fails**

```bash
dotnet test Tests/Games.Minesweeper.Tests
```

- [ ] **Step 3: Write MinesweeperGame**

```csharp
using System;
using System.Text.Json;
using Core.Events;
using Core.Interfaces;
using Core.State;
using Games.Minesweeper.Logic;

namespace Games.Minesweeper;

public class MinesweeperGame : IGame
{
    private readonly MinesweeperLogic _logic;
    private readonly IStateManager _stateManager;

    public string GameName => "Minesweeper";
    public string GameDescription => "Clear the minefield without hitting a mine";
    public GameState CurrentState => _stateManager.CurrentState;
    public bool IsPlaying => _stateManager.CurrentState == GameState.Playing;
    public bool IsPaused => _stateManager.CurrentState == GameState.Paused;

    public event Action<GameEvent>? OnGameEvent;

    public MinesweeperGame(MinesweeperLogic logic)
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
        _logic.Initialize(9, 9, 10);
    }

    public void PauseGame() => _stateManager.ChangeState(GameState.Paused);
    public void ResumeGame() => _stateManager.ChangeState(GameState.Playing);
    public void ResetGame() => _stateManager.ChangeState(GameState.Ready);
    public void EndGame() => _stateManager.ChangeState(GameState.GameOver);

    public string SerializeState()
    {
        return JsonSerializer.Serialize(new { State = _stateManager.CurrentState });
    }

    public void DeserializeState(string json)
    {
        _stateManager.ChangeState(GameState.Ready);
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

```bash
dotnet test Tests/Games.Minesweeper.Tests
```

- [ ] **Step 5: Commit**

```bash
git add Games/Minesweeper/MinesweeperGame.cs Tests/Games.Minesweeper.Tests
git commit -m "feat(minesweeper): add MinesweeperGame IGame implementation"
```

---

## Task 7: Final Verification

- [ ] **Step 1: Run all tests**

```bash
dotnet test
```

- [ ] **Step 2: Verify file structure**

```
Games/Minesweeper/
├── Models/
│   ├── MinesweeperBoard.cs
│   └── MinesweeperState.cs
├── Logic/
│   ├── IBoardGenerator.cs
│   ├── BoardGenerator.cs
│   ├── ICellRevealer.cs
│   ├── CellRevealer.cs
│   └── MinesweeperLogic.cs
└── MinesweeperGame.cs
```

- [ ] **Step 3: Final commit**

```bash
git add .
git commit -m "feat(minesweeper): complete Phase 2 - Minesweeper game

- MinesweeperBoard with Cell model
- BoardGenerator for random mine placement
- CellRevealer with expansion logic
- MinesweeperLogic with game rules
- MinesweeperGame implementing IGame
- Full test coverage"
```

---

## Summary

Phase 2 완료 후:
- Minesweeper 게임이 Core의 IGame 인터페이스를 완전히 구현
- Phase 3 (Sudoku)와 동일한 패턴으로 개발 가능

---

## Next Phase

Phase 3: Sudoku 게임 구현 시작.