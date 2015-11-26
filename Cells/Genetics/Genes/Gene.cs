using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cells.Genetics.Genes
{
    public interface IGeneDefinition
    {
        byte Marker { get; }
        int NumArguments { get; }

    }
}
