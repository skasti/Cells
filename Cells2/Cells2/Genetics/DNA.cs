using Cells.Genetics.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cells.Genetics
{
    public class DNA
    {
        private const int MinimumFragmentLength = 2;
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

        public DNA(String filename)
        {
            List<String> lines = File.ReadAllLines(filename).ToList();
            Data = lines.Select(line => Convert.ToByte(line.Substring(0,2), 16)).ToArray();
        }

        public DNA(int minLength, int maxLength)
        {
            Data = CreateRandomFragment(minLength, maxLength);
        }

        private byte[] CreateRandomFragment(int minLength, int maxLength)
        {
            var fragment = new byte[minLength + Random.Next(maxLength - minLength)];

            for (int i = 0; i < fragment.Length; i++)
            {
                fragment[i] = (byte)Random.Next(byte.MaxValue);
            }

            return fragment;
        }

        public DNA(params DNA[] parents)
        {
            var numParents = parents.Length;

            if (numParents < 1)
                throw new NotEnoughParentDNAException();

            var fragments = new List<byte[]>();

            if (numParents == 1)
            {
                fragments.Add(parents[0].GetFragment(0, parents[0].Size));
            }
            else
            {
                var orderedParents = parents.OrderBy(p => p.Data.Length).ToList();

                var numSplits = numParents - 1;

                var largestParent = orderedParents.Last().Data.Length;

                int lastSplit = 0;

                for (int i = 0; i < numSplits; i++)
                {
                    var splitIndex = Random.Next(lastSplit + MinimumFragmentLength,
                        largestParent - MinimumFragmentLength*(numSplits - i));

                    if (splitIndex >= orderedParents[i].Size)
                        splitIndex = orderedParents[i].Size - 1;

                    if (splitIndex <= lastSplit)
                        continue;

                    fragments.Add(orderedParents[i].GetFragment(lastSplit, splitIndex));
                    lastSplit = splitIndex;
                }

                if (orderedParents.Count > 1)
                    fragments.Add(orderedParents.Last().GetFragment(lastSplit, orderedParents.Last().Size - 1));
            }

            if (Random.NextDouble() < DefaultMutationRate)
                fragments.Add(CreateRandomFragment(MinimumFragmentLength, MinimumFragmentLength * 5));

            Data = fragments.Join().ToArray();
        }

        public byte[] GetFragment(int start, int end, float mutationRate = DefaultMutationRate)
        {
            if (start >= end)
                throw new ArgumentException("Start must be before end");

            if (end > Size)
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

        public float RelatedPercent(DNA other, int samples = -1)
        {
            return Data.Compare(other.Data, samples);
        }

        public void Save(string filename)
        {
            File.WriteAllLines(filename, Data.Select(b => b.ToString("X2")));
        }
    }
}
