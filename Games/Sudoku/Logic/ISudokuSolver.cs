using Games.Sudoku.Models;

namespace Games.Sudoku.Logic;

public interface ISudokuSolver
{
    bool Solve(SudokuBoard board);
}