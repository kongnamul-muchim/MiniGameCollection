namespace Core.Interfaces;

public interface IGameSerializable
{
    string SerializeState();
    void DeserializeState(string json);
}