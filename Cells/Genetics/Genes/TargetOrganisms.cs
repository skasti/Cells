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

        public TargetOrganisms(float targetingRange, byte trackingCapacity, byte targetMemoryLocation, byte noTargetsGoto)
        {
            _targetingRange = targetingRange;
            _trackingCapacity = trackingCapacity;
            _targetMemoryLocation = targetMemoryLocation;
            _noTargetsGoto = noTargetsGoto;
        }

        public int Update(Organism self, float deltaTime)
        {
            if (Game1.Debug == self)
                Debug.WriteLine("[TargetOrganisms] " + _targetingRange);

            var organismsInRange = ObjectManager.Instance.GetObjectsWithinRange<Organism>(self, _targetingRange)
                .OrderBy(self.Distance).ToList();

            if (Game1.Debug == self)
                Debug.WriteLine("[TargetOrganisms] " + organismsInRange.Count);

            if (organismsInRange.Count < 1)
                return _noTargetsGoto;

            var memoryLocation = _targetMemoryLocation;

            for (int i = 0; i < _trackingCapacity; i++)
            {
                if (i >= organismsInRange.Count)
                    break;

                self.Remember(memoryLocation++, organismsInRange[i]);
            }

            return 0;
        }
    }
}
