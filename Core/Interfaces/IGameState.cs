namespace Core.Interfaces;

public interface IGameState
{
    int CurrentPlayer { get; }
    bool IsGameOver { get; }
    int? Winner { get; }
}