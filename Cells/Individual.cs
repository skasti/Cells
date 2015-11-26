using Cells.Genetics;
using Cells.Genetics.Genes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cells.Genetics.Genes.Behaviour;

namespace Cells
{
    public class Individual: GameObject
    {
        public Dictionary<byte, object> Memory = new Dictionary<byte, object>();
        public DNA Genes { get; private set; }

        public float Energy { get; private set; }
        public float BaseMetabolicRate { get; set; }
        public float MovementMetabolicRate { get; set; }

        public Color Color { get; set; }
        public float Radius
        {
            get
            {
                return (float)Math.Sqrt(Energy / Math.PI);
            }
        }

        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration { get; set; }
        public Vector2 Force { get; set; }
        public float Mass
        {
            get
            {
                return Energy / 100f;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                var r = Radius;
                return new Rectangle((int)(Position.X - r), (int)(Position.Y - r), (int)r * 2, (int)r * 2);
            }
        }

        public float DrawPriority
        {
            get { return Radius; }
        }

        public Individual(DNA genes)
        {
            Position = new Vector2(Game1.r.Next(Game1.Width), Game1.r.Next(Game1.Height));
            Velocity = new Vector2(Game1.r.Next(500), Game1.r.Next(500));
            Energy = 500;
            Genes = genes;
            Genes.ApplyTraits(this);
        }

        public Individual()
        {
            Position = new Vector2(Game1.r.Next(Game1.Width), Game1.r.Next(Game1.Height));
            Velocity = new Vector2(Game1.r.Next(500), Game1.r.Next(500));
            Energy = 500;
            Color = Color.RoyalBlue;

            BaseMetabolicRate = 1f;
            MovementMetabolicRate = 0.01f;
        }

        public void Update(float deltaTime)
        {
            Energy -= BaseMetabolicRate * deltaTime;
            
            if (Energy > 0f)
            {
                var speed = Velocity.Length();
                var radius = Radius;
                Energy -= speed * radius * MovementMetabolicRate * deltaTime;
            }

            if (Energy < 0f)
            {
                ObjectManager.Instance.Remove(this);
                return;
            }

            //Perform genetic behaviour, which can set Velocity
            new CancelMotion().Run(this, deltaTime);

            Acceleration = Force / Mass;
            Force = -Velocity;
            Velocity += Acceleration * deltaTime;
            Position += Velocity * deltaTime;

            if (Position.X > Game1.Width)
                Velocity = FlipX(Velocity);
            if (Position.X < 0)
                Velocity = FlipX(Velocity);

            if (Position.Y > Game1.Height)
                Velocity = FlipY(Velocity);
            if (Position.Y < 0)
                Velocity = FlipY(Velocity);
        }

        private Vector2 FlipY(Vector2 input)
        {
            return new Vector2(input.X, -input.Y);
        }

        private Vector2 FlipX(Vector2 input)
        {
            return new Vector2(-input.X, input.Y);
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Game1.circle, Bounds, Color);
        }


        public void HandleCollision(GameObject other)
        {
            //throw new NotImplementedException();
        }
    }
}
