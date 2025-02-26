using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Text.RegularExpressions;
using System.Threading;
using Cells.GameObjects;
using Cells.Genetics.GeneTypes;
using Cells2.Events;
using Microsoft.Xna.Framework;

namespace Cells.Genetics.Genes
{
    public class Breed : ICanUpdate, ITrait
    {
        public class Maker : GeneMaker<Breed>
        {
            public Maker(byte markerFrom = 0x20, byte markerTo = 0x21)
                : base(markerFrom, markerTo, 6)
            {
            }

            public override Breed Make(byte[] fragment)
            {
                return new Breed(
                    targetAddress: fragment[1].AsByte(0x20),
                    energyThreshold: fragment[2].AsFloat(0.1f, 0.9f),
                    fitnessThreshold: fragment[3].AsFloat(1f, 1000f),
                    childSize: fragment[4].AsFloat(0.1f, 0.5f),
                    spawnFrequency: fragment[5].AsFloat(0.5f, 5f)
                );
            }

            public override byte[] MakeFragment(Breed gene)
            {
                var fragment = base.MakeFragment(gene);
                fragment[1] = gene.TargetAddress.AsGeneByte(0x20);
                fragment[2] = gene.EnergyThreshold.AsGeneByte(0.1f, 0.9f);
                fragment[3] = gene.FitnessThreshold.AsGeneByte(1f, 1000f);
                fragment[4] = gene.ChildSize.AsGeneByte(0.1f, 0.5f);
                fragment[5] = gene.SpawnFrequency.AsGeneByte(0.5f, 5f);
                return fragment;
            }
        }

        public string Name { get; } = "BREED";

        public float Cost { get; private set; }

        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public readonly byte TargetAddress;
        public float EnergyThreshold { get; private set; }
        public float FitnessThreshold { get; private set; }
        public readonly float ChildSize;
        private readonly byte _skipOnBirth = 0;
        private readonly byte _defaultSkip = 1;
        public readonly float SpawnFrequency;

        public Breed(byte targetAddress, float energyThreshold, float fitnessThreshold, float childSize, float spawnFrequency)
        {
            TargetAddress = targetAddress;
            EnergyThreshold = energyThreshold;
            FitnessThreshold = fitnessThreshold;
            ChildSize = childSize;
            SpawnFrequency = spawnFrequency;
        }

        public void Apply(Organism self, List<IAmAGene> genes)
        {
            self.AddTexture(Game1.Virus, 1);
        }

        public int Update(Organism self, float deltaTime)
        {
            if (self.SpawnTime < SpawnFrequency)
            {
                Cost = 0.1f;
                return _defaultSkip;
            }

            Cost = 1f;
            if (self.Energy >= self.MaxEnergy * EnergyThreshold && self.Fitness >= FitnessThreshold)
            {
                var mate = self.Remember<Organism>(TargetAddress);

                if (mate == null)
                {
                    this.Log($"no mate");
                    return _defaultSkip;
                }

                if (mate.Dead)
                {
                    this.Log($"mate is dead");
                    return _defaultSkip;
                }

                if ((mate.Position - self.Position).Length() > (self.Radius + mate.Radius) * 1.5f)
                {
                    this.Log($"mate is too far away");
                    return _defaultSkip;
                }

                var spawnDistance = self.Radius * 2;
                var spawnDirection = new Vector2(Game1.Random.NextSingle() * 2f - 1f, Game1.Random.NextSingle() * 2f - 1f);
                spawnDirection.Normalize();

                var selfEnergy = self.Energy * ChildSize;
                var mateEnergy = mate.Energy * ChildSize;
                var child = new Organism(new DNA(self.MutationOptions, self.DNA, mate.DNA), selfEnergy + mateEnergy, self.Position + spawnDirection * spawnDistance);
                if (ObjectManager.Instance.Add(child))
                {
                    self.TakeEnergy(selfEnergy);
                    mate.TakeEnergy(mateEnergy);
                    self.AddChild(child);
                    mate.AddChild(child);
                    this.Log($"birth [{child.Position.ToShortString()} - {child.Energy:0.}]");
                    new Event(new EventType("birth.breed"), new Dictionary<string, object> {
                        {"selfEnergy", selfEnergy},
                        {"mateEnergy", mateEnergy},
                        {"position", child.Position}
                    });
                    Cost = Math.Max(13f - SpawnFrequency, 1f);
                    return _skipOnBirth;
                }
                else
                {
                    self.TakeEnergy(selfEnergy * 0.5f);
                    this.Log($"failed");
                    Cost = Math.Max(12f - SpawnFrequency, 1f);
                    return _defaultSkip;
                }
            }
            this.Log($"not enough energy ({self.Energy:0.} < {self.MaxEnergy * EnergyThreshold:0.})");
            return _defaultSkip;
        }
        private string _string = null;
        public override string ToString()
        {
            if (_string == null)
                _string = $"{Name} [T: 0x{TargetAddress:X2} eT: {EnergyThreshold:0.0} cS: {ChildSize:0.0} sF: {SpawnFrequency:0.0}]";

            return _string;
        }
    }
}
