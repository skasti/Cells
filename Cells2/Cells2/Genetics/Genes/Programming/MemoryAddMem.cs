using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Versioning;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes.Programming
{
    public class MemoryAddMem : ICanUpdate
    {
        public class Maker : GeneMaker<MemoryAddMem>
        {
            public Maker(byte markerFrom, byte markerTo)
                : base(markerFrom, markerTo, 3)
            {
            }

            public override MemoryAddMem Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new MemoryAddMem(
                    address: fragment[1].AsByte(0xFF, 0x11),
                    otherAddress: fragment[2].AsByte(0xFF, 0x11)
                    );
            }

            public override byte[] MakeFragment(MemoryAddMem gene)
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

        public string Name { get; } = "ADD";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public MemoryAddMem(byte address, byte otherAddress)
        {
            Address = address;
            OtherAddress = otherAddress;
        }

        private dynamic _lastResult = null;

        public int Update(Organism self, float deltaTime)
        {
            var value = self.Remember(Address);
            var other = self.Remember(OtherAddress);

            // if (IsCached(value, other))
            // {
            //     var result = _lastResult.result;
            //     self.Remember(Address, result);
            //     this.Log($"{value} - {other} = {result}");
            //     return 0;
            // }

            if (value == null && other != null)
            {
                self.Remember(Address, other);
                // Cache(value, other, other);
                this.Log($"{value} + {other} = {other}");
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
                var newValue = Utils.Add(value, other);
                // Cache(value, other, newValue);
                self.Remember(Address, newValue);
                this.Log($"{value} + {other} = {newValue}");
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

        private bool IsCached(object value, object other)
        {
            return _lastResult != null && _lastResult.value == value && _lastResult.other == other;
        }

        private void Cache(object value, object other, object result)
        {
            _lastResult = new
            {
                value = value,
                other = other,
                result = result
            };
        }

        private string _string;
        public override string ToString()
        {
            if (_string == null)
                _string = $"{Name} [0x{Address:X2}] += [0x{OtherAddress:X2}]";

            return _string;
        }
    }
}
