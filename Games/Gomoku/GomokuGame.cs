using System;
using System.Text.Json;
using Core.Events;
using Core.Interfaces;
using Core.State;
using Games.Gomoku.Logic;
using Games.Gomoku.Models;

namespace Games.Gomoku;

public class GomokuGame : IGame
{
    private readonly GomokuLogic _logic;
    private readonly IStateManager _stateManager;

    public string GameName => "Gomoku";
    public string GameDescription => "Connect five stones in a row to win";
    public GameState CurrentState => _stateManager.CurrentState;
    public bool IsPlaying => _stateManager.CurrentState == GameState.Playing;
    public bool IsPaused => _stateManager.CurrentState == GameState.Paused;

    public event Action<GameEvent>? OnGameEvent;
    
    // Expose logic and board for web UI
    public GomokuLogic Logic => _logic;
    public GomokuBoard Board => _logic.Board;
    public int CurrentPlayer => _logic.CurrentPlayer;
    public bool IsBlackTurn => _logic.CurrentPlayer == 1;
    public bool IsGameOver => _logic.IsGameOver;
    public int? Winner => _logic.Winner;
    public bool HasAI => _logic.HasAI;
    public string? AIDifficulty => _logic.AIDifficulty;
    public bool AIIsBlack => _logic.AIIsBlack;

    public GomokuGame(GomokuLogic logic)
    {
        _logic = logic ?? throw new ArgumentNullException(nameof(logic));
        _stateManager = new StateManager(new DefaultStateTransitionRule());
        
        _stateManager.OnStateChanged += (prev, current) =>
        {
            var evt = new GameStateChangedEvent(prev, current);
            OnGameEvent?.Invoke(evt);
        };
    }
    
    /// <summary>
    /// Create a new GomokuGame with AI opponent.
    /// </summary>
    public static GomokuGame CreateWithAI(int aiDepth = 2)
    {
        var validator = new GomokuValidator();
        var logic = new GomokuLogic(validator, useAI: true, aiDepth: aiDepth);
        return new GomokuGame(logic);
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
    
    public bool PlaceStone(int row, int col)
    {
        var result = _logic.PlaceStone(row, col);
        
        // Check for game over
        if (_logic.IsGameOver && _logic.Winner.HasValue)
        {
            if (_logic.Winner == 1)
                _stateManager.ChangeState(GameState.Victory);
            else
                _stateManager.ChangeState(GameState.GameOver);
        }
        
        return result;
    }
    
    /// <summary>
    /// Check if it's currently AI's turn.
    /// </summary>
    public bool IsAITurn() => _logic.IsAITurn();
    
    /// <summary>
    /// Get AI's move and apply it.
    /// </summary>
    public (bool success, int row, int col)? MakeAIMoveWithPosition()
    {
        if (!_logic.HasAI || _logic.IsGameOver || !IsPlaying)
            return null;
        
        var move = _logic.GetAIMove();
        if (move == null)
            return null;
        
        var result = PlaceStone(move.Row, move.Column);
        return (result, move.Row, move.Column);
    }
    
    /// <summary>
    /// Get AI's move and apply it.
    /// </summary>
    public bool MakeAIMove()
    {
        var result = MakeAIMoveWithPosition();
        return result?.success ?? false;
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
            CurrentPlayer = _logic.CurrentPlayer
        });
    }

    public void DeserializeState(string json)
    {
        _stateManager.ChangeState(GameState.Ready);
    }
}