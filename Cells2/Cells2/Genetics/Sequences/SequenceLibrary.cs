using System;
using System.Collections.Generic;
using Cells2.Genetics.Sequences;

namespace Cells.Genetics.Sequences;

public static class SequenceLibrary
{
    public static List<Sequence> Sequences = new List<Sequence> {
        new Breeding(),
        new Hunting(),
        new Forage()
    };

    private static Random _random = new Random((int)DateTime.Now.Ticks);
    internal static Sequence RandomSequence()
    {
        return Sequences[_random.Next(Sequences.Count - 1)];
    }
}
