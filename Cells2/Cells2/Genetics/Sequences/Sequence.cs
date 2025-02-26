using System;
using System.Collections.Generic;
using System.Linq;
using Cells.Genetics;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Sequences;

public class Sequence
{
    public readonly List<IAmAGene> Genes = new List<IAmAGene>();
    public byte[] MakeFragment()
    {
        var parts = new List<byte[]>();
        var makers = GeneInterpreter.Makers;
        foreach (var gene in Genes)
        {
            var maker = makers.FirstOrDefault(m => m.GeneType == gene.GetType());
            if (maker == null)
                throw new MissingMakerException(gene);

            parts.Add(maker.ToFragment(gene));
        }

        return parts.Join().ToArray();
    }

    [Serializable]
    private class MissingMakerException : Exception
    {
        private IAmAGene gene;

        public MissingMakerException()
        {
        }

        public MissingMakerException(IAmAGene gene)
        :base($"Missing maker for {gene.GetType().Name}")
        {
            this.gene = gene;
        }

        public MissingMakerException(string message) : base(message)
        {
        }

        public MissingMakerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
