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
        // Create a valid completed board pattern
        var puzzle = GenerateBasePattern();
        
        // Remove cells based on difficulty
        int cellsToRemove = difficulty switch
        {
            1 => 35, // Easy - 44 given
            2 => 45, // Medium - 36 given
            3 => 55, // Hard - 26 given
            _ => 35
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
            puzzle[r, c] = 0;
        }
        
        return puzzle;
    }
    
    private int[,] GenerateBasePattern()
    {
        // Use a valid Sudoku pattern (rotated/shuffled)
        var baseGrid = new int[,]
        {
            {1,2,3,4,5,6,7,8,9},
            {4,5,6,7,8,9,1,2,3},
            {7,8,9,1,2,3,4,5,6},
            {2,3,4,5,6,7,8,9,1},
            {5,6,7,8,9,1,2,3,4},
            {8,9,1,2,3,4,5,6,7},
            {3,4,5,6,7,8,9,1,2},
            {6,7,8,9,1,2,3,4,5},
            {9,1,2,3,4,5,6,7,8}
        };
        
        // Apply transformations
        var result = new int[9, 9];
        
        // Shuffle rows within each 3x3 box
        for (int box = 0; box < 3; box++)
        {
            var rows = new[] { box * 3, box * 3 + 1, box * 3 + 2 };
            ShuffleArray(rows);
            
            for (int i = 0; i < 3; i++)
            {
                for (int c = 0; c < 9; c++)
                    result[rows[i], c] = baseGrid[box * 3 + i, c];
            }
        }
        
        // Shuffle columns within each 3x3 box
        for (int c = 0; c < 9; c++)
        {
            var temp = new int[9];
            for (int r = 0; r < 9; r++)
                temp[r] = result[r, c];
            
            // Shuffle columns in groups of 3
            for (int box = 0; box < 3; box++)
            {
                var cols = new[] { box * 3, box * 3 + 1, box * 3 + 2 };
                ShuffleArray(cols);
            }
        }
        
        // Shuffle numbers (e.g., swap all 1s with all 2s)
        var numberMap = new int[10];
        var numbers = Enumerable.Range(1, 9).ToList();
        Shuffle(numbers);
        for (int i = 0; i < 9; i++)
            numberMap[numbers[i]] = i + 1;
        
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
    
    private void ShuffleArray<T>(T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }
}