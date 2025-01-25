using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes
{
    public class TargetFood : ICanUpdate
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0xA0, 0xA2, 5)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new TargetFood(
                    fragment[1].AsFloat(10f, 1000f),
                    fragment[2].AsByte(0x10, 0x01),
                    fragment[3].AsByte(0x10),
                    fragment[4].AsByte(0x05));
            }
        }

        private readonly float _targetingRange;
        private readonly byte _targetMemoryLocation;
        private readonly byte _trackingCapacity;
        private readonly byte _noTargetsGoto;
        public float Cost => _targetingRange / 400f;
        public string Name { get; } = "TARGET FOOD";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public TargetFood(float targetingRange, byte trackingCapacity, byte targetMemoryLocation, byte noTargetsGoto)
        {
            _targetingRange = targetingRange;
            _trackingCapacity = trackingCapacity;
            _targetMemoryLocation = targetMemoryLocation;
            _noTargetsGoto = noTargetsGoto;
        }

        public int Update(Organism self, float deltaTime)
        {
            this.Log(ToString(), 1);
            var foodInRange = ObjectManager.Instance.GetObjectsWithinRange<Food>(self, _targetingRange)
                .OrderBy(self.Distance).ToList();
            this.Log($"in range: {foodInRange.Count}");

            if (foodInRange.Count < 1)
            {
                this.Log($"no targets");
                return _noTargetsGoto;
            }

            var memoryLocation = _targetMemoryLocation;

            for (int i = 0; i < _trackingCapacity; i++)
            {
                if (i >= foodInRange.Count)
                    break;
                this.Log($"remembering [{foodInRange[i].Position.ToShortString()}] at [{memoryLocation:X2}x0]");
                self.Remember(memoryLocation++, foodInRange[i]);
            }

            this.Log($"done");
            return 0;
        }

        private string _string = null;
        public override string ToString()
        {
            if (_string == null)
                _string = $"TARGET Food [R: {_targetingRange:0.} M:{_targetMemoryLocation:X2}x0 C:{_trackingCapacity}]";

            return _string;
        }
    }
}
