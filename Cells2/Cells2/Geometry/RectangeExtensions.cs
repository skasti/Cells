using System;
using Microsoft.Xna.Framework;
using XNARectangle = Microsoft.Xna.Framework.Rectangle;

namespace Cells.Geometry
{
    public static class RectangleExtensions
    {
        public static Rectangle Translate(this Rectangle input, Rectangle view, float viewZoom)
        {
            return new Rectangle(
                (input.X - view.X) * viewZoom,
                (input.Y - view.Y) * viewZoom,
                input.Width * viewZoom,
                input.Height * viewZoom
            );
        }

        public static Rectangle Surround(this Rectangle input, float scale, Vector2 maxSize)
        {
            var newWidth = Math.Min(input.Width * scale, maxSize.X);
            var newHeight = Math.Min(input.Height * scale, maxSize.Y);

            return new Rectangle(
                input.X - newWidth / 2,
                input.Y - newHeight / 2,
                newWidth,
                newHeight
            );
        }

        public static bool Intersects(this Rectangle rectangle, Circle circle)
        {
            var closest = new Vector2(
                Math.Clamp(circle.Position.X, rectangle.Left, rectangle.Right),
                Math.Clamp(circle.Position.Y, rectangle.Top, rectangle.Bottom)
            );

            return circle.Contains(closest);
        }

        public static XNARectangle ToRectangle(this Rectangle rectangle)
        {
            return new XNARectangle(
                (int)rectangle.X,
                (int)rectangle.Y,
                (int)rectangle.Width,
                (int)rectangle.Height
            );
        }
    }
}
