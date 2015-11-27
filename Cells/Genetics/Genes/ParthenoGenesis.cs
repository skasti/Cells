using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;
using Microsoft.Xna.Framework;

namespace Cells.Genetics.Genes
{
    public class ParthenoGenesis: ICanUpdate
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0x80, 0x8F, 3)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new ParthenoGenesis(fragment[1].AsFloat(500f, 5000f), fragment[2].AsFloat(0.1f, 0.5f));
            }
        }

        public float EnergyThreshold { get; private set; }
        private readonly float _childSize;

        public ParthenoGenesis(float energyThreshold, float childSize)
        {
            EnergyThreshold = energyThreshold;
            _childSize = childSize;
        }

        public int Update(Organism self, float deltaTime)
        {
            if (self.Energy > EnergyThreshold)
            {
                var spawnDistance = self.Radius*2;
                var spawnDirection = self.Position - new Vector2(Game1.Width*0.5f, Game1.Height*0.5f);
                spawnDirection.Normalize();
                spawnDirection = -spawnDirection;

                var energy = self.TakeEnergy(self.Energy * _childSize);
                var child = new Organism(new DNA(self.DNA), energy, self.Position + spawnDirection * spawnDistance);
                ObjectManager.Instance.Add(child);

                Debug.WriteLine("[ParthenoGenesis][Birth]" + child.Position);

                return 0;
            }

            return 1;
        }
    }
}
