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
            if (Game1.Debug == self)
                Debug.WriteLine("[EatFood][Collision] " + other.Position);

            StartIndex = 0;
            var food = other as Food;

            if (other.Alive)
            {
                var distance = (self.Position - other.Position).Length();

                if (distance < self.Radius + other.Bounds.Width*0.5f)
                {
                    if (Game1.Debug == self)
                        Debug.WriteLine("[EatFood][Eating]");
                    self.GiveEnergy(food.Energy);
                    other.Die(true);
                }
                else
                {
                    if (Game1.Debug == self)
                        Debug.WriteLine("[EatFood][TooFar]");
                    self.Remember(_targetMemoryLocation, food);

                    StartIndex = _tooFarGoto;
                }
            }
            else
            {
                if (Game1.Debug == self)
                    Debug.WriteLine("[EatFood][WasDead]");

                StartIndex = _deadGoto;
            }

            base.HandleCollision(self, other, deltaTime);
        }
    }
}
