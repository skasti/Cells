using System;
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

        public static string ToShortString(this Vector2 vector)
        {
            return $"{{X: {vector.X:0.} Y: {vector.Y:0.}}}";
        }

        public static string ToShortString(this Vector4 vector, int decimals = 0)
        {
            switch (decimals)
            {
                case 0:
                    return $"{{X: {vector.X:0.} Y: {vector.Y:0.} Z: {vector.Z:0.} W: {vector.W:0.}}}";
                case 1:
                    return $"{{X: {vector.X:0.0} Y: {vector.Y:0.0} Z: {vector.Z:0.0} W: {vector.W:0.0}}}";
                case 2:
                    return $"{{X: {vector.X:0.00} Y: {vector.Y:0.00} Z: {vector.Z:0.00} W: {vector.W:0.00}}}";
                case 3:
                    return $"{{X: {vector.X:0.000} Y: {vector.Y:0.000} Z: {vector.Z:0.000} W: {vector.W:0.000}}}";
                case 4:
                    return $"{{X: {vector.X:0.0000} Y: {vector.Y:0.0000} Z: {vector.Z:0.0000} W: {vector.W:0.0000}}}";
                default:
                    throw new ArgumentException($"unsupported number of decimals: {decimals}", "decimals");
            }
        }
    }
}
