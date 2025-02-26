using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes.Programming
{
    public class MemorySubtractMem : ICanUpdate
    {
        public class Maker : GeneMaker<MemorySubtractMem>
        {
            public Maker(byte markerFrom, byte markerTo)
                : base(markerFrom, markerTo, 3)
            {
            }

            public override MemorySubtractMem Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new MemorySubtractMem(
                    address: fragment[1].AsByte(0xFF, 0x11),
                    otherAddress: fragment[2].AsByte(0xFF, 0x11)
                    );
            }

            public override byte[] MakeFragment(MemorySubtractMem gene)
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
        public string Name { get; } = "SUB";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public MemorySubtractMem(byte address, byte otherAddress)
        {
            Address = address;
            OtherAddress = otherAddress;
        }
        public int Update(Organism self, float deltaTime)
        {
            var value = self.Remember(Address);
            var other = self.Remember(OtherAddress);

            if (value == null && other != null && Utils.IsNumeric(other))
            {
                var zero = Convert.ChangeType(0, other.GetType());
                var result = Utils.Subtract(zero, other);
                self.Remember(Address, result);
                this.Log($"{value} - {other} = {result}");
                return 0;
            }
            else if (value == null || other == null)
            {
                this.Log($"no value at 0x{Address:X2}({value}) or 0x{OtherAddress:X2}({other})");
                return 0;
            }
            else if (!Utils.IsNumeric(value))
            {
                this.Log($"unsupported type at [0x{Address:X2}]: {value.GetType().Name}");
                return 0;
            }
            else if (!Utils.IsNumeric(other))
            {
                this.Log($"unsupported type at [0x{OtherAddress:X2}]: {other.GetType().Name}");
                return 0;
            }

            try
            {
                var newValue = Utils.Subtract(value, other);
                self.Remember(Address, newValue);
                this.Log($"{value} - {other} = {newValue}");
            }
            catch (ArgumentNullException)
            {
                this.Log($"no value at 0x{Address:X2}({value}) or 0x{OtherAddress:X2}({other})");
            }
            catch (ArgumentException ex)
            {
                this.Log($"unsupported types: {ex.Message}");
            }

            return 0;
        }
        private string _string;
        public override string ToString()
        {
            if (_string == null)
                _string = $"{Name} [0x{Address:X2}] -= [0x{OtherAddress:X2}]";

            return _string;
        }
    }
}
