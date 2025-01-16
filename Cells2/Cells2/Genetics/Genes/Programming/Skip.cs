using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes.Programming
{
    public class Skip: ICanUpdate
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0x03, 2)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new Skip(fragment[1].AsByte(0x10));
            }
        }

        private readonly byte _jumpSize;

        public Skip(byte jumpSize)
        {
            _jumpSize = jumpSize;
        }

        public int Update(Organism self, float deltaTime)
        {
            return _jumpSize;
        }
    }
}
