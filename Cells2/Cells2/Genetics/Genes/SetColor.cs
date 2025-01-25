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
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0x30, 0x38, 5)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new SetColor(
                    fragment[1].AsFloat(0f, 1f),
                    fragment[2].AsFloat(0f, 1f),
                    fragment[3].AsFloat(0f, 1f),
                    fragment[4].AsFloat(0.5f, 1f)
                    );
            }
        }

        public string Name { get; } = "COLOR";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        private readonly Color _color;
        public float Cost { get; private set; } = 1f;

        public SetColor(float r, float g, float b, float a)
        {
            _color = new Color(r,g,b,a);
        }

        public void Apply(Organism self)
        {
            self.Color = _color;
        }

        public int Update(Organism self, float deltaTime)
        {
            this.Log(ToString());
            self.Color = _color;
            return 0;
        }

        private string _string = null;
        public override string ToString()
        {
            if (_string == null)
                _string = $"SET Color[{_color}]";

            return _string;
        }
    }
}
