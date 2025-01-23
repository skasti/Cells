using System.Collections.Generic;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;
using Microsoft.Xna.Framework;

namespace Cells.Genetics.Genes
{
    public class SmoothColorChange : ICanUpdate
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
        public float Cost { get; private set; } = 1f;
        public string Name { get; } = "SMOOTH COLOR CHANGE";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public SmoothColorChange(float red, float green, float blue, float alpha, float changeTime)
        {
            _targetColor = new Vector4(red, green, blue, alpha);
            _changeTime = changeTime;
        }

        public int Update(Organism self, float deltaTime)
        {
            Cost = 0f;
            var currentColor = self.Color.ToVector4();

            if (currentColor == _targetColor)
            {
                if (_timeUsed > 0f)
                    _timeUsed = -1f;

                return 0;
            }

            Cost = 1f;

            if (_timeUsed < 0f)
                Init(currentColor);

            _timeUsed += deltaTime;

            if (_timeUsed > _changeTime)
                _timeUsed = _changeTime;

            var lerpFactor = _timeUsed/_changeTime;

            var newColor = Vector4.Lerp(_startColor, _targetColor, lerpFactor);

            self.Color = new Color(newColor);

            this.Log($"SMOOTH COLOR CHANGE {_startColor} -> {_targetColor} ({_timeUsed}/{_changeTime}) = {newColor}");

            return 0;
        }

        private void Init(Vector4 currentColor)
        {
            _startColor = currentColor;
            _timeUsed = 0f;
        }

        private Dictionary<int,string> _string = new Dictionary<int, string>();
        public string ToString(int level = 0)
        {
            if (!_string.ContainsKey(level))
                _string.Add(level, $"{this.Indent(level)}SmoothColorChange[{_startColor} -> {_targetColor}]");

            return _string[level];
        }
    }
}
