namespace Games.Sudoku.Logic;

public interface ISudokuGenerator
{
    int[,] GeneratePuzzle(int difficulty);
}