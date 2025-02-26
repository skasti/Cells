using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;
using Cells2.Events;
using Microsoft.Xna.Framework;

namespace Cells.Genetics.Genes
{
    public class ParthenoGenesis : ICanUpdate
    {
        public class Maker : GeneMaker<ParthenoGenesis>
        {
            public Maker(byte markerFrom, byte markerTo)
                : base(markerFrom, markerTo, 6)
            {
            }

            public override ParthenoGenesis Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new ParthenoGenesis(
                    energyThreshold: fragment[1].AsFloat(0.1f, 0.9f),
                    childSize: fragment[2].AsFloat(0.3f, 0.5f),
                    birthSkip: fragment[3].AsByte(0x10),
                    defaultSkip: fragment[4].AsByte(0x10),
                    SpawnInterval: fragment[5].AsFloat(0.5f, 5f));
            }

            public override byte[] MakeFragment(ParthenoGenesis gene)
            {
                var fragment = base.MakeFragment(gene);
                fragment[1] = gene.ChildSize.AsGeneByte(0.1f, 0.9f);
                fragment[2] = gene.ChildSize.AsGeneByte(0.3f, 0.5f);
                fragment[3] = gene.BirthSkip;
                fragment[4] = gene.DefaultSkip;
                fragment[5] = gene.SpawnInterval.AsGeneByte(0.5f, 20f);
                return fragment;
            }
        }

        public float EnergyThreshold { get; private set; }
        public readonly float ChildSize;
        public readonly byte BirthSkip;
        public readonly byte DefaultSkip;
        public readonly float SpawnInterval;
        public float Cost { get; private set; } = 1f;
        public string Name { get; } = "PARTHENOGENESIS";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public ParthenoGenesis(float energyThreshold, float childSize, byte birthSkip, byte defaultSkip, float SpawnInterval)
        {
            EnergyThreshold = energyThreshold;
            ChildSize = childSize;
            BirthSkip = birthSkip;
            DefaultSkip = defaultSkip;
            this.SpawnInterval = SpawnInterval;
        }

        public int Update(Organism self, float deltaTime)
        {
            if (self.SpawnTime < SpawnInterval)
            {
                Cost = 0.1f;
                return DefaultSkip;
            }

            Cost = 1f;
            if (self.Energy >= self.MaxEnergy * EnergyThreshold)
            {
                this.Log($"enough energy ({self.Energy:0.} >= {self.MaxEnergy * EnergyThreshold:0.})");
                var spawnDistance = self.Radius * 2;
                var spawnDirection = new Vector2(Game1.Random.NextSingle() * 2f - 1f, Game1.Random.NextSingle() * 2f - 1f);
                spawnDirection.Normalize();
                /*self.Position - (Game1.WorldBounds * 0.5f);
                spawnDirection.Normalize();
                spawnDirection = -spawnDirection;*/

                var energy = self.Energy * ChildSize;
                var child = new Organism(new DNA(self.MutationOptions, self.DNA), energy, self.Position + spawnDirection * spawnDistance);
                if (ObjectManager.Instance.Add(child))
                {
                    self.TakeEnergy(energy * 1.5f);
                    self.AddChild(child);
                    this.Log($"birth [{child.Position.ToShortString()} - {child.Energy:0.}]");
                    new Event(new EventType("birth.parthenogenesis"), new Dictionary<string, object> {
                        {"selfEnergy", energy * 1.5f},
                        {"position", child.Position}
                    });
                    Cost = Math.Max(13f - SpawnInterval, 1f);
                    return BirthSkip;
                }
                else
                {
                    self.TakeEnergy(energy * 0.75f);
                    this.Log($"failed");
                    Cost = Math.Max(12f - SpawnInterval, 1f);
                    return DefaultSkip;
                }
            }
            this.Log($"not enough energy ({self.Energy:0.} < {self.MaxEnergy * EnergyThreshold:0.})");
            return DefaultSkip;
        }

        private string _string;
        public override string ToString()
        {
            if (_string == null)
                _string = $"{Name} (T: {EnergyThreshold:0.} S: {ChildSize:0.00} F: {SpawnInterval:0.0})";

            return _string;
        }
    }
}
