using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;
using Microsoft.Xna.Framework;

namespace Cells.Genetics.Genes
{
    public class Armor : ITrait, ICanUpdate
    {
        public class Maker : GeneMaker<Armor>
        {
            public Maker(byte markerFrom, byte markerTo)
                : base(markerFrom, markerTo, 3)
            {
            }

            public override Armor Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new Armor(
                    regenSpeed: fragment[1].AsFloat(0.1f, 1f),
                    regenInterval: fragment[2].AsFloat(0.5f, 5f)
                    );
            }

            public override byte[] MakeFragment(Armor gene)
            {
                var fragment = base.MakeFragment(gene);
                fragment[1] = gene.RegenSpeed.AsGeneByte(0.1f, 1f);
                fragment[2] = gene.RegenInterval.AsGeneByte(0.5f, 5f);
                return fragment;
            }
        }

        public readonly float RegenSpeed;
        public readonly float RegenInterval;
        private float _timeSinceLastRegen = 0f;
        public float Cost { get; private set; } = 0f;
        public string Name { get; } = "ARMOR";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public Armor(float regenSpeed, float regenInterval)
        {
            RegenSpeed = regenSpeed;
            RegenInterval = regenInterval;
        }

        public void Apply(Organism self, List<IAmAGene> genes)
        {
            self.AddArmor(RegenSpeed * self.MaxArmor);
        }

        public int Update(Organism self, float deltaTime)
        {
            _timeSinceLastRegen += deltaTime;

            if (_timeSinceLastRegen > RegenInterval && self.Armor < self.MaxArmor)
            {
                Cost = self.AddArmor(RegenSpeed*self.MaxArmor*RegenInterval) * 0.1f;
                _timeSinceLastRegen = 0f;
            }
            else
                Cost = 0;

            return 0;
        }

        private string _string = null;
        public override string ToString()
        {
            if (_string == null)
                _string = $"{Name} [R: {RegenSpeed*100f:0.0}% I: {RegenInterval:0.00}s]";

            return _string;
        }
    }
}
