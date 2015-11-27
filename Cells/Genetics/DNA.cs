using Cells.Genetics.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cells.Genetics
{
    public class DNA
    {
        private const int MinimumFragmentLength = 10;
        private const float DefaultMutationRate = 0.001f;

        static Random _random;
        static Random Random
        {
            get { return _random ?? (_random = new Random((int) DateTime.Now.Ticks)); }
        }

        public byte[] Data { get; private set; }

        public int Size
        {
            get { return Data.Length; }
        }

        public DNA(int minLength, int maxLength)
        {
            Data = new byte[minLength + Random.Next(maxLength - minLength)];

            for (int i = 0; i < Data.Length; i++)
            {
                Data[i] = (byte)Random.Next(byte.MaxValue);
            }
        }

        public DNA(params DNA[] parents)
        {
            var numParents = parents.Length;

            if (numParents < 1)
                throw new NotEnoughParentDNAException();

            if (numParents == 1)
            {
                Data = parents[0].GetFragment(0, parents[0].Size - 1);
            }
            else
            {
                var orderedParents = parents.OrderBy(p => p.Data.Length).ToList();

                var numSplits = numParents - 1;

                var fragments = new List<byte[]>();

                var largestParent = orderedParents.Last().Data.Length;

                int lastSplit = 0;

                for (int i = 0; i < numSplits; i++)
                {
                    var splitIndex = Random.Next(lastSplit + MinimumFragmentLength,
                        largestParent - MinimumFragmentLength*(numSplits - i));

                    if (splitIndex >= orderedParents[i].Size)
                        splitIndex = orderedParents[i].Size - 1;

                    fragments.Add(orderedParents[i].GetFragment(lastSplit, splitIndex - 1));
                    lastSplit = splitIndex;
                }

                if (orderedParents.Count > 1)
                    fragments.Add(orderedParents.Last().GetFragment(lastSplit, orderedParents.Last().Size - 1));

                Data = fragments.Join().ToArray();
            }
        }

        public byte[] GetFragment(int start, int end, float mutationRate = DefaultMutationRate)
        {
            if (start >= end)
                throw new ArgumentException("Start must be before end");

            if (end >= Size)
                throw new ArgumentException("End must be smaller than Size");

            var fragment = new byte[end - start];
            var fragmentIndex = 0;

            for (int i = start; i < end; i++)
            {
                if (Random.NextDouble() <= mutationRate)
                    fragment[fragmentIndex++] = Data[i].Mutate();
                else
                    fragment[fragmentIndex++] = Data[i];
            }

            return fragment;
        }

        public byte[] GetFragment(int start, IMakeAGene maker)
        {
            return GetFragment(start, start + maker.Size, 0f);
        }
    }
}
