using System.Collections.Generic;
using System.Linq;
using Cells.Genetics.Genes;
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
            new SmoothColorChange.Maker(),
            new RandomMovement.Maker(),
            new UpdateBlock.Maker(),
            new ParthenoGenesis.Maker(),
            new ChaseGameObject.Maker(),
            new TargetFood.Maker(),
            new TargetOrganisms.Maker()
        }; 

        public static List<IAmAGene> Interprit(DNA dna)
        {
            var genes = new List<IAmAGene>();
            for (int i = 0; i < dna.Data.Length; i++)
            {
                var marker = dna.Data[i];
                var maker = GeneMakers.FirstOrDefault(m => m.MarkerFrom <= marker && m.MarkerTo >= marker);

                if (maker == null)
                    continue;

                if (dna.Size > i + maker.Size)
                {
                    genes.Add(maker.Make(dna.GetFragment(i, maker)));
                    i += maker.ArgumentBytes;
                }
            }

            return genes;
        }
    }
}
