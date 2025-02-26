using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cells.GameObjects;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes
{
    public abstract class CollisionHandler : IHandleCollisions
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
            var sw = new Stopwatch();
            this.Log($"Handler ({_updates.Count}) {{", 1);
            for (int i = StartIndex; i < _updates.Count; i++)
            {
                var updater = _updates[i];
                updater.Log.Clear();
                sw.Restart();
                var skip = updater.Update(self, deltaTime);
                sw.Stop();
                var dt = sw.Elapsed * (1f/deltaTime);

                if (updater.Log.Count > 0)
                {
                    this.Log($"{updater.ToString()} {{", 1);
                    updater.Log.ForEach((l) => this.Log(l));
                    LogIndentLevel -= 1;
                    this.Log($"}} [C: {updater.Cost} S: {skip} dT: {dt.Microseconds}]");
                }
                else
                    this.Log($"{updater.ToString()} [C: {updater.Cost} S: {skip} dT: {dt.Microseconds}]");

                for (var j = i + 1; j < i + skip && j < _updates.Count; j++)
                {
                    this.Log($"- {_updates[j].ToString()}");
                    Cost += 0.2f;
                }

                i += skip;
                Cost += Math.Max(updater.Cost, 0.2f);
            }
            LogIndentLevel -= 1;
            this.Log($"}}");
        }

        public virtual void Update(float deltaTime)
        {
            
        }
    }
}
