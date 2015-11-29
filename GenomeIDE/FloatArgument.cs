using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GenomeIDE
{
    public class FloatArgument: IGeneArgument
    {
        public FloatArgument(string name, string text, float mapFrom, float mapTo)
        {
            Name = name;
            Text = text;
            MapFrom = mapFrom;
            MapTo = mapTo;
        }

        public string Name { get; private set; }
        public string Text { get; private set; }
        public float MapFrom { get; private set; }
        public float MapTo { get; private set; }

        [JsonIgnore]
        public string Description {
            get { return "{0}: {1} (0x{2:0.000}-0x{3:0.000})".Inject(Name, Text, MapFrom, MapTo); }
        }
        public string Calculate(byte input)
        {
            var value = input.AsFloat(MapFrom, MapTo);
            return "{0:0.000}f".Inject(value);
        }
    }
}
