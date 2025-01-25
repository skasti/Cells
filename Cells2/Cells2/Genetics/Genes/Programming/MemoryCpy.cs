using System.Collections.Generic;
using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes.Programming
{
    public class MemoryCpy : ICanUpdate
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0xF2, 0xF3, 3)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new MemoryCpy(
                    memoryLocation: fragment[1].AsByte(0xFF),
                    otherMemoryLocation: fragment[2].AsByte(0xFF)
                    );
            }
        }
        private readonly byte _otherMemoryLocation;
        private readonly byte _memoryLocation;
        public float Cost { get; private set; } = 1f;
        public string Name { get; } = "SET";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public MemoryCpy(byte memoryLocation, byte otherMemoryLocation)
        {
            _memoryLocation = memoryLocation;
            _otherMemoryLocation = otherMemoryLocation;
        }

        public int Update(Organism self, float deltaTime)
        {
            var otherValue = self.Remember<byte>(_otherMemoryLocation);
            self.Remember(_memoryLocation, otherValue);
            this.Log(ToString());
            return 0;
        }

        private string _string;
        public override string ToString()
        {
            if (_string == null)
                _string = $"{Name} [{_memoryLocation:X2}x0] = [{_otherMemoryLocation:X2}x0]";

            return _string;
        }
    }
}
