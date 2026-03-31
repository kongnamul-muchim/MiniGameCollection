using Games.Gomoku.Models;

namespace Games.Gomoku.Logic;

public interface IGomokuValidator
{
    bool IsValidMove(GomokuBoard board, int row, int col);
    bool CheckWin(GomokuBoard board, int row, int col, int player);
}