using System.Collections.Generic;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes
{
    public class UpdateBlock: ICanUpdate
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0x50, 0x51, 2)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new UpdateBlock(fragment[1]);
            }
        }

        public int BlockLength { get; private set; }

        private readonly List<ICanUpdate> _updates = new List<ICanUpdate>(); 

        public UpdateBlock(int blockLength)
        {
            BlockLength = blockLength;
        }

        public void ReadGenes(int startIndex, List<IAmAGene> genes)
        {
            if (BlockLength == 0)
                return;

            for (int i = startIndex; i <= startIndex + BlockLength; i++)
            {
                if (i >= genes.Count)
                    break;

                if (genes[i] == this)
                    continue;

                if (genes[i] is ICanUpdate)
                    _updates.Add(genes[i] as ICanUpdate);
            }
        }

        public int Update(Organism self, float deltaTime)
        {
            for (int i = 0; i < _updates.Count; i++)
            {
                i += _updates[i].Update(self, deltaTime);
            }

            return 0;
        }
    }
}
