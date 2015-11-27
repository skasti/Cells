using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes
{
    public class ChaseObject: ICanUpdate
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0x90, 0x92, 3)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new ChaseObject(fragment[1].AsByte(0x20), fragment[2].AsFloat(1f, 250f));
            }
        }

        private readonly byte _targetMemoryLocation;
        private readonly float _desiredSpeed;

        public ChaseObject(byte targetMemoryLocation, float desiredSpeed)
        {
            _targetMemoryLocation = targetMemoryLocation;
            _desiredSpeed = desiredSpeed;
        }

        public int Update(Organism self, float deltaTime)
        {
            if (Game1.Debug == self)
                Debug.WriteLine("[ChaseObject] " + _targetMemoryLocation.ToString("X2"));

            var target = self.Remember<GameObject>(_targetMemoryLocation);

            if (target == null)
                return 0;

            if (target.Removed)
            {
                if (Game1.Debug == self)
                    Debug.WriteLine("[ChaseGameObject][Forget]" + _targetMemoryLocation.ToString("X2"));
                self.Forget(_targetMemoryLocation);
                return 0;
            }
            
            if (Game1.Debug == self)
                Debug.WriteLine("[ChaseObject][Chasing] " + target.Position);

            var direction = target.Position - self.Position;
            direction.Normalize();
            direction *= _desiredSpeed;

            self.Force = (direction / deltaTime) * self.Mass;

            if (Game1.Debug == self)
                Debug.WriteLine("[ChaseObject][Force] " + self.Force);

            return 1;
        }
    }
}
