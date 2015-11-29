using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GenomeIDE
{
    public class Gene
    {
        public Gene(byte markerFrom, byte markerTo, int argumentBytes, string name, params string[] argumentHelp)
        {
            if (argumentHelp.Length != argumentBytes)
                throw new ArgumentException("ArgumentHelp must match umber of arguments");

            MarkerFrom = markerFrom;
            MarkerTo = markerTo;
            ArgumentBytes = argumentBytes;
            Name = name;
            ArgumentHelp = argumentHelp;
        }

        public byte MarkerFrom { get; private set; }
        public byte MarkerTo { get; private set; }
        public int ArgumentBytes { get; private set; }

        public string Name { get; private set; }
        public string[] ArgumentHelp { get; private set; }

        public TreeNode CreateNode(List<byte> fragment)
        {
            var fI = 0;
            var node = new TreeNode(fragment[fI++].ToString("X2") + ": " + Name);

            foreach (var argument in ArgumentHelp)
            {
                var child = new TreeNode(fragment[fI++].ToString("X2") + " " + argument);
                node.Nodes.Add(child);
            }

            return node;
        }

        public bool Match(byte marker)
        {
            return marker >= MarkerFrom && marker <= MarkerTo;
        }

        public bool Crash(Gene gene)
        {
            return Match(gene.MarkerFrom) || Match(gene.MarkerTo);
        }

        
    }
}
