using Cells.GameObjects;

namespace Cells.Genetics.GeneTypes
{
    public interface ITrait
    {
        void Apply(Organism self);
    }
}
