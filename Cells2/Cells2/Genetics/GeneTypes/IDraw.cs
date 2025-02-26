using System.Collections.Generic;
using Cells.GameObjects;
using Microsoft.Xna.Framework;

namespace Cells.Genetics.GeneTypes
{
    public interface IDraw : IAmAGene
    {
        void Apply(int size, List<Color> colorData);
    }
}
