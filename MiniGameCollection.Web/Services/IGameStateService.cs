using Core.Interfaces;
using Core.Events;

namespace MiniGameCollection.Web.Services;

public interface IGameStateService
{
    IGame? CurrentGame { get; }
    string? CurrentGameName { get; }
    bool IsPlaying { get; }
    bool IsPaused { get; }
    
    event Action? OnStateChanged;
    event Action<GameEvent>? OnGameEvent;
    
    void StartGame<T>() where T : IGame, new();
    void StartGame(IGame game);
    void PauseGame();
    void ResumeGame();
    void ResetGame();
    void EndGame();
    
    Task SaveStateAsync();
    Task LoadStateAsync(string gameName);
}
