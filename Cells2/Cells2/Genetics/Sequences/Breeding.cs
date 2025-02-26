using System;
using Cells.GameObjects;
using Cells.Genetics.Genes;
using Cells.Genetics.Genes.Programming;

namespace Cells.Genetics.Sequences;

public class Breeding : Sequence
{
    public Breeding()
    {
        Genes.Add(
            new UpdateBlock(2).WithGenes(
                new ChaseObject(0x10, 200f),
                new TargetOrganisms(500f, 0x01, 0x10, 1f),
                new UpdateBlock(4).WithGenes(
                    new MemoryReadProperty(0x15, 0x10, GameObject.ReadableProperty.Fitness),
                    new MemoryReadProperty(0x16, 0x11, GameObject.ReadableProperty.Fitness),
                    new SkipIfLTMem(0x15, 0x16, 0x01),
                    new Breed(
                        targetAddress: 0x10,
                            energyThreshold: 1000f,
                            fitnessThreshold: 100f,
                            childSize: 0.25f,
                            spawnFrequency: 1f
                            )
                )));
    }
}
