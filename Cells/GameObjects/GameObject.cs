using System.Linq.Expressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cells
{
    public abstract class GameObject
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration { get; set; }
        public Vector2 Force { get; set; }
        
        public virtual float Mass { get; protected set; }
        public abstract Rectangle Bounds { get; }
        public abstract float DrawPriority { get; }
        public bool Alive { get; private set; }

        public bool Dead
        {
            get { return !Alive; }
        }

        protected GameObject()
        {
            Mass = 10f;
            Acceleration = Vector2.Zero;
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;
            Force = Vector2.Zero;
            Alive = true;
        }

        public virtual void Update(float deltaTime)
        {
            CalculatePhysics(deltaTime);
            CheckBorders();
        }

        protected virtual void CheckBorders()
        {
            if (Position.X > Game1.Width)
                Velocity = Velocity.FlipX();
            if (Position.X < 0)
                Velocity = Velocity.FlipX();

            if (Position.Y > Game1.Height)
                Velocity = Velocity.FlipY();
            if (Position.Y < 0)
                Velocity = Velocity.FlipY();
        }

        protected virtual void CalculatePhysics(float deltaTime)
        {
            Acceleration = Force/Mass;
            Force = -Velocity;
            Velocity += Acceleration*deltaTime;
            Position += Velocity*deltaTime;
        }

        public virtual void Die(bool remove)
        {
            Alive = false;

            if (remove)
                ObjectManager.Instance.Remove(this);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            
        }


        public virtual void HandleCollision(GameObject other, float deltaTime)
        {
            
        }
    }
}
