using Games.Gomoku.Models;

namespace Games.Gomoku.Logic;

public class GomokuValidator : IGomokuValidator
{
    private static readonly (int, int)[] Directions = { (0, 1), (1, 0), (1, 1), (1, -1) };
    
    public bool IsValidMove(GomokuBoard board, int row, int col)
    {
        if (row < 0 || row >= board.Size || col < 0 || col >= board.Size)
            return false;
        
        return board.IsEmpty(row, col);
    }
    
    public bool CheckWin(GomokuBoard board, int row, int col, int player)
    {
        foreach (var (dr, dc) in Directions)
        {
            int count = 1;
            
            // Count in positive direction
            count += CountInDirection(board, row, col, dr, dc, player);
            
            // Count in negative direction
            count += CountInDirection(board, row, col, -dr, -dc, player);
            
            if (count >= 5)
                return true;
        }
        
        return false;
    }
    
    private int CountInDirection(GomokuBoard board, int row, int col, int dr, int dc, int player)
    {
        int count = 0;
        int r = row + dr;
        int c = col + dc;
        
        while (r >= 0 && r < board.Size && c >= 0 && c < board.Size && board.GetCell(r, c) == player)
        {
            count++;
            r += dr;
            c += dc;
        }
        
        return count;
    }
}