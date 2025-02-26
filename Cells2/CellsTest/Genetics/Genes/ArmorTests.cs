using System;
using Cells.Genetics;
using Cells.Genetics.Exceptions;
using Cells.Genetics.Genes;

namespace CellsTest.Genetics.Genes;

public class ArmorTests
{
    [Fact]
    public void MakeFragment_ShouldEncodeArmorGeneCorrectly()
    {
        var maker = new Armor.Maker(0x30, 0x31);
        var armor = new Armor(regenSpeed: 0.5f, regenInterval: 2.5f);

        byte[] fragment = maker.MakeFragment(armor);

        Assert.Equal(0x71, fragment[1]); // Precomputed for 0.5f
        Assert.Equal(0x71, fragment[2]); // Precomputed for 2.5f
    }

    [Fact]
    public void Make_ShouldDecodeFragmentIntoArmorGene()
    {
        var maker = new Armor.Maker(0x30, 0x31);
        byte[] fragment = [0x30, 0x71, 0x71];

        var armor = maker.Make(fragment);

        Assert.InRange(armor.RegenSpeed, 0.49f, 0.51f);
        Assert.InRange(armor.RegenInterval, 2.49f, 2.51f);
    }

    [Fact]
    public void Make_ShouldThrowException_WhenFragmentIsTooShort()
    {
        var maker = new Armor.Maker(0x30, 0x31);
        byte[] invalidFragment = [0x30];

        Assert.Throws<GenomeTooShortException>(() => maker.Make(invalidFragment));
    }
}
