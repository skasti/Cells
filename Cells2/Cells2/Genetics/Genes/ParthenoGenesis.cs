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
                : base(0x80, 0x8F, 5)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new ParthenoGenesis(
                    fragment[1].AsFloat(500f, 5000f), 
                    fragment[2].AsFloat(0.1f, 0.5f),
                    fragment[3].AsByte(0x10),
                    fragment[4].AsByte(0x10));
            }
        }

        public float EnergyThreshold { get; private set; }
        private readonly float _childSize;
        private readonly byte _skipOnBirth;
        private readonly byte _defaultSkip;

        public ParthenoGenesis(float energyThreshold, float childSize, byte skipOnBirth, byte defaultSkip)
        {
            EnergyThreshold = energyThreshold;
            _childSize = childSize;
            _skipOnBirth = skipOnBirth;
            _defaultSkip = defaultSkip;
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

                if (Game1.Debug == self)
                    Debug.WriteLine("[ParthenoGenesis][Birth]" + child.Position);

                return _skipOnBirth;
            }

            return _defaultSkip;
        }
    }
}
