using Games.Gomoku.Models;

namespace Games.Gomoku.Logic;

public interface IGomokuValidator
{
    bool IsValidMove(IGomokuBoard board, int row, int col);
    bool CheckWin(IGomokuBoard board, int row, int col, int player);
}