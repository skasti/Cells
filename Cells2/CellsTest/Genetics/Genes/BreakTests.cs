using System;
using Cells.Genetics.Genes;

namespace CellsTest.Genetics.Genes;

public class BreakTests
{
    [Fact]
    public void MakeFragment_ShouldEncodeBreakGeneCorrectly()
    {
        var maker = new Break.Maker(0x50);
        var breakGene = new Break(0.5f);

        byte[] fragment = maker.MakeFragment(breakGene);

        Assert.Equal(0x7E, fragment[1]); // Precomputed for 0.5f
    }

    [Fact]
    public void Make_ShouldDecodeFragmentIntoBreakGene()
    {
        var maker = new Break.Maker(0x50);
        byte[] fragment = [0x50, 0x80];

        var breakGene = maker.Make(fragment);

        Assert.InRange(breakGene.PercentToBreak, 0.49f, 0.51f);
    }

    [Fact]
    public void MakeFragmentAndMake_ShouldBeConsistent()
    {
        var maker = new Break.Maker(0x50);
        var originalBreak = new Break(0.5f);

        byte[] fragment = maker.MakeFragment(originalBreak);
        var reconstructedBreak = maker.Make(fragment);

        Assert.InRange(reconstructedBreak.PercentToBreak, 0.49f, 0.51f);
    }
}

