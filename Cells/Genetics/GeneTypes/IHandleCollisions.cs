using System;

namespace Cells.Genetics.GeneTypes
{
    public interface IHandleCollisions
    {
        Type CollidesWith { get; }
        int HandleCollision(Organism self, GameObject other, float deltaTime);
    }
}
