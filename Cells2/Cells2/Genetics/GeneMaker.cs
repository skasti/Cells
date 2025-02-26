using System;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics
{
    public abstract class GeneMaker<T>: IMakeAGene where T: IAmAGene
    {
        public Type GeneType { get; }
        public byte MarkerFrom { get; private set; }
        public byte MarkerTo { get; private set; }
        public int Size { get; private set; }
        public int ArgumentBytes { get { return Size - 1; } }

        protected GeneMaker(byte markerFrom, byte markerTo, int size)
        {
            GeneType = typeof(T);
            MarkerFrom = markerFrom;
            MarkerTo = markerTo;
            Size = size;
        }

        protected GeneMaker(byte marker, int size)
            :this(marker,marker,size)
        {}

        public abstract IAmAGene Make(byte[] fragment);
        public byte[] ToFragment(IAmAGene gene)
        {
            if (gene is T tGene)
                return MakeFragment(tGene);

            throw new ArgumentException($"Gene type mismatch. {gene.GetType().Name} is not an instance of {typeof(T).Name}");
        }
        public virtual byte[] MakeFragment(T gene)
        {
            var frag = new byte[Size];
            frag[0] = MarkerFrom;
            return frag;
        }
    }
}
