using Games.Sudoku.Models;

namespace Games.Sudoku.Logic;

public class SudokuLogic
{
    private readonly ISudokuGenerator _generator;
    private readonly ISudokuValidator _validator;
    private readonly SudokuState _state;
    
    public SudokuBoard? Board { get; private set; }
    public bool IsGameOver => _state.IsGameOver;
    public bool IsVictory => _state.IsVictory;

    public SudokuLogic(ISudokuGenerator generator, ISudokuValidator validator)
    {
        _generator = generator;
        _validator = validator;
        _state = new SudokuState();
    }

    public void NewGame(int difficulty)
    {
        _state.Difficulty = difficulty;
        _state.Mistakes = 0;
        _state.IsVictory = false;
        
        var puzzle = _generator.GeneratePuzzle(difficulty);
        
        Board = new SudokuBoard();
        for (int r = 0; r < 9; r++)
            for (int c = 0; c < 9; c++)
                if (puzzle[r, c] != 0)
                    Board.SetCell(r, c, puzzle[r, c], isFixed: true);
    }

    public bool PlaceNumber(int row, int col, int value)
    {
        if (Board == null) return false;
        
        if (!_validator.IsValidMove(Board, row, col, value))
        {
            _state.IncrementMistakes();
            return false;
        }
        
        Board.SetCell(row, col, value);
        
        if (_validator.IsSolved(Board))
            _state.SetVictory();
        
        return true;
    }

    public bool CheckVictory()
    {
        if (Board == null) return false;
        return _validator.IsSolved(Board);
    }
}