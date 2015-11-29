using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNACoder;
using Newtonsoft.Json;

namespace GenomeIDE
{
    public static class GeneLibrary
    {
        static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Binder = new TypeNameSerializationBinder("GenomeIDE.{0}, GenomeIDE"),
        };

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
            File.WriteAllText("GeneLibrary.genes", JsonConvert.SerializeObject(Genes, Formatting.Indented, SerializerSettings));
        }

        public static void Load()
        {
            var json = File.ReadAllText("GeneLibrary.genes");
            var genes = JsonConvert.DeserializeObject<List<Gene>>(json, SerializerSettings);
            genes.ForEach(Add);
        }
    }
}
