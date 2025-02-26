using System.Collections.Generic;
using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;
using Microsoft.Xna.Framework;

namespace Cells.Genetics.Genes
{
    public class SetTopSpeed : ITrait
    {
        public class Maker : GeneMaker<SetTopSpeed>
        {
            public Maker(byte markerFrom, byte markerTo)
                : base(markerFrom, markerTo, 2)
            {
            }

            public override SetTopSpeed Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new SetTopSpeed(
                    topSpeed: fragment[1].AsFloat(50f, 500f)
                    );
            }

            public override byte[] MakeFragment(SetTopSpeed gene)
            {
                var fragment = base.MakeFragment(gene);
                fragment[1] = gene.TopSpeed.AsGeneByte(50f, 500f);
                return fragment;
            }
        }

        public readonly float TopSpeed;
        public float Cost { get; private set; } = 2f;
        public string Name { get; } = "TOP SPEED";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public SetTopSpeed(float topSpeed)
        {
            TopSpeed = topSpeed;
        }

        public void Apply(Organism self, List<IAmAGene> genes)
        {
            self.TopSpeed = TopSpeed;
        }

        private string _string = null;
        public override string ToString()
        {
            if (_string == null)
                _string = $"SET TopSpeed[{TopSpeed}]";

            return _string;
        }
    }
}
