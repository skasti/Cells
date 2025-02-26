using System;
using Cells.Genetics;
using Cells.Genetics.Genes;

namespace CellsTest.Genetics.Genes;

public class AttackTests
{
    [Fact]
    public void MakeFragment_ShouldEncodeAttackGeneCorrectly()
    {
        var maker = new Attack.Maker(0x20, 0x21);
        var attack = new Attack(
            blockLength: 0x08,
            targetAddress: 0x0A,
            dnaSampleSize: 0.5f,
            relationThreshold: 0.7f,
            attackForce: 0.05f,
            attackInterval: 2.5f
        );

        byte[] fragment = maker.MakeFragment(attack);

        Assert.Equal(0x0A, fragment[2]); // Direct byte value
        Assert.Equal(0x80, fragment[3]); // Manually precomputed value for 0.5f
        Assert.Equal(0xB3, fragment[4]); // Precomputed for 0.7f
        Assert.Equal(0x1F, fragment[5]); // Precomputed for 0.05f
        Assert.Equal(0x80, fragment[6]); // Precomputed for 2.5f
    }

    [Fact]
    public void Make_ShouldDecodeFragmentIntoAttackGene()
    {
        var maker = new Attack.Maker(0x20, 0x21);
        byte[] fragment = [0x20, 0x08, 0x0A, 0x80, 0xB3, 0x1F, 0x80, 0x21];

        var attack = maker.Make(fragment);

        Assert.Equal(0x0A, attack.TargetAddress);
        Assert.InRange(attack.DnaSampleSize, 0.49f, 0.51f);
        Assert.InRange(attack.RelationThreshold, 0.69f, 0.71f);
        Assert.InRange(attack.AttackForce, 0.049f, 0.051f);
        Assert.InRange(attack.AttackInterval, 2.49f, 2.51f);
    }

}
