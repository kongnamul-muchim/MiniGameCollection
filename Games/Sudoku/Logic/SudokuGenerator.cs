namespace Games.Sudoku.Logic;

public class SudokuGenerator : ISudokuGenerator
{
    private readonly Random _random;
    
    public SudokuGenerator(Random random)
    {
        _random = random;
    }
    
    public int[,] GeneratePuzzle(int difficulty)
    {
        // Create a valid completed board
        var solved = GenerateSolvedBoard();
        
        // Remove cells based on difficulty
        int cellsToRemove = difficulty switch
        {
            1 => 30, // Easy - 51 given
            2 => 40, // Medium - 41 given
            3 => 50, // Hard - 31 given
            4 => 60, // Extreme - 21 given
            _ => 30
        };
        
        // Create list of positions to remove
        var positions = new List<(int, int)>();
        for (int r = 0; r < 9; r++)
            for (int c = 0; c < 9; c++)
                positions.Add((r, c));
        
        // Shuffle positions
        Shuffle(positions);
        
        // Remove cells
        for (int i = 0; i < cellsToRemove && i < positions.Count; i++)
        {
            var (r, c) = positions[i];
            solved[r, c] = 0;
        }
        
        return solved;
    }
    
    private int[,] GenerateSolvedBoard()
    {
        // Use a simple valid base pattern with number substitution
        // Base pattern: each row shifts by 3 from the previous row
        var result = new int[9, 9];
        
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                // Valid pattern formula
                result[r, c] = ((r * 3) + (r / 3) + c) % 9 + 1;
            }
        }
        
        // Shuffle digits only (simpler and safer)
        var numbers = Enumerable.Range(1, 9).ToList();
        Shuffle(numbers);
        var numberMap = new int[10];
        for (int i = 0; i < 9; i++)
            numberMap[i + 1] = numbers[i];
        
        for (int r = 0; r < 9; r++)
            for (int c = 0; c < 9; c++)
                result[r, c] = numberMap[result[r, c]];
        
        return result;
    }
    
    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}