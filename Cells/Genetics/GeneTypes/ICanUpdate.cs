namespace Cells.Genetics.GeneTypes
{
    public interface ICanUpdate
    {
        int Update(Organism self, float deltaTime);
    }
}
