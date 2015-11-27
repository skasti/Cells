using System;
using System.Collections.Generic;
using System.Linq;

namespace Cells.Genetics
{
    public static class DNAExtensions
    {
        static Random _random;
        static Random Random
        {
            get { return _random ?? (_random = new Random((int)DateTime.Now.Ticks)); }
        }

        public static float Fraction = (1f / byte.MaxValue);

        public static float AsFloat(this byte input)
        {
            return Fraction * input;
        }
        
        public static float AsFloat(this byte input, float minValue, float maxValue)
        {
            if (minValue >= maxValue)
                throw new ArgumentException("minValue must be lower than maxValue");

            return minValue + (input.AsFloat() * (maxValue - minValue)); 
        }

        public static IEnumerable<byte> Join(this IEnumerable<IEnumerable<byte>> fragments)
        {
            var result = new List<byte>();
            fragments.ToList().ForEach(result.AddRange);

            return result;
        }

        public static byte Mutate(this byte input)
        {
            return (byte)(input + Random.Next(byte.MaxValue));
        }
    }
}
