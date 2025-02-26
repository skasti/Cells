using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;
using Microsoft.Xna.Framework;

namespace Cells.Genetics.Genes
{
    public class SinglePixel : IDraw
    {
        public class Maker : GeneMaker<SinglePixel>
        {
            public Maker(byte markerFrom, byte markerTo)
                : base(markerFrom, markerTo, 7)
            {
            }

            public override SinglePixel Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new SinglePixel(
                    x: fragment[1],
                    y: fragment[2],
                    r: fragment[3].AsFloat(0f, 1f),
                    g: fragment[4].AsFloat(0f, 1f),
                    b: fragment[5].AsFloat(0f, 1f),
                    a: fragment[6].AsFloat(0.5f, 1f)
                );
            }
        }

        public string Name => "Single Pixel";

        public Point Position { get; private set; }
        public Color Color { get; private set; }

        public SinglePixel(int x, int y, float r, float g, float b, float a)
        {
            Position = new Point(x,y);
            Color = new Color(r,g,b,a);
        }

        public void Apply(int size, List<Color> colorData)
        {
            var index = Math.Min(Position.X, size-1) + Math.Min(Position.Y, size-1) * size;
            colorData[index] = Color;
        }
    }
}
