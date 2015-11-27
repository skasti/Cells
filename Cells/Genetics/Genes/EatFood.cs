using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
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

        public int HandleCollision(Organism self, GameObject other, float deltaTime)
        {
            if (!(other is Food))
                return 0;

            if (other.Dead)
                return 0;

            self.GiveEnergy((other as Food).Energy);
            other.Die(true);

            return 0;
        }
    }
}
