using System;
using Cells.GameObjects;

namespace Cells.Genetics.GeneTypes
{
    public interface IHandleCollisions
    {
        Type CollidesWith { get; }
        void HandleCollision(Organism self, GameObject other, float deltaTime);
    }
}
