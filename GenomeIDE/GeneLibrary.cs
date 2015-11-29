using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GenomeIDE
{
    public static class GeneLibrary
    {
        static readonly List<Gene> Genes = new List<Gene>();

        public static List<Gene> List
        {
            get { return Genes.ToList(); }
        }

        public static Gene Find(byte marker)
        {
            return Genes.FirstOrDefault(g => g.Match(marker));
        }

        public static void Add(Gene newGene)
        {
            var crashesWith = Genes.FirstOrDefault(g => g.Crash(newGene));

            if (crashesWith != null)
                throw new ArgumentException("Gene markers crashes with '" + crashesWith.Name + "'");

            Genes.Add(newGene);
        }

        public static void Save()
        {
            File.WriteAllLines("GeneLibrary.dna", Genes.Select(JsonConvert.SerializeObject));
        }

        public static void Load()
        {
            var lines = File.ReadAllLines("GeneLibrary.dna");
            foreach (var line in lines)
            {
                var gene = JsonConvert.DeserializeObject<Gene>(line);
                Add(gene);
            }
        }
    }
}
