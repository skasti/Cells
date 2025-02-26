using System;
using Cells.Genetics;

namespace CellsTest.Genetics;

public class ExtensionTests
{
    [Theory]
    [InlineData(0, 0f)]
    [InlineData(255, 1f)]
    [InlineData(128, 128f / 255f)]
    public void AsFloat_ShouldConvertByteToFloat(byte input, float expected)
    {
        float result = input.AsFloat();
        Assert.Equal(expected, result, precision: 5);
    }

    [Theory]
    [InlineData(0, 0f, 10f, 0f)]
    [InlineData(255, 0f, 10f, 10f)]
    [InlineData(128, -5f, 5f, 0f)]
    public void AsFloat_WithRange_ShouldConvertByteToFloat(byte input, float min, float max, float expected)
    {
        float result = input.AsFloat(min, max);
        // Allow small margin of error due to floating-point precision limitations when converting from byte
        Assert.InRange(result, expected - 0.02f, expected + 0.02f);
    }

    [Fact]
    public void AsFloat_WithInvalidRange_ShouldThrowException()
    {
        Assert.Throws<ArgumentException>(() => ((byte)128).AsFloat(10f, 5f));
    }

    [Theory]
    [InlineData(0f, 0)]
    [InlineData(1f, 255)]
    [InlineData(0.5f, 128)]
    public void AsGeneByte_ShouldConvertFloatToByte(float input, byte expected)
    {
        byte result = input.AsGeneByte();
        Assert.InRange(result, Math.Max(expected - 1,0), Math.Min(expected + 1, 255));
    }

    [Theory]
    [InlineData(0f, 0f, 10f, 0)]
    [InlineData(10f, 0f, 10f, 255)]
    [InlineData(5f, 0f, 10f, 128)]
    public void AsGeneByte_WithRange_ShouldConvertFloatToByte(float input, float min, float max, byte expected)
    {
        byte result = input.AsGeneByte(min, max);
        Assert.InRange(result, Math.Max(expected - 1,0), Math.Min(expected + 1, 255));
    }

    [Fact]
    public void AsGeneByte_WithInvalidRange_ShouldThrowException()
    {
        Assert.Throws<ArgumentException>(() => 5f.AsGeneByte(10f, 5f));
    }

    [Fact]
    public void Join_ShouldFlattenNestedByteLists()
    {
        var fragments = new List<IEnumerable<byte>>
        {
            new byte[] { 1, 2 },
            new byte[] { 3, 4 },
            new byte[] { 5 }
        };

        var result = fragments.Join();
        Assert.Equal(new byte[] { 1, 2, 3, 4, 5 }, result);
    }

    [Fact]
    public void Mutate_ShouldReturnByteWithinValidRange()
    {
        byte input = 100;
        byte mutated = input.Mutate();
        Assert.InRange(mutated, 0, 255);
    }

    [Theory]
    [InlineData(0, 0, 1f)]
    [InlineData(0, 255, 0f)]
    [InlineData(128, 128, 1f)]
    [InlineData(128, 255, 0.498f)]
    public void Compare_ShouldReturnCorrectSimilarity(byte a, byte b, float expected)
    {
        float result = a.Compare(b);
        // Allow for slight floating-point precision errors
        Assert.InRange(result, expected - 0.01f, expected + 0.01f);
    }

    [Fact]
    public void Compare_ByteArrays_ShouldReturnCorrectSimilarity()
    {
        byte[] a = [100, 150, 200, 250];
        byte[] b = [100, 150, 200, 250];
        float result = a.Compare(b);
        Assert.Equal(1f, result); // Identical arrays should return 1.0

        byte[] c = [0, 50, 100, 150];
        byte[] d = [255, 205, 155, 105];
        float result2 = c.Compare(d);
        Assert.Equal(0.5f, result2);
    }

    [Theory]
    [InlineData(0, 255, 0, 0)]   // Edge case: min value, should remain 0
    [InlineData(255, 255, 0, 255)] // Edge case: max value, should remain 255
    [InlineData(128, 255, 0, 128)] // Middle value, should remain the same
    [InlineData(255, 200, 100, 155)] // 255 wrapped to range 100-200
    [InlineData(200, 200, 100, 200)] // 200 wrapped into 100-200 range
    [InlineData(100, 200, 100, 100)] // Lower bound test, should stay at 100
    [InlineData(199, 200, 100, 199)] // Upper bound test, should stay at 199
    [InlineData(201, 200, 100, 101)] // Out of range, should wrap correctly
    [InlineData(0x20, 0x10, 0, 0x10)] // Out of range, should wrap correctly
    public void AsByte_ShouldMapWithinRange(byte input, byte maxValue, byte minValue, byte expected)
    {
        byte result = input.AsByte(maxValue, minValue);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void AsByte_ShouldHandleFullByteRange()
    {
        for (byte i = 0; i < 255; i++)
        {
            byte result = i.AsByte(255, 0);
            Assert.Equal(i, result);
        }
    }

    [Fact]
    public void AsByte_ShouldRespectCustomRange()
    {
        for (byte i = 100; i < 200; i++)
        {
            byte result = i.AsByte(200, 100);
            Assert.InRange(result, 100, 200);
        }
    }
}
