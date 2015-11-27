using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Cells
{
    public static class VectorExtensions
    {
        public static Vector2 FlipY(this Vector2 input)
        {
            return new Vector2(input.X, -input.Y);
        }

        public static Vector2 FlipX(this Vector2 input)
        {
            return new Vector2(-input.X, input.Y);
        }
    }
}
