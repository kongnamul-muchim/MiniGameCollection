using System;
using System.Collections.Generic;
using System.Linq;

namespace Games.PatternMemory.Logic;

public class PatternGenerator : IPatternGenerator
{
    private readonly Random _random;

    public PatternGenerator(Random random)
    {
        _random = random ?? throw new ArgumentNullException(nameof(random));
    }

    public List<int> GeneratePattern(int length)
    {
        return Enumerable.Range(0, length)
            .Select(_ => _random.Next(0, 9))
            .ToList();
    }
}
