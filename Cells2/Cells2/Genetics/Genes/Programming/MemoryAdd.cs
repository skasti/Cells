using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes.Programming
{
    public class MemoryAdd : ICanUpdate
    {
        public class Maker : GeneMaker<MemoryAdd>
        {
            public Maker(byte markerFrom, byte markerTo)
                : base(markerFrom, markerTo, 3)
            {
            }

            public override MemoryAdd Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new MemoryAdd(
                    address: fragment[1].AsByte(0xFF, 0x11),
                    value: fragment[2]
                    );
            }

            public override byte[] MakeFragment(MemoryAdd gene)
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

        public string Name { get; } = "ADD";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public MemoryAdd(byte address, byte value)
        {
            Address = address;
            Value = value;
        }

        private dynamic _lastResult = null;

        public int Update(Organism self, float deltaTime)
        {
            var value = self.Remember(Address);

            // if (IsCached(currentValue))
            // {
            //     var result = _lastResult.result;
            //     self.Remember(Address, result);
            //     this.Log($"{currentValue} - {_value} = {result}");
            //     return 0;
            // }
            // else 
            if (value == null)
            {
                // Cache(currentValue, _value);
                self.Remember(Address, Value);
                this.Log($"{value} + {Value} = {Value}");
                return 0;
            }
            else if (!Utils.IsNumeric(value))
            {
                this.Log($"unsupported type at [0x{Address:X2}]: {value.GetType().Name}");
                return 0;
            }

            try
            {
                var newValue = Utils.Add(value, Value);
                // Cache(currentValue, newValue);
                self.Remember(Address, newValue);
                this.Log($"{value} + {Value} = {newValue}");
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

        private bool IsCached(object value)
        {
            return _lastResult != null && _lastResult.value == value;
        }

        private void Cache(object value, object result)
        {
            _lastResult = new
            {
                value = value,
                result = result
            };
        }

        private string _string;
        public override string ToString()
        {
            if (_string == null)
                _string = $"{Name} [0x{Address:X2}] += {Value}";

            return _string;
        }
    }
}
