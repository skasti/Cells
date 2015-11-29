using System;
using System.Collections.Generic;
using System.Linq;
using Cells.Genetics;
using Cells.Genetics.Genes;
using Cells.Genetics.GeneTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cells.GameObjects
{
    public class Organism: GameObject
    {
        public Dictionary<byte, object> Memory = new Dictionary<byte, object>();
        public DNA DNA { get; private set; }

        public float DistanceMoved { get; private set; }

        public float MaxEnergy { get; set; }
        public float Energy { get; private set; }
        public float EnergyGiven { get; private set; }
        public float BaseMetabolicRate { get; set; }
        public float MovementMetabolicRate { get; set; }

        public float MaxAge { get; set; }

        public Color Color { get; set; }
        public float Radius
        {
            get
            {
                return (float)Math.Sqrt(Energy / Math.PI);
            }
        }

        public override float Mass
        {
            get
            {
                return Energy / 100f;
            }
        }

        public override Rectangle Bounds
        {
            get
            {
                var r = Radius;
                return new Rectangle((int)(Position.X - r), (int)(Position.Y - r), (int)r * 2, (int)r * 2);
            }
        }

        public override float DrawPriority
        {
            get { return Radius; }
        }

        private readonly List<UpdateBlock> _updateBlocks = new List<UpdateBlock>();
        private readonly Dictionary<Type, List<IHandleCollisions>> _collisionHandlers = new Dictionary<Type, List<IHandleCollisions>>();
 
        public Organism()
        {
            Position = new Vector2(Game1.Random.Next(Game1.Width), Game1.Random.Next(Game1.Height));
            Velocity = Vector2.Zero;
            Energy = 500;
            Color = Color.RoyalBlue;
            Energy = 500;
            BaseMetabolicRate = 50f;
            MovementMetabolicRate = 0.03f;
            TopSpeed = 100f;
            MaxAge = 100f;
            MaxEnergy = 10000f;
            DistanceMoved = 0f;
        }

        public Organism(DNA dna, float energy, Vector2 position)
            : this(dna)
        {
            Energy = energy;
            Position = position;
        }

        public Organism(DNA dna)
            :this()
        {
            DNA = dna;

            var genes = GeneInterpreter.Interprit(DNA);

            ApplyTraits(genes);

            LoadUpdateBlocks(genes);
            LoadCollisionHandlers(genes);
        }

        private void AddCollisionHandler(IHandleCollisions collisionHandler)
        {
            if (_collisionHandlers.ContainsKey(collisionHandler.CollidesWith))
                _collisionHandlers[collisionHandler.CollidesWith].Add(collisionHandler);
            else
                _collisionHandlers.Add(collisionHandler.CollidesWith, new List<IHandleCollisions> {collisionHandler});
        }

        private void LoadCollisionHandlers(List<IAmAGene> genes)
        {
            var collisionHandlerIndex = genes.FirstIndexOf<IHandleCollisions>();
            while (collisionHandlerIndex >= 0)
            {
                var collisionHandler = genes[collisionHandlerIndex] as IHandleCollisions;
                if (collisionHandler == null) continue;
                
                collisionHandler.LoadBlock(collisionHandlerIndex, genes);
                AddCollisionHandler(collisionHandler);

                collisionHandlerIndex = genes.FirstIndexOf<IHandleCollisions>(collisionHandlerIndex + collisionHandler.BlockLength + 1);
            }
        }

        private void LoadUpdateBlocks(List<IAmAGene> genes)
        {
            var updateBlockIndex = genes.FirstIndexOf<UpdateBlock>();
            while (updateBlockIndex >= 0)
            {
                var updateBlock = genes[updateBlockIndex] as UpdateBlock;
                if (updateBlock == null) continue;
                
                updateBlock.ReadGenes(updateBlockIndex, genes);
                _updateBlocks.Add(updateBlock);

                updateBlockIndex = genes.FirstIndexOf<UpdateBlock>(updateBlockIndex + updateBlock.BlockLength + 1);
            }
        }

        private void ApplyTraits(IEnumerable<IAmAGene> genes)
        {
            var traits = genes.Where(g => g is ITrait).Cast<ITrait>();

            foreach (var trait in traits)
                trait.Apply(this);
        }

        public override void Update(float deltaTime)
        {
            if (Dead)
            {
                if (Energy < 0f)
                    Die(true);

                return;
            }

            CalculateEnergyConsumption(deltaTime);

            if (Age > MaxAge)
                Die(true);

            if (Dead)
                return;

            foreach (var updateBlock in _updateBlocks)
                updateBlock.Update(this, deltaTime);

            if (Dead)
                return;

            DistanceMoved += (Velocity*deltaTime).Length();

            base.Update(deltaTime);
        }

        private void CalculateEnergyConsumption(float deltaTime)
        {
            Energy -= BaseMetabolicRate*deltaTime;

            if (Energy > 0f)
            {
                var force = Force.Length();
                Energy -= force*MovementMetabolicRate*deltaTime;
            }

            if (Energy < 0f)
            {
                Die(true);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Game1.Circle, Bounds, Color);
        }
        
        public override void HandleCollision(GameObject other, float deltaTime)
        {
            if (_collisionHandlers.ContainsKey(other.GetType()))
            {
                foreach (var collisionHandler in _collisionHandlers[other.GetType()])
                {
                    collisionHandler.HandleCollision(this, other, deltaTime);
                }
            }
        }

        public void GiveEnergy(float amount)
        {
            Energy += amount;
            EnergyGiven += amount;

            if (Energy > MaxEnergy)
                Energy = MaxEnergy;
        }

        public float TakeEnergy(float desiredAmount)
        {
            var taken = desiredAmount;

            if (Energy < desiredAmount)
            {
                taken = Energy;
                Energy = 0f;
            }
            else
                Energy -= taken;

            EnergyGiven -= taken;

            return taken;
        }

        public void Remember(byte key, object item)
        {
            if (Memory.ContainsKey(key))
                Memory[key] = item;
            else Memory.Add(key, item);
        }

        public T Remember<T>(byte key)
        {
            if (!Memory.ContainsKey(key))
                return default(T);

            if (Memory[key] is T)
                return (T)Memory[key];

            return default(T);
        }

        public void Forget(byte key)
        {
            if (Memory.ContainsKey(key))
                Memory.Remove(key);
        }

        public float Distance(GameObject go)
        {
            return (go.Position - Position).Length();
        }
    }
}
