using System.Collections.Generic;
using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;
using Microsoft.Xna.Framework;

namespace Cells.Genetics.Genes
{
    public class SetMovementMetabolicRate : ITrait, ICanUpdate
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0x48, 0x49, 2)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new SetMovementMetabolicRate(fragment[1].AsFloat(0.0001f, 1f));
            }
        }
        private readonly float _rate;
        public float Cost { get; private set; } = 2f;
        public string Name { get; } = "MOVEMENT METABOLISM";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public SetMovementMetabolicRate(float rate) {
            _rate = rate;
        }

        public void Apply(Organism self)
        {
            self.MovementMetabolicRate = _rate;
        }

        public int Update(Organism self, float deltaTime)
        {
            self.MovementMetabolicRate = _rate;
            return 0;
        }

        private string _string = null;
        public override string ToString()
        {
            if (_string == null)
                _string = $"SET MovementMetabolicRate[{_rate:0.0000}]";

            return _string;
        }
    }
}
