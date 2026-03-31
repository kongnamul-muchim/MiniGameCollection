using System.Text.Json;

namespace Core.Data;

public class JsonSerializer : ISerializer
{
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };

    public string Serialize<T>(T data)
    {
        return System.Text.Json.JsonSerializer.Serialize(data, _options);
    }

    public T Deserialize<T>(string json)
    {
        return System.Text.Json.JsonSerializer.Deserialize<T>(json, _options)!;
    }
}