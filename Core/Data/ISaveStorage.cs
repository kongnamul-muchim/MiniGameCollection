namespace Core.Data;

public interface ISaveStorage
{
    Task SaveAsync(string key, string data);
    Task<string?> LoadAsync(string key);
    Task<bool> ExistsAsync(string key);
    Task DeleteAsync(string key);
}