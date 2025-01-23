using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace Cells.Geometry
{
    public struct Circle: IEquatable<Circle>
    {
        [DataMember]
        public Vector2 Position;

        [DataMember]
        public float Radius;

        public bool Equals(Circle other)
        {
            return Position == other.Position && Radius == other.Radius;
        }

        public bool Intersects(Circle other)
        {
            var r = (Radius + other.Radius)*(Radius + other.Radius);
            return (Position - other.Position).LengthSquared() <= r;
        }

        public bool Contains(Vector2 point)
        {
            return (Position - point).LengthSquared() <= Radius*Radius;
        }
    }
}