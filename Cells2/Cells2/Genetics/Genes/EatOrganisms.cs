using System;
using System.Diagnostics;
using System.Dynamic;
using Cells.GameObjects;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes
{
    public class EatOrganisms : CollisionHandler, ITrait
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0x20, 0x21, 8)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                return new EatOrganisms(
                    blockLength: fragment[1].AsByte(0x10),
                    targetMemoryLocation: fragment[2].AsByte(0x10),
                    deadGoto: fragment[3].AsByte(0x10),
                    tooFarGoto: fragment[4].AsByte(0x10),
                    biggerGoto: fragment[5].AsByte(0x10),
                    dnaSampleSize: fragment[6].AsFloat(0.01f, 0.99f),
                    relationThreshold: fragment[7].AsFloat(0.01f, 1f));
            }
        }

        public override string Name { get; } = "EAT ORGANISMS";

        private readonly byte _targetMemoryLocation;
        private readonly byte _deadGoto;
        private readonly byte _tooFarGoto;
        private readonly byte _biggerGoto;
        private readonly float _dnaSampleSize;
        private readonly float _relationThreshold;

        public EatOrganisms(byte blockLength, byte targetMemoryLocation, byte deadGoto, byte tooFarGoto, byte biggerGoto, float dnaSampleSize, float relationThreshold)
            : base(blockLength, typeof(Organism))
        {
            AllowMultiple = false;
            _targetMemoryLocation = targetMemoryLocation;
            _deadGoto = deadGoto;
            _tooFarGoto = tooFarGoto;
            _biggerGoto = biggerGoto;
            _dnaSampleSize = dnaSampleSize;
            _relationThreshold = relationThreshold;
        }

        public override void HandleCollision(Organism self, GameObject other, float deltaTime)
        {
            Cost = 0f;
            this.Log($"EAT ORGANISM {other.Position.ToShortString()}",1);
            StartIndex = 0;

            var prey = other as Organism;

            if (!other.Alive)
            {
                this.Log($"is dead, forget target [{_targetMemoryLocation:X2}x0]");
                StartIndex = _deadGoto;
                self.Forget(_targetMemoryLocation);
                base.HandleCollision(self, other, deltaTime);
                this.Log("done", -1);
                return;
            }

            if (prey.Mass > self.Mass)
            {
                this.Log($"is bigger, forget target [{_targetMemoryLocation:X2}x0]");
                StartIndex = _biggerGoto;
                self.Forget(_targetMemoryLocation);
                base.HandleCollision(self, other, deltaTime);
                this.Log("done", -1);
                return;
            }

            Cost += 1f;

            var distance = (self.Position - other.Position).Length();

            if (distance > self.Radius)
            {
                this.Log($"too far, remember target [{_targetMemoryLocation:X2}x0]");
                StartIndex = _tooFarGoto;
                self.Remember(_targetMemoryLocation, prey);
            }
            else
            {
                Cost += 1f;
                var selfC = (float)Math.Abs(self.Color.GetHashCode());
                var preyC = (float)Math.Abs(prey.Color.GetHashCode());
                var relativism = preyC > selfC ? selfC / preyC : preyC / selfC;  //self.DNA.RelatedPercent(prey.DNA, (int)(_dnaSampleSize * self.DNA.Size));

                if (relativism > _relationThreshold)
                    this.Log($"looks like me, not eating");
                else
                {
                    var taken = prey.TakeEnergy(self.Energy*deltaTime);
                    self.GiveEnergy(taken);
                    this.Log($"eating ({taken})");
                }
            }

            base.HandleCollision(self, other, deltaTime);
            this.Log("done", -1);
        }

        public void Apply(Organism self)
        {
            self.Texture = Game1.Virus;
        }
    }
}
