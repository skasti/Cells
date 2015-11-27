using Cells.Genetics.GeneTypes;

namespace Cells.Genetics
{
    public interface IMakeAGene
    {
        byte MarkerFrom { get; }
        byte MarkerTo { get; }

        int Size { get; }
        int ArgumentBytes { get; }

        IAmAGene Make(byte[] fragment);
    }
}
