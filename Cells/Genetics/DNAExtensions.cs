using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cells.Genetics
{
    public static class DNAExtensions
    {
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
    }
}
