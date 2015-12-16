using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace GenomeIDE
{
    public class Gene
    {
        public Gene(byte markerFrom, byte markerTo, string name, List<IGeneArgument> arguments)
        {
            MarkerFrom = markerFrom;
            MarkerTo = markerTo;
            Name = name;
            Arguments = arguments;
        }

        public byte MarkerFrom { get; private set; }
        public byte MarkerTo { get; private set; }

        public int ArgumentBytes
        {
            get { return Arguments.Count; }
        }

        public string Name { get; private set; }
        public List<IGeneArgument> Arguments { get; private set; }
            
        [JsonIgnore]
        public int Size
        {
            get { return ArgumentBytes + 1; }
        }

        public TreeNode CreateNode(List<byte> fragment)
        {
            var fI = 0;
            var node = new TreeNode("{0:X2}: {1}".Inject(fragment[fI++], Name));
            var argumentsNode = new TreeNode("Arguments");
            foreach (var argument in Arguments)
            {
                var b = fragment[fI++];
                var bValue = argument.Calculate(b);
                var nodeText = "{0:X2} (={1})".Inject(b, bValue);
                var child = new TreeNode(nodeText)
                {
                    ToolTipText = argument.Description
                };
                argumentsNode.Nodes.Add(child);
            }
            node.Nodes.Add(argumentsNode);

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

        public override string ToString()
        {
            return "0x{0:X2}-0x{1:X2}: {2} ({3})".Inject(
                MarkerFrom,
                MarkerTo,
                Name,
                ArgumentBytes);
        }
    }
}
