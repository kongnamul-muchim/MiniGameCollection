namespace Games.Minesweeper.Logic;

public interface ICellRevealer
{
    void Reveal(Models.MinesweeperBoard board, int row, int col);
}
