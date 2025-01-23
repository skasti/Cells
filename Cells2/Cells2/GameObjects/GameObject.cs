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
        public float MaxForce => Mass * MaxForceRatio;

        protected GameObject()
        {
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

            if (Position != prevPosition)
                CurrentNode = CurrentNode?.UpdateObjectNode(this);

            prevPosition = Position;
        }

        protected virtual void CheckBorders()
        {
            if (Position.X > Game1.WorldBounds.X)
            {
                Position = new Vector2(Game1.WorldBounds.X, Position.Y);
                Velocity = Velocity.FlipX();
            }
            if (Position.X < 0)
            {
                Position = new Vector2(0, Position.Y);
                Velocity = Velocity.FlipX();
            }

            if (Position.Y > Game1.WorldBounds.Y)
            {
                Position = new Vector2(Position.X, Game1.WorldBounds.Y);
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

            Position += Velocity * deltaTime;
            ExternalForce = ((-Velocity * Mass) / deltaTime) * Game1.Friction;
        }

        public virtual void Die(bool remove)
        {
            Alive = false;

            if (remove)
            {
                Removed = true;
                ObjectManager.Instance.Remove(this);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }


        public virtual void HandleCollision(GameObject other, float deltaTime)
        {

        }
    }
}
