using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;
using Microsoft.Xna.Framework;

namespace Cells.Genetics.Genes
{
    public class SmoothColorChange : IAmAGene, ICanUpdate
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0x40, 0x45, 6)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new SmoothColorChange(
                    fragment[1].AsFloat(0f, 1f),
                    fragment[2].AsFloat(0f, 1f),
                    fragment[3].AsFloat(0f, 1f),
                    fragment[4].AsFloat(0.5f, 1f),
                    fragment[5].AsFloat(0.01f, 10f)
                    );
            }
        }

        private readonly Vector4 _targetColor;
        private readonly float _changeTime;

        private Vector4 _startColor;
        private float _timeUsed = -1f;

        public SmoothColorChange(float red, float green, float blue, float alpha, float changeTime)
        {
            _targetColor = new Vector4(red, green, blue, alpha);
            _changeTime = changeTime;
        }

        public int Update(Organism self, float deltaTime)
        {
            var currentColor = self.Color.ToVector4();

            if (currentColor == _targetColor)
            {
                if (_timeUsed > 0f)
                    _timeUsed = -1f;

                return 0;
            }

            if (_timeUsed < 0f)
                Init(currentColor);

            _timeUsed += deltaTime;

            if (_timeUsed > _changeTime)
                _timeUsed = _changeTime;

            var lerpFactor = _timeUsed/_changeTime;

            var newColor = Vector4.Lerp(_startColor, _targetColor, lerpFactor);

            self.Color = new Color(newColor);

            return 0;
        }

        private void Init(Vector4 currentColor)
        {
            _startColor = currentColor;
            _timeUsed = 0f;
        }
    }
}
