using Games.Minesweeper.Models;

namespace Games.Minesweeper.Logic;

public class CellRevealer : ICellRevealer
{
    public void Reveal(MinesweeperBoard board, int row, int col)
    {
        var cell = board.Cells[row, col];
        if (cell.IsRevealed || cell.IsFlagged) return;
        
        cell.IsRevealed = true;
        
        // Expand if no adjacent mines
        if (cell.AdjacentMines == 0 && !cell.IsMine)
        {
            for (int dr = -1; dr <= 1; dr++)
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0) continue;
                    int nr = row + dr, nc = col + dc;
                    if (nr >= 0 && nr < board.Rows && nc >= 0 && nc < board.Columns)
                        Reveal(board, nr, nc);
                }
        }
    }
}
