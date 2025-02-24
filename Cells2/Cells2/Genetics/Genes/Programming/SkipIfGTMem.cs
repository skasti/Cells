﻿using System.Collections.Generic;
using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes.Programming
{
    public class SkipIfGTMem: ICanUpdate
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0xF8, 0xF9, 4)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new SkipIfGTMem(
                    memoryLocation: fragment[1].AsByte(0xFF),
                    otherMemoryLocation: fragment[2].AsByte(0xFF),
                    skipSize: fragment[3].AsByte(0x10, 0x01)
                    );
            }
        }
        private readonly byte _otherMemoryLocation;
        private readonly byte _memoryLocation;
        private readonly byte _skipSize;
        public float Cost { get; private set; } = 0.5f;
        public string Name { get; } = "IFGT";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public SkipIfGTMem(byte memoryLocation, byte otherMemoryLocation, byte skipSize)
        {
            _memoryLocation = memoryLocation;
            _otherMemoryLocation = otherMemoryLocation;
            _skipSize = skipSize;
        }

        public int Update(Organism self, float deltaTime)
        {
            var value = self.Remember<byte>(_memoryLocation);
            var otherValue = self.Remember<byte>(_otherMemoryLocation);
            var equals = value > otherValue;
            this.Log($"{value} > {otherValue} ({equals})");
            return @equals ? _skipSize : 0;
        }

        private string _string;
        public override string ToString()
        {
            if (_string == null)
                _string = $"{Name} ([{_memoryLocation:X2}x0] > [{_otherMemoryLocation:X2}x0]) SKIP {_skipSize}";

            return _string;
        }
    }
}
