using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;
using Microsoft.Xna.Framework;

namespace Cells.Genetics.Genes
{
    public class ParthenoGenesis : ICanUpdate
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0x80, 0x8F, 5)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new ParthenoGenesis(
                    fragment[1].AsFloat(500f, 100000f),
                    fragment[2].AsFloat(0.1f, 0.5f),
                    fragment[3].AsByte(0x10),
                    fragment[4].AsByte(0x10));
            }
        }

        public float EnergyThreshold { get; private set; }
        private readonly float _childSize;
        private readonly byte _skipOnBirth;
        private readonly byte _defaultSkip;
        public float Cost { get; private set; } = 1f;
        public string Name { get; } = "PARTHENOGENESIS";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public ParthenoGenesis(float energyThreshold, float childSize, byte skipOnBirth, byte defaultSkip)
        {
            EnergyThreshold = energyThreshold;
            _childSize = childSize;
            _skipOnBirth = skipOnBirth;
            _defaultSkip = defaultSkip;
        }

        public int Update(Organism self, float deltaTime)
        {
            Cost = 1f;
            if (self.Energy > EnergyThreshold)
            {
                this.Log($"PARTHENOGENESIS - ENOUGH ENERGY ({self.Energy} > {EnergyThreshold})", 1);
                var spawnDistance = self.Radius * 2;
                var spawnDirection = self.Position - (Game1.WorldBounds * 0.5f);
                spawnDirection.Normalize();
                spawnDirection = -spawnDirection;

                var energy = self.TakeEnergy(self.Energy * _childSize);
                var child = new Organism(new DNA(self.DNA), energy, self.Position + spawnDirection * spawnDistance);
                if (ObjectManager.Instance.Add(child))
                {
                    self.BreedCount++;
                    this.Log($"birth [{child.Position} - {child.Energy}], skipping {_skipOnBirth}", -1);
                    Cost = 3f;
                    return _skipOnBirth;
                }
                else
                {
                    self.GiveEnergy(energy * 0.75f);
                    this.Log($"no birth, skipping {_defaultSkip}", -1);
                    Cost = 2f;
                    return _defaultSkip;
                }
            }
            this.Log($"PARTHENOGENESIS - NOT ENOUGH ENERGY ({EnergyThreshold})");
            return _defaultSkip;
        }

        private string _string;
        public override string ToString()
        {
            if (_string == null)
                _string = $"{Name}";

            return _string;
        }
    }
}
