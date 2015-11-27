using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes.Programming
{
    public class SkipIfLT: ICanUpdate
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0x0A, 4)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new SkipIfLT(
                    fragment[1].AsByte(0xFF),
                    fragment[2].AsByte(0xFF),
                    fragment[3].AsByte(0x10, 0x01)
                    );
            }
        }
        private readonly byte _value;
        private readonly byte _memoryLocation;
        private readonly byte _skipSize;

        public SkipIfLT(byte memoryLocation, byte value, byte skipSize)
        {
            _memoryLocation = memoryLocation;
            _value = value;
            _skipSize = skipSize;
        }

        public int Update(Organism self, float deltaTime)
        {
            var value = self.Remember<byte>(_memoryLocation);
            var equals = value < _value;

            if (Game1.Debug == self)
            {
                Debug.WriteLine("[MEM][{0}({1})] < {2} ({3}) SKIP {4}",
                    _memoryLocation.ToString("X2"),
                    value,
                    _value.ToString("X2"),
                    equals,
                    _skipSize);
            }

            return @equals ? _skipSize : 0;
        }
    }
}
