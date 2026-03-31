# Mini Game Collection Design Document

**Date:** 2026-03-31  
**Author:** AI Assistant  
**Target Platform:** Blazor WebAssembly  

---

## 1. Project Overview

### 1.1 Purpose
미니게임 컬렉션 웹 애플리케이션. 6개의 독립적인 미니게임을 포함하며, 사용자가 원하는 게임을 선택해서 플레이 가능.

### 1.2 Goals
- Unity/플랫폼 의존 없는 순수 C# Core 게임 로직
- Blazor WebAssembly로 웹에서 실행
- SOLID 원칙과 DI 준수
- 포트폴리오 웹에 통합 가능

### 1.3 Games Included
| Game | Description | AI |
|------|-------------|----|
| Pattern Memory | 패턴 기억 게임 | No |
| Minesweeper | 지뢰찾기 | No |
| Sudoku | 스도쿠 퍼즐 | No |
| Tetris | 테트리스 | No |
| Gomoku | 오목 | Yes |
| Chess | 체스 | Yes |

---

## 2. Architecture

### 2.1 Project Structure

```
MiniGameCollection/
├── Core/
│   ├── Interfaces/
│   │   ├── IGame.cs
│   │   ├── IGameInfo.cs
│   │   ├── IGameControllable.cs
│   │   ├── IGameSerializable.cs
│   │   ├── IGameEventEmitter.cs
│   │   └── IGameState.cs
│   ├── State/
│   │   ├── GameState.cs
│   │   ├── IStateManager.cs
│   │   ├── StateManager.cs
│   │   └── IStateTransitionRule.cs
│   ├── Events/
│   │   ├── GameEvent.cs
│   │   ├── IEventBus.cs
│   │   └ EventBus.cs
│   │   ├── ScoreChangedEvent.cs
│   │   └ GameOverEvent.cs
│   ├── Data/
│   │   ├── ISerializer.cs
│   │   ├── ISaveStorage.cs
│   │   ├── ISaveSystem.cs
│   │   ├── JsonSerializer.cs
│   │   ├── FileStorage.cs
│   │   ├── SaveSystem.cs
│   │   └ GameSettings.cs
│   ├── DI/
│   │   ├── IServiceContainer.cs
│   │   └ ServiceContainer.cs
│   └── AI/
│       ├── IAIPlayer.cs
│       ├── IGameStateEvaluator.cs
│       ├── MinimaxAI.cs
│       ├── IAIPlayerFactory.cs
│       └ AIPlayerFactory.cs
│
├── Games/
│   ├── PatternMemory/
│   ├── Minesweeper/
│   ├── Sudoku/
│   ├── Tetris/
│   ├── Gomoku/
│   └── Chess/
│
├── BlazorAdapter/
│   ├── DI/
│   │   └ GameDependencyContainer.cs
│   ├── Components/
│   │   ├── Layout/
│   │   │   ├── MainLayout.razor
│   │   │   └ NavigationMenu.razor
│   │   ├── Pages/
│   │   │   ├── Index.razor
│   │   │   ├── PatternMemory.razor
│   │   │   ├── Minesweeper.razor
│   │   │   ├── Sudoku.razor
│   │   │   ├── Tetris.razor
│   │   │   ├── Gomoku.razor
│   │   │   └ Chess.razor
│   │   ├── Shared/
│   │   │   ├── GameCard.razor
│   │   │   ├── ScoreDisplay.razor
│   │   │   └ StatusPanel.razor
│   ├── Services/
│   │   ├── BlazorEventBridge.cs
│   │   ├── LocalStorageAdapter.cs
│   │   └ TimerService.cs
│   └ wwwroot/
│       ├── css/
│       ├── js/
│       └ index.html
│
└── Tests/
    ├── Core.Tests/
    └── Games.Tests/
```

---

## 3. Core Interfaces

### 3.1 IGame Interface (ISP Applied)

```csharp
namespace Core.Interfaces
{
    public interface IGameInfo
    {
        string GameName { get; }
        string GameDescription { get; }
    }
    
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
    
    public interface IGameSerializable
    {
        string SerializeState();
        void DeserializeState(string json);
    }
    
    public interface IGameEventEmitter
    {
        event Action<GameEvent> OnGameEvent;
    }
    
    public interface IGame : IGameInfo, IGameControllable, 
                              IGameSerializable, IGameEventEmitter
    {
    }
    
    public enum GameState
    {
        None,
        Ready,
        Playing,
        Paused,
        GameOver,
        Victory
    }
}
```

---

## 4. State Management

### 4.1 State Manager (SRP, DIP)

```csharp
namespace Core.State
{
    public interface IStateManager
    {
        GameState CurrentState { get; }
        event Action<GameState, GameState> OnStateChanged;
        void ChangeState(GameState newState);
        bool CanTransitionTo(GameState target);
    }
    
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
        public event Action<GameState, GameState> OnStateChanged;
        
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
    
    public interface IStateTransitionRule
    {
        bool CanTransition(GameState from, GameState to);
    }
    
    public class DefaultStateTransitionRule : IStateTransitionRule
    {
        public bool CanTransition(GameState from, GameState to)
        {
            return (from, to) switch
            {
                (GameState.Ready, GameState.Playing) => true,
                (GameState.Playing, GameState.Paused) => true,
                (GameState.Playing, GameState.GameOver) => true,
                (GameState.Playing, GameState.Victory) => true,
                (GameState.Paused, GameState.Playing) => true,
                (GameState.GameOver, GameState.Ready) => true,
                (GameState.Victory, GameState.Ready) => true,
                _ => false
            };
        }
    }
}
```

---

## 5. Event System

### 5.1 Event Bus (DIP)

```csharp
namespace Core.Events
{
    public abstract class GameEvent
    {
        public string EventType { get; protected set; }
        public float Timestamp { get; }
        public Dictionary<string, object> Data { get; }
        
        protected GameEvent(string type)
        {
            EventType = type;
            Timestamp = DateTime.Now.Ticks;
            Data = new Dictionary<string, object>();
        }
    }
    
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
    
    public interface IEventBus
    {
        void Subscribe<T>(Action<T> handler) where T : GameEvent;
        void Unsubscribe<T>(Action<T> handler) where T : GameEvent;
        void Publish<T>(T gameEvent) where T : GameEvent;
    }
    
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
            if (_handlers.ContainsKey(type))
                _handlers[type].Remove(handler);
        }
        
        public void Publish<T>(T gameEvent) where T : GameEvent
        {
            var type = typeof(T);
            if (_handlers.TryGetValue(type, out var handlers))
            {
                foreach (var handler in handlers.ToList())
                    (handler as Action<T>)?.Invoke(gameEvent);
            }
        }
    }
}
```

---

## 6. Save System

### 6.1 Save System (SRP, DIP)

```csharp
namespace Core.Data
{
    public interface ISerializer
    {
        string Serialize<T>(T data);
        T Deserialize<T>(string json);
    }
    
    public interface ISaveStorage
    {
        void Save(string key, string data);
        string Load(string key);
        bool Exists(string key);
        void Delete(string key);
    }
    
    public interface ISaveSystem
    {
        void SaveGame<T>(string gameId, T gameState);
        T LoadGame<T>(string gameId);
        bool HasSavedGame(string gameId);
        void DeleteSavedGame(string gameId);
        void SaveSettings(GameSettings settings);
        GameSettings LoadSettings();
    }
    
    public class JsonSerializer : ISerializer
    {
        public string Serialize<T>(T data) => System.Text.Json.JsonSerializer.Serialize(data);
        public T Deserialize<T>(string json) => System.Text.Json.JsonSerializer.Deserialize<T>(json);
    }
    
    public class FileStorage : ISaveStorage
    {
        private readonly string _saveDirectory;
        
        public FileStorage(string saveDirectory)
        {
            _saveDirectory = saveDirectory;
            Directory.CreateDirectory(_saveDirectory);
        }
        
        public void Save(string key, string data) => File.WriteAllText(GetFilePath(key), data);
        public string Load(string key) => File.Exists(GetFilePath(key)) ? File.ReadAllText(GetFilePath(key)) : null;
        public bool Exists(string key) => File.Exists(GetFilePath(key));
        public void Delete(string key) { if (File.Exists(GetFilePath(key))) File.Delete(GetFilePath(key)); }
        private string GetFilePath(string key) => Path.Combine(_saveDirectory, $"{key}.json");
    }
    
    public class SaveSystem : ISaveSystem
    {
        private readonly ISerializer _serializer;
        private readonly ISaveStorage _storage;
        
        public SaveSystem(ISerializer serializer, ISaveStorage storage)
        {
            _serializer = serializer;
            _storage = storage;
        }
        
        public void SaveGame<T>(string gameId, T gameState) => _storage.Save($"game_{gameId}", _serializer.Serialize(gameState));
        public T LoadGame<T>(string gameId) => _storage.Load($"game_{gameId}") != null ? _serializer.Deserialize<T>(_storage.Load($"game_{gameId}")) : default;
        public bool HasSavedGame(string gameId) => _storage.Exists($"game_{gameId}");
        public void DeleteSavedGame(string gameId) => _storage.Delete($"game_{gameId}");
        public void SaveSettings(GameSettings settings) => _storage.Save("settings", _serializer.Serialize(settings));
        public GameSettings LoadSettings() => _storage.Load("settings") != null ? _serializer.Deserialize<GameSettings>(_storage.Load("settings")) : new GameSettings();
    }
    
    public class GameSettings
    {
        public Dictionary<string, int> HighScores { get; set; } = new();
        public Dictionary<string, bool> GameProgress { get; set; } = new();
        public int TotalPlayTime { get; set; }
    }
}
```

---

## 7. AI Module

### 7.1 AI Interfaces (DIP)

```csharp
namespace Core.AI
{
    public interface IGameStateEvaluator<TMove>
    {
        int Evaluate(IGameState state);
        IEnumerable<TMove> GetValidMoves(IGameState state);
        IGameState ApplyMove(IGameState state, TMove move);
    }
    
    public interface IAIPlayer<TMove>
    {
        string Difficulty { get; set; }
        TMove GetBestMove(IGameState state, IGameStateEvaluator<TMove> evaluator);
        event Action<string> OnThinking;
    }
    
    public interface IGameState
    {
        int CurrentPlayer { get; }
        bool IsGameOver { get; }
        int? Winner { get; }
    }
    
    public interface IAIPlayerFactory
    {
        IAIPlayer<TMove> Create<TMove>(string difficulty);
    }
}
```

### 7.2 Minimax AI Implementation

```csharp
namespace Core.AI
{
    public class MinimaxAI<TMove> : IAIPlayer<TMove>
    {
        private readonly int _maxDepth;
        private readonly Random _random;
        
        public string Difficulty { get; set; } = "Normal";
        public event Action<string> OnThinking;
        
        public MinimaxAI(int maxDepth = 3)
        {
            _maxDepth = maxDepth;
            _random = new Random();
        }
        
        public TMove GetBestMove(IGameState state, IGameStateEvaluator<TMove> evaluator)
        {
            OnThinking?.Invoke("AI thinking...");
            
            var validMoves = evaluator.GetValidMoves(state).ToList();
            if (validMoves.Count == 0) throw new InvalidOperationException("No valid moves");
            if (validMoves.Count == 1) return validMoves[0];
            
            TMove bestMove = default;
            int bestScore = int.MinValue;
            
            foreach (var move in validMoves)
            {
                var newState = evaluator.ApplyMove(state, move);
                int score = Minimax(newState, _maxDepth - 1, int.MinValue, int.MaxValue, false, evaluator);
                
                if (score > bestScore || (score == bestScore && _random.Next(2) == 0))
                {
                    bestScore = score;
                    bestMove = move;
                }
            }
            return bestMove;
        }
        
        private int Minimax(IGameState state, int depth, int alpha, int beta, 
                           bool isMaximizing, IGameStateEvaluator<TMove> evaluator)
        {
            if (depth == 0 || state.IsGameOver) return evaluator.Evaluate(state);
            
            var validMoves = evaluator.GetValidMoves(state);
            
            if (isMaximizing)
            {
                int maxScore = int.MinValue;
                foreach (var move in validMoves)
                {
                    var newState = evaluator.ApplyMove(state, move);
                    int score = Minimax(newState, depth - 1, alpha, beta, false, evaluator);
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
                    int score = Minimax(newState, depth - 1, alpha, beta, true, evaluator);
                    minScore = Math.Min(minScore, score);
                    beta = Math.Min(beta, score);
                    if (beta <= alpha) break;
                }
                return minScore;
            }
        }
    }
    
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
            int depth = _difficultyDepths.GetValueOrDefault(difficulty, 3);
            return new MinimaxAI<TMove>(depth);
        }
    }
}
```

---

## 8. Game Modules

### 8.1 Common Game Module Structure

Each game follows this pattern:

```
Games/[GameName]/
├── Interfaces/
│   └── I[Game]State.cs
├── Models/
│   ├── [Game]Board.cs
│   ├── [Game]Move.cs
│   └── [Game]Config.cs
├── Logic/
│   ├── [Game]Logic.cs
│   └── [Game]Validator.cs
├── AI/ (if needed)
│   └── [Game]Evaluator.cs
└── [Game]Game.cs (IGame implementation)
```

### 8.2 Pattern Memory

```csharp
namespace Games.PatternMemory
{
    public interface IPatternMemoryState : IGameState
    {
        IReadOnlyList<int> CurrentPattern { get; }
        int CurrentLevel { get; }
        int PlayerScore { get; }
        int MistakesAllowed { get; }
    }
    
    public interface IPatternGenerator
    {
        List<int> GeneratePattern(int length);
    }
    
    public class PatternGenerator : IPatternGenerator
    {
        private readonly Random _random;
        public PatternGenerator(Random random) { _random = random; }
        public List<int> GeneratePattern(int length) => 
            Enumerable.Range(0, length).Select(_ => _random.Next(0, 9)).ToList();
    }
    
    public class PatternMemoryLogic
    {
        private readonly IPatternGenerator _generator;
        private PatternMemoryState _state;
        
        public PatternMemoryLogic(IPatternGenerator generator) 
        { _generator = generator; _state = new PatternMemoryState(); }
        
        public void NextLevel()
        {
            _state.CurrentLevel++;
            _state.Pattern = _generator.GeneratePattern(_state.CurrentLevel + 2);
        }
        
        public bool CheckInput(IReadOnlyList<int> playerInput)
        {
            for (int i = 0; i < _state.Pattern.Count; i++)
            {
                if (i >= playerInput.Count || playerInput[i] != _state.Pattern[i])
                {
                    _state.MistakesAllowed--;
                    return false;
                }
            }
            _state.PlayerScore += _state.CurrentLevel * 10;
            return true;
        }
    }
}
```

### 8.3 Minesweeper

```csharp
namespace Games.Minesweeper
{
    public interface IBoardGenerator
    {
        void GenerateBoard(MinesweeperBoard board, int mineCount);
    }
    
    public interface ICellRevealer
    {
        void Reveal(MinesweeperBoard board, int row, int col);
    }
    
    public class MinesweeperBoard
    {
        public Cell[,] Cells { get; }
        public int Rows { get; }
        public int Columns { get; }
        
        public MinesweeperBoard(int rows, int columns)
        {
            Rows = rows; Columns = columns;
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
    
    public class Cell
    {
        public int Row { get; }
        public int Column { get; }
        public bool IsMine { get; set; }
        public bool IsRevealed { get; set; }
        public bool IsFlagged { get; set; }
        public int AdjacentMines { get; set; }
    }
    
    public class MinesweeperLogic
    {
        private readonly IBoardGenerator _generator;
        private readonly ICellRevealer _revealer;
        private MinesweeperBoard _board;
        
        public MinesweeperLogic(IBoardGenerator generator, ICellRevealer revealer)
        { _generator = generator; _revealer = revealer; }
        
        public void Initialize(int rows, int cols, int mines)
        {
            _board = new MinesweeperBoard(rows, cols);
            _generator.GenerateBoard(_board, mines);
        }
        
        public void RevealCell(int row, int col) => _revealer.Reveal(_board, row, col);
        public void ToggleFlag(int row, int col)
        {
            var cell = _board.Cells[row, col];
            if (!cell.IsRevealed) cell.IsFlagged = !cell.IsFlagged;
        }
    }
}
```

### 8.4 Sudoku

```csharp
namespace Games.Sudoku
{
    public interface ISudokuGenerator
    {
        int[,] GeneratePuzzle(int difficulty);
    }
    
    public interface ISudokuValidator
    {
        bool IsValidMove(int[,] board, int row, int col, int value);
        bool IsSolved(int[,] board);
    }
    
    public interface ISudokuSolver
    {
        bool Solve(int[,] board);
    }
    
    public class SudokuLogic
    {
        private readonly ISudokuGenerator _generator;
        private readonly ISudokuValidator _validator;
        private readonly ISudokuSolver _solver;
        private int[,] _puzzle, _playerBoard;
        
        public SudokuLogic(ISudokuGenerator gen, ISudokuValidator val, ISudokuSolver sol)
        { _generator = gen; _validator = val; _solver = sol; }
        
        public void NewGame(int difficulty)
        {
            _puzzle = _generator.GeneratePuzzle(difficulty);
            _playerBoard = (int[,])_puzzle.Clone();
        }
        
        public bool PlaceNumber(int row, int col, int value)
        {
            if (_puzzle[row, col] != 0) return false;
            if (!_validator.IsValidMove(_playerBoard, row, col, value)) return false;
            _playerBoard[row, col] = value;
            return true;
        }
        
        public bool CheckVictory() => _validator.IsSolved(_playerBoard);
    }
}
```

### 8.5 Tetris

```csharp
namespace Games.Tetris
{
    public enum Direction { Left, Right, Down }
    public enum Rotation { Clockwise, CounterClockwise }
    
    public class Tetromino
    {
        public int[,] Shape { get; }
        public int Color { get; }
        public int Row { get; set; }
        public int Column { get; set; }
        
        public Tetromino(int[,] shape, int color) { Shape = shape; Color = color; }
        
        public Tetromino Rotate(Rotation rotation)
        {
            var rotated = rotation == Rotation.Clockwise 
                ? RotateClockwise(Shape) : RotateCounterClockwise(Shape);
            return new Tetromino(rotated, Color);
        }
    }
    
    public interface ITetrominoGenerator { Tetromino Generate(); }
    public interface ITetrisCollisionChecker { bool CanPlace(int[,] board, Tetromino piece, int row, int col); }
    public interface ILineClearer { int ClearLines(int[,] board); }
    
    public class TetrisLogic
    {
        private readonly ITetrominoGenerator _generator;
        private readonly ITetrisCollisionChecker _checker;
        private readonly ILineClearer _clearer;
        private int[,] _board;
        private Tetromino _current, _next;
        
        public TetrisLogic(ITetrominoGenerator gen, ITetrisCollisionChecker chk, ILineClearer clr)
        { _generator = gen; _checker = chk; _clearer = clr; }
        
        public void InitializeBoard()
        {
            _board = new int[20, 10];
            _current = _generator.Generate();
            _next = _generator.Generate();
        }
        
        public bool MovePiece(Direction dir) { /* movement logic */ }
        public bool RotatePiece(Rotation rot) { /* rotation logic */ }
        public void LockPiece() { /* lock and clear lines */ }
    }
}
```

### 8.6 Gomoku (with AI)

```csharp
namespace Games.Gomoku
{
    public class GomokuMove
    {
        public int Row { get; }
        public int Column { get; }
        public GomokuMove(int row, int column) { Row = row; Column = column; }
    }
    
    public interface IGomokuValidator
    {
        bool IsValidMove(int[,] board, int row, int col);
        bool CheckWin(int[,] board, int row, int col, int player);
    }
    
    public class GomokuEvaluator : IGameStateEvaluator<GomokuMove>
    {
        private readonly IGomokuValidator _validator;
        private const int WIN_SCORE = 100000;
        
        public GomokuEvaluator(IGomokuValidator validator) { _validator = validator; }
        
        public int Evaluate(IGameState state)
        {
            var gomoku = (IGomokuState)state;
            if (gomoku.Winner.HasValue)
                return gomoku.Winner == 1 ? WIN_SCORE : -WIN_SCORE;
            return EvaluateBoard(gomoku.Board);
        }
        
        public IEnumerable<GomokuMove> GetValidMoves(IGameState state)
        {
            var gomoku = (IGomokuState)state;
            for (int r = 0; r < gomoku.BoardSize; r++)
                for (int c = 0; c < gomoku.BoardSize; c++)
                    if (gomoku.Board[r, c] == 0)
                        yield return new GomokuMove(r, c);
        }
        
        public IGameState ApplyMove(IGameState state, GomokuMove move)
        {
            var gomoku = (IGomokuState)state;
            var newBoard = (int[,])gomoku.Board.Clone();
            newBoard[move.Row, move.Column] = gomoku.CurrentPlayer;
            return new GomokuState(newBoard, gomoku.BoardSize, gomoku.CurrentPlayer == 1 ? 2 : 1);
        }
        
        private int EvaluateBoard(int[,] board) { /* pattern evaluation */ }
    }
    
    public class GomokuLogic
    {
        private readonly IGomokuValidator _validator;
        private readonly IAIPlayer<GomokuMove> _ai;
        private readonly GomokuEvaluator _evaluator;
        private GomokuState _state;
        
        public GomokuLogic(IGomokuValidator v, IAIPlayer<GomokuMove> a, GomokuEvaluator e)
        { _validator = v; _ai = a; _evaluator = e; }
        
        public bool PlaceStone(int row, int col) { /* player move */ }
        public GomokuMove GetAIMove() => _ai.GetBestMove(_state, _evaluator);
    }
}
```

### 8.7 Chess (with AI)

```csharp
namespace Games.Chess
{
    public enum PieceType { None, Pawn, Knight, Bishop, Rook, Queen, King }
    public enum PieceColor { White, Black }
    
    public class ChessPiece
    {
        public PieceType Type { get; }
        public PieceColor Color { get; }
        public ChessPiece(PieceType type, PieceColor color) { Type = type; Color = color; }
    }
    
    public class ChessMove
    {
        public Position From { get; }
        public Position To { get; }
        public PieceType? Promotion { get; }
        public ChessMove(Position from, Position to, PieceType? promo = null)
        { From = from; To = to; Promotion = promo; }
    }
    
    public struct Position { public int Row; public int Column; }
    
    public interface IMoveGenerator { IEnumerable<ChessMove> GenerateMoves(ChessPiece[,] board, Position pos, IChessState state); }
    public interface IChessValidator { bool IsValidMove(ChessPiece[,] board, ChessMove move, IChessState state); bool IsInCheck(ChessPiece[,] board, PieceColor color); }
    
    public class ChessEvaluator : IGameStateEvaluator<ChessMove>
    {
        private readonly Dictionary<PieceType, int> _pieceValues = new()
        {
            { PieceType.Pawn, 100 }, { PieceType.Knight, 320 },
            { PieceType.Bishop, 330 }, { PieceType.Rook, 500 },
            { PieceType.Queen, 900 }, { PieceType.King, 20000 }
        };
        
        public int Evaluate(IGameState state) { /* material + position */ }
        public IEnumerable<ChessMove> GetValidMoves(IGameState state) { /* all moves */ }
        public IGameState ApplyMove(IGameState state, ChessMove move) { /* new state */ }
    }
    
    public class ChessLogic
    {
        private readonly IMoveGenerator _generator;
        private readonly IChessValidator _validator;
        private readonly IAIPlayer<ChessMove> _ai;
        private readonly ChessEvaluator _evaluator;
        
        public ChessLogic(IMoveGenerator g, IChessValidator v, IAIPlayer<ChessMove> a, ChessEvaluator e)
        { _generator = g; _validator = v; _ai = a; _evaluator = e; }
        
        public bool MakeMove(ChessMove move) { /* execute move */ }
        public ChessMove GetAIMove() => _ai.GetBestMove(_state, _evaluator);
    }
}
```

---

## 9. Blazor Adapter

### 9.1 DI Container

```csharp
namespace BlazorAdapter.DI
{
    public class GameDependencyContainer
    {
        private readonly IServiceContainer _container;
        
        public GameDependencyContainer(IServiceContainer container)
        {
            _container = container;
            RegisterCoreServices();
            RegisterGames();
        }
        
        private void RegisterCoreServices()
        {
            _container.RegisterSingleton<IEventBus, EventBus>();
            _container.RegisterSingleton<ISerializer, JsonSerializer>();
            _container.RegisterSingleton<IAIPlayerFactory, AIPlayerFactory>();
            
            // Blazor uses LocalStorage instead of FileStorage
            _container.RegisterSingleton<ISaveStorage, LocalStorageAdapter>();
            _container.RegisterSingleton<ISaveSystem, SaveSystem>();
        }
        
        private void RegisterGames()
        {
            // PatternMemory
            _container.RegisterTransient<IPatternGenerator, PatternGenerator>();
            _container.RegisterTransient<PatternMemoryLogic>();
            _container.RegisterTransient<IGame, PatternMemoryGame>();
            
            // Minesweeper
            _container.RegisterTransient<IBoardGenerator, BoardGenerator>();
            _container.RegisterTransient<ICellRevealer, CellRevealer>();
            _container.RegisterTransient<MinesweeperLogic>();
            _container.RegisterTransient<IGame, MinesweeperGame>();
            
            // ... other games
        }
    }
}
```

### 9.2 Blazor Components

```razor
// Pages/Index.razor
@page "/"
@inject IEnumerable<IGame> Games

<h1>Mini Game Collection</h1>

<div class="game-grid">
    @foreach (var game in Games)
    {
        <GameCard Game="@game" />
    }
</div>

// Components/Shared/GameCard.razor
<div class="game-card" @onclick="@(() => NavigateToGame(game))">
    <h3>@game.GameName</h3>
    <p>@game.GameDescription</p>
</div>

@code {
    [Parameter] public IGame Game { get; set; }
    
    private void NavigateToGame(IGame game)
    {
        NavigationManager.NavigateTo($"/{game.GameName.ToLower()}");
    }
}
```

### 9.3 LocalStorage Adapter

```csharp
namespace BlazorAdapter.Services
{
    public class LocalStorageAdapter : ISaveStorage
    {
        private readonly IJSRuntime _js;
        
        public LocalStorageAdapter(IJSRuntime js) { _js = js; }
        
        public async void Save(string key, string data)
        {
            await _js.InvokeVoidAsync("localStorage.setItem", key, data);
        }
        
        public async Task<string> Load(string key)
        {
            return await _js.InvokeAsync<string>("localStorage.getItem", key);
        }
        
        public async Task<bool> Exists(string key)
        {
            var value = await Load(key);
            return value != null;
        }
        
        public async void Delete(string key)
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", key);
        }
    }
}
```

---

## 10. Development Phases

| Phase | Content | Dependencies |
|-------|---------|--------------|
| **Phase 0** | Core Architecture | None |
| **Phase 1** | Pattern Memory | Core |
| **Phase 2** | Minesweeper | Core |
| **Phase 3** | Sudoku | Core |
| **Phase 4** | Tetris | Core |
| **Phase 5** | Gomoku + AI | Core.AI |
| **Phase 6** | Chess + AI | Core.AI |
| **Phase 7** | Blazor UI | Core + All Games |

---

## 11. SOLID Compliance Summary

| Principle | Application |
|-----------|-------------|
| **SRP** | Each class has single responsibility (Logic, Validator, Evaluator separated) |
| **OCP** | New games can be added without modifying Core |
| **LSP** | All IGame implementations can substitute each other |
| **ISP** | IGame split into IGameInfo, IGameControllable, IGameSerializable, IGameEventEmitter |
| **DIP** | All dependencies injected through interfaces |

---

## 12. Success Criteria

1. All 6 games playable in Blazor WebAssembly
2. AI opponents in Gomoku and Chess with adjustable difficulty
3. Game state persistence (save/load)
4. SOLID principles verified through code review
5. Unit test coverage > 80% for Core module
6. Performance: AI move calculation < 2 seconds

---

## 13. Next Steps

Proceed to `writing-plans` skill to create detailed implementation plan for Phase 0.