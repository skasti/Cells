using Cells.Genetics.Genes.Programming;

namespace CellsTest.Genetics.Genes.Programming;

public class UtilsTests
{
    [Theory]
    [InlineData(10, 5, true)]
    [InlineData(5, 10, false)]
    [InlineData(5, 5, false)]
    [InlineData(3.14, 2.71, true)]
    [InlineData("b", "a", true)]
    public void IsGreaterThan_ShouldReturnExpectedResult(object a, object b, bool expected)
    {
        bool result = Utils.IsGreaterThan(a, b);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(10, 5, false)]
    [InlineData(5, 10, true)]
    [InlineData(5, 5, false)]
    [InlineData(3.14, 2.71, false)]
    [InlineData("a", "b", true)]
    public void IsLessThan_ShouldReturnExpectedResult(object a, object b, bool expected)
    {
        bool result = Utils.IsLessThan(a, b);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(10, 10, true)]
    [InlineData(5, 10, false)]
    [InlineData(5.0, 5, true)] // Cross-type comparison
    [InlineData("hello", "hello", true)]
    [InlineData("test", "TEST", false)] // Case-sensitive
    public void IsEqual_ShouldReturnExpectedResult(object a, object b, bool expected)
    {
        bool result = Utils.IsEqual(a, b);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(10, 5, true)]
    [InlineData(5, 10, false)]
    [InlineData(5, 5, true)]
    [InlineData(3.14, 3.14, true)]
    [InlineData("b", "a", true)]
    public void IsGreaterThanOrEqual_ShouldReturnExpectedResult(object a, object b, bool expected)
    {
        bool result = Utils.IsGreaterThanOrEqual(a, b);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(10, 5, false)]
    [InlineData(5, 10, true)]
    [InlineData(5, 5, true)]
    [InlineData(3.14, 4.0, true)]
    [InlineData("a", "b", true)]
    public void IsLessThanOrEqual_ShouldReturnExpectedResult(object a, object b, bool expected)
    {
        bool result = Utils.IsLessThanOrEqual(a, b);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(null, 5)]
    [InlineData(5, null)]
    public void Compare_WithNullValues_ShouldThrowArgumentNullException(object a, object b)
    {
        Assert.Throws<ArgumentNullException>(() => Utils.IsGreaterThan(a, b));
    }

    [Theory]
    [InlineData(5, "string")]
    [InlineData(3.14, true)]
    public void Compare_IncompatibleTypes_ShouldThrowArgumentException(object a, object b)
    {
        Assert.Throws<ArgumentException>(() => Utils.IsGreaterThan(a, b));
    }

    [Fact]
    public void Add_IntAndInt_ReturnsInt()
    {
        object result = Utils.Add(5, 10);
        Assert.True(result is int);
        Assert.Equal(15, result);
    }

    [Fact]
    public void Add_ByteAndByte_ReturnsByte()
    {
        object result = Utils.Add((byte)0x00, (byte)0x10);
        Assert.True(result is byte);
        Assert.Equal((byte)0x10, result);
    }

    [Fact]
    public void Add_ByteOverflow_WrapsAround()
    {
        object result = Utils.Add((byte)0xFF, (byte)0x01);
        Assert.True(result is byte);
        Assert.Equal((byte)0x00, result);
    }

    [Fact]
    public void Add_FloatAndByte_ReturnsFloat()
    {
        float a = 5f;
        byte b = 0x01;
        object result = Utils.Add(a, b);
        Assert.True(result is float);
        Assert.Equal(6f, result);
    }

    [Fact]
    public void Add_DoubleAndInt_ReturnsDouble()
    {
        object result = Utils.Add(4.5, 2);
        Assert.True(result is double);
        Assert.Equal(6.5, result);
    }

    [Fact]
    public void Add_DecimalAndInt_ReturnsDecimal()
    {
        object result = Utils.Add(10m, 2);
        Assert.True(result is decimal);
        Assert.Equal(12m, result);
    }

    [Fact]
    public void Add_NonNumeric_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Utils.Add("abc", 5));
    }

    [Fact]
    public void Add_NullValues_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Utils.Add(null, 5));
    }
}
