using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GenomeIDE
{
    public class ByteArgument: IGeneArgument
    {
        public ByteArgument(string name, string text, byte mapFrom, byte mapTo)
        {
            Name = name;
            Text = text;
            MapFrom = mapFrom;
            MapTo = mapTo;
        }

        public string Name { get; private set; }
        public string Text { get; private set; }
        public byte MapFrom { get; private set; }
        public byte MapTo { get; private set; }

        [JsonIgnore]
        public string Description {
            get { return "{0}: {1} (0x{2:X2}-0x{3:X2})".Inject(Name, Text, MapFrom, MapTo); }
        }
        public string Calculate(byte input)
        {
            var value = input.AsByte(MapTo, MapFrom);
            return "0x{0:X2}".Inject(value);
        }
    }
}
