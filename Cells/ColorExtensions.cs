using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Cells
{
    public static class ColorExtensions
    {
        public static Color AddColor(this Color input, Vector4 change)
        {
            return new Color(input.ToVector4() + change);
        }
    }
}
