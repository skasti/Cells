using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes
{
    public class Eat : ICanUpdate
    {
        public class Maker : GeneMaker<Eat>
        {
            public Maker(byte markerFrom, byte markerTo)
                : base(markerFrom, markerTo, 2)
            {
            }

            public override Eat Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new Eat(
                    targetAddress: fragment[1].AsByte(0x10)
                );
            }

            public override byte[] MakeFragment(Eat gene)
            {
                var fragment = base.MakeFragment(gene);
                fragment[1] = gene.TargetAddress;
                return fragment;
            }
        }

        public readonly byte TargetAddress;
        public string Name { get; } = "EAT";

        public float Cost { get; private set; }

        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public Eat(byte targetAddress)
        {
            TargetAddress = targetAddress;
        }

        public int Update(Organism self, float deltaTime)
        {
            Cost = 0f;
            var food = self.Remember<Food>(TargetAddress);

            if (food == null)
            {
                this.Log("no target");
                return 2;
            }

            if (!food.Alive)
            {
                this.Log($"target is dead");
                return 2;
            }

            Cost += 1f;

            if (self.MaxEnergy - self.Energy < 1f)
            {
                this.Log($"full");
                return 0;
            }

            var distance = (self.Position - food.Position).Length();

            if (distance < self.Radius + food.Bounds.Width * 0.5f)
            {
                var energyToTake = Math.Max(self.Energy, 500) * deltaTime;
                energyToTake = Math.Min(energyToTake, self.MaxEnergy - self.Energy);

                self.Status = "Eating";
                var taken = food.TakeEnergy(energyToTake);
                self.GiveEnergy(taken);
                this.Log($"eating ({taken})");
                return 1;
            }
            else
            {
                this.Log($"too far, remember target [0x{TargetAddress:X2}]");
                self.Remember(TargetAddress, food);
                self.Status = "Not Eating - Too Far";
                return 2;
            }
        }

        private string _string = null;
        public override string ToString()
        {
            if (_string == null)
                _string = $"{Name} [T: 0x{TargetAddress:X2}]";

            return _string;
        }
    }
}
