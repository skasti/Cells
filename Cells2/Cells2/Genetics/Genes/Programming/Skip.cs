using System.Collections.Generic;
using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes.Programming
{
    public class Skip: ICanUpdate
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0x03, 2)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new Skip(fragment[1].AsByte(0x10));
            }
        }

        private readonly byte _jumpSize;
        public float Cost { get; private set; } = 1f;
        public string Name { get; } = "SKIP";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public Skip(byte jumpSize)
        {
            _jumpSize = jumpSize;
        }

        public int Update(Organism self, float deltaTime)
        {
            this.Log(ToString());
            return _jumpSize;
        }

        private string _string;
        public override string ToString()
        {
            if (_string == null)
                _string = $"{Name} [{_jumpSize}]";

            return _string;
        }
    }
}
