using Core.AI;
using Core.Interfaces;
using Games.Chess.Models;

namespace Games.Chess.Logic;

public class ChessLogic
{
    private readonly ChessValidator _validator;
    private readonly ChessState _state;
    private readonly MinimaxAI<ChessMove>? _ai;
    private readonly ChessEvaluator? _evaluator;
    
    public ChessBoard Board { get; }
    public PieceColor CurrentColor => _state.CurrentColor;
    public bool IsGameOver => _state.IsGameOver;
    public int? Winner => _state.Winner;
    public bool HasAI => _ai != null;
    public bool AIIsWhite { get; private set; } = true;

    public ChessLogic(bool useAI = false, int aiDepth = 2)
    {
        _validator = new ChessValidator();
        _state = new ChessState();
        Board = new ChessBoard();
        
        if (useAI)
        {
            _evaluator = new ChessEvaluator();
            _ai = new MinimaxAI<ChessMove>(aiDepth);
            _ai.Difficulty = aiDepth switch
            {
                1 => "Easy",
                2 => "Normal",
                3 => "Hard",
                _ => "Normal"
            };
        }
    }

    public void StartGame()
    {
        var newBoard = new ChessBoard();
        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
                Board.Cells[r, c] = newBoard.Cells[r, c];
        
        _state.CurrentPlayer = 1;
        _state.IsGameOver = false;
        _state.Winner = null;
        
        // Random first player for AI games
        if (HasAI)
        {
            AIIsWhite = new Random().Next(2) == 0;
            // AI is White = AI goes second (Black goes first)
            // AI is Black = AI goes first
        }
    }

    public bool MakeMove(ChessMove move)
    {
        if (_state.IsGameOver) return false;
        if (!_validator.IsValidMove(Board, move, _state.CurrentColor)) return false;
        
        // Check if move leaves own king in check
        if (WouldBeInCheck(move, _state.CurrentColor)) return false;
        
        Board.MovePiece(move.From.Row, move.From.Column, move.To.Row, move.To.Column);
        
        // Handle pawn promotion
        var movedPiece = Board.GetPiece(move.To.Row, move.To.Column);
        if (movedPiece?.Type == PieceType.Pawn)
        {
            if ((movedPiece.Color == PieceColor.White && move.To.Row == 0) ||
                (movedPiece.Color == PieceColor.Black && move.To.Row == 7))
            {
                Board.Cells[move.To.Row, move.To.Column] = new ChessPiece(PieceType.Queen, movedPiece.Color);
            }
        }
        
        // Check for checkmate
        var enemyColor = _state.CurrentColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
        if (_validator.IsCheckmate(Board, enemyColor))
        {
            _state.SetWinner(_state.CurrentPlayer);
            return true;
        }
        
        // Check for stalemate
        if (_validator.IsStalemate(Board, enemyColor))
        {
            _state.IsGameOver = true;
            _state.Winner = null; // Draw
            return true;
        }
        
        _state.SwitchPlayer();
        return true;
    }
    
    private bool WouldBeInCheck(ChessMove move, PieceColor color)
    {
        // Simulate move
        var piece = Board.GetPiece(move.From.Row, move.From.Column);
        var captured = Board.GetPiece(move.To.Row, move.To.Column);
        
        Board.Cells[move.To.Row, move.To.Column] = piece;
        Board.Cells[move.From.Row, move.From.Column] = null;
        
        bool inCheck = _validator.IsInCheck(Board, color);
        
        // Undo move
        Board.Cells[move.From.Row, move.From.Column] = piece;
        Board.Cells[move.To.Row, move.To.Column] = captured;
        
        return inCheck;
    }
    
    /// <summary>
    /// Get AI move using Minimax.
    /// </summary>
    public ChessMove? GetAIMove()
    {
        if (_state.IsGameOver || _ai == null || _evaluator == null)
            return null;
        
        try
        {
            var aiState = CreateAIState();
            return _ai.GetBestMove(aiState, _evaluator);
        }
        catch
        {
            return GetRandomMove();
        }
    }
    
    /// <summary>
    /// Check if it's currently AI's turn.
    /// </summary>
    public bool IsAITurn()
    {
        if (!HasAI || _state.IsGameOver) return false;
        return (AIIsWhite && _state.CurrentColor == PieceColor.White) ||
               (!AIIsWhite && _state.CurrentColor == PieceColor.Black);
    }
    
    private ChessMove? GetRandomMove()
    {
        var validMoves = new List<ChessMove>();
        
        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                if (Board.GetPiece(r, c) is { Color: var color } && color == _state.CurrentColor)
                {
                    for (int tr = 0; tr < 8; tr++)
                    {
                        for (int tc = 0; tc < 8; tc++)
                        {
                            var move = new ChessMove(new Position(r, c), new Position(tr, tc));
                            if (_validator.IsValidMove(Board, move, _state.CurrentColor) && !WouldBeInCheck(move, _state.CurrentColor))
                                validMoves.Add(move);
                        }
                    }
                }
            }
        }
        
        if (validMoves.Count == 0) return null;
        return validMoves[new Random().Next(validMoves.Count)];
    }
    
    private ChessAIState CreateAIState()
    {
        var boardArray = new ChessPiece?[8, 8];
        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
                boardArray[r, c] = Board.Cells[r, c];
        
        return new ChessAIState(boardArray, _state.CurrentColor);
    }
}

/// <summary>
/// AI-specific state for chess.
/// </summary>
public class ChessAIState : IGameState
{
    public ChessPiece?[,] Board { get; }
    public PieceColor CurrentColor { get; }
    public int CurrentPlayer => CurrentColor == PieceColor.White ? 1 : 2;
    public bool IsGameOver { get; }
    public int? Winner { get; }
    
    public ChessAIState(ChessPiece?[,] board, PieceColor currentColor)
    {
        Board = board;
        CurrentColor = currentColor;
        IsGameOver = false;
        Winner = null;
    }
}

/// <summary>
/// Evaluator for Chess AI using material + positional scoring.
/// </summary>
public class ChessEvaluator : IGameStateEvaluator<ChessMove>
{
    private static readonly ChessValidator _validator = new();
    
    // Piece values
    private static readonly Dictionary<PieceType, int> PieceValues = new()
    {
        { PieceType.Pawn, 100 },
        { PieceType.Knight, 320 },
        { PieceType.Bishop, 330 },
        { PieceType.Rook, 500 },
        { PieceType.Queen, 900 },
        { PieceType.King, 20000 }
    };
    
    // Piece-square tables for positional evaluation
    private static readonly int[,] PawnTable = {
        { 0,  0,  0,  0,  0,  0,  0,  0},
        {50, 50, 50, 50, 50, 50, 50, 50},
        {10, 10, 20, 30, 30, 20, 10, 10},
        { 5,  5, 10, 25, 25, 10,  5,  5},
        { 0,  0,  0, 20, 20,  0,  0,  0},
        { 5, -5,-10,  0,  0,-10, -5,  5},
        { 5, 10, 10,-20,-20, 10, 10,  5},
        { 0,  0,  0,  0,  0,  0,  0,  0}
    };
    
    private static readonly int[,] KnightTable = {
        {-50,-40,-30,-30,-30,-30,-40,-50},
        {-40,-20,  0,  0,  0,  0,-20,-40},
        {-30,  0, 10, 15, 15, 10,  0,-30},
        {-30,  5, 15, 20, 20, 15,  5,-30},
        {-30,  0, 15, 20, 20, 15,  0,-30},
        {-30,  5, 10, 15, 15, 10,  5,-30},
        {-40,-20,  0,  5,  5,  0,-20,-40},
        {-50,-40,-30,-30,-30,-30,-40,-50}
    };
    
    private static readonly int[,] BishopTable = {
        {-20,-10,-10,-10,-10,-10,-10,-20},
        {-10,  0,  0,  0,  0,  0,  0,-10},
        {-10,  0, 10, 10, 10, 10,  0,-10},
        {-10,  5,  5, 10, 10,  5,  5,-10},
        {-10,  0, 10, 10, 10, 10,  0,-10},
        {-10, 10, 10, 10, 10, 10, 10,-10},
        {-10,  5,  0,  0,  0,  0,  5,-10},
        {-20,-10,-10,-10,-10,-10,-10,-20}
    };
    
    private static readonly int[,] QueenTable = {
        {-20,-10,-10, -5, -5,-10,-10,-20},
        {-10,  0,  0,  0,  0,  0,  0,-10},
        {-10,  0,  5,  5,  5,  5,  0,-10},
        { -5,  0,  5,  5,  5,  5,  0, -5},
        {  0,  0,  5,  5,  5,  5,  0, -5},
        {-10,  5,  5,  5,  5,  5,  0,-10},
        {-10,  0,  5,  0,  0,  0,  0,-10},
        {-20,-10,-10, -5, -5,-10,-10,-20}
    };
    
    private static readonly int[,] KingMiddleTable = {
        {-30,-40,-40,-50,-50,-40,-40,-30},
        {-30,-40,-40,-50,-50,-40,-40,-30},
        {-30,-40,-40,-50,-50,-40,-40,-30},
        {-30,-40,-40,-50,-50,-40,-40,-30},
        {-20,-30,-30,-40,-40,-30,-30,-20},
        {-10,-20,-20,-20,-20,-20,-20,-10},
        { 20, 20,  0,  0,  0,  0, 20, 20},
        { 20, 30, 10,  0,  0, 10, 30, 20}
    };
    
    public int Evaluate(IGameState state)
    {
        var cs = (ChessAIState)state;
        return EvaluateBoard(cs.Board, cs.CurrentColor);
    }
    
    private int EvaluateBoard(ChessPiece?[,] board, PieceColor perspective)
    {
        int score = 0;
        
        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                var piece = board[r, c];
                if (piece == null) continue;
                
                int value = PieceValues.GetValueOrDefault(piece.Type, 0);
                
                // Add positional bonus
                int posBonus = piece.Type switch
                {
                    PieceType.Pawn => piece.Color == PieceColor.White ? PawnTable[r, c] : PawnTable[7 - r, c],
                    PieceType.Knight => piece.Color == PieceColor.White ? KnightTable[r, c] : KnightTable[7 - r, c],
                    PieceType.Bishop => piece.Color == PieceColor.White ? BishopTable[r, c] : BishopTable[7 - r, c],
                    PieceType.Queen => piece.Color == PieceColor.White ? QueenTable[r, c] : QueenTable[7 - r, c],
                    PieceType.King => piece.Color == PieceColor.White ? KingMiddleTable[r, c] : KingMiddleTable[7 - r, c],
                    _ => 0
                };
                
                int totalValue = value + posBonus;
                
                if (piece.Color == perspective)
                    score += totalValue;
                else
                    score -= totalValue;
            }
        }
        
        return score;
    }
    
    public IEnumerable<ChessMove> GetValidMoves(IGameState state)
    {
        var cs = (ChessAIState)state;
        var moves = new List<(ChessMove Move, int Score)>();
        
        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                if (cs.Board[r, c] != null && cs.Board[r, c]!.Color == cs.CurrentColor)
                {
                    for (int tr = 0; tr < 8; tr++)
                    {
                        for (int tc = 0; tc < 8; tc++)
                        {
                            var move = new ChessMove(new Position(r, c), new Position(tr, tc));
                            if (IsValidMoveForAI(cs.Board, move, cs.CurrentColor))
                            {
                                // Quick score for move ordering
                                int score = 0;
                                var captured = cs.Board[tr, tc];
                                if (captured != null)
                                    score = PieceValues.GetValueOrDefault(captured.Type, 0) * 10 - PieceValues.GetValueOrDefault(cs.Board[r, c]!.Type, 0);
                                moves.Add((move, score));
                            }
                        }
                    }
                }
            }
        }
        
        // Sort by capture value (MVV-LVA) and take top candidates
        return moves
            .OrderByDescending(x => x.Score)
            .Take(20)
            .Select(x => x.Move);
    }
    
    private bool IsValidMoveForAI(ChessPiece?[,] board, ChessMove move, PieceColor color)
    {
        var piece = board[move.From.Row, move.From.Column];
        if (piece == null || piece.Color != color) return false;
        
        var target = board[move.To.Row, move.To.Column];
        if (target != null && target.Color == color) return false;
        
        // Use validator logic
        var tempBoard = new TempChessBoard(board);
        return _validator.IsValidMove(tempBoard, move, color);
    }
    
    public IGameState ApplyMove(IGameState state, ChessMove move)
    {
        var cs = (ChessAIState)state;
        var newBoard = new ChessPiece?[8, 8];
        
        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
                newBoard[r, c] = cs.Board[r, c];
        
        newBoard[move.To.Row, move.To.Column] = newBoard[move.From.Row, move.From.Column];
        newBoard[move.From.Row, move.From.Column] = null;
        
        // Pawn promotion
        var movedPiece = newBoard[move.To.Row, move.To.Column];
        if (movedPiece?.Type == PieceType.Pawn)
        {
            if ((movedPiece.Color == PieceColor.White && move.To.Row == 0) ||
                (movedPiece.Color == PieceColor.Black && move.To.Row == 7))
            {
                newBoard[move.To.Row, move.To.Column] = new ChessPiece(PieceType.Queen, movedPiece.Color);
            }
        }
        
        var nextColor = cs.CurrentColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
        return new ChessAIState(newBoard, nextColor);
    }
}

/// <summary>
/// Temporary board wrapper for validator.
/// </summary>
public class TempChessBoard : ChessBoard
{
    private readonly ChessPiece?[,] _cells;
    
    public TempChessBoard(ChessPiece?[,] cells)
    {
        _cells = cells;
    }
    
    public override ChessPiece? GetPiece(int row, int col)
    {
        if (row < 0 || row >= 8 || col < 0 || col >= 8) return null;
        return _cells[row, col];
    }
}
