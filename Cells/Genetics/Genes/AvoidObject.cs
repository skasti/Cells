using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes
{
    public class AvoidObject: ICanUpdate
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0x93, 0x94, 3)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new AvoidObject(
                    fragment[1].AsByte(0x20), 
                    fragment[2].AsFloat(1f, 500f));
            }
        }

        private readonly byte _targetMemoryLocation;
        private readonly float _desiredSpeed;

        public AvoidObject(byte targetMemoryLocation, float desiredSpeed)
        {
            _targetMemoryLocation = targetMemoryLocation;
            _desiredSpeed = desiredSpeed;
        }

        public int Update(Organism self, float deltaTime)
        {
            if (Game1.Debug == self)
                Debug.WriteLine("[AvoidObject] " + _targetMemoryLocation.ToString("X2"));

            var target = self.Remember<GameObject>(_targetMemoryLocation);

            if (target == null)
                return 0;

            if (target.Removed)
            {
                if (Game1.Debug == self)
                    Debug.WriteLine("[AvoidObject][Forget]" + _targetMemoryLocation.ToString("X2"));
                self.Forget(_targetMemoryLocation);
                return 0;
            }

            if (Game1.Debug == self)
                Debug.WriteLine("[AvoidObject][Avoiding] " + target.Position);

            var direction = self.Position - target.Position;
            direction.Normalize();
            direction *= _desiredSpeed;

            self.Force = (direction / deltaTime) * self.Mass;

            if (Game1.Debug == self)
                Debug.WriteLine("[AvoidObject][Force] " + self.Force);

            return 1;
        }
    }
}
