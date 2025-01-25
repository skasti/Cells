using System.Collections.Generic;
using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes.Programming
{
    public class MemorySubtract: ICanUpdate
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0x06, 3)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new MemorySubtract(
                    memoryLocation: fragment[1].AsByte(0xFF),
                    value: fragment[2].AsByte(0xFF)
                    );
            }
        }
        private readonly byte _value;
        private readonly byte _memoryLocation;
        public float Cost { get; private set; } = 1f;
        public string Name { get; } = "SUB";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public MemorySubtract(byte memoryLocation, byte value)
        {
            _memoryLocation = memoryLocation;
            _value = value;
        }

        public int Update(Organism self, float deltaTime)
        {
            var currentValue = self.Remember<byte>(_memoryLocation);
            var newValue = (byte)(currentValue - _value);
            self.Remember(_memoryLocation, newValue);

            this.Log($"{ToString()} ({newValue})");
            return 0;
        }

        private string _string;
        public override string ToString()
        {
            if (_string == null)
                _string = $"{Name} [{_memoryLocation:X2}x0] -= {_value}";

            return _string;
        }
    }
}
