﻿using System.Collections.Generic;
using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes.Programming
{
    public class MemorySet: ICanUpdate
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0x04, 3)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new MemorySet(
                    memoryLocation: fragment[1].AsByte(0xFF),
                    value: fragment[2].AsByte(0xFF)
                    );
            }
        }
        private readonly byte _value;
        private readonly byte _memoryLocation;
        public float Cost { get; private set; } = 0.5f;
        public string Name { get; } = "SET";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public MemorySet(byte memoryLocation, byte value)
        {
            _memoryLocation = memoryLocation;
            _value = value;
        }

        public int Update(Organism self, float deltaTime)
        {
            self.Remember(_memoryLocation, _value);
            return 0;
        }

        private string _string;
        public override string ToString()
        {
            if (_string == null)
                _string = $"{Name} [{_memoryLocation:X2}x0] = {_value}";

            return _string;
        }
    }
}
