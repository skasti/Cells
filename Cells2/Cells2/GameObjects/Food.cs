using System;
using Cells.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = Cells.Geometry.Rectangle;

namespace Cells.GameObjects
{
    public class Food: GameObject
    {
        public float Energy { get; private set; }

        public override Rectangle Bounds
        {
            get
            {
                var size = new Vector2(30,30);
                return new Rectangle(Position - (size * 0.5f), size);
            }
        }

        public override float DrawPriority
        {
            get { return 0f; }
        }

        public Food(Vector2 position, float energy)
        {
            Position = position;
            Energy = energy;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Game1.View.Contains(Bounds) || Game1.View.Intersects(Bounds)) {
                spriteBatch.Draw(Game1.Sprint, Bounds.Translate(Game1.View, Game1.ViewZoom).ToRectangle(), Color.Fuchsia);
            }
        }

        internal float TakeEnergy(float desiredAmount)
        {
            var taken = desiredAmount;

            if (Energy < desiredAmount)
            {
                taken = Energy;
                Energy = 0f;
                Die(true);
            }
            else
                Energy -= taken;

            return taken;
        }
    }
}
