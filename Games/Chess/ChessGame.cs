using System;
using System.Text.Json;
using Core.Events;
using Core.Interfaces;
using Core.State;
using Games.Chess.Logic;

namespace Games.Chess;

public class ChessGame : IGame
{
    private readonly ChessLogic _logic;
    private readonly IStateManager _stateManager;

    public string GameName => "Chess";
    public string GameDescription => "Classic strategy board game";
    public GameState CurrentState => _stateManager.CurrentState;
    public bool IsPlaying => _stateManager.CurrentState == GameState.Playing;
    public bool IsPaused => _stateManager.CurrentState == GameState.Paused;

    public event Action<GameEvent>? OnGameEvent;

    public ChessGame(ChessLogic logic)
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

    public void PauseGame() => _stateManager.ChangeState(GameState.Paused);
    public void ResumeGame() => _stateManager.ChangeState(GameState.Playing);
    public void ResetGame() => _stateManager.ChangeState(GameState.Ready);
    public void EndGame() => _stateManager.ChangeState(GameState.GameOver);

    public string SerializeState() => JsonSerializer.Serialize(new { State = _stateManager.CurrentState });
    public void DeserializeState(string json) => _stateManager.ChangeState(GameState.Ready);
}