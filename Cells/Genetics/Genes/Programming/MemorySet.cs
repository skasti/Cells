using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes.Programming
{
    public class MemorySet: ICanUpdate
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0x04, 3)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new MemorySet(
                    fragment[1].AsByte(0xFF),
                    fragment[2].AsByte(0xFF)
                    );
            }
        }
        private readonly byte _value;
        private readonly byte _memoryLocation;

        public MemorySet(byte memoryLocation, byte value)
        {
            _memoryLocation = memoryLocation;
            _value = value;
        }

        public int Update(Organism self, float deltaTime)
        {
            self.Remember(_memoryLocation, _value);

            if (Game1.Debug == self)
            {
                Debug.WriteLine("[MEM][{0}] = {1}",
                    _memoryLocation.ToString("X2"),
                    _value.ToString("X2"));
            }

            return 0;
        }
    }
}
