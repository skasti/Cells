using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes.Programming
{
    public class SkipIfLT: ICanUpdate
    {
        public class Maker : GeneMaker<SkipIfLT>
        {
            public Maker(byte markerFrom, byte markerTo)
                : base(markerFrom, markerTo, 4)
            {
            }

            public override SkipIfLT Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new SkipIfLT(
                    address: fragment[1].AsByte(0xFF),
                    value: fragment[2].AsByte(0xFF),
                    skipCount: fragment[3].AsByte(0x10, 0x01)
                    );
            }

            public override byte[] MakeFragment(SkipIfLT gene)
            {
                var fragment = base.MakeFragment(gene);
                fragment[1] = gene.Address;
                fragment[2] = gene.Value;
                fragment[3] = gene.SkipCount;
                return fragment;
            }
        }
        public readonly byte Value;
        public readonly byte Address;
        public readonly byte SkipCount;
        public float Cost { get; private set; } = 0.5f;
        public string Name { get; } = "IFLT";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public SkipIfLT(byte address, byte value, byte skipCount)
        {
            Address = address;
            Value = value;
            SkipCount = skipCount;
        }

        public int Update(Organism self, float deltaTime)
        {
            var value = self.Remember(Address);

            if (value == null)
            {
                var lt = Value > 0;

                if (lt)
                    this.Log($"{value} < {Value}");
                else
                    this.Log($"{value} >= {Value}");

                return lt ? SkipCount : 0;
            }
            else if (value is not IComparable)
            {
                this.Log($"unsupported type: {value.GetType().Name}");
                return 0;
            }

            try
            {
                var lt = Utils.IsLessThan(value, Value);

                if (lt)
                    this.Log($"{value} < {Value}");
                else
                    this.Log($"{value} >= {Value}");

                return lt ? SkipCount : 0;
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
                _string = $"{Name} ([0x{Address:X2}] < {Value}) SKIP {SkipCount}";

            return _string;
        }
    }
}
