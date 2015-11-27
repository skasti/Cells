using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics
{
    public interface IMakeAGene
    {
        byte MarkerFrom { get; }
        byte MarkerTo { get; }

        int Size { get; }
        int ArgumentBytes { get; }

        IAmAGene Make(byte[] fragment);
    }
}
