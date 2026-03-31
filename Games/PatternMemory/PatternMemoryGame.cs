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
        var currentState = _stateManager.CurrentState;
        if (currentState == GameState.Playing)
        {
            _stateManager.ChangeState(GameState.Paused);
        }
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
