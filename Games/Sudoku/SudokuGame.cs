using System;
using System.Text.Json;
using Core.Events;
using Core.Interfaces;
using Core.State;
using Games.Sudoku.Logic;

namespace Games.Sudoku;

public class SudokuGame : IGame
{
    private readonly SudokuLogic _logic;
    private readonly IStateManager _stateManager;

    public string GameName => "Sudoku";
    public string GameDescription => "Fill the 9x9 grid with digits 1-9";
    public GameState CurrentState => _stateManager.CurrentState;
    public bool IsPlaying => _stateManager.CurrentState == GameState.Playing;
    public bool IsPaused => _stateManager.CurrentState == GameState.Paused;

    public event Action<GameEvent>? OnGameEvent;

    public SudokuGame(SudokuLogic logic)
    {
        _logic = logic ?? throw new ArgumentNullException(nameof(logic));
        _stateManager = new StateManager(new Core.State.DefaultStateTransitionRule());
        
        _stateManager.OnStateChanged += (prev, current) =>
        {
            var evt = new Core.Events.GameStateChangedEvent(prev, current);
            OnGameEvent?.Invoke(evt);
        };
    }

    public void StartGame()
    {
        _stateManager.ChangeState(GameState.Ready);
        _stateManager.ChangeState(GameState.Playing);
        _logic.NewGame(1);
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
        return System.Text.Json.JsonSerializer.Serialize(new
        {
            State = _stateManager.CurrentState,
            Difficulty = 1
        });
    }

    public void DeserializeState(string json)
    {
        _stateManager.ChangeState(GameState.Ready);
    }
}