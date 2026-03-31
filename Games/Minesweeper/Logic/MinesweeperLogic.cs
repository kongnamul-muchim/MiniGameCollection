using System;
using Games.Minesweeper.Models;

namespace Games.Minesweeper.Logic;

public class MinesweeperLogic
{
    private readonly IBoardGenerator _generator;
    private readonly ICellRevealer _revealer;
    private readonly MinesweeperState _state;
    
    public MinesweeperBoard? Board { get; private set; }
    public bool IsGameOver => _state.IsGameOver;
    public bool IsVictory => _state.IsVictory;

    public MinesweeperLogic(IBoardGenerator generator, ICellRevealer revealer)
    {
        _generator = generator ?? throw new ArgumentNullException(nameof(generator));
        _revealer = revealer ?? throw new ArgumentNullException(nameof(revealer));
        _state = new MinesweeperState();
    }

    public void Initialize(int rows, int cols, int mines)
    {
        _state.Rows = rows;
        _state.Columns = cols;
        _state.TotalMines = mines;
        
        Board = new MinesweeperBoard(rows, cols);
        _generator.GenerateBoard(Board, mines);
    }

    public void RevealCell(int row, int col)
    {
        if (Board == null) return;
        
        var cell = Board.Cells[row, col];
        if (cell.IsRevealed || cell.IsFlagged) return;
        
        if (cell.IsMine)
        {
            _state.SetHitMine();
            return;
        }
        
        _revealer.Reveal(Board, row, col);
        
        // Count revealed cells
        int revealed = 0;
        for (int r = 0; r < Board.Rows; r++)
            for (int c = 0; c < Board.Columns; c++)
                if (Board.Cells[r, c].IsRevealed) revealed++;
        
        int safeCells = (Board.Rows * Board.Columns) - _state.TotalMines;
        if (revealed >= safeCells)
            _state.SetVictory();
    }

    public void ToggleFlag(int row, int col)
    {
        if (Board == null) return;
        
        var cell = Board.Cells[row, col];
        if (cell.IsRevealed) return;
        
        if (cell.IsFlagged)
            _state.DecrementFlagged();
        else
            _state.IncrementFlagged();
        
        cell.IsFlagged = !cell.IsFlagged;
    }
}
