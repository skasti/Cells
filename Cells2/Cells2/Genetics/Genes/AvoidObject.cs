using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
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
        public float Cost { get; private set; } = 1f;
        public string Name { get; } = "AVOID OBJECT";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public AvoidObject(byte targetMemoryLocation, float desiredSpeed)
        {
            _targetMemoryLocation = targetMemoryLocation;
            _desiredSpeed = desiredSpeed;
        }

        public int Update(Organism self, float deltaTime)
        {
            this.Log($"AVOID OBJECT [{_targetMemoryLocation:X2}]",1);
            Cost = 1;
            var target = self.Remember<GameObject>(_targetMemoryLocation);

            if (target == null)
            {
                this.Log("no target",-1);
                return 2;
            }

            this.Log($"target: [{target.GetType().Name} ({target.Position.ToShortString()})]:");

            if (target.Removed)
            {
                this.Log($"forget [{_targetMemoryLocation:X2}x0]",-1);
                self.Forget(_targetMemoryLocation);
                return 0;
            }

            var direction = self.Position - target.Position;
            direction.Normalize();
            direction *= _desiredSpeed;
            var forceAdd = (direction / deltaTime) * self.Mass;
            self.Force += forceAdd;

            self.Status =  $"Avoiding [{target.GetType().Name} ({target.Position.ToShortString()})]";
            this.Log($"add force: {forceAdd.ToShortString()} ({self.Force.ToShortString()})",-1);
            Cost = 2;
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
