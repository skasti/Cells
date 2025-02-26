using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cells.Genetics.Genes;
using Cells.Genetics.Genes.Programming;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics
{
    public static class GeneInterpreter
    {
        static readonly List<IMakeAGene> GeneMakers = new List<IMakeAGene>
        {
            new Break.Maker(0x00),
            new Skip.Maker(0x03, 0x03),
            new MemorySet.Maker(0x04, 0x04),
            new MemoryAdd.Maker(0x05, 0x05),
            new MemorySubtract.Maker(0x06, 0x06),
            new SkipIfEQ.Maker(0x07, 0x07),
            new SkipIfNE.Maker(0x08, 0x08),
            new SkipIfGT.Maker(0x09, 0x09),
            new SkipIfLT.Maker(0x0A, 0x0A),
            // 0x0B-0x0F available

            new Eat.Maker(0x10, 0x19),
            // 0x1A-0x1F available

            new Attack.Maker(0x20, 0x22),
            // 0x22-0x2F available

            new SetColor.Maker(0x30, 0x38),
            new SetTopSpeed.Maker(0x39, 0x3A),
            new Armor.Maker(0x3B, 0x3C),
            // 0x3D-0x3F available

            new SmoothColorChange.Maker(0x40, 0x42),
            new SetBaseMetabolicRate.Maker(0x46),
            new SetMovementMetabolicRate.Maker(0x48),
            // 0x4A-0x4F available

            new UpdateBlock.Maker(0x50, 0x55),

            // 0x60-0x6F available

            new RandomMovement.Maker(0x70, 0x78),
            new AddForceMem.Maker(0x79, 0x7F),
            new ParthenoGenesis.Maker(0x80, 0x82),
            new Breed.Maker(0x83, 0x85),

            new ChaseObject.Maker(0x90, 0x92),
            new AvoidObject.Maker(0x93, 0X94),
            // 0x95-0x9F available

            new TargetFood.Maker(0xA0, 0xA2),
            new TargetOrganisms.Maker(0xA4, 0xA6),
            // 0xA7-0xAF available

            // 0xB0-0xBF available
            // 0xC0-0xCF available

            new SinglePixel.Maker(0xD0, 0xD5),
            new Dot.Maker(0xD6, 0xDA),
            // 0xDB-0xDF available

            new Texture.Maker(0xE0, 0xE5),
            // 0xE6-0xEF available

            new MemoryAddMem.Maker(0xF0, 0xF1),
            new MemoryCpy.Maker(0xF2, 0xF3),
            new MemorySubtractMem.Maker(0xF4, 0xF5),
            new SkipIfEQMem.Maker(0xF6, 0xF7),
            new SkipIfGTMem.Maker(0xF8, 0xF9),
            new SkipIfLTMem.Maker(0xFA, 0xFB),
            new SkipIfNEMem.Maker(0xFC, 0xFD),
            new MemoryReadProperty.Maker(0xFE, 0xFF),
        };

        public static List<IMakeAGene> Makers =>  GeneMakers.ToList();

        public static void CheckMakerIntegrity()
        {
            int numErrors = 0;
            for (int i = 0; i < byte.MaxValue; i++)
            {
                var marker = i;
                var makers = GeneMakers.Where(m => m.MarkerFrom <= marker && m.MarkerTo >= marker).ToList();

                if (makers.Count > 1)
                {
                    Debug.WriteLine("[GeneInterpreter][ERROR] Multiple Makers for marker 0x{0}", marker.ToString("X2"));
                    numErrors++;
                }
                else if (makers.Any())
                    Debug.WriteLine("[GeneInterpreter][0x{0}] {1} ({2})",
                        marker.ToString("X2"),
                        makers.First().GetType().FullName,
                        makers.First().Size);
            }

            if (numErrors > 0)
                throw new Exception("There were errors: " + numErrors);
        }

        public static List<IAmAGene> Interprit(DNA dna)
        {
            var genes = new List<IAmAGene>();
            for (int i = 0; i < dna.Data.Length; i++)
            {
                var marker = dna.Data[i];
                var maker = GeneMakers.FirstOrDefault(m => m.MarkerFrom <= marker && m.MarkerTo >= marker);

                if (maker == null)
                    continue;

                if (dna.Size >= i + maker.Size)
                {
                    var gene = maker.Make(dna.GetFragment(i, maker));

                    genes.Add(gene);
                    i += maker.ArgumentBytes;
                }
            }

            return genes;
        }

        public static List<byte[]> GetNaturalFragments(DNA dna)
        {
            var fragments = new List<byte[]>();
            var fragmentStart = 0;
            var fragmentEnd = -1;
            var maxLength = Game1.Random.Next(2, 10);

            for (int i = 0; i < dna.Data.Length;)
            {
                var marker = dna.Data[i];
                var maker = GeneMakers.FirstOrDefault(m => m.MarkerFrom <= marker && m.MarkerTo >= marker);

                if (maker != null)
                {
                    if (fragmentStart < fragmentEnd)
                        fragments.Add(dna.GetFragment(DNA.NoMutation, fragmentStart, fragmentEnd));

                    fragmentStart = i;
                    fragmentEnd = Math.Min(i + maker.Size, dna.Size);
                    var fragment = dna.GetFragment(DNA.NoMutation, fragmentStart, fragmentEnd);
                    fragments.Add(fragment);
                    i += fragment.Length;
                    fragmentStart = i;
                }
                else
                {
                    fragmentEnd = i + 1;
                    i++;
                    if (fragmentEnd - fragmentStart + 1 >= maxLength)
                    {
                        fragments.Add(dna.GetFragment(DNA.NoMutation, fragmentStart, fragmentEnd));
                        fragmentStart = i;
                        maxLength = Game1.Random.Next(2, 10);
                    }
                }
            }

            return fragments;
        }
    }
}
