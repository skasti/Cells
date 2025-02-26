using System;
using Cells.GameObjects;
using Cells.Genetics.Genes.Programming;
using Microsoft.Xna.Framework;

namespace CellsTest.Genetics.Genes.Programming;

public class MathTests
{
    [Theory]
    [InlineData(null, 0x10, (byte)0x10)]
    [InlineData((byte)0x00, 0x10, (byte)0x10)]
    [InlineData(5f, 0x10, 21f)]
    [InlineData((byte)0xFE, 0x02,(byte)0x00)]
    public void Add_ShouldWorkAsExpected(object value, byte amount, object expectedResult)
    {
        var self = new Organism(new Cells.Genetics.DNA(Array.Empty<byte>()), 100f, Vector2.One);
        self.Remember(0x20, value);

        var gene = new MemoryAdd(0x20, amount);
        gene.Update(self, 1f  / 60);

        var actualResult = self.Remember(0x20);
        Assert.Equal(expectedResult, actualResult);
    }
}
