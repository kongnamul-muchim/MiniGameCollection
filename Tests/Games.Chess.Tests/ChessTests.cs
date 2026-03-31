using Core.Interfaces;
using Games.Chess.Models;
using Xunit;

namespace Games.Chess.Tests;

public class ChessBoardTests
{
    [Fact]
    public void Board_ShouldHaveCorrectSize()
    {
        var board = new ChessBoard();
        Assert.Equal(8, board.Size);
        Assert.Equal(64, board.Cells.Length);
    }

    [Fact]
    public void Board_ShouldSetupInitialPosition()
    {
        var board = new ChessBoard();
        
        // Check white pieces
        Assert.NotNull(board.GetPiece(7, 4));
        Assert.Equal(PieceType.King, board.GetPiece(7, 4)!.Type);
        Assert.Equal(PieceColor.White, board.GetPiece(7, 4)!.Color);
        
        // Check black pieces
        Assert.NotNull(board.GetPiece(0, 4));
        Assert.Equal(PieceType.King, board.GetPiece(0, 4)!.Type);
        Assert.Equal(PieceColor.Black, board.GetPiece(0, 4)!.Color);
    }

    [Fact]
    public void Board_ShouldMovePieces()
    {
        var board = new ChessBoard();
        
        board.MovePiece(6, 4, 4, 4); // e4
        
        Assert.Null(board.GetPiece(6, 4));
        Assert.NotNull(board.GetPiece(4, 4));
        Assert.Equal(PieceType.Pawn, board.GetPiece(4, 4)!.Type);
    }
}

public class ChessValidatorTests
{
    [Fact]
    public void Validator_ShouldAcceptValidPawnMove()
    {
        var board = new ChessBoard();
        var validator = new Logic.ChessValidator();
        var move = new ChessMove(new Position(6, 4), new Position(4, 4)); // e4
        
        Assert.True(validator.IsValidMove(board, move, PieceColor.White));
    }

    [Fact]
    public void Validator_ShouldAcceptKnightMove()
    {
        var board = new ChessBoard();
        var validator = new Logic.ChessValidator();
        var move = new ChessMove(new Position(7, 1), new Position(5, 2)); // Nf3
        
        Assert.True(validator.IsValidMove(board, move, PieceColor.White));
    }

    [Fact]
    public void Validator_ShouldRejectInvalidMove()
    {
        var board = new ChessBoard();
        var validator = new Logic.ChessValidator();
        var move = new ChessMove(new Position(6, 4), new Position(6, 5)); // Invalid pawn move
        
        Assert.False(validator.IsValidMove(board, move, PieceColor.White));
    }
}

public class ChessLogicTests
{
    [Fact]
    public void ChessLogic_ShouldInitialize()
    {
        var logic = new Logic.ChessLogic();
        
        Assert.Equal(PieceColor.White, logic.CurrentColor);
        Assert.False(logic.IsGameOver);
    }

    [Fact]
    public void ChessLogic_ShouldMakeMove()
    {
        var logic = new Logic.ChessLogic();
        logic.StartGame();
        
        var move = new ChessMove(new Position(6, 4), new Position(4, 4));
        var result = logic.MakeMove(move);
        
        Assert.True(result);
        Assert.Equal(PieceColor.Black, logic.CurrentColor);
    }

    [Fact]
    public void ChessLogic_ShouldGetAIMove()
    {
        var logic = new Logic.ChessLogic();
        logic.StartGame();
        
        var move = logic.GetAIMove();
        
        Assert.NotNull(move);
    }
}

public class ChessGameTests
{
    [Fact]
    public void Game_ShouldHaveCorrectInfo()
    {
        var logic = new Logic.ChessLogic();
        var game = new ChessGame(logic);
        
        Assert.Equal("Chess", game.GameName);
        Assert.NotEmpty(game.GameDescription);
    }

    [Fact]
    public void StartGame_ShouldInitialize()
    {
        var logic = new Logic.ChessLogic();
        var game = new ChessGame(logic);
        
        game.StartGame();
        
        Assert.Equal(GameState.Playing, game.CurrentState);
    }
}