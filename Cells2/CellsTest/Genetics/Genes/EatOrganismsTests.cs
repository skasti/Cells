using System;

namespace CellsTest.Genetics.Genes;

using System;
using System.Collections.Generic;
using Xunit;
using Cells.Genetics;
using Cells.Genetics.Genes;

public class EatOrganismsTests
{
    [Fact]
    public void MakeFragment_ShouldEncodeEatOrganismsGeneCorrectly()
    {
        var maker = new EatOrganisms.Maker(0x70, 0x71);
        var eatOrganisms = new EatOrganisms(0x10, 0x20, 0x30, 0x40, 0x50, 0.5f, 0.7f);

        byte[] fragment = maker.MakeFragment(eatOrganisms);

        Assert.Equal(0x10, fragment[1]); // BlockLength
        Assert.Equal(0x20, fragment[2]); // TargetAddress
        Assert.Equal(0x30, fragment[3]); // DeadGoto
        Assert.Equal(0x40, fragment[4]); // TooFarGoto
        Assert.Equal(0x50, fragment[5]); // BiggerGoto
        Assert.Equal(0x80, fragment[6]); // Precomputed for 0.5f (dnaSampleSize)
        Assert.Equal(0xB3, fragment[7]); // Precomputed for 0.7f (relationThreshold)
    }

    [Fact]
    public void Make_ShouldDecodeFragmentIntoEatOrganismsGene()
    {
        var maker = new EatOrganisms.Maker(0x70, 0x71);
        byte[] fragment = [0x70, 0x10, 0x20, 0x30, 0x40, 0x50, 0x80, 0xB3, 0x71];

        var eatOrganisms = maker.Make(fragment);

        Assert.Equal(0x10, eatOrganisms.BlockLength);
        Assert.Equal(0x20, eatOrganisms.TargetAddress);
        Assert.Equal(0x30, eatOrganisms.DeadGoto);
        Assert.Equal(0x40, eatOrganisms.TooFarGoto);
        Assert.Equal(0x50, eatOrganisms.BiggerGoto);
        Assert.InRange(eatOrganisms.DnaSampleSize, 0.49f, 0.51f);
        Assert.InRange(eatOrganisms.RelationThreshold, 0.69f, 0.71f);
    }

    [Fact]
    public void MakeFragmentAndMake_ShouldBeConsistent()
    {
        var maker = new EatOrganisms.Maker(0x70, 0x71);
        var originalEatOrganisms = new EatOrganisms(0x10, 0x20, 0x30, 0x40, 0x50, 0.5f, 0.7f);

        byte[] fragment = maker.MakeFragment(originalEatOrganisms);
        var reconstructedEatOrganisms = maker.Make(fragment);

        Assert.Equal(originalEatOrganisms.BlockLength, reconstructedEatOrganisms.BlockLength);
        Assert.Equal(originalEatOrganisms.TargetAddress, reconstructedEatOrganisms.TargetAddress);
        Assert.Equal(originalEatOrganisms.DeadGoto, reconstructedEatOrganisms.DeadGoto);
        Assert.Equal(originalEatOrganisms.TooFarGoto, reconstructedEatOrganisms.TooFarGoto);
        Assert.Equal(originalEatOrganisms.BiggerGoto, reconstructedEatOrganisms.BiggerGoto);
        Assert.InRange(reconstructedEatOrganisms.DnaSampleSize, 0.49f, 0.51f);
        Assert.InRange(reconstructedEatOrganisms.RelationThreshold, 0.69f, 0.71f);
    }
}

