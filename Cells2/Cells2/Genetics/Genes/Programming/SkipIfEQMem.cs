using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;
using Cells.Genetics.Genes.Programming;

namespace Cells.Genetics.Genes.Programming
{
    public class SkipIfEQMem: ICanUpdate
    {
        public class Maker : GeneMaker<SkipIfEQMem>
        {
            public Maker(byte markerFrom, byte markerTo)
                : base(markerFrom, markerTo, 4)
            {
            }

            public override SkipIfEQMem Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new SkipIfEQMem(
                    address: fragment[1].AsByte(0xFF),
                    otherAddress: fragment[2].AsByte(0xFF),
                    skipCount: fragment[3].AsByte(0x10, 0x01)
                    );
            }

            public override byte[] MakeFragment(SkipIfEQMem gene)
            {
                var fragment = base.MakeFragment(gene);
                fragment[1] = gene.Address;
                fragment[2] = gene.OtherAddress;
                fragment[3] = gene.SkipCount;
                return fragment;
            }
        }
        public readonly byte OtherAddress;
        public readonly byte Address;
        public readonly byte SkipCount;
        public float Cost { get; private set; } = 0.5f;
        public string Name { get; } = "IFEQ";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public SkipIfEQMem(byte address, byte otherAddress, byte skipCount)
        {
            Address = address;
            OtherAddress = otherAddress;
            SkipCount = skipCount;
        }

        public int Update(Organism self, float deltaTime)
        {
            var value = self.Remember(Address);
            var other = self.Remember(OtherAddress);

            if (value == null && other == null)
            {
                this.Log($"{value} == {other}");
                return SkipCount;
            }
            else if (value == null || other == null)
            {
                this.Log($"{value} != {other}");
                return 0;
            }
            else if (value is not IComparable)
            {
                this.Log($"unsupported type: {value.GetType().Name}");
                return 0;
            }
            else if (other is not IComparable)
            {
                this.Log($"unsupported type: {other.GetType().Name}");
                return 0;
            }

            try {
                var eq = Utils.IsEqual(value, other);

                if (eq)
                    this.Log($"{value} == {other}");
                else
                    this.Log($"{value} != {other}");

                return eq ? SkipCount : 0;
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
                _string = $"{Name} ([0x{Address:X2}] == [0x{OtherAddress:X2}]) SKIP {SkipCount}";

            return _string;
        }
    }
}
