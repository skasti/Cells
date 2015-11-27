using System.Collections.Generic;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics
{
    public static class GeneExtensions
    {
        public static int FirstIndexOf<T>(this List<IAmAGene> genes, int startIndex = 0) where T : IAmAGene
        {
            for (int i = startIndex; i < genes.Count; i++)
            {
                if (genes[i] is T)
                    return i;
            }

            return -1;
        }
    }
}
