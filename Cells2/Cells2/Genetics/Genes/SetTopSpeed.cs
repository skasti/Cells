using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;
using Microsoft.Xna.Framework;

namespace Cells.Genetics.Genes
{
    public class SetTopSpeed : ITrait, ICanUpdate
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0x39, 0x3F, 2)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new SetTopSpeed(
                    fragment[1].AsFloat(50f, 500f)
                    );
            }
        }

        private readonly float _topSpeed;

        public SetTopSpeed(float topSpeed)
        {
            _topSpeed = topSpeed;
        }

        public void Apply(Organism self)
        {
            self.TopSpeed = _topSpeed;
        }

        public int Update(Organism self, float deltaTime)
        {
            if (Game1.Debug == self)
                Debug.WriteLine("[TopSpeed] " + _topSpeed);

            self.TopSpeed = _topSpeed;
            return 0;
        }
    }
}
