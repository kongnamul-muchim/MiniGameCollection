using System;
using Games.Minesweeper.Models;

namespace Games.Minesweeper.Logic;

public class BoardGenerator : IBoardGenerator
{
    private readonly Random _random;

    public BoardGenerator(Random random)
    {
        _random = random ?? throw new ArgumentNullException(nameof(random));
    }

    public void GenerateBoard(MinesweeperBoard board, int mineCount)
    {
        int placed = 0;
        while (placed < mineCount)
        {
            int r = _random.Next(board.Rows);
            int c = _random.Next(board.Columns);
            
            if (!board.Cells[r, c].IsMine)
            {
                board.Cells[r, c].IsMine = true;
                placed++;
            }
        }
        
        // Calculate adjacent mines
        for (int r = 0; r < board.Rows; r++)
            for (int c = 0; c < board.Columns; c++)
                if (!board.Cells[r, c].IsMine)
                    board.Cells[r, c].AdjacentMines = CountAdjacentMines(board, r, c);
    }

    private int CountAdjacentMines(MinesweeperBoard board, int row, int col)
    {
        int count = 0;
        for (int dr = -1; dr <= 1; dr++)
            for (int dc = -1; dc <= 1; dc++)
            {
                if (dr == 0 && dc == 0) continue;
                int nr = row + dr, nc = col + dc;
                if (nr >= 0 && nr < board.Rows && nc >= 0 && nc < board.Columns)
                    if (board.Cells[nr, nc].IsMine) count++;
            }
        return count;
    }
}
