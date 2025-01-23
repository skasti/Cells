using System.Runtime.InteropServices.Marshalling;
using Cells.GameObjects;

namespace Cells.Genetics.GeneTypes
{
    public interface ICanUpdate: IAmAGene, IDoStuff
    {
        int Update(Organism self, float deltaTime);
    }
}
