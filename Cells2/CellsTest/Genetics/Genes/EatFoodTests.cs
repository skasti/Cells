using System;
using Cells.Genetics.Genes;

namespace CellsTest.Genetics.Genes;

public class EatFoodTests
{
    [Fact]
    public void MakeFragment_ShouldEncodeEatFoodGeneCorrectly()
    {
        var maker = new EatFood.Maker(0x60, 0x61);
        var eatFood = new EatFood(0x10, 0x20, 0x30, 0x40);

        byte[] fragment = maker.MakeFragment(eatFood);

        Assert.Equal(0x10, fragment[1]);
        Assert.Equal(0x20, fragment[2]);
        Assert.Equal(0x30, fragment[3]);
        Assert.Equal(0x40, fragment[4]);
    }

    [Fact]
    public void Make_ShouldDecodeFragmentIntoEatFoodGene()
    {
        var maker = new EatFood.Maker(0x60, 0x61);
        byte[] fragment = [0x60, 0x10, 0x20, 0x30, 0x40, 0x61];

        var eatFood = maker.Make(fragment);

        Assert.Equal(0x10, eatFood.BlockLength);
        Assert.Equal(0x10, eatFood.TargetAddress);
        Assert.Equal(0x10, eatFood.TooFarGoto);
        Assert.Equal(0x01, eatFood.DeadGoto);
    }

    [Fact]
    public void MakeFragmentAndMake_ShouldBeConsistent()
    {
        var maker = new EatFood.Maker(0x60, 0x61);
        var originalEatFood = new EatFood(0x10, 0x20, 0x30, 0x40);

        byte[] fragment = maker.MakeFragment(originalEatFood);
        var reconstructedEatFood = maker.Make(fragment);

        Assert.Equal(originalEatFood.BlockLength, reconstructedEatFood.BlockLength);
        Assert.Equal(originalEatFood.TargetAddress, reconstructedEatFood.TargetAddress);
        Assert.Equal(originalEatFood.TooFarGoto, reconstructedEatFood.TooFarGoto);
        Assert.Equal(originalEatFood.DeadGoto, reconstructedEatFood.DeadGoto);
    }
}


