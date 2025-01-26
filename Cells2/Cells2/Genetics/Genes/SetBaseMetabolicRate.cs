using System.Collections.Generic;
using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;
using Microsoft.Xna.Framework;

namespace Cells.Genetics.Genes
{
    public class SetBaseMetabolicRate : ITrait, ICanUpdate
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0x46, 0x47, 2)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new SetBaseMetabolicRate(fragment[1].AsFloat(0.001f, 10f));
            }
        }

        public string Name { get; } = "BASE METABOLISM";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        private readonly float _rate;
        public float Cost { get; private set; } = 2f;

        public SetBaseMetabolicRate(float rate)
        {
            _rate = rate;
        }

        public void Apply(Organism self)
        {
            self.BaseMetabolicRate = _rate;
        }

        public int Update(Organism self, float deltaTime)
        {
            self.BaseMetabolicRate = _rate;
            return 0;
        }

        private string _string = null;
        public override string ToString()
        {
            if (_string == null)
                _string = $"SET BaseMetabolicRate[{_rate:0.0000}]";

            return _string;
        }
    }
}
