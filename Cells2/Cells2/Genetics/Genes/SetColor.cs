using System.Collections.Generic;
using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;
using Microsoft.Xna.Framework;

namespace Cells.Genetics.Genes
{
    public class SetColor : ITrait, ICanUpdate
    {
        public class Maker : GeneMaker<SetColor>
        {
            public Maker(byte markerFrom, byte markerTo)
                : base(markerFrom, markerTo, 5)
            {
            }

            public override SetColor Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new SetColor(
                    r: fragment[1],
                    g: fragment[2],
                    b: fragment[3],
                    a: fragment[4].AsByte(0xFF, 0x80)
                    );
            }

            public override byte[] MakeFragment(SetColor gene)
            {
                var fragment = base.MakeFragment(gene);
                fragment[1] = gene.Color.R;
                fragment[2] = gene.Color.G;
                fragment[3] = gene.Color.B;
                fragment[4] = gene.Color.A;
                return fragment;
            }

        }

        public string Name { get; } = "COLOR";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public readonly Color Color;
        public float Cost { get; private set; } = 1f;

        public SetColor(byte r, byte g, byte b, byte a)
        {
            Color = new Color(r, g, b, a);
        }

        public void Apply(Organism self, List<IAmAGene> genes)
        {
            self.Color = Color;
        }

        public int Update(Organism self, float deltaTime)
        {
            self.Color = Color;
            return 0;
        }

        private string _string = null;
        public override string ToString()
        {
            if (_string == null)
                _string = $"SET Color[{Color}]";

            return _string;
        }
    }
}
