namespace Core.Data;

public interface ISaveSystem
{
    Task SaveGameAsync<T>(string gameId, T gameState);
    Task<T?> LoadGameAsync<T>(string gameId);
    Task<bool> HasSavedGameAsync(string gameId);
    Task DeleteSavedGameAsync(string gameId);
    Task SaveSettingsAsync(GameSettings settings);
    Task<GameSettings> LoadSettingsAsync();
}