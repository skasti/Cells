using System;
using System.Collections.Generic;
using Cells.GameObjects;

namespace Cells.Genetics.GeneTypes
{
    public interface IHandleCollisions : IAmAGene, IDoStuff
    {
        bool AllowMultiple { get; }
        int BlockLength { get; }
        Type CollidesWith { get; }
        void HandleCollision(Organism self, GameObject other, float deltaTime);
        void LoadBlock(int startIndex, List<IAmAGene> genes);
    }
}
