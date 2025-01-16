using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cells.GameObjects
{
    public abstract class GameObject
    {
        public Vector2 Position { get; set; }
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

        protected GameObject()
        {
            Mass = 10f;
            Acceleration = Vector2.Zero;
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;
            Force = Vector2.Zero;
            Alive = true;
            Age = 0;
        }

        public virtual void Update(float deltaTime)
        {
            if (Dead)
                return;

            Age += deltaTime;
            CalculatePhysics(deltaTime);
            CheckBorders();
        }

        protected virtual void CheckBorders()
        {
            if (Position.X > Game1.Width)
            {
                Position = new Vector2(Game1.Width, Position.Y);
                Velocity = Velocity.FlipX();
            }
            if (Position.X < 0)
            {
                Position = new Vector2(0, Position.Y);
                Velocity = Velocity.FlipX();
            }

            if (Position.Y > Game1.Height)
            {
                Position = new Vector2(Position.X, Game1.Height);
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
            Acceleration = Force/Mass + ExternalForce/Mass;
            Velocity += Acceleration*deltaTime;

            if (Velocity.Length() > TopSpeed)
            {
                var newVelocity = Velocity;
                newVelocity.Normalize();
                newVelocity *= TopSpeed;

                Velocity = newVelocity;
            }

            Position += Velocity*deltaTime;
            ExternalForce = ((-Velocity*Mass)/deltaTime)*0.002f;
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
