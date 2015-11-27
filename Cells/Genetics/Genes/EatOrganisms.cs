using System;
using Cells.GameObjects;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes
{
    public class EatOrganisms : IAmAGene, IHandleCollisions
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0x20, 0x21, 1)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                return new EatOrganisms();
            }
        }

        public Type CollidesWith { get { return typeof (Organism); } }

        public void HandleCollision(Organism self, GameObject other, float deltaTime)
        {
            if (!(other is Organism))
                return;

            var prey = other as Organism;

            var distance = (self.Position - other.Position).Length();

            if (distance > self.Radius)
            {
                self.Remember(0x10, prey);
                return;
            }

            if (prey.Radius > self.Radius)
                return;

            self.GiveEnergy(prey.TakeEnergy(self.Energy * deltaTime));
        }
    }
}
