using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes
{
    public class TargetFood: ICanUpdate
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
                    fragment[1].AsFloat(10f, 500f),
                    fragment[2].AsByte(0x10, 0x01),
                    fragment[3].AsByte(0x10),
                    fragment[4].AsByte(0x10));
            }
        }

        private readonly float _targetingRange;
        private readonly byte _targetMemoryLocation;
        private readonly byte _trackingCapacity;
        private readonly byte _noTargetsGoto;

        public TargetFood(float targetingRange, byte trackingCapacity, byte targetMemoryLocation, byte noTargetsGoto)
        {
            _targetingRange = targetingRange;
            _trackingCapacity = trackingCapacity;
            _targetMemoryLocation = targetMemoryLocation;
            _noTargetsGoto = noTargetsGoto;
        }

        public int Update(Organism self, float deltaTime)
        {
            var foodInRange = ObjectManager.Instance.GetObjectsWithinRange<Food>(self, _targetingRange).ToList();

            if (foodInRange.Count < 1)
                return _noTargetsGoto;

            var memoryLocation = _targetMemoryLocation;

            for (int i = 0; i < _trackingCapacity; i++)
            {
                if (i >= foodInRange.Count)
                    break;

                self.Remember(memoryLocation++, foodInRange[i]);
            }

            return 0;
        }
    }
}
