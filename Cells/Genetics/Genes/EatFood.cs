using System;
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
                    fragment[2].AsByte(0x20),
                    fragment[3].AsByte(0x10),
                    fragment[3].AsByte(0x10));
            }
        }

        private readonly byte _targetMemoryLocation;
        private readonly byte _tooFarGoto;
        private readonly byte _deadGoto;

        public EatFood(byte blockLength, byte targetMemoryLocation, byte tooFarGoto, byte deadGoto)
            :base(blockLength, typeof(Food))
        {
            _targetMemoryLocation = targetMemoryLocation;
            _tooFarGoto = tooFarGoto;
            _deadGoto = deadGoto;
        }

        public override void HandleCollision(Organism self, GameObject other, float deltaTime)
        {
            StartIndex = 0;

            if (other.Alive)
            {
                var distance = (self.Position - other.Position).Length();

                if (distance < self.Radius)
                {
                    self.GiveEnergy((other as Food).Energy);
                    other.Die(true);
                }
                else
                {
                    self.Remember(_targetMemoryLocation, other as Food);
                    StartIndex = _tooFarGoto;
                }
            }
            else
            {
                StartIndex = _deadGoto;
            }

            base.HandleCollision(self, other, deltaTime);
        }
    }
}
