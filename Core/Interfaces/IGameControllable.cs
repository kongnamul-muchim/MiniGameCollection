namespace Core.Interfaces;

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