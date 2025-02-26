using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes.Programming
{
    public class MemorySubtract : ICanUpdate
    {
        public class Maker : GeneMaker<MemorySubtract>
        {
            public Maker(byte markerFrom, byte markerTo)
                : base(markerFrom, markerTo, 3)
            {
            }
            public override MemorySubtract Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new MemorySubtract(
                    address: fragment[1].AsByte(0xFF, 0x11),
                    value: fragment[2].AsByte(0xFF)
                    );
            }

            public override byte[] MakeFragment(MemorySubtract gene)
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
        public string Name { get; } = "SUB";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public MemorySubtract(byte address, byte value)
        {
            Address = address;
            Value = value;
        }

        public int Update(Organism self, float deltaTime)
        {
            var value = self.Remember(Address);

            if (value == null)
            {
                var result = (byte)(0xFF - Value);
                self.Remember(Address, result);
                this.Log($"{value} - {Value} = {result}");
                return 0;
            }
            else if (!Utils.IsNumeric(value))
            {
                this.Log($"unsupported type at [0x{Address:X2}]: {value.GetType().Name}");
                return 0;
            }

            try
            {
                var result = Utils.Subtract(value, Value);
                self.Remember(Address, result);
                this.Log($"{value} - {Value} = {result}");
            }
            catch (ArgumentNullException)
            {
                this.Log($"no value at 0x{Address:X2}");
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
                _string = $"{Name} [0x{Address:X2}] -= {Value}";

            return _string;
        }
    }
}
