using System.Collections.Generic;
using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;
using Microsoft.Xna.Framework;

namespace Cells.Genetics.Genes
{
    public class SetBaseMetabolicRate : ITrait
    {
        public class Maker : GeneMaker<SetBaseMetabolicRate>
        {
            public Maker(byte markerFrom, byte? markerTo = null)
                : base(markerFrom, markerTo ?? markerFrom, 2)
            {
            }

            public override SetBaseMetabolicRate Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new SetBaseMetabolicRate(fragment[1].AsFloat(0.001f, 10f));
            }

            public override byte[] MakeFragment(SetBaseMetabolicRate gene)
            {
                var fragment = base.MakeFragment(gene);
                fragment[1] = gene.Rate.AsGeneByte(0.001f, 10f);
                return fragment;
            }
        }

        public string Name { get; } = "BASE METABOLISM";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public readonly float Rate;
        public float Cost { get; private set; } = 2f;

        public SetBaseMetabolicRate(float rate)
        {
            Rate = rate;
        }

        public void Apply(Organism self, List<IAmAGene> genes)
        {
            self.BaseMetabolicRate = Rate;
        }

        private string _string = null;
        public override string ToString()
        {
            if (_string == null)
                _string = $"SET BaseMetabolicRate[{Rate:0.0000}]";

            return _string;
        }
    }
}
