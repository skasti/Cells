using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cells.GameObjects;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes
{
    public abstract class CollisionHandler: IHandleCollisions
    {
        public bool AllowMultiple { get; protected set; } = true;
        public int BlockLength { get; private set; }
        public Type CollidesWith { get; private set; }
        public float Cost { get; protected set; }
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;
        public abstract string Name { get; }

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

            for (int i = startIndex; i <= startIndex + BlockLength; i++)
            {
                if (i >= genes.Count)
                    break;

                if (genes[i] == this)
                    continue;

                if (genes[i] is ICanUpdate)
                    _updates.Add(genes[i] as ICanUpdate);
            }
        }

        public virtual void HandleCollision(Organism self, GameObject other, float deltaTime)
        {
            for (int i = StartIndex; i < _updates.Count; i++)
            {
                var updater = _updates[i];
                updater.Log.Clear();
                updater.LogIndentLevel = LogIndentLevel;
                var skip = updater.Update(self, deltaTime);
                Log.AddRange(updater.Log);

                for (var j = i + 1; j < i + skip && j < _updates.Count; j++)
                    this.Log($"- {_updates[j].ToString()}");

                i += skip;
                Cost += updater.Cost;
            }
        }
    }
}
