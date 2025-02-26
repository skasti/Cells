using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;
using Cells.Genetics.Genes;

namespace Cells.Genetics.Genes
{
    public class TargetOrganisms: TargetBase<Organism>
    {
        public static readonly float MaxTrackingInterval = 2.5f;
        protected override float maxTrackingInterval => MaxTrackingInterval;
        public class Maker : GeneMaker<TargetOrganisms>
        {
            public Maker(byte markerFrom, byte markerTo)
                : base(markerFrom, markerTo, 5)
            {
            }

            public override TargetOrganisms Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new TargetOrganisms(
                    range: fragment[1].AsFloat(10f, 1500f),
                    capacity: fragment[2].AsByte(0x10, 0x01),
                    targetAddress: fragment[3].AsByte(0x10),
                    trackingInterval: fragment[4].AsFloat(0.1f, MaxTrackingInterval));
            }

            public override byte[] MakeFragment(TargetOrganisms gene)
            {
                var fragment = base.MakeFragment(gene);
                fragment[1] = gene.Range.AsGeneByte(10f, 1500f);
                fragment[2] = gene.Capacity;
                fragment[3] = gene.TargetAddress;
                fragment[4] = gene.TrackingInterval.AsGeneByte(0.1f, MaxTrackingInterval);
                return fragment;
            }
        }

        public TargetOrganisms(float range, byte capacity, byte targetAddress, float trackingInterval)
        :base(range, capacity, targetAddress,  trackingInterval)
        {
        }
    }
}
