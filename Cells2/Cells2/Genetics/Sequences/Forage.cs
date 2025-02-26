using System;
using Cells.GameObjects;
using Cells.Genetics.Genes;
using Cells.Genetics.Genes.Programming;
using Cells.Genetics.Sequences;

namespace Cells2.Genetics.Sequences;

public class Forage: Sequence
{
    public Forage()
    {
        Genes.Add(
            new UpdateBlock(2).WithGenes(
                new Eat(0x02),
                new Armor(0.1f, 1f),
                new SmoothColorChange(0f, 1f, 0f, 1f, 1f),
                new UpdateBlock(2).WithGenes(
                    new ChaseObject(0x02, 500f),
                    new TargetFood(500f, 1, 0x02, 1f)
                )
            )
        );
    }
}
