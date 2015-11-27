using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes
{
    public class EatOrganisms : CollisionHandler
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0x20, 0x21, 7)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                return new EatOrganisms(
                    fragment[1].AsByte(0x10),
                    fragment[2].AsByte(0x10),
                    fragment[3].AsByte(0x10),
                    fragment[4].AsByte(0x10),
                    fragment[5].AsByte(0x20, 0x01),
                    fragment[6].AsFloat(0.01f, 1f));
            }
        }

        private readonly byte _targetMemoryLocation;
        private readonly byte _tooFarGoto;
        private readonly byte _biggerGoto;
        private readonly int _dnaSampleSize;
        private readonly float _relationThreshold;

        public EatOrganisms(byte blockLength, byte targetMemoryLocation, byte tooFarGoto, byte biggerGoto, int dnaSampleSize, float relationThreshold)
            : base(blockLength, typeof(Organism))
        {
            _targetMemoryLocation = targetMemoryLocation;
            _tooFarGoto = tooFarGoto;
            _biggerGoto = biggerGoto;
            _dnaSampleSize = dnaSampleSize;
            _relationThreshold = relationThreshold;
        }

        public override void HandleCollision(Organism self, GameObject other, float deltaTime)
        {
            StartIndex = 0;

            var prey = other as Organism;

            if (prey.Radius < self.Radius)
            {
                var distance = (self.Position - other.Position).Length();

                if (distance > self.Radius)
                {
                    StartIndex = _tooFarGoto;
                    self.Remember(_targetMemoryLocation, prey);
                }
                else
                {
                    var relativism = self.DNA.RelatedPercent(prey.DNA, _dnaSampleSize);

                    if (relativism < _relationThreshold)
                    {
                        if (Game1.Debug == self)
                            Debug.WriteLine("[EatOrganisms][CloseEnough] " + relativism);
                    }
                    else
                    {
                        if (Game1.Debug == self)
                            Debug.WriteLine("[EatOrganisms][IEatYou] " + relativism);
                        self.GiveEnergy(prey.TakeEnergy(self.Energy*deltaTime));
                    }
                }
            }
            else
            {
                StartIndex = _biggerGoto;
                self.Forget(_targetMemoryLocation);
            }

            base.HandleCollision(self, other, deltaTime);
        }
    }
}
