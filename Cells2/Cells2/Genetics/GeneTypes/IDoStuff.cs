using System.Collections.Generic;

namespace Cells.Genetics.GeneTypes
{
    public interface IDoStuff
    {
        float Cost { get; }
        List<string> Log { get; }
        int LogIndentLevel { get; set; }
    }
}