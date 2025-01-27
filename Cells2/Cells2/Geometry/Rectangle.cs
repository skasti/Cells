using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace Cells.Geometry
{
    public struct Rectangle: IEquatable<Rectangle>
    {
        [DataMember]
        public float X;
        [DataMember]
        public float Y;
        [DataMember]
        public float Width;
        [DataMember]
        public float Height;

        public Rectangle(float x, float y, float width, float height) : this()
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Rectangle(Vector2 position, Vector2 size)
        {
            X = position.X;
            Y = position.Y;
            Width = size.X;
            Height = size.Y;
        }

        public Vector2 Position => new Vector2(X, Y);
        public Vector2 Size => new Vector2(Width, Height);
        public float Left => X;
        public float Right => X + Width;
        public float Top => Y;
        public float Bottom => Y + Width;
        public Vector2 Center => new Vector2(X + Width * 0.5f, Y + Height * 0.5f);

        public void Inflate(float horizontalAmount, float verticalAmount)
        {
            X -= horizontalAmount;
            Y -= verticalAmount;
            Width += horizontalAmount * 2;
            Height += verticalAmount * 2;
        }

        public bool Intersects(Rectangle value)
        {
            if (value.Left < Right && Left < value.Right && value.Top < Bottom)
            {
                return Top < value.Bottom;
            }

            return false;
        }

        public void Intersects(ref Rectangle value, out bool result)
        {
            result = value.Left < Right && Left < value.Right && value.Top < Bottom && Top < value.Bottom;
        }

        public static Rectangle Intersect(Rectangle value1, Rectangle value2)
        {
            Intersect(ref value1, ref value2, out var result);
            return result;
        }

        public static void Intersect(ref Rectangle value1, ref Rectangle value2, out Rectangle result)
        {
            if (value1.Intersects(value2))
            {
                float num = Math.Min(value1.X + value1.Width, value2.X + value2.Width);
                float num2 = Math.Max(value1.X, value2.X);
                float num3 = Math.Max(value1.Y, value2.Y);
                float num4 = Math.Min(value1.Y + value1.Height, value2.Y + value2.Height);
                result = new Rectangle(num2, num3, num - num2, num4 - num3);
            }
            else
            {
                result = new Rectangle(0, 0, 0, 0);
            }
        }

        public void Offset(float offsetX, float offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        public void Offset(Vector2 amount)
        {
            X += amount.X;
            Y += amount.Y;
        }

        public override string ToString()
        {
            return "{X:" + X + " Y:" + Y + " Width:" + Width + " Height:" + Height + "}";
        }

        public static Rectangle Union(Rectangle value1, Rectangle value2)
        {
            float num = Math.Min(value1.X, value2.X);
            float num2 = Math.Min(value1.Y, value2.Y);
            return new Rectangle(num, num2, Math.Max(value1.Right, value2.Right) - num, Math.Max(value1.Bottom, value2.Bottom) - num2);
        }

        public static void Union(ref Rectangle value1, ref Rectangle value2, out Rectangle result)
        {
            result.X = Math.Min(value1.X, value2.X);
            result.Y = Math.Min(value1.Y, value2.Y);
            result.Width = Math.Max(value1.Right, value2.Right) - result.X;
            result.Height = Math.Max(value1.Bottom, value2.Bottom) - result.Y;
        }

        public void Deconstruct(out float x, out float y, out float width, out float height)
        {
            x = X;
            y = Y;
            width = Width;
            height = Height;
        }

        public override int GetHashCode()
        {
            return (((17 * 23 + X.GetHashCode()) * 23 + Y.GetHashCode()) * 23 + Width.GetHashCode()) * 23 + Height.GetHashCode();
        }

        public bool Equals(Rectangle other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (obj is Rectangle)
            {
                return this == (Rectangle)obj;
            }

            return false;
        }

        public void Contains(ref Rectangle value, out bool result)
        {
            result = X <= value.X && value.X + value.Width <= X + Width && Y <= value.Y && value.Y + value.Height <= Y + Height;
        }

        public bool Contains(Rectangle value)
        {
            if (X <= value.X && value.X + value.Width <= X + Width && Y <= value.Y)
            {
                return value.Y + value.Height <= Y + Height;
            }

            return false;
        }

        public void Contains(ref Vector2 value, out bool result)
        {
            result = X <= value.X && value.X < (X + Width) && Y <= value.Y && value.Y < (Y + Height);
        }

        public bool Contains(Vector2 value)
        {
            if (X <= value.X && value.X < (X + Width) && Y <= value.Y)
            {
                return value.Y < (Y + Height);
            }

            return false;
        }

        public bool Contains(float x, float y)
        {
            if (X <= x && x < (X + Width) && Y <= y)
            {
                return y < (Y + Height);
            }

            return false;
        }

        public Rectangle Copy()
        {
            return new Rectangle(X, Y, Width, Height);
        }

        internal Rectangle Inflated(float horizontalAmount, float verticalAmount)
        {
            var copy = Copy();
            copy.Inflate(horizontalAmount, verticalAmount);
            return copy;
        }

        public static bool operator !=(Rectangle a, Rectangle b)
        {
            return !(a == b);
        }

        public static bool operator ==(Rectangle a, Rectangle b)
        {
            if (a.X == b.X && a.Y == b.Y && a.Width == b.Width)
            {
                return a.Height == b.Height;
            }

            return false;
        }
    }
}