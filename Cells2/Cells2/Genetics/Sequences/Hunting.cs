using System;
using Cells.GameObjects;
using Cells.Genetics.Genes;
using Cells.Genetics.Genes.Programming;
using Cells.Genetics.Sequences;

namespace Cells2.Genetics.Sequences;

public class Hunting: Sequence
{
    public Hunting()
    {
        Genes.Add(
            new UpdateBlock(2).WithGenes(
                new Attack(0x05, 0.5f, 0.95f, 0.25f, 0.5f),
                new SmoothColorChange(1f, 0f, 0f, 1f, 1f),
                new UpdateBlock(2).WithGenes(
                    new ChaseObject(0x05, 500f),
                    new TargetOrganisms(800f, 1, 0x05, 1f)
                )
            )
        );
    }
}
