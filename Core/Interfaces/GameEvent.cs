namespace Core.Interfaces;

public class GameEvent
{
    public string Name { get; }
    public object? Data { get; }
    public DateTime Timestamp { get; }
    
    public GameEvent(string name, object? data = null)
    {
        Name = name;
        Data = data;
        Timestamp = DateTime.UtcNow;
    }
}