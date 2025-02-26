using System.Collections.Generic;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;
using Microsoft.Xna.Framework;

namespace Cells.Genetics.Genes
{
    public class SmoothColorChange : ICanUpdate
    {
        public class Maker : GeneMaker<SmoothColorChange>
        {
            public Maker(byte markerFrom, byte markerTo)
                : base(markerFrom, markerTo, 6)
            {
            }

            public override SmoothColorChange Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new SmoothColorChange(
                    red: fragment[1].AsFloat(0f, 1f),
                    green: fragment[2].AsFloat(0f, 1f),
                    blue: fragment[3].AsFloat(0f, 1f),
                    alpha: fragment[4].AsFloat(0.5f, 1f),
                    changeTime: fragment[5].AsFloat(0.01f, 10f)
                    );
            }

            public override byte[] MakeFragment(SmoothColorChange gene)
            {
                var fragment = base.MakeFragment(gene);
                fragment[1] = gene.TargetColor.X.AsGeneByte(0f, 1f);
                fragment[2] = gene.TargetColor.Y.AsGeneByte(0f, 1f);
                fragment[3] = gene.TargetColor.Z.AsGeneByte(0f, 1f);
                fragment[4] = gene.TargetColor.W.AsGeneByte(0f, 1f);
                fragment[5] = gene.ChangeTime.AsGeneByte(0.01f, 10f);
                return fragment;
            }
        }

        public readonly Vector4 TargetColor;
        public readonly float ChangeTime;

        private Vector4 _startColor;
        private float _timeUsed = -1f;
        public float Cost { get; private set; } = 1f;
        public string Name { get; } = "SMOOTH COLOR CHANGE";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public SmoothColorChange(float red, float green, float blue, float alpha, float changeTime)
        {
            TargetColor = new Vector4(red, green, blue, alpha);
            ChangeTime = changeTime;
        }

        public int Update(Organism self, float deltaTime)
        {
            Cost = 0f;
            var currentColor = self.Color.ToVector4();

            if (currentColor == TargetColor)
            {
                if (_timeUsed > 0f)
                    _timeUsed = -1f;

                return 0;
            }

            this.Log($"timing: {_timeUsed:0.0}/{ChangeTime:0.0}");
            Cost = 0.5f;

            if (_timeUsed < 0f)
                Init(currentColor);

            _timeUsed += deltaTime;

            if (_timeUsed > ChangeTime)
                _timeUsed = ChangeTime;

            var lerpFactor = _timeUsed/ChangeTime;

            var newColor = Vector4.Lerp(_startColor, TargetColor, lerpFactor);

            self.Color = new Color(newColor);

            this.Log($"result: {newColor.ToShortString(2)}");

            return 0;
        }

        private void Init(Vector4 currentColor)
        {
            _startColor = currentColor;
            _timeUsed = 0f;
        }

        private string _string = null;
        public override string ToString()
        {
            if (_string == null)
                _string = $"SMOOTH Color Change[{_startColor.ToShortString(2)} -> {TargetColor.ToShortString(2)}]";

            return _string;
        }
    }
}
