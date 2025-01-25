using System.Collections.Generic;
using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes
{
    public class ChaseObject : ICanUpdate
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
        public float Cost { get; private set; } = 1f;
        public string Name { get; } = "CHASE OBJECT";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public ChaseObject(byte targetMemoryLocation, float desiredSpeed)
        {
            _targetMemoryLocation = targetMemoryLocation;
            _desiredSpeed = desiredSpeed;
        }

        public int Update(Organism self, float deltaTime)
        {
            this.Log($"CHASE OBJECT [{_targetMemoryLocation:X2}]", 1);
            Cost = 1f;
            var target = self.Remember<GameObject>(_targetMemoryLocation);

            if (target == null)
            {
                this.Log("no target", -1);
                return 0;
            }

            this.Log($"target: [{target.GetType().Name} ({target.Position.ToShortString()})]:");

            if (target.Removed)
            {
                this.Log($"forget [{_targetMemoryLocation:X2}x0]", -1);
                self.Forget(_targetMemoryLocation);
                return 2;
            }

            if ((target.Position - self.Position).Length() < self.Radius * 0.5)
            {
                this.Log("reached target", -1);
                self.Status = $"Chasing {target.GetType().Name} - Reached";
                return 0;
            }

            self.Status = $"Chasing {target.GetType().Name}";

            var direction = target.Position - self.Position;
            direction.Normalize();
            direction *= _desiredSpeed;
            var forceAdd = (direction / deltaTime) * self.Mass;
            self.Force += forceAdd;

            this.Log($"add force: {forceAdd.ToShortString()} ({self.Force.ToShortString()})",-1);
            Cost = 2f;
            return 1;
        }

        private string _string;
        public override string ToString()
        {
            if (_string == null)
                _string = $"{Name} [{_targetMemoryLocation}]";

            return _string;
        }
    }
}
