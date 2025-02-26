using System.Collections.Generic;
using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;
using Microsoft.Xna.Framework;

namespace Cells.Genetics.Genes
{
    public class SetMovementMetabolicRate : ITrait
    {
        public class Maker : GeneMaker<SetMovementMetabolicRate>
        {
            public Maker(byte markerFrom, byte? markerTo = null)
                : base(markerFrom, markerTo ?? markerFrom, 2)
            {
            }

            public override SetMovementMetabolicRate Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new SetMovementMetabolicRate(fragment[1].AsFloat(0.01f, 10f));
            }
            public override byte[] MakeFragment(SetMovementMetabolicRate gene)
            {
                var fragment = base.MakeFragment(gene);
                fragment[1] = gene.Rate.AsGeneByte(0.01f, 10f);
                return fragment;
            }
        }
        public readonly float Rate;
        public float Cost { get; private set; } = 2f;
        public string Name { get; } = "MOVEMENT METABOLISM";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public SetMovementMetabolicRate(float rate)
        {
            Rate = rate;
        }

        public void Apply(Organism self, List<IAmAGene> genes)
        {
            self.MovementMetabolicRate = Rate;
        }

        private string _string = null;
        public override string ToString()
        {
            if (_string == null)
                _string = $"SET MovementMetabolicRate[{Rate:0.0000}]";

            return _string;
        }
    }
}
