using System.Collections.Generic;
using System.Runtime.Serialization;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cells.Genetics.Genes
{
    public class Texture : ITrait
    {
        public class Maker : GeneMaker<Texture>
        {
            public Maker(byte markerFrom, byte markerTo)
                : base(markerFrom, markerTo, 3)
            {
            }

            public override Texture Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new Texture(
                    numDrawCommands: fragment[1].AsByte(0xFF, 0x01),
                    textureSize: fragment[2].AsByte(0xFF, 0x0A)
                );
            }

            public override byte[] MakeFragment(Texture gene)
            {
                var fragment = base.MakeFragment(gene);
                fragment[1] = gene.NumDrawCommands;
                fragment[2] = gene.TextureSize;
                return fragment;
            }
        }

        public string Name => "Texture";
        public byte NumDrawCommands { get; private set; }
        public readonly byte TextureSize;
        public Texture2D RenderedTexture { get; private set; }

        public Texture(byte numDrawCommands, byte textureSize)
        {
            NumDrawCommands = numDrawCommands;
            TextureSize = textureSize;
        }

        public void Apply(Organism self, List<IAmAGene> genes)
        {
            var drawCommands = new List<IDraw>();
            var selfIndex = genes.IndexOf(this);
            for (var i = selfIndex + 1; i < genes.Count && i < selfIndex + NumDrawCommands + 1; i++)
            {
                if (genes[i] is IDraw)
                    drawCommands.Add(genes[i] as IDraw);
            }

            if (drawCommands.Count == 0)
                return;

            var colorData = new List<Color>();

            for (var i = 0; i < TextureSize*TextureSize; i++)
                colorData.Add(Color.Transparent);

            foreach (var command in drawCommands)
                command.Apply(TextureSize, colorData);

            var texture = new Texture2D(Game1.PublicGraphicsDevice, TextureSize, TextureSize);
            texture.SetData<Color>(colorData.ToArray());

            RenderedTexture = texture;
            self.AddTexture(RenderedTexture, drawCommands.Count);
        }
    }
}