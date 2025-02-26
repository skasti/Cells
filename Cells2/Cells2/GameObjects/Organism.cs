using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class Organism : GameObject, ICollide
    {
        public Dictionary<byte, object> Memory = new Dictionary<byte, object>();
        public DNA DNA { get; private set; }

        public List<Texture2D> Textures { get; set; }
        public float NaturalFitness
        {
            get
            {
                if (EnergyGiven <= 0f)
                    return 0f;

                return EnergyGiven * 0.01f;
            }
        }

        public float Fitness => NaturalFitness + BreedFitness + ChildrenFitness;

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

        public float HP { get; private set; }
        public float Armor { get; private set; }
        public float MaxHP { get; private set; }
        public float MaxArmor { get; private set; }

        private float _prevEnergy;

        public float EnergyConsumption { get; private set; }
        public float EnergyChangeRate { get; private set; }
        public float EnergyGiven { get; private set; }
        public float BaseMetabolicRate { get; set; }
        public float MovementMetabolicRate { get; set; }

        public float MaxAge => Math.Clamp(Fitness*2, 20f, 200f);

        public float BreedFitness { get; private set; }
        private int _breedCount = 0;
        public int BreedCount {
            get { return _breedCount; }
            private set {
                var change = value - _breedCount;

                if (change <= 0)
                    return;

                SpawnTime = 0;
                BreedFitness += Math.Clamp(change * NaturalFitness * 0.1f, 1f, 500f);
                _breedCount = value;
            }
        }

        public List<Organism> Children { get; private set; } = new List<Organism>();
        public int MaxAliveChildren { get; private set; }
        public float ChildrenFitness {get; private set; }
        private float _childrenFitnessUpdate = 0f;

        public Color Color { get; set; }
        public float Radius
        {
            get
            {
                return (float)Math.Min(Math.Sqrt(Energy / Math.PI), 1500);
            }
        }

        public override float Mass => Math.Max(Energy * 0.001f, 1f);

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

        public float SpawnTime { get; private set; }

        public string Status { get; set; }
        public string Capabilities { get; private set; }
        public List<string> UpdateLog { get; private set; } = new List<string>();
        public int UpdateLogIndentLevel = 1;

        public List<string> CollisionLog { get; private set; } = new List<string>();
        public DNA.MutationOptions MutationOptions { get; set; } = new DNA.MutationOptions();

        public int CollisionLogIndentLevel = 1;
        private readonly List<UpdateBlock> _updateBlocks = new List<UpdateBlock>();
        private readonly Dictionary<Type, List<IHandleCollisions>> _collisionHandlers = new Dictionary<Type, List<IHandleCollisions>>();

        public Organism(DNA dna, float energy, Vector2 position)
        {
            Energy = energy;
            Position = position;
            Velocity = Vector2.Zero;
            Color = Color.RoyalBlue;
            BaseMetabolicRate = 10f;
            MovementMetabolicRate = 0.01f;
            TopSpeed = 200f;
            MaxForceRatio = 100f;
            MaxEnergy = 10000f;
            DistanceMoved = 0f;
            Textures = new List<Texture2D>();
            _prevEnergy = Energy;

            HP = 100;
            Armor = 0;
            MaxHP = 100;
            MaxArmor = 100;
            DNA = dna;
            var genes = GeneInterpreter.Interprit(DNA);

            ApplyTraits(genes);

            LoadUpdateBlocks(genes);
            LoadCollisionHandlers(genes);
        }

        public Organism(DNA dna)
            : this(dna, Game1.Random.Next(500, 2000), Game1.RandomPosition(Game1.WorldBounds.Shrunk(0.6f)))
        {}

        public Organism()
        :this(new DNA(10,100), Game1.Random.Next(500, 2000), Game1.RandomPosition(Game1.WorldBounds.Shrunk(0.6f)))
        {}

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
            var updateBlockIndex = genes.FirstIndexOf<UpdateBlock>();
            while (updateBlockIndex >= 0)
            {
                var updateBlock = genes[updateBlockIndex] as UpdateBlock;
                if (updateBlock == null) continue;

                var minIndex = updateBlock.ReadGenes(updateBlockIndex + 1, genes);
                if (updateBlock.BlockLength > 0)
                    _updateBlocks.Add(updateBlock);

                updateBlockIndex = genes.FirstIndexOf<UpdateBlock>(minIndex + 1);
            }
        }

        private void ApplyTraits(List<IAmAGene> genes)
        {
            var traits = genes.Where(g => g is ITrait).Cast<ITrait>();

            foreach (var trait in traits)
            {
                AddCapability(trait.Name);
                trait.Apply(this, genes);
            }
        }

        public override void Update(float deltaTime)
        {
            foreach (var collisionHandler in _collisionHandlers)
                collisionHandler.Value.ForEach(h => h.Update(deltaTime));

            Status = "Idle";

            _childrenFitnessUpdate += deltaTime;

            if (_childrenFitnessUpdate >= 1f) {
                Children.RemoveAll(c => c.Dead);

                if (Children.Count > MaxAliveChildren)
                    MaxAliveChildren = Children.Count;

                ChildrenFitness = Math.Max(Children.Sum(c => c.Fitness), ChildrenFitness);
                _childrenFitnessUpdate = 0f;
            }

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

            if (HP <= 0)
            {
                Status = "DEAD FROM DAMAGE";
                Die(true);
            }

            if (Dead)
                return;

            var sw = new Stopwatch();
            sw.Start();
            UpdateCost = 0;
            CollisionCost = 0;
            UpdateLog.Clear();
            Force = Vector2.Zero;
            UpdateLogIndentLevel = 0;

            foreach (var updateBlock in _updateBlocks)
            {
                updateBlock.LogIndentLevel = UpdateLogIndentLevel;
                updateBlock.Update(this, deltaTime);
                UpdateCost += updateBlock.Cost;
                UpdateLog.AddRange(updateBlock.Log);
            }

            sw.Stop();
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
            SpawnTime += deltaTime;
        }

        private void CalculateEnergyConsumption(float deltaTime)
        {
            var consumption = (
                (Mass * 0.01f * BaseMetabolicRate) +
                (BaseMetabolicRate * Math.Max(UpdateCost + CollisionCost, 1) * 0.1f) +
                (MovementMetabolicRate * Force.Length() * 0.01f)
            );

            if (consumption == float.NaN)
            {
                consumption = 100f;
            }

            EnergyConsumption = consumption;
            Energy -= consumption * deltaTime;

            if (Energy < 0f || Energy == float.NaN)
            {
                Status = "DEAD";
                Die(true);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Game1.View.Contains(Bounds) || Game1.View.Intersects(Bounds))
                DrawAt(spriteBatch,Bounds.Translate(Game1.View, Game1.ViewZoom));
        }

        public void DrawAt(SpriteBatch spriteBatch, Rectangle bounds)
        {
            spriteBatch.Draw(Game1.Circle, bounds.ToRectangle(), Color);
                foreach (var texture in Textures)
                    spriteBatch.Draw(texture, bounds.ToRectangle(), Color);
        }

        public override void HandleCollision(GameObject other, float deltaTime)
        {
            if (_collisionHandlers.ContainsKey(other.GetType()))
            {
                if (CollisionLog.Count > 0)
                    CollisionLog.Clear();
                CollisionLogIndentLevel = 0;
                foreach (var collisionHandler in _collisionHandlers[other.GetType()])
                {
                    collisionHandler.LogIndentLevel = 0;
                    collisionHandler.Log.Clear();
                    collisionHandler.HandleCollision(this, other, deltaTime);
                    CollisionCost += collisionHandler.Cost;
                    CollisionLog.AddRange(collisionHandler.Log);
                }
            }

            //if (other is Food)
                base.HandleCollision(other, deltaTime);
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

        public object Remember(byte key)
        {
            if (!Memory.ContainsKey(key))
                return null;

            return Memory[key];
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

        internal void AddTexture(Texture2D texture, int features)
        {
            Textures.Add(texture);
        }

        public override void Die(bool remove)
        {
            base.Die(remove);

            if (HP > 0)
                return;

            while (Energy > 0)
            {
                var foodEnergy = TakeEnergy(Math.Max(Energy * 0.5f, 50f));
                var foodPosition = new Vector2(
                    Bounds.X + Game1.Random.NextSingle() * Bounds.Width,
                    Bounds.Y + Game1.Random.NextSingle() * Bounds.Height
                );
                var foodDirection = Vector2.Normalize(foodPosition-Position);
                var foodVelocity = foodDirection * Math.Max(50f - (foodPosition-Position).Length(),0f);
                ObjectManager.Instance.Add(
                    new Food(
                        foodPosition,
                        foodEnergy,
                        foodVelocity
                    )
                );
            }
        }

        internal float AddHP(float amount)
        {
            HP += amount;
            if (HP > MaxHP)
            {
                var extra = HP - MaxHP;
                HP = MaxHP;
                return amount - extra;
            }
            return amount;
        }

        internal float AddArmor(float amount)
        {
            if (Armor == MaxArmor)
                return 0;

            Armor += amount;
            if (Armor > MaxArmor)
            {
                var extra = Armor - MaxArmor;
                Armor = MaxArmor;
                amount -= extra;
            }

            if (amount > 0)
                ObjectManager.Instance.Add(FloatingNumber.ArmorRegen(Position, amount, 4f * Game1.DisplayedTimewarp));

            return amount;
        }

        internal AttackResult Attack(Organism target, float attackForce)
        {
            var energyCost = TakeEnergy(Energy * attackForce);
            var damage = target.Defend(energyCost * 0.5f);
            var relativePos = target.Position - Position;
            var midPoint = relativePos.Normalized() * (relativePos.Length() * 0.5f);
            ObjectManager.Instance.Add(FloatingNumber.Damage(Position + midPoint, damage, 4f * Game1.DisplayedTimewarp));
            return new AttackResult{
                EnergyCost = energyCost,
                Damage = damage
            };
        }

        private float Defend(float damage)
        {
            if (Armor > damage)
            {
                Armor -= damage;
            } else {
                Armor = 0f;
            }

            var armorDefense = MaxArmor > 0 ? Armor / MaxArmor : 0f;
            var penetratingDamage = damage - (damage * armorDefense);

            HP -= penetratingDamage;
            if (HP <= 0)
            {
                Die(true);
                var damageDone = penetratingDamage + HP;
                HP = 0f;
                return damageDone;
            }

            return penetratingDamage;
        }

        internal void AddChild(Organism child)
        {
            Children.Add(child);
            BreedCount++;
        }

        public record AttackResult {
            public float EnergyCost;
            public float Damage;
        }
    }
}
