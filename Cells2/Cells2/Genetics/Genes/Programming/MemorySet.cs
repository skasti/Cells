using System.Collections.Generic;
using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes.Programming
{
    public class MemorySet: ICanUpdate
    {
        public class Maker : GeneMaker<MemorySet>
        {
            public Maker(byte markerFrom, byte markerTo)
                : base(markerFrom, markerTo, 3)
            {
            }

            public override MemorySet Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new MemorySet(
                    address: fragment[1].AsByte(0xFF, 0x11),
                    value: fragment[2]
                    );
            }

            public override byte[] MakeFragment(MemorySet gene)
            {
                var fragment = base.MakeFragment(gene);
                fragment[1] = gene.Address;
                fragment[2] = gene.Value;
                return fragment;
            }
        }
        public readonly byte Value;
        public readonly byte Address;
        public float Cost { get; private set; } = 0.5f;
        public string Name { get; } = "SET";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public MemorySet(byte address, byte value)
        {
            Address = address;
            Value = value;
        }

        public int Update(Organism self, float deltaTime)
        {
            self.Remember(Address, Value);
            return 0;
        }

        private string _string;
        public override string ToString()
        {
            if (_string == null)
                _string = $"{Name} [0x{Address:X2}] = {Value}";

            return _string;
        }
    }
}
