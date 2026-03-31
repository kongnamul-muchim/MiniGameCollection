using Games.PatternMemory.Logic;
using Xunit;

namespace Games.PatternMemory.Tests;

public class PatternGeneratorTests
{
    [Fact]
    public void Generate_ShouldReturnCorrectLength()
    {
        var generator = new PatternGenerator(new Random(42));
        var pattern = generator.GeneratePattern(5);
        
        Assert.Equal(5, pattern.Count);
    }

    [Fact]
    public void Generate_ShouldReturnNumbersInRange()
    {
        var generator = new PatternGenerator(new Random(42));
        var pattern = generator.GeneratePattern(10);
        
        foreach (var num in pattern)
        {
            Assert.InRange(num, 0, 8);
        }
    }

    [Fact]
    public void Generate_ShouldBeDeterministicWithSameSeed()
    {
        var gen1 = new PatternGenerator(new Random(42));
        var gen2 = new PatternGenerator(new Random(42));
        
        var pattern1 = gen1.GeneratePattern(5);
        var pattern2 = gen2.GeneratePattern(5);
        
        Assert.Equal(pattern1, pattern2);
    }
}
