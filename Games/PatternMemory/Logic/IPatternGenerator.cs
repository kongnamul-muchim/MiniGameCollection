using System.Collections.Generic;

namespace Games.PatternMemory.Logic;

public interface IPatternGenerator
{
    List<int> GeneratePattern(int length);
}
