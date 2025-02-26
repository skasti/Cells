using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes
{
    public class AvoidObject : ICanUpdate
    {
        public class Maker : GeneMaker<AvoidObject>
        {
            public Maker(byte markerFrom, byte markerTo)
                : base(markerFrom, markerTo, 3)
            {
            }

            public override AvoidObject Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new AvoidObject(
                    fragment[1].AsByte(0x20),
                    fragment[2].AsFloat(1f, 500f));
            }

            public override byte[] MakeFragment(AvoidObject gene)
            {
                var fragment = base.MakeFragment(gene);
                fragment[1] = gene.TargetAddress.AsByte(0x20);
                fragment[2] = gene.DesiredSpeed.AsGeneByte(1f, 500f);
                return fragment;
            }
        }

        public readonly byte TargetAddress;
        public readonly float DesiredSpeed;
        public float Cost { get; private set; } = 1f;
        public string Name { get; } = "AVOID OBJECT";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public AvoidObject(byte targetAddress, float desiredSpeed)
        {
            TargetAddress = targetAddress;
            DesiredSpeed = desiredSpeed;
        }

        public int Update(Organism self, float deltaTime)
        {
            this.Log($"AVOID OBJECT [0x{TargetAddress:X2}]", 1);
            Cost = 1;
            var target = self.Remember<GameObject>(TargetAddress);

            if (target == null)
            {
                this.Log("no target", -1);
                return 0;
            }

            this.Log($"target: [{target.GetType().Name} ({target.Position.ToShortString()})]:");

            if (target.Removed)
            {
                this.Log($"forget [0x{TargetAddress:X2}]", -1);
                self.Forget(TargetAddress);
                return 0;
            }

            var direction = self.Position - target.Position;
            direction.Normalize();
            direction *= DesiredSpeed;
            var forceAdd = (direction / deltaTime) * self.Mass;
            self.Force += forceAdd;

            self.Status = $"Avoiding [{target.GetType().Name} ({target.Position.ToShortString()})]";
            this.Log($"add force: {forceAdd.ToShortString()} ({self.Force.ToShortString()})", -1);
            Cost = 2;
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
