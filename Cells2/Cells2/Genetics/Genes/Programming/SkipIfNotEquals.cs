using System.Collections.Generic;
using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes.Programming
{
    public class SkipIfNotEquals: ICanUpdate
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0x08, 4)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new SkipIfNotEquals(
                    memoryLocation: fragment[1].AsByte(0xFF),
                    value: fragment[2].AsByte(0xFF),
                    skipSize: fragment[3].AsByte(0x10, 0x01)
                    );
            }
        }
        private readonly byte _value;
        private readonly byte _memoryLocation;
        private readonly byte _skipSize;
        public float Cost { get; private set; } = 0.5f;
        public string Name { get; } = "IFNE";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public SkipIfNotEquals(byte memoryLocation, byte value, byte skipSize)
        {
            _memoryLocation = memoryLocation;
            _value = value;
            _skipSize = skipSize;
        }

        public int Update(Organism self, float deltaTime)
        {
            var value = self.Remember<byte>(_memoryLocation);
            var equals = value != _value;

            this.Log($"IF ([{_memoryLocation:X2}x0({value})] != {_value} ({equals})) SKIP {_skipSize}");
            return @equals ? _skipSize : 0;
        }

        private string _string;
        public override string ToString()
        {
            if (_string == null)
                _string = $"{Name} ([{_memoryLocation:X2}x0] != {_value}) SKIP {_skipSize}";

            return _string;
        }
    }
}
