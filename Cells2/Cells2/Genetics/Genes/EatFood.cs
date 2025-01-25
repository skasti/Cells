using System.Collections.Generic;
using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes
{
    public class EatFood : CollisionHandler
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0x10, 0x19, 5)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new EatFood(
                    fragment[1].AsByte(0x10),
                    fragment[2].AsByte(0x10),
                    fragment[3].AsByte(0x10),
                    fragment[3].AsByte(0x10));
            }
        }

        private readonly byte _targetMemoryLocation;
        private readonly byte _tooFarGoto;
        private readonly byte _deadGoto;
        public override string Name { get; } = "EAT FOOD";

        public EatFood(byte blockLength, byte targetMemoryLocation, byte tooFarGoto, byte deadGoto)
            :base(blockLength, typeof(Food))
        {
            AllowMultiple = false;
            _targetMemoryLocation = targetMemoryLocation;
            _tooFarGoto = tooFarGoto;
            _deadGoto = deadGoto;
        }

        public override void HandleCollision(Organism self, GameObject other, float deltaTime)
        {
            Cost = 0f;
            this.Log($"EAT FOOD {other.Position.ToShortString()}",1);
            StartIndex = 0;
            var food = other as Food;

            if (!other.Alive)
            {
                this.Log($"is dead, forget target [{_targetMemoryLocation:X2}x0]");
                StartIndex = _deadGoto;
                self.Forget(_targetMemoryLocation);
                base.HandleCollision(self, other, deltaTime);
                this.Log("done", -1);
                return;
            }

            Cost += 1f;

            var distance = (self.Position - other.Position).Length();

            if (distance < self.Radius + other.Bounds.Width*0.5f)
            {
                self.Status = "Eating";
                var taken = food.TakeEnergy(self.Energy*deltaTime);
                self.GiveEnergy(taken);
                this.Log($"eating ({taken})");
            }
            else
            {
                this.Log($"too far, remember target [{_targetMemoryLocation:X2}x0]");
                self.Remember(_targetMemoryLocation, food);
                self.Status = "Not Eating - Too Far";
                StartIndex = _tooFarGoto;
            }

            base.HandleCollision(self, other, deltaTime);
            this.Log("done", -1);
        }
    }
}
