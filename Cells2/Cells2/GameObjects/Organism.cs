using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cells.Genetics;
using Cells.Genetics.Genes;
using Cells.Genetics.GeneTypes;
using Cells.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = Cells.Geometry.Rectangle;

namespace Cells.GameObjects
{
    public class Organism : GameObject
    {
        public Dictionary<byte, object> Memory = new Dictionary<byte, object>();
        public DNA DNA { get; private set; }

        public Texture2D Texture { get; set; }
        public float NaturalFitness
        {
            get
            {
                if (EnergyGiven <= 0f)
                    return 0f;

                return EnergyGiven * 0.01f + DistanceMoved * 0.005f;
            }
        }

        public float Fitness => NaturalFitness * 0.5f + BreedFitness;

        public float DistanceMoved { get; private set; }

        public float MaxEnergy { get; set; }
        private float _energy;
        public float Energy
        {
            get { return _energy; }
            private set
            {
                if (value == float.NaN)
                {
                    Console.WriteLine("Attempt setting NAN");
                }
                else
                {
                    _energy = value;
                }
            }
        }

        private float _prevEnergy;

        public float EnergyConsumption { get; private set; }
        public float EnergyChangeRate { get; private set; }
        public float EnergyGiven { get; private set; }
        public float BaseMetabolicRate { get; set; }
        public float MovementMetabolicRate { get; set; }

        public float MaxAge => Math.Clamp(Fitness, 50f, 500f);

        public float BreedFitness { get; private set; }
        private int _breedCount = 0;
        public int BreedCount {
            get { return _breedCount; }
            set {
                var change = value - _breedCount;

                if (change <= 0)
                    return;

                BreedFitness += change * Fitness * 0.1f;
                _breedCount = value;
            }
        }

        public Color Color { get; set; }
        public float Radius
        {
            get
            {
                return (float)Math.Min(Math.Sqrt(Energy / Math.PI), 15000);
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
                return new Rectangle(Position - (Vector2.One * r), Vector2.One * r * 2);
            }
        }

        public override float DrawPriority
        {
            get { return Radius; }
        }

        public float UpdateCost { get; private set; }
        public float CollisionCost { get; private set; }

        public string Status { get; set; }
        public string Capabilities { get; private set; }
        public string UpdateCode { get; private set; }
        public List<string> UpdateLog { get; private set; } = new List<string>();
        public int UpdateLogIndentLevel = 1;
        private readonly List<UpdateBlock> _updateBlocks = new List<UpdateBlock>();
        private readonly Dictionary<Type, List<IHandleCollisions>> _collisionHandlers = new Dictionary<Type, List<IHandleCollisions>>();
        public Organism()
        {
            Position = Game1.RandomPosition();
            Velocity = Vector2.Zero;
            Energy = 500;
            Color = Color.RoyalBlue;
            Energy = 500;
            BaseMetabolicRate = 10f;
            MovementMetabolicRate = 0.01f;
            TopSpeed = 200f;
            MaxForceRatio = 100f;
            MaxEnergy = 1000000f;
            DistanceMoved = 0f;
            Texture = Game1.Circle;
            _prevEnergy = Energy;
        }

        public Organism(DNA dna, float energy, Vector2 position)
            : this(dna)
        {
            Energy = energy;
            Position = position;
        }

        public Organism(DNA dna)
            : this()
        {
            DNA = dna;

            var genes = GeneInterpreter.Interprit(DNA);

            ApplyTraits(genes);

            LoadUpdateBlocks(genes);
            LoadCollisionHandlers(genes);
        }

        private void AddCapability(string capability)
        {
            if (Capabilities?.Contains(capability) == true)
                return;

            if (Capabilities?.Length > 0)
                Capabilities += $"\n    {capability}";
            else
                Capabilities = $"    {capability}";
        }

        private void AddCollisionHandler(IHandleCollisions collisionHandler)
        {
            if (_collisionHandlers.ContainsKey(collisionHandler.CollidesWith))
            {
                if (!collisionHandler.AllowMultiple && _collisionHandlers[collisionHandler.CollidesWith].Any(h => h.GetType() == collisionHandler.GetType()))
                {
                    Console.WriteLine("[AddCollisionHandler][Skipped] - Already has same type of handler");
                }
                else
                {
                    AddCapability(collisionHandler.GetType().Name);
                    _collisionHandlers[collisionHandler.CollidesWith].Add(collisionHandler);
                }
            }
            else
            {
                AddCapability(collisionHandler.GetType().Name);
                _collisionHandlers.Add(collisionHandler.CollidesWith, new List<IHandleCollisions> { collisionHandler });
            }
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
            var updateCode = new StringBuilder();
            var updateBlockIndex = genes.FirstIndexOf<UpdateBlock>();
            while (updateBlockIndex >= 0)
            {
                var updateBlock = genes[updateBlockIndex] as UpdateBlock;
                if (updateBlock == null) continue;

                updateBlock.ReadGenes(updateBlockIndex, genes);
                _updateBlocks.Add(updateBlock);
                updateCode.AppendLine(updateBlock.ToString());

                updateBlockIndex = genes.FirstIndexOf<UpdateBlock>(updateBlockIndex + updateBlock.BlockLength + 1);
            }
            UpdateCode = updateCode.ToString();
        }

        private void ApplyTraits(IEnumerable<IAmAGene> genes)
        {
            var traits = genes.Where(g => g is ITrait).Cast<ITrait>();

            foreach (var trait in traits)
            {
                AddCapability(trait.Name);
                trait.Apply(this);
            }
        }

        public override void Update(float deltaTime)
        {
            Status = "Idle";

            if (Dead)
            {
                if (Energy < 0f)
                    Die(true);

                return;
            }

            CalculateEnergyConsumption(deltaTime);

            if (Age > MaxAge)
            {
                Status = "DEAD OF OLD AGE";
                Die(true);
            }

            if (Dead)
                return;

            UpdateCost = 0;
            CollisionCost = 0;

            if (UpdateLog.Count > 0)
                UpdateLog.Clear();

            Force = Vector2.Zero;
            foreach (var updateBlock in _updateBlocks)
            {
                UpdateLogIndentLevel = 0;
                UpdateLog.Add($"Block {_updateBlocks.IndexOf(updateBlock)} ({updateBlock.BlockLength}):");
                UpdateLogIndentLevel = 1;
                updateBlock.Update(this, deltaTime);
                UpdateCost += updateBlock.Cost;
            }

            if (Force.Length() < 2f)
            {
                Force = Vector2.Zero;
            }

            if (Dead)
                return;

            DistanceMoved += (Velocity * deltaTime).Length();

            base.Update(deltaTime);
            EnergyChangeRate = (Energy - _prevEnergy) * (1f / deltaTime);
            _prevEnergy = Energy;
        }

        private void CalculateEnergyConsumption(float deltaTime)
        {
            var consumption = (
                (Mass * 0.01f * BaseMetabolicRate) +
                (BaseMetabolicRate * Math.Max(UpdateCost + CollisionCost, 1))
            ) * deltaTime;

            if (Energy > 0f)
            {
                var force = Force.Length() * 0.001f;
                consumption += force * MovementMetabolicRate * deltaTime;
            }

            EnergyConsumption = consumption * 1f / deltaTime;
            Energy -= consumption;

            if (Energy < 0f)
            {
                Status = "DEAD";
                Die(true);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Game1.View.Contains(Bounds) || Game1.View.Intersects(Bounds))
            {
                spriteBatch.Draw(Texture, Bounds.Translate(Game1.View, Game1.ViewZoom).ToRectangle(), Color);
            }
        }

        public override void HandleCollision(GameObject other, float deltaTime)
        {
            if (_collisionHandlers.ContainsKey(other.GetType()))
            {
                foreach (var collisionHandler in _collisionHandlers[other.GetType()])
                {
                    collisionHandler.HandleCollision(this, other, deltaTime);
                    CollisionCost += collisionHandler.Cost;
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

        private void Log(string logLine, int indentChange = 0)
        {
            if (Game1.Observing != this) {
                return;
            }
            UpdateLog.Add(Indent(UpdateLogIndentLevel) + logLine);
            UpdateLogIndentLevel += indentChange;
        }

        private string Indent(int level)
        {
            var output = new StringBuilder();
            for (var i = 0; i < level; i++)
                output.Append("    ");
            return output.ToString();
        }
    }
}
