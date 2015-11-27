using System;
using System.Collections.Generic;
using Cells.GameObjects;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes
{
    public abstract class CollisionHandler: IAmAGene, IHandleCollisions
    {
        public int BlockLength { get; private set; }
        public Type CollidesWith { get; private set; }

        private readonly List<ICanUpdate> _updates = new List<ICanUpdate>();
        protected int StartIndex = 0;

        protected CollisionHandler(int blockLength, Type collidesWith)
        {
            CollidesWith = collidesWith;
            BlockLength = blockLength;
        }

        public void LoadBlock(int startIndex, List<IAmAGene> genes)
        {
            if (BlockLength == 0)
                return;

            for (int i = startIndex; i < startIndex + BlockLength; i++)
            {
                if (i >= genes.Count)
                    break;

                if (genes[i] == this)
                    continue;

                if (genes[i] is ICanUpdate)
                    _updates.Add(genes[i] as ICanUpdate);
            }

            BlockLength = _updates.Count;
        }

        public virtual void HandleCollision(Organism self, GameObject other, float deltaTime)
        {
            for (int i = StartIndex; i < _updates.Count; i++)
            {
                i += _updates[i].Update(self, deltaTime);
            }
        }
    }
}
