using Cells.GameObjects;

namespace Cells.Genetics.GeneTypes
{
    public interface ICanUpdate: IAmAGene
    {
        int Update(Organism self, float deltaTime);
    }
}
