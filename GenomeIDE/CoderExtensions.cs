using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNACoder
{
    public static class CoderExtensions
    {
        public static byte[] ToBytes(this string[] input)
        {
            return input.Select(i => Convert.ToByte(i.Substring(0,2), 16)).ToArray();
        }
    }
}
