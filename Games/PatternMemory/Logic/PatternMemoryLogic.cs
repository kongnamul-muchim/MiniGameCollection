using System;
using System.Collections.Generic;
using Games.PatternMemory.Models;

namespace Games.PatternMemory.Logic;

public class PatternMemoryLogic
{
    private readonly IPatternGenerator _generator;
    private readonly PatternMemoryState _state;

    public int CurrentLevel => _state.CurrentLevel;
    public int PlayerScore => _state.PlayerScore;
    public int MistakesAllowed => _state.MistakesAllowed;
    public IReadOnlyList<int> CurrentPattern => _state.CurrentPattern;
    public bool IsGameOver => _state.IsGameOver;
    public bool IsVictory => _state.IsVictory;
    public int MaxLevel { get; set; } = 10;

    public PatternMemoryLogic(IPatternGenerator generator)
    {
        _generator = generator ?? throw new ArgumentNullException(nameof(generator));
        _state = new PatternMemoryState();
    }

    public void StartGame()
    {
        _state.Reset();
        _state.CurrentLevel = 1;
        GenerateNewPattern();
    }

    public void ResetGame()
    {
        _state.Reset();
    }

    public bool CheckInput(IReadOnlyList<int> playerInput)
    {
        if (playerInput.Count != _state.Pattern.Count)
        {
            _state.SetMistakesAllowed(_state.MistakesAllowed - 1);
            return false;
        }

        for (int i = 0; i < _state.Pattern.Count; i++)
        {
            if (playerInput[i] != _state.Pattern[i])
            {
                _state.SetMistakesAllowed(_state.MistakesAllowed - 1);
                return false;
            }
        }

        _state.AddScore(_state.CurrentLevel * 10);
        
        if (_state.CurrentLevel >= MaxLevel)
        {
            _state.SetVictory();
            return true;
        }
        
        _state.CurrentLevel++;
        GenerateNewPattern();
        return true;
    }

    private void GenerateNewPattern()
    {
        var pattern = _generator.GeneratePattern(_state.CurrentLevel + 2);
        _state.SetPattern(pattern);
    }
}