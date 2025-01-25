using System.Collections.Generic;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes
{
    public class Break: ICanUpdate
    {
        public class Maker: GeneMaker
        {
            public Maker()
                : base(0x00, 0x02, 2)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new Break(fragment[1].AsFloat(0.01f,1f));
            }
        }

        public float PercentToBreak { get; private set; }
        public float Cost { get; private set; } = 1f;
        public string Name { get; } = "BREAK";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public Break(float percentToBreak)
        {
            PercentToBreak = percentToBreak;
        }

        public int Update(Organism self, float deltaTime)
        {
            var forceAdd = (-self.Velocity / deltaTime) * self.Mass * PercentToBreak;
            self.Force += forceAdd;
            this.Log($"BREAKING({PercentToBreak*100f:0.}%): {forceAdd.ToShortString()} ({self.Force.ToShortString()})");
            return 0;
        }

        private string _string;
        public override string ToString()
        {
            if (_string == null)
                _string = $"{Name} [{PercentToBreak*100f:0.}%]";

            return _string;
        }
    }
}
