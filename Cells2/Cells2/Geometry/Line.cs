using System;
using System.Numerics;

namespace Cells.Geometry
{
    public struct Line: IEquatable<Line>
    {
        Vector2 A;
        Vector2 B;

        public bool Equals(Line other)
        {
            return A == other.A && B == other.B;
        }
    }
}