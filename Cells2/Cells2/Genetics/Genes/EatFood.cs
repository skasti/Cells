using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes
{
    public class EatFood : CollisionHandler
    {
        public class Maker : GeneMaker<EatFood>
        {
            public Maker(byte markerFrom, byte markerTo)
                : base(markerFrom, markerTo, 5)
            {
            }

            public override EatFood Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new EatFood(
                    blockLength: fragment[1].AsByte(0x10),
                    targetAddress: fragment[2].AsByte(0x10),
                    tooFarGoto: fragment[3].AsByte(0x10),
                    deadGoto: fragment[3].AsByte(0x10));
            }

            public override byte[] MakeFragment(EatFood gene)
            {
                var fragment = base.MakeFragment(gene);
                fragment[1] = gene.BlockLength.AsGeneByte();
                fragment[2] = gene.TargetAddress;
                fragment[3] = gene.TooFarGoto;
                fragment[4] = gene.DeadGoto;
                return fragment;
            }
        }

        public readonly byte TargetAddress;
        public readonly byte TooFarGoto;
        public readonly byte DeadGoto;
        public override string Name { get; } = "EAT FOOD";

        public EatFood(byte blockLength, byte targetAddress, byte tooFarGoto, byte deadGoto)
            : base(blockLength, typeof(Food))
        {
            AllowMultiple = false;
            TargetAddress = targetAddress;
            TooFarGoto = tooFarGoto;
            DeadGoto = deadGoto;
        }

        public override void HandleCollision(Organism self, GameObject other, float deltaTime)
        {
            Cost = 0f;
            this.Log($"EAT FOOD {other.Position.ToShortString()}", 1);
            StartIndex = 0;
            var food = other as Food;

            if (!other.Alive)
            {
                this.Log($"is dead, forget target [0x{TargetAddress:X2}]");
                StartIndex = DeadGoto;
                self.Forget(TargetAddress);
                base.HandleCollision(self, other, deltaTime);
                this.Log("done", -1);
                return;
            }

            Cost += 1f;

            var distance = (self.Position - other.Position).Length();

            if (distance < self.Radius + other.Bounds.Width * 0.5f)
            {
                var energyToTake = Math.Max(self.Energy, 500) * deltaTime;
                energyToTake = Math.Min(energyToTake, self.MaxEnergy - self.Energy);

                if (energyToTake > 1f)
                {
                    self.Status = "Eating";
                    var taken = food.TakeEnergy(energyToTake);
                    self.GiveEnergy(taken);
                    this.Log($"eating ({taken})");
                }
                else
                    this.Log($"full");
            }
            else
            {
                this.Log($"too far, remember target [0x{TargetAddress:X2}]");
                self.Remember(TargetAddress, food);
                self.Status = "Not Eating - Too Far";
                StartIndex = TooFarGoto;
            }

            base.HandleCollision(self, other, deltaTime);
            this.Log("done", -1);
        }
    }
}
