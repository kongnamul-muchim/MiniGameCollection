using System.Text.Json;
using Core.Events;
using Core.Interfaces;
using MiniGameCollection.Web.Storage;

namespace MiniGameCollection.Web.Services;

public class GameStateService : IGameStateService
{
    private readonly IBrowserStorage _storage;
    private IGame? _currentGame;
    
    public IGame? CurrentGame => _currentGame;
    public string? CurrentGameName => _currentGame?.GameName;
    public bool IsPlaying => _currentGame?.IsPlaying ?? false;
    public bool IsPaused => _currentGame?.IsPaused ?? false;
    
    public event Action? OnStateChanged;
    public event Action<GameEvent>? OnGameEvent;
    
    public GameStateService(IBrowserStorage storage)
    {
        _storage = storage;
    }
    
    public void StartGame<T>() where T : IGame, new()
    {
        var game = new T();
        StartGame(game);
    }
    
    public void StartGame(IGame game)
    {
        if (_currentGame != null)
        {
            _currentGame.OnGameEvent -= HandleGameEvent;
        }
        
        _currentGame = game;
        _currentGame.OnGameEvent += HandleGameEvent;
        _currentGame.StartGame();
        
        NotifyStateChanged();
    }
    
    public void PauseGame()
    {
        _currentGame?.PauseGame();
        NotifyStateChanged();
    }
    
    public void ResumeGame()
    {
        _currentGame?.ResumeGame();
        NotifyStateChanged();
    }
    
    public void ResetGame()
    {
        _currentGame?.ResetGame();
        NotifyStateChanged();
    }
    
    public void EndGame()
    {
        _currentGame?.EndGame();
        NotifyStateChanged();
    }
    
    public async Task SaveStateAsync()
    {
        if (_currentGame == null) return;
        
        var state = _currentGame.SerializeState();
        var key = $"game_{_currentGame.GameName}";
        await _storage.SetItemAsync(key, state);
    }
    
    public async Task LoadStateAsync(string gameName)
    {
        var key = $"game_{gameName}";
        var state = await _storage.GetItemAsync<string>(key);
        
        if (state == null || _currentGame == null) return;
        
        _currentGame.DeserializeState(state);
        NotifyStateChanged();
    }
    
    private void HandleGameEvent(GameEvent evt)
    {
        OnGameEvent?.Invoke(evt);
        
        if (evt is GameStateChangedEvent)
        {
            NotifyStateChanged();
        }
    }
    
    private void NotifyStateChanged()
    {
        OnStateChanged?.Invoke();
    }
}
