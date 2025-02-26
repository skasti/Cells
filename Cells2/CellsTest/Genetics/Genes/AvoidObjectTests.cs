using System;
using System.Collections.Generic;
using Xunit;
using Cells.Genetics;
using Cells.Genetics.Genes;
using Cells.Genetics.Exceptions;

namespace CellsTest.Genetics.Genes;
public class AvoidObjectMakerTests
{
    [Fact]
    public void MakeFragment_ShouldEncodeAvoidObjectGeneCorrectly()
    {
        var maker = new AvoidObject.Maker(0x40, 0x41);
        var avoidObject = new AvoidObject(targetAddress: 0x0F, desiredSpeed: 250f);

        byte[] fragment = maker.MakeFragment(avoidObject);

        Assert.Equal(0x0F, fragment[1]); // Direct byte value
        Assert.Equal(0x7F, fragment[2]); // Manually precomputed value for 250f mapped to byte
    }

    [Fact]
    public void Make_ShouldDecodeFragmentIntoAvoidObjectGene()
    {
        var maker = new AvoidObject.Maker(0x40, 0x41);
        byte[] fragment = [0x40, 0x0F, 0x7F, 0x41]; // Direct byte values

        var avoidObject = maker.Make(fragment);

        Assert.Equal(0x0F, avoidObject.TargetAddress);
        Assert.InRange(avoidObject.DesiredSpeed, 249f, 251f); // Allow small precision error
    }

    [Fact]
    public void MakeFragmentAndMake_ShouldBeConsistent()
    {
        var maker = new AvoidObject.Maker(0x40, 0x41);
        var originalAvoidObject = new AvoidObject(targetAddress: 0x0F, desiredSpeed: 250f);

        byte[] fragment = maker.MakeFragment(originalAvoidObject);
        var reconstructedAvoidObject = maker.Make(fragment);

        Assert.Equal(0x0F, reconstructedAvoidObject.TargetAddress);
        Assert.InRange(reconstructedAvoidObject.DesiredSpeed, 249f, 251f);
    }

    [Fact]
    public void Make_ShouldThrowException_WhenFragmentIsTooShort()
    {
        var maker = new AvoidObject.Maker(0x40, 0x41);
        byte[] invalidFragment = [0x40];

        Assert.Throws<GenomeTooShortException>(() => maker.Make(invalidFragment));
    }
}
