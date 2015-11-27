using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics
{
    public abstract class GeneMaker: IMakeAGene
    {
        public byte MarkerFrom { get; private set; }
        public byte MarkerTo { get; private set; }
        public int Size { get; private set; }
        public int ArgumentBytes { get { return Size - 1; } }

        protected GeneMaker(byte markerFrom, byte markerTo, int size)
        {
            MarkerFrom = markerFrom;
            MarkerTo = markerTo;
            Size = size;
        }

        public abstract IAmAGene Make(byte[] fragment);
    }
}
