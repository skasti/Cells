using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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

        public static string Indent(this IAmAGene gene, int level)
        {
            return Indent(level);
        }

        public static Stopwatch LogStopwatch = new Stopwatch();
        public static void Log(this IDoStuff gene, string logLine, int indentChange = 0)
        {
            LogStopwatch.Start();
            gene.Log.Add(Indent(gene.LogIndentLevel) + logLine);
            gene.LogIndentLevel += indentChange;
            LogStopwatch.Stop();
        }

        public static string Indent(int level)
        {
            var output = new StringBuilder();
            for (var i = 0; i < level; i++)
                output.Append("    ");
            return output.ToString();
        }
    }
}
