using System;
using Cells.Genetics.Genes;

namespace CellsTest.Genetics.Genes;

public class SetColorTests
{
    [Fact]
    public void MakeFragment_ShouldEncodeSetColorGeneCorrectly()
    {
        var maker = new SetColor.Maker(0x80, 0x81);
        var setColor = new SetColor(0x80, 0x99, 0xB3, 0xCC);

        byte[] fragment = maker.MakeFragment(setColor);

        Assert.Equal(0x80, fragment[1]); // Precomputed for 0.5f
        Assert.Equal(0x99, fragment[2]); // Precomputed for 0.6f
        Assert.Equal(0xB3, fragment[3]); // Precomputed for 0.7f
        Assert.Equal(0xCC, fragment[4]); // Precomputed for 0.8f
    }

    [Fact]
    public void Make_ShouldDecodeFragmentIntoSetColorGene()
    {
        var maker = new SetColor.Maker(0x80, 0x81);
        byte[] fragment = [0x80, 0x80, 0x99, 0xB3, 0xCC, 0x81];

        var setColor = maker.Make(fragment);

        Assert.Equal(0x80, setColor.Color.R);
        Assert.Equal(0x99, setColor.Color.G);
        Assert.Equal(0xB3, setColor.Color.B);
        Assert.Equal(0xCC, setColor.Color.A);
    }

    [Fact]
    public void MakeFragmentAndMake_ShouldBeConsistent()
    {
        var maker = new SetColor.Maker(0x80, 0x81);
        var originalSetColor = new SetColor(0x80, 0x99, 0xCC, 0x81);

        byte[] fragment = maker.MakeFragment(originalSetColor);
        var reconstructedSetColor = maker.Make(fragment);

        Assert.Equal(0x80, reconstructedSetColor.Color.R);
        Assert.Equal(0x99, reconstructedSetColor.Color.G);
        Assert.Equal(0xB3, reconstructedSetColor.Color.B);
        Assert.Equal(0xCC, reconstructedSetColor.Color.A);
    }
}

