namespace Core.Data;

public class SaveSystem : ISaveSystem
{
    private readonly ISerializer _serializer;
    private readonly ISaveStorage _storage;
    private const string SettingsKey = "gamesettings";

    public SaveSystem(ISerializer serializer, ISaveStorage storage)
    {
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    public async Task SaveGameAsync<T>(string gameId, T gameState)
    {
        var json = _serializer.Serialize(gameState);
        await _storage.SaveAsync($"game_{gameId}", json);
    }

    public async Task<T?> LoadGameAsync<T>(string gameId)
    {
        var json = await _storage.LoadAsync($"game_{gameId}");
        return json is null ? default : _serializer.Deserialize<T>(json);
    }

    public async Task<bool> HasSavedGameAsync(string gameId)
    {
        return await _storage.ExistsAsync($"game_{gameId}");
    }

    public async Task DeleteSavedGameAsync(string gameId)
    {
        await _storage.DeleteAsync($"game_{gameId}");
    }

    public async Task SaveSettingsAsync(GameSettings settings)
    {
        var json = _serializer.Serialize(settings);
        await _storage.SaveAsync(SettingsKey, json);
    }

    public async Task<GameSettings> LoadSettingsAsync()
    {
        var json = await _storage.LoadAsync(SettingsKey);
        return json is null ? new GameSettings() : _serializer.Deserialize<GameSettings>(json);
    }
}