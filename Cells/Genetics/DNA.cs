using Cells.Genetics.Exceptions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cells.Genetics
{
    public class DNA
    {
        static Random _random;
        static Random Random
        {
            get
            {
                if (_random == null)
                    _random = new Random((int)DateTime.Now.Ticks);

                return _random;
            }
        }

        public byte[] Data { get; private set; }

        public DNA()
        {
            Data = new byte[6 + Random.Next(100)];

            for (int i = 0; i < Data.Length; i++)
            {
                Data[i] = (byte)Random.Next(byte.MaxValue);
            }
        }

        public void ApplyTraits(Individual individual)
        {
            if (Data.Length < 6)
                throw new GenomeTooShortException();

            individual.BaseMetabolicRate = Data[0].AsFloat(0.01f, 10f);
            individual.MovementMetabolicRate = Data[1].AsFloat(0.01f, 0.5f);
            individual.Color = new Color(Data[2].AsFloat(), Data[3].AsFloat(), Data[4].AsFloat(), Data[5].AsFloat());
        }
    }
}
