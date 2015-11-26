using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cells
{
    public interface GameObject
    {
        Vector2 Position { get; }
        Vector2 Velocity { get; }

        Rectangle Bounds { get; }
        float DrawPriority { get; }

        void Update(float deltaTime);
        void HandleCollision(GameObject other);
        void Draw(SpriteBatch spriteBatch);
    }
}
