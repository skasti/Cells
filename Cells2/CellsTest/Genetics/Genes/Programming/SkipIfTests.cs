using System;
using Cells.GameObjects;
using Cells.Genetics.Genes.Programming;
using Microsoft.Xna.Framework;

namespace CellsTest.Genetics.Genes.Programming;

public class SkipIfTests
{
    [Theory]
    [InlineData(0x02, 1f, 1)]
    [InlineData((byte)0x02, 1f, 1)]
    [InlineData(2f, 1f, 1)]
    [InlineData(2f, 0x02, 0)]
    [InlineData(2f, 0.0, 1)]
    [InlineData(null, 0.1f, 0)]
    [InlineData(5, null, 0)]
    public void SkipIfGTMem_ShouldReturnExpectedSkipValue(object value, object other, int expectedSkip)
    {
        var self = new Organism(new Cells.Genetics.DNA(Array.Empty<byte>()), 100f, Vector2.One);
        self.Remember(0x20, value);
        self.Remember(0x21, other);

        var gene = new SkipIfGTMem(0x20, 0x21, 0x01);
        var actualSkip = gene.Update(self, 1f  / 60);

        Assert.Equal(expectedSkip, actualSkip);
    }

    [Theory]
    [InlineData(0x02, 0x01, 1)]
    [InlineData((byte)0x02, 0x01, 1)]
    [InlineData(2f, 0x01, 1)]
    [InlineData(2f, 0x02, 0)]
    [InlineData(2f, 0x00, 1)]
    [InlineData(null, 0x01, 0)]
    [InlineData(5, 0x00, 1)]
    public void SkipIfGT_ShouldReturnExpectedSkipValue(object value, byte other, int expectedSkip)
    {
        var self = new Organism(new Cells.Genetics.DNA(Array.Empty<byte>()), 100f, Vector2.One);
        self.Remember(0x20, value);

        var gene = new SkipIfGT(0x20, other, 0x01);
        var actualSkip = gene.Update(self, 1f  / 60);

        Assert.Equal(expectedSkip, actualSkip);
    }

    [Theory]
    [InlineData(0x02, 3f, 1)]
    [InlineData((byte)0x02, 3f, 1)]
    [InlineData(2f, 3f, 1)]
    [InlineData(2f, 0x02, 0)]
    [InlineData(2f, 3.0, 1)]
    [InlineData(null, 0.1f, 0)]
    [InlineData(5, null, 0)]
    public void SkipIfLTMem_ShouldReturnExpectedSkipValue(object value, object other, int expectedSkip)
    {
        var self = new Organism(new Cells.Genetics.DNA(Array.Empty<byte>()), 100f, Vector2.One);
        self.Remember(0x20, value);
        self.Remember(0x21, other);

        var gene = new SkipIfLTMem(0x20, 0x21, 0x01);
        var actualSkip = gene.Update(self, 1f  / 60);

        Assert.Equal(expectedSkip, actualSkip);
    }

    [Theory]
    [InlineData(0x02, 0x03, 1)]
    [InlineData((byte)0x02, 0x04, 1)]
    [InlineData(2f, 0x0A, 1)]
    [InlineData(2f, 0x02, 0)]
    [InlineData(2f, 0x05, 1)]
    [InlineData(null, 0x01, 0)]
    [InlineData(5, 0x0A, 1)]
    public void SkipIfLT_ShouldReturnExpectedSkipValue(object value, byte other, int expectedSkip)
    {
        var self = new Organism(new Cells.Genetics.DNA(Array.Empty<byte>()), 100f, Vector2.One);
        self.Remember(0x20, value);

        var gene = new SkipIfLT(0x20, other, 0x01);
        var actualSkip = gene.Update(self, 1f  / 60);

        Assert.Equal(expectedSkip, actualSkip);
    }

    [Theory]
    [InlineData(0x02, 2f, 1)]
    [InlineData((byte)0x02, 2f, 1)]
    [InlineData(2f, 2f, 1)]
    [InlineData(2f, 0x01, 0)]
    [InlineData(2f, 2.0, 1)]
    [InlineData(null, 0.1f, 0)]
    [InlineData(5, null, 0)]
    public void SkipIfEQMem_ShouldReturnExpectedSkipValue(object value, object other, int expectedSkip)
    {
        var self = new Organism(new Cells.Genetics.DNA(Array.Empty<byte>()), 100f, Vector2.One);
        self.Remember(0x20, value);
        self.Remember(0x21, other);

        var gene = new SkipIfEQMem(0x20, 0x21, 0x01);
        var actualSkip = gene.Update(self, 1f  / 60);

        Assert.Equal(expectedSkip, actualSkip);
    }

    [Theory]
    [InlineData(0x02, 0x02, 1)]
    [InlineData((byte)0x02, 0x02, 1)]
    [InlineData(2f, 0x02, 1)]
    [InlineData(2f, 0x03, 0)]
    [InlineData(2f, 0x01, 0)]
    [InlineData(null, 0x01, 0)]
    [InlineData(5, 0x05, 1)]
    public void SkipIfEQ_ShouldReturnExpectedSkipValue(object value, byte other, int expectedSkip)
    {
        var self = new Organism(new Cells.Genetics.DNA(Array.Empty<byte>()), 100f, Vector2.One);
        self.Remember(0x20, value);

        var gene = new SkipIfEQ(0x20, other, 0x01);
        var actualSkip = gene.Update(self, 1f  / 60);

        Assert.Equal(expectedSkip, actualSkip);
    }

    [Theory]
    [InlineData(0x02, 2f, 0)]
    [InlineData((byte)0x02, 2f, 0)]
    [InlineData(2f, 2f, 0)]
    [InlineData(2f, 0x01, 1)]
    [InlineData(2f, 2.0, 0)]
    [InlineData(null, 0.1f, 1)]
    [InlineData(5, null, 1)]
    public void SkipIfNEMem_ShouldReturnExpectedSkipValue(object value, object other, int expectedSkip)
    {
        var self = new Organism(new Cells.Genetics.DNA(Array.Empty<byte>()), 100f, Vector2.One);
        self.Remember(0x20, value);
        self.Remember(0x21, other);

        var gene = new SkipIfNEMem(0x20, 0x21, 0x01);
        var actualSkip = gene.Update(self, 1f  / 60);

        Assert.Equal(expectedSkip, actualSkip);
    }

    [Theory]
    [InlineData(0x02, 0x02, 0)]
    [InlineData((byte)0x02, 0x02, 0)]
    [InlineData(2f, 0x02, 0)]
    [InlineData(2f, 0x03, 1)]
    [InlineData(2f, 0x01, 1)]
    [InlineData(null, 0x01, 1)]
    [InlineData(5, 0x05, 0)]
    public void SkipIfNE_ShouldReturnExpectedSkipValue(object value, byte other, int expectedSkip)
    {
        var self = new Organism(new Cells.Genetics.DNA(Array.Empty<byte>()), 100f, Vector2.One);
        self.Remember(0x20, value);

        var gene = new SkipIfNE(0x20, other, 0x01);
        var actualSkip = gene.Update(self, 1f  / 60);

        Assert.Equal(expectedSkip, actualSkip);
    }
}
