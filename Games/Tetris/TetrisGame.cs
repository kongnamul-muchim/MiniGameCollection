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
    
    // Expose logic and board for web UI
    public TetrisLogic Logic => _logic;
    public Models.TetrisBoard? Board => _logic.Board;
    public Models.Tetromino? CurrentPiece => _logic.CurrentPiece;
    public Models.Tetromino? NextPiece => _logic.NextPiece;
    public int Score => _logic.GetScore();
    public int Level => _logic.GetLevel();
    public int LinesCleared => _logic.GetLinesCleared();

    public TetrisGame(TetrisLogic logic, IStateManager? stateManager = null)
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
        _logic.StartGame();
        _stateManager.ChangeState(GameState.Ready);
    }
    
    public bool MoveLeft() => _logic.MoveLeft();
    public bool MoveRight() => _logic.MoveRight();
    public bool MoveDown() => _logic.MoveDown();
    public bool Rotate() => _logic.Rotate();
    
    public void Tick() => _logic.MoveDown();

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