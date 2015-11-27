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

        public Break(float percentToBreak)
        {
            PercentToBreak = percentToBreak;
        }

        public int Update(Organism self, float deltaTime)
        {
            self.Force = (-self.Velocity / deltaTime) * self.Mass * PercentToBreak;
            return 0;
        }
    }
}
