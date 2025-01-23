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

        public static byte AsByte(this byte input, byte maxValue, byte minValue = 0)
        {
            return (byte) ((input%(maxValue - minValue)) + minValue);
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

        public static float Compare(this byte a, byte b)
        {
            var diff = Math.Abs(a - b);
            if (diff == 0) return 1f;
            return 1f - ((float)diff / (float)byte.MaxValue);
            //return (float) diff/a;
        }

        public static float Compare(this byte[] a, byte[] b, int numSamples = -1)
        {
            int maxIndex = Math.Min(a.Length, b.Length) - 1;
            float results = 0f;

            if (numSamples < 0) {
                numSamples = (int)(maxIndex * 0.75f);
            }

            for (int i = 0; i < numSamples; i++)
            {
                var index = Random.Next(maxIndex);
                var subResult = a[index].Compare(b[index]);
                results += subResult;
            }

            return results/numSamples;
        }
    }
}
