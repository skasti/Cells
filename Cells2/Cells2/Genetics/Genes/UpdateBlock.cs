using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes
{
    public class UpdateBlock : ICanUpdate
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0x50, 0x5F, 2)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new UpdateBlock(
                    blockLength: fragment[1]
                );
            }
        }

        public int BlockLength { get; private set; }
        public float Cost { get; private set; } = 0f;
        public string Name { get; } = "UPDATE BLOCK";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        private readonly List<ICanUpdate> _updates = new List<ICanUpdate>();

        public UpdateBlock(int blockLength)
        {
            BlockLength = blockLength;
        }

        public void ReadGenes(int startIndex, List<IAmAGene> genes)
        {
            if (BlockLength == 0)
                return;

            for (int i = startIndex; i <= startIndex + BlockLength; i++)
            {
                if (i >= genes.Count)
                    break;

                if (genes[i] == this)
                    continue;

                if (genes[i] is ICanUpdate)
                    _updates.Add(genes[i] as ICanUpdate);
            }

            BlockLength = _updates.Count;
        }

        public int Update(Organism self, float deltaTime)
        {
            Log.Clear();
            if (_updates.Count == 0)
            {
                this.Log($"Update Block ({_updates.Count})");
                return 0;
            }
            var sw = new Stopwatch();
            Cost = 0;
            this.Log($"Update Block ({_updates.Count}) {{", 1);
            for (int i = 0; i < _updates.Count; i++)
            {
                var updater = _updates[i];
                updater.Log.Clear();
                sw.Restart();
                var skip = updater.Update(self, deltaTime);
                sw.Stop();
                var dt = sw.Elapsed * (1f/deltaTime);
                if (updater.Log.Count > 0)
                {
                    this.Log($"{updater.ToString()} {{", 1);
                    updater.Log.ForEach((l) => this.Log(l));
                    LogIndentLevel -= 1;
                    this.Log($"}} [C: {updater.Cost} S: {skip} dT: {dt}]");
                }
                else
                    this.Log($"{updater.ToString()} [C: {updater.Cost} S: {skip} dT: {dt}]");

                for (var j = i + 1; j < i + skip && j < _updates.Count; j++)
                    this.Log($"- {_updates[j].ToString()}");

                i += skip;
                Cost += updater.Cost;
            }
            LogIndentLevel -= 1;
            this.Log($"}}");

            return 0;
        }

        private string _string;
        public override string ToString()
        {
            if (_string == null)
                _string = $"UpdateBlock[{_updates.Count}]";

            return _string;
        }
    }
}
