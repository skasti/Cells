using System;
using Cells.GameObjects;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes
{
    public class EatFood : IAmAGene, IHandleCollisions
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0x10, 0x19, 1)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                return new EatFood();
            }
        }

        public Type CollidesWith { get { return typeof (Food); }}

        public void HandleCollision(Organism self, GameObject other, float deltaTime)
        {
            if (!(other is Food))
                return;

            if (other.Dead)
                return;

            var distance = (self.Position - other.Position).Length();

            if (distance < self.Radius)
            {
                self.GiveEnergy((other as Food).Energy);
                other.Die(true);
            }
            else
            {
                self.Remember(0x10, other as Food);
            }
        }
    }
}
