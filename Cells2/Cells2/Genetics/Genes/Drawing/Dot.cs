using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;
using Microsoft.Xna.Framework;

namespace Cells.Genetics.Genes
{
    public class Dot : IDraw
    {
        public class Maker : GeneMaker<Dot>
        {
            public Maker(byte markerFrom, byte markerTo)
                : base(markerFrom, markerTo, 8)
            {
            }

            public override Dot Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new Dot(
                    x: fragment[1],
                    y: fragment[2],
                    radius: fragment[3].AsByte(100,5),
                    r: fragment[4].AsByte(),
                    g: fragment[5].AsByte(),
                    b: fragment[6].AsByte(),
                    a: fragment[7].AsByte(minValue: 128)
                );
            }

            public override byte[] MakeFragment(Dot g)
            {
                var frag = base.MakeFragment(g);
                frag[1] = (byte)g.Position.X;
                frag[2] = (byte)g.Position.Y;
                frag[3] = g.Radius.AsGeneByte(5);
                frag[4] = g.Color.R.AsGeneByte();
                frag[5] = g.Color.G.AsGeneByte();
                frag[6] = g.Color.B.AsGeneByte();
                frag[7] = g.Color.A.AsGeneByte(128);

                return frag;
            }
        }

        public string Name => "Single Pixel";

        public Point Position { get; private set; }
        public int Radius { get; private set; }
        public Color Color { get; private set; }

        public Dot(int x, int y, int radius, byte r, byte g, byte b, byte a)
        {
            Position = new Point(x,y);
            Color = new Color(r,g,b,a);
            Radius = radius;
        }

        public void Apply(int size, List<Color> colorData)
        {
            var posV = Position.ToVector2();
            var rS = Radius * Radius;
            for (var x = Math.Max(Position.X - Radius, 0); x < size && x < Position.X + Radius; x++)
            {
                for (var y = Math.Max(Position.Y - Radius, 0); y < size && y < Position.Y + Radius; y++)
                {
                    var xyV = new Vector2(x,y);
                    var xyR = (xyV - posV).LengthSquared();
                    if (xyR <= rS)
                    {
                        var index = x + y * size;
                        colorData[index] = Color;
                    }
                }
            }
        }
    }
}
