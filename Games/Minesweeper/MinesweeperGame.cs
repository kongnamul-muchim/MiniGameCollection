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
    
    // Expose logic for web UI access
    public MinesweeperLogic Logic => _logic;
    public Models.MinesweeperBoard? Board => _logic.Board;
    
    // Expose cell for web UI
    public Models.Cell? GetCell(int row, int col) => _logic.Board?.Cells[row, col];
    public int Rows => _logic.Board?.Rows ?? 9;
    public int Columns => _logic.Board?.Columns ?? 9;
    public int MineCount => _logic is { Board: var b } ? b.Cells.Cast<Models.Cell>().Count(c => c.IsMine) : 10;
    public int RevealedCount => _logic is { Board: var b } ? b.Cells.Cast<Models.Cell>().Count(c => c.IsRevealed) : 0;

    public MinesweeperGame(MinesweeperLogic logic, IStateManager? stateManager = null)
    {
        _logic = logic ?? throw new ArgumentNullException(nameof(logic));
        _stateManager = stateManager ?? new StateManager(new DefaultStateTransitionRule());
        
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
        _logic.Initialize(9, 9, 10);
        _stateManager.ChangeState(GameState.Ready);
    }

    public void NewGame(int rows = 9, int cols = 9, int mines = 10)
    {
        _logic.Initialize(rows, cols, mines);
        _stateManager.ChangeState(GameState.Playing);
    }

    public void RevealCell(int row, int col)
    {
        if (_logic.Board == null) return;
        _logic.RevealCell(row, col);
    }

    public void ToggleFlag(int row, int col)
    {
        if (_logic.Board == null) return;
        _logic.ToggleFlag(row, col);
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
            IsGameOver = _logic.IsGameOver,
            IsVictory = _logic.IsVictory
        });
    }

    public void DeserializeState(string json)
    {
        _stateManager.ChangeState(GameState.Ready);
    }
}