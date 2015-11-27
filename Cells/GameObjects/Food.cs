using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cells.GameObjects
{
    public class Food: GameObject
    {
        public float Energy { get; private set; }

        public override Rectangle Bounds
        {
            get { return new Rectangle(Position.ToPoint(), new Point(30,30));}
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
            spriteBatch.Draw(Game1.Sprint, Bounds, Color.Fuchsia);
        }
    }
}
