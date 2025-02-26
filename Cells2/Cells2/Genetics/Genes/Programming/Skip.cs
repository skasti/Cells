using System.Collections.Generic;
using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes.Programming
{
    public class Skip: ICanUpdate
    {
        public class Maker : GeneMaker<Skip>
        {
            public Maker(byte markerFrom, byte markerTo)
                : base(markerFrom, markerTo, 2)
            {
            }

            public override Skip Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new Skip(fragment[1].AsByte(0x10));
            }

            public override byte[] MakeFragment(Skip gene)
            {
                var fragment = base.MakeFragment(gene);
                fragment[1] = gene.SkipCount;
                return fragment;
            }
        }

        public readonly byte SkipCount;
        public float Cost { get; private set; } = 0.5f;
        public string Name { get; } = "SKIP";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public Skip(byte skipCount)
        {
            SkipCount = skipCount;
        }

        public int Update(Organism self, float deltaTime)
        {
            return SkipCount;
        }

        private string _string;
        public override string ToString()
        {
            if (_string == null)
                _string = $"{Name} [{SkipCount}]";

            return _string;
        }
    }
}
