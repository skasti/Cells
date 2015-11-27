using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;
using Microsoft.Xna.Framework;

namespace Cells.Genetics.Genes
{
    public class ChaseGameObject: IAmAGene, ICanUpdate
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

                return new ChaseGameObject(fragment[1].AsByte(0x20), fragment[2].AsFloat(1f, 500f));
            }
        }

        private readonly byte _targetMemoryLocation;
        private readonly float _desiredSpeed;

        public ChaseGameObject(byte targetMemoryLocation, float desiredSpeed)
        {
            _targetMemoryLocation = targetMemoryLocation;
            _desiredSpeed = desiredSpeed;
        }

        public int Update(Organism self, float deltaTime)
        {
            var target = self.Remember<GameObject>(_targetMemoryLocation);

            if (target == null)
                return 0;

            if (target.Removed)
            {
                Debug.WriteLine("[ChaseGameObject][Forget]" + _targetMemoryLocation.ToString("X2"));
                self.Forget(_targetMemoryLocation);
                return 0;
            }

            var direction = target.Position - self.Position;
            direction.Normalize();
            direction *= _desiredSpeed * deltaTime;

            self.Force = (direction / deltaTime) * self.Mass;

            return 1;
        }
    }
}
