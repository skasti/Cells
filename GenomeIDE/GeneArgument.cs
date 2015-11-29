using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenomeIDE
{
    public interface IGeneArgument
    {
        string Description { get; }
        string Calculate(byte input);
    }
}
