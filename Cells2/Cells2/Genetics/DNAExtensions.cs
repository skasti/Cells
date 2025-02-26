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

        public static byte AsByte(this byte input, byte maxValue = byte.MaxValue, byte minValue = 0)
        {
            var output = input;
            while (output > maxValue)
                output -= maxValue;

            while (output < minValue)
                output += minValue;

            return output;
        }

        // public static byte AsByte(this byte input, byte maxValue = byte.MaxValue, byte minValue = 0)
        // {
        //     int range = maxValue - minValue + 1; // Ensure inclusive max
        //     return (byte)((((input - minValue) % range) + range) % range + minValue);
        // }

        // public static byte AsByte(this byte input, byte maxValue = byte.MaxValue, byte minValue = 0)
        // {
        //     return (byte)((input % (maxValue - minValue)) + minValue);
        // }

        public static byte AsGeneByte(this int value, int min = 0)
        {
            return (byte)(value - min);
        }

        public static byte AsGeneByte(this byte value, byte min = 0)
        {
            return (byte)(value - min);
        }

        public static byte AsGeneByte(this float input)
        {
            return (byte)Math.Clamp(input / Fraction, byte.MinValue, byte.MaxValue);
        }

        public static byte AsGeneByte(this float input, float minValue, float maxValue)
        {
            if (minValue >= maxValue)
                throw new ArgumentException("minValue must be lower than maxValue");

            float normalized = (input - minValue) / (maxValue - minValue);
            return (byte)Math.Clamp(normalized * byte.MaxValue, byte.MinValue, byte.MaxValue);
        }

        public static IEnumerable<byte> Join(this IEnumerable<IEnumerable<byte>> fragments)
        {
            var result = new List<byte>();
            fragments.ToList().ForEach(result.AddRange);

            return result;
        }

        public static byte Mutate(this byte input, double mutationRange = DNA.DefaultMutationRange)
        {
            var mutationRangeValue = (int)(byte.MaxValue * mutationRange);
            var mutationAmount = -mutationRangeValue + Random.Next(mutationRangeValue * 2);
            var result = (byte)(input + mutationAmount);
            return result;
        }

        public static float Compare(this byte a, byte b)
        {
            var diff = Math.Abs(a - b);
            if (diff == 0) return 1f;
            return 1f - ((float)diff / (float)byte.MaxValue);
            //return (float) diff/a;
        }

        public static float Compare(this byte[] arr1, byte[] arr2)
        {
            if (arr1 == null || arr2 == null)
            throw new ArgumentNullException("Input arrays cannot be null");

            int maxLength = Math.Max(arr1.Length, arr2.Length);
            int minLength = Math.Min(arr1.Length, arr2.Length);

            double totalDifference = 0;

            // Compare overlapping bytes
            for (int i = 0; i < minLength; i++)
                totalDifference += Math.Abs(arr1[i] - arr2[i]) / 255.0;

            // Account for extra bytes in the longer array (as max possible difference)
            for (int i = minLength; i < maxLength; i++)
                totalDifference += 1; // 255/255 full difference for missing bytes

            // Normalize difference as percentage
            return (float)(1.0 - (totalDifference / maxLength));
        }
    }
}
