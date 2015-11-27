using Cells.GameObjects;

namespace Cells.Genetics.GeneTypes
{
    public interface ITrait: IAmAGene
    {
        void Apply(Organism self);
    }
}
