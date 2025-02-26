using System.Collections.Generic;
using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes.Programming
{
    public class MemoryCpy : ICanUpdate
    {
        public class Maker : GeneMaker<MemoryCpy>
        {
            public Maker(byte markerFrom, byte markerTo)
                : base(markerFrom, markerTo, 3)
            {
            }

            public override MemoryCpy Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new MemoryCpy(
                    address: fragment[1],
                    otherAddress: fragment[2]
                    );
            }

            public override byte[] MakeFragment(MemoryCpy gene)
            {
                var fragment = base.MakeFragment(gene);
                fragment[1] = gene.Address;
                fragment[2] = gene.OtherAddress;
                return fragment;
            }
        }
        public readonly byte OtherAddress;
        public readonly byte Address;
        public float Cost { get; private set; } = 0.5f;
        public string Name { get; } = "CPY";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public MemoryCpy(byte address, byte otherAddress)
        {
            Address = address;
            OtherAddress = otherAddress;
        }

        public int Update(Organism self, float deltaTime)
        {
            var otherValue = self.Remember(OtherAddress);
            self.Remember(Address, otherValue);
            return 0;
        }

        private string _string;
        public override string ToString()
        {
            if (_string == null)
                _string = $"{Name} [0x{Address:X2}] = [0x{OtherAddress:X2}]";

            return _string;
        }
    }
}
