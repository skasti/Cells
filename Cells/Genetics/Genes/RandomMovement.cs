using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;
using Microsoft.Xna.Framework;

namespace Cells.Genetics.Genes
{
    public class RandomMovement: IAmAGene, ICanUpdate
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0x70, 0x7F, 2)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new RandomMovement(fragment[1].AsFloat(20f, 500f));
            }
        }

        public float DesiredSpeed { get; private set; }

        public RandomMovement(float desiredSpeed)
        {
            DesiredSpeed = desiredSpeed;
        }

        public int Update(Organism self, float deltaTime)
        {
            var speed = self.Velocity.Length();

            if (speed < DesiredSpeed)
            {
                var direction = new Vector2((float)Game1.Random.NextDouble()*2f - 1f,(float)Game1.Random.NextDouble()*2f - 1f);
                direction.Normalize();
                direction *= DesiredSpeed * deltaTime;

                self.Force = (direction/deltaTime)*self.Mass;
            }

            return 0;
        }
    }
}
