using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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

        public int HandleCollision(Organism self, GameObject other, float deltaTime)
        {
            if (!(other is Organism))
                return 0;

            var prey = other as Organism;

            if (prey.Radius > self.Radius)
                return 0;

            self.GiveEnergy(prey.TakeEnergy(self.Energy * deltaTime));

            return 0;
        }
    }
}
