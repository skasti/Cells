using System;
using Cells.Geometry;
using Cells.QuadTree;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = Cells.Geometry.Rectangle;

namespace Cells.GameObjects
{
    public abstract class GameObject
    {
        public enum ReadableProperty {
            Position_X,
            Position_Y,
            Relative_X,
            Relative_Y,
            Mass,
            Radius,
            Color_R,
            Color_G,
            Color_B,
            Age,
            Alive,
            Energy,
            RelatedPercent,
            Fitness
        }
        public Node CurrentNode { get; set; }
        public Vector2 Position { get; set; }
        private Vector2 prevPosition { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration { get; set; }
        public Vector2 Force { get; set; }
        public Vector2 ExternalForce { get; set; }

        public virtual float Mass { get; protected set; }
        public abstract Rectangle Bounds { get; }
        public abstract float DrawPriority { get; }

        public float Age { get; private set; }
        public bool Alive { get; private set; }
        public bool Dead
        {
            get { return !Alive; }
        }
        public bool Removed { get; private set; }
        public float TopSpeed { get; set; }
        public float MaxForceRatio { get; set; }
        public float MaxForce => Math.Clamp(Mass * MaxForceRatio, 10f, 100000f);
        protected float Friction = Game1.Friction;

        protected GameObject()
        {
            TopSpeed = 200f;
            Mass = 10f;
            Acceleration = Vector2.Zero;
            Position = Vector2.Zero;
            prevPosition = Position;
            Velocity = Vector2.Zero;
            Force = Vector2.Zero;
            MaxForceRatio = 100f;
            Alive = true;
            Age = 0;
        }

        public virtual void Update(float deltaTime)
        {
            if (Dead)
                return;

            Age += deltaTime;
            CalculatePhysics(deltaTime);
            //CheckBorders();

            if (Position != prevPosition || CurrentNode == null)
                CurrentNode = (CurrentNode ?? ObjectManager.Instance.SearchTree).UpdateObjectNode(this);

            prevPosition = Position;
        }

        protected virtual void CheckBorders()
        {
            if (Position.X > Game1.WorldSize.X)
            {
                Position = new Vector2(Game1.WorldSize.X, Position.Y);
                Velocity = Velocity.FlipX();
            }
            if (Position.X < 0)
            {
                Position = new Vector2(0, Position.Y);
                Velocity = Velocity.FlipX();
            }

            if (Position.Y > Game1.WorldSize.Y)
            {
                Position = new Vector2(Position.X, Game1.WorldSize.Y);
                Velocity = Velocity.FlipY();
            }

            if (Position.Y < 0)
            {
                Position = new Vector2(Position.X, 0);
                Velocity = Velocity.FlipY();
            }
        }

        protected virtual void CalculatePhysics(float deltaTime)
        {
            if (Force.LengthSquared() > MaxForce*MaxForce)
                Force = Vector2.Normalize(Force) * MaxForce;

            Acceleration = Force / Mass + ExternalForce / Mass;
            Velocity += Acceleration * deltaTime;

            if (Velocity.Length() > TopSpeed)
            {
                var newVelocity = Velocity;
                newVelocity.Normalize();
                newVelocity *= TopSpeed;

                Velocity = newVelocity;
            }

            if (Velocity.X == float.NaN || Velocity.Y == float.NaN)
            {
                Velocity = Vector2.Zero;
            }

            Position += Velocity * deltaTime;
            ExternalForce = ((-Velocity * Mass) * Friction);
        }

        public virtual void Die(bool remove)
        {
            Alive = false;

            if (remove)
            {
                Removed = true;
                ObjectManager.Instance.Remove(this);
                CurrentNode?.Remove(this);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }

        public virtual void HandleCollision(GameObject other, float deltaTime)
        {
            var relativePos = other.Position - Position;
            var bounds = Bounds.Width * 0.5f + other.Bounds.Width * 0.5f;
            var distance = relativePos.Length();
            if (distance < bounds)
            {
                var relativeVelocity = other.Velocity - Velocity;
                var direction = Vector2.Normalize(relativePos);
                var vAlongDirection = relativeVelocity.X * direction.X + relativeVelocity.Y * direction.Y;

                var impulse = -vAlongDirection * 1.5f / (1f / Mass + 1f / other.Mass);
                var collisionForce = -direction * impulse / deltaTime;
                //var collisionForce = (-direction * (other.Velocity * other.Mass)) / deltaTime;
                var repulsionForce = (-direction * Math.Max(bounds*0.9f-distance, 0f) * Mass) / deltaTime;
                ExternalForce += collisionForce + repulsionForce;
            }
        }
    }
}
