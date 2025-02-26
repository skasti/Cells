using System.Collections.Generic;
using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes
{
    public class ChaseObject : ICanUpdate
    {
        public class Maker : GeneMaker<ChaseObject>
        {
            public Maker(byte markerFrom, byte markerTo)
                : base(markerFrom, markerTo, 3)
            {
            }

            public override ChaseObject Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new ChaseObject(
                    targetAddress: fragment[1].AsByte(0x20),
                    desiredSpeed: fragment[2].AsFloat(1f, 250f)
                    );
            }

            public override byte[] MakeFragment(ChaseObject gene)
            {
                var fragment = base.MakeFragment(gene);
                fragment[1] = gene.TargetAddress;
                fragment[2] = gene.DesiredSpeed.AsGeneByte(1f, 250f);
                return fragment;
            }
        }

        public readonly byte TargetAddress;
        public readonly float DesiredSpeed;
        public float Cost { get; private set; } = 1f;
        public string Name { get; } = "CHASE OBJECT";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public ChaseObject(byte targetAddress, float desiredSpeed)
        {
            TargetAddress = targetAddress;
            DesiredSpeed = desiredSpeed;
        }

        public int Update(Organism self, float deltaTime)
        {
            Cost = 1f;
            var target = self.Remember<GameObject>(TargetAddress);

            if (target == null)
            {
                this.Log("no target");
                return 0;
            }

            this.Log($"target: [{target.GetType().Name} ({target.Position.ToShortString()})]:");

            if (target.Removed)
            {
                this.Log($"forget [0x{TargetAddress:X2}]");
                self.Forget(TargetAddress);
                return 0;
            }

            if ((target.Position - self.Position).Length() < self.Radius * 0.5)
            {
                this.Log("reached target");
                self.Status = $"Chasing {target.GetType().Name} - Reached";
                return 1;
            }

            self.Status = $"Chasing {target.GetType().Name}";

            var direction = target.Position - self.Position;
            direction.Normalize();
            direction *= DesiredSpeed;
            var forceAdd = (direction / deltaTime) * self.Mass;
            self.Force += forceAdd;

            this.Log($"add force: {forceAdd.ToShortString()} ({self.Force.ToShortString()})");
            Cost = 2f;
            return 1;
        }

        private string _string;
        public override string ToString()
        {
            if (_string == null)
                _string = $"{Name} [{TargetAddress}]";

            return _string;
        }
    }
}
