using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes
{
    public class TargetOrganisms: ICanUpdate
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0xA4, 0xA4, 5)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new TargetOrganisms(
                    targetingRange: fragment[1].AsFloat(10f, 1000f),
                    trackingCapacity: fragment[2].AsByte(0x10, 0x01),
                    targetMemoryLocation: fragment[3].AsByte(0x10),
                    noTargetsGoto: fragment[4].AsByte(0x10));
            }
        }

        private readonly float _targetingRange;
        private readonly byte _targetMemoryLocation;
        private readonly byte _trackingCapacity;
        private readonly byte _noTargetsGoto;
        public float Cost => _targetingRange / 200f;
        public string Name { get; } = "TARGET ORGANISMS";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public TargetOrganisms(float targetingRange, byte trackingCapacity, byte targetMemoryLocation, byte noTargetsGoto)
        {
            _targetingRange = targetingRange;
            _trackingCapacity = trackingCapacity;
            _targetMemoryLocation = targetMemoryLocation;
            _noTargetsGoto = noTargetsGoto;
        }

        public int Update(Organism self, float deltaTime)
        {
            var organismsInRange = ObjectManager.Instance.GetObjectsWithinRange<Organism>(self, _targetingRange).Where(t => t.Color != self.Color && t.Radius < self.Radius)
                .OrderBy(self.Distance).ToList();

            this.Log($"in range: {organismsInRange.Count}");

            if (organismsInRange.Count < 1)
            {
                this.Log($"no targets");
                return _noTargetsGoto;
            }

            var memoryLocation = _targetMemoryLocation;

            for (int i = 0; i < _trackingCapacity; i++)
            {
                if (i >= organismsInRange.Count)
                    break;
                this.Log($"remembering [{organismsInRange[i].Position.ToShortString()}] at [{memoryLocation:X2}x0]");
                self.Remember(memoryLocation++, organismsInRange[i]);
            }
            return 0;
        }

        private string _string = null;
        public override string ToString()
        {
            if (_string == null)
                _string = $"TARGET Organisms [R: {_targetingRange:0.} M:{_targetMemoryLocation:X2}x0 C:{_trackingCapacity}]";

            return _string;
        }
    }
}
