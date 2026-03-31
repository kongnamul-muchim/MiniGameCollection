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
            IsGameOver = _logic.IsGameOver,
            IsVictory = _logic.IsVictory
        });
    }

    public void DeserializeState(string json)
    {
        _stateManager.ChangeState(GameState.Ready);
    }
}