using System;
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
                : base(0x80, 0x8F, 6)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new ParthenoGenesis(
                    energyThreshold: fragment[1].AsFloat(500f, 100000f),
                    childSize: fragment[2].AsFloat(0.3f, 0.5f),
                    skipOnBirth: fragment[3].AsByte(0x10),
                    defaultSkip: fragment[4].AsByte(0x10),
                    spawnFrequency: fragment[5].AsFloat(0.5f, 20f));
            }
        }

        public float EnergyThreshold { get; private set; }
        private readonly float _childSize;
        private readonly byte _skipOnBirth;
        private readonly byte _defaultSkip;
        private readonly float _spawnFrequency;
        public float Cost { get; private set; } = 1f;
        public string Name { get; } = "PARTHENOGENESIS";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public ParthenoGenesis(float energyThreshold, float childSize, byte skipOnBirth, byte defaultSkip, float spawnFrequency)
        {
            EnergyThreshold = energyThreshold;
            _childSize = childSize;
            _skipOnBirth = skipOnBirth;
            _defaultSkip = defaultSkip;
            _spawnFrequency = spawnFrequency;
        }

        public int Update(Organism self, float deltaTime)
        {
            if (self.SpawnTime < _spawnFrequency)
            {
                Cost = 0.1f;
                return _defaultSkip;
            }

            Cost = 1f;
            if (self.Energy >= EnergyThreshold)
            {
                this.Log($"enough energy ({self.Energy:0.} >= {EnergyThreshold:0.})");
                var spawnDistance = self.Radius * 2;
                var spawnDirection = new Vector2(Game1.Random.NextSingle() * 2f - 1f, Game1.Random.NextSingle() * 2f - 1f);
                spawnDirection.Normalize();
                /*self.Position - (Game1.WorldBounds * 0.5f);
                spawnDirection.Normalize();
                spawnDirection = -spawnDirection;*/

                var energy = self.Energy * _childSize;
                var child = new Organism(new DNA(self.DNA), energy, self.Position + spawnDirection * spawnDistance);
                if (ObjectManager.Instance.Add(child))
                {
                    self.TakeEnergy(energy);
                    self.BreedCount++;
                    this.Log($"birth [{child.Position.ToShortString()} - {child.Energy:0.}]");
                    Cost = Math.Max(13f - _spawnFrequency, 1f);
                    return _skipOnBirth;
                }
                else
                {
                    self.TakeEnergy(energy * 0.5f);
                    this.Log($"failed");
                    Cost = Math.Max(12f - _spawnFrequency, 1f);
                    return _defaultSkip;
                }
            }
            this.Log($"not enough energy ({self.Energy:0.} < {EnergyThreshold:0.})");
            return _defaultSkip;
        }

        private string _string;
        public override string ToString()
        {
            if (_string == null)
                _string = $"{Name} (T: {EnergyThreshold:0.} S: {_childSize:0.00} F: {_spawnFrequency:0.0})";

            return _string;
        }
    }
}
