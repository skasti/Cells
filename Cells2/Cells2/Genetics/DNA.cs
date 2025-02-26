using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;
using Cells.Genetics.Sequences;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Cells.Genetics
{
    public class DNA
    {
        private const int MinimumFragmentLength = 2;
        public const double DefaultMutationRate = 0.01;
        public const double DefaultMutationRange = 0.2;
        public const double DefaultDuplicationRate = 0.001;
        public const double DefaultDeletionRate = 0.005;
        public const double DefaultGrowthRate = 0.1;

        public record MutationOptions
        {
            public double MutationRate = DefaultMutationRate;
            public double MutationRange = DefaultMutationRange;
            public double DuplicationRate = DefaultDuplicationRate;
            public double GrowthRate = DefaultGrowthRate;
            public double DeletionRate = DefaultDeletionRate;

            public static MutationOptions operator *(MutationOptions o, double factor) {
                return new MutationOptions {
                    MutationRate = o.MutationRate * factor,
                    MutationRange = o.MutationRange * factor,
                    DuplicationRate = o.DuplicationRate * factor,
                    GrowthRate = o.GrowthRate * factor,
                    DeletionRate = o.DeletionRate * factor
                };
            }

            public MutationState NewState() {
                return new MutationState(this, Random.NextDouble());
            }

            public record MutationState
            {
                private MutationOptions options;
                private double r;

                public MutationState(MutationOptions options, double r)
                {
                    this.options = options;
                    this.r = r;
                }

                public bool Mutate => r <= options.MutationRate;
                public double MutationRange => options.MutationRange;
                public bool Duplicate => Mutate && Random.NextDouble() <= options.DuplicationRate;
                public bool Grow => Mutate && Random.NextDouble() <= options.GrowthRate;
                public bool Delete => Mutate && Random.NextDouble() <= options.DeletionRate;
            }
        }

        public static MutationOptions DefaultMutation = new MutationOptions();
        public static MutationOptions NoMutation = new MutationOptions{
            MutationRange = 0f,
            MutationRate = 0f,
            DuplicationRate = 0f,
            GrowthRate = 0f,
            DeletionRate = 0f
        };

        public static MutationOptions HighMutation = DefaultMutation * 5f;

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

        public DNA(string filename)
        {
            List<string> lines = File.ReadAllLines(filename).ToList();
            Data = lines.Select(line => Convert.ToByte(line.Substring(0,2), 16)).ToArray();
        }

        public DNA(params Sequence[] sequences)
        {
            var fragments = sequences.Select(s => s.MakeFragment()).Join();
            Data = fragments.ToArray();
        }

        public DNA(int minLength, int maxLength)
        {
            Data = CreateRandomFragment(minLength, maxLength);
        }

        private static byte[] CreateRandomFragment(int minLength, int maxLength)
        {
            var fragment = new byte[minLength + Random.Next(maxLength - minLength)];

            for (int i = 0; i < fragment.Length; i++)
            {
                fragment[i] = (byte)Random.Next(byte.MaxValue);
            }

            return fragment;
        }

        public DNA(MutationOptions options, params DNA[] parents)
        {
            if (options == null)
                options = new MutationOptions();

            var numParents = parents.Length;

            if (numParents < 1)
                throw new NotEnoughParentDNAException();

            var parentFragments = parents.Select(p => p.GetFragments());
            var maxFragments = parentFragments.Max(pf => pf.Count);
            var fragments = new List<byte[]>();

            for (var fi = 0; fi < maxFragments; fi++)
            {
                var fiFragments = new List<byte[]>();
                foreach (var pfs in parentFragments) {
                    if (fi < pfs.Count)
                        fiFragments.Add(pfs[fi]);
                }

                if (fiFragments.Count == 0)
                    continue;

                fragments.Add(GetFragment(options, fiFragments));
            }


            if (options.NewState().Grow)
            {
                if (Random.NextDouble() < 0.1)
                    fragments.Add(SequenceLibrary.RandomSequence().MakeFragment());
                else
                    fragments.Add(CreateRandomFragment(MinimumFragmentLength, MinimumFragmentLength * 20));
            }

            Data = fragments.Join().ToArray();
        }

        public DNA(byte[] bytes)
        {
            this.Data = bytes;
        }

        public static DNA FromRandomizedFragments(MutationOptions options, params DNA[] parents)
        {
            if (options == null)
                options = new MutationOptions();

            var numParents = parents.Length;

            if (numParents < 1)
                throw new NotEnoughParentDNAException();

            var fragments = new List<byte[]>();

            if (numParents == 1)
            {
                fragments.Add(parents[0].GetFragment(options, 0, parents[0].Size));
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

                    fragments.Add(orderedParents[i].GetFragment(options, lastSplit, splitIndex));
                    lastSplit = splitIndex;
                }

                if (orderedParents.Count > 1)
                    fragments.Add(orderedParents.Last().GetFragment(options, lastSplit, orderedParents.Last().Size - 1));
            }

            if (Random.NextDouble() < options.GrowthRate)
            {
                if (Random.NextDouble() < 0.01)
                    fragments.Add(SequenceLibrary.RandomSequence().MakeFragment());
                else
                    fragments.Add(CreateRandomFragment(MinimumFragmentLength, MinimumFragmentLength * 5));
            }

            return new DNA(fragments.Join().ToArray());
        }

        public List<byte[]> GetFragments()
        {
            return GeneInterpreter.GetNaturalFragments(this);
        }

        public static byte[] GetFragment(MutationOptions options, List<byte[]> parents)
        {
            if (parents.Count() == 0)
                throw new NotEnoughParentDNAException();

            var mutation = options.NewState();

            if (mutation.Delete)
                return Array.Empty<byte>();

            var parentFragment = parents[Random.Next(0, parents.Count - 1)];
            var growth = mutation.Grow ? Random.Next(2) : 0;
            var fragment = new List<byte>();//new byte[parentFragment.Length + growth];

            for (var i = 0; i < parentFragment.Length + growth; i++)
            {
                if (i < parentFragment.Length)
                {
                    if (mutation.Delete)
                        continue;

                    var value = parentFragment[i];

                    if (mutation.Mutate)
                        fragment.Add(value.Mutate(options.MutationRange));
                    else
                        fragment.Add(value);
                }
                else
                    fragment.Add((byte)Random.Next(byte.MaxValue));
            }

            if (mutation.Duplicate)
                fragment.AddRange(fragment);

            return fragment.ToArray();
        }

        public byte[] GetFragment(MutationOptions options, int start, int end)
        {
            if (start >= end)
                throw new ArgumentException("Start must be before end");

            if (end > Size)
                throw new ArgumentException("End must be smaller than Size");

            var fragment = new byte[end - start];
            var fragmentIndex = 0;

            for (int i = start; i < end; i++)
            {
                var state = options.NewState();
                if (state.Mutate)
                    fragment[fragmentIndex++] = Data[i].Mutate(options.MutationRange);
                else
                    fragment[fragmentIndex++] = Data[i];
            }

            return fragment;
        }

        public byte[] GetFragment(int start, IMakeAGene maker)
        {
            return GetFragment(NoMutation, start, start + maker.Size);
        }

        public float RelatedPercent(DNA other)
        {
            return Data.Compare(other.Data);
        }

        public void Save(string filename)
        {
            File.WriteAllLines(filename, Data.Select(b => b.ToString("X2")));
        }
    }
}
