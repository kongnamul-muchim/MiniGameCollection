using System;
using System.Text.Json;
using Core.Events;
using Core.Interfaces;
using Core.State;
using Games.Chess.Logic;

namespace Games.Chess;

public class ChessGame : IGame
{
    private readonly ChessLogic _logic;
    private readonly IStateManager _stateManager;

    public string GameName => "Chess";
    public string GameDescription => "Classic strategy board game";
    public GameState CurrentState => _stateManager.CurrentState;
    public bool IsPlaying => _stateManager.CurrentState == GameState.Playing;
    public bool IsPaused => _stateManager.CurrentState == GameState.Paused;

    public event Action<GameEvent>? OnGameEvent;
    
    // Expose logic and board for web UI
    public ChessLogic Logic => _logic;
    public Models.ChessBoard Board => _logic.Board;
    public Models.PieceColor CurrentColor => _logic.CurrentColor;
    public bool IsWhiteTurn => _logic.CurrentColor == Models.PieceColor.White;
    public bool IsGameOver => _logic.IsGameOver;
    public int? Winner => _logic.Winner;
    public bool HasAI => _logic.HasAI;
    public bool AIIsWhite => _logic.AIIsWhite;
    
    public Models.ChessPiece? GetPiece(int row, int col) => _logic.Board.GetPiece(row, col);
    public bool MakeMove(int fromRow, int fromCol, int toRow, int toCol)
    {
        return _logic.MakeMove(new Models.ChessMove(new Models.Position(fromRow, fromCol), new Models.Position(toRow, toCol)));
    }

    public ChessGame(ChessLogic logic, IStateManager? stateManager = null)
    {
        _logic = logic ?? throw new ArgumentNullException(nameof(logic));
        _stateManager = stateManager ?? new StateManager(new DefaultStateTransitionRule());
        
        _stateManager.OnStateChanged += (prev, current) =>
        {
            var evt = new GameStateChangedEvent(prev, current);
            OnGameEvent?.Invoke(evt);
        };
    }

    public void StartGame()
    {
        _stateManager.ChangeState(GameState.Ready);
        _stateManager.ChangeState(GameState.Playing);
        _logic.StartGame();
    }

    public void PauseGame() => _stateManager.ChangeState(GameState.Paused);
    public void ResumeGame() => _stateManager.ChangeState(GameState.Playing);
    
    public void ResetGame()
    {
        _logic.StartGame();
        _stateManager.ChangeState(GameState.Ready);
    }
    
    public bool MovePiece(int fromRow, int fromCol, int toRow, int toCol)
    {
        return MakeMove(fromRow, fromCol, toRow, toCol);
    }
    
    /// <summary>
    /// Check if it's currently AI's turn.
    /// </summary>
    public bool IsAITurn() => _logic.IsAITurn();
    
    /// <summary>
    /// Get AI's move and apply it.
    /// </summary>
    public (bool success, int fromRow, int fromCol, int toRow, int toCol)? MakeAIMoveWithPosition()
    {
        if (!_logic.HasAI || _logic.IsGameOver || !IsPlaying)
            return null;
        
        var move = _logic.GetAIMove();
        if (move == null)
            return null;
        
        var result = MakeMove(move.From.Row, move.From.Column, move.To.Row, move.To.Column);
        return (result, move.From.Row, move.From.Column, move.To.Row, move.To.Column);
    }
    
    public void EndGame() => _stateManager.ChangeState(GameState.GameOver);

    public string SerializeState() => JsonSerializer.Serialize(new { State = _stateManager.CurrentState });
    public void DeserializeState(string json) => _stateManager.ChangeState(GameState.Ready);
}