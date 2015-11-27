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
            new Break.Maker(),
            new EatFood.Maker(),
            new EatOrganisms.Maker(),
            new SetColor.Maker(),
            new SetTopSpeed.Maker(),
            new SmoothColorChange.Maker(),
            new RandomMovement.Maker(),
            new UpdateBlock.Maker(),
            new ParthenoGenesis.Maker(),
            new ChaseObject.Maker(),
            new TargetFood.Maker(),
            new TargetOrganisms.Maker(),
            new AvoidObject.Maker(),

            new Goto.Maker(),
            new MemorySet.Maker(),
            new MemoryAdd.Maker(),
            new MemorySubtract.Maker(),
            new SkipIfEquals.Maker(),
            new SkipIfNotEquals.Maker(),
            new SkipIfGT.Maker(),
            new SkipIfLT.Maker()
        };

        public static List<IMakeAGene> GetMakers()
        {
            return GeneMakers.ToList();
        }

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

                    if (Game1.Debug == null)
                        Debug.WriteLine(String.Format("[{0}]", gene.GetType().Name));
                    
                    genes.Add(gene);
                    i += maker.ArgumentBytes;
                }
            }

            return genes;
        }
    }
}
