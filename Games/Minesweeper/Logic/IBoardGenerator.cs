namespace Games.Minesweeper.Logic;

public interface IBoardGenerator
{
    void GenerateBoard(Models.MinesweeperBoard board, int mineCount);
}
