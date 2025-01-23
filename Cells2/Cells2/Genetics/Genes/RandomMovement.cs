﻿using System.Collections.Generic;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;
using Microsoft.Xna.Framework;

namespace Cells.Genetics.Genes
{
    public class RandomMovement: ICanUpdate
    {
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0x70, 0x7F, 2)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new RandomMovement(fragment[1].AsFloat(20f, 500f));
            }
        }

        public float DesiredSpeed { get; private set; }
        public float Cost { get; private set; } = 1f;

        public float timeToChange = 0f;
        public float changeRate = 10f;
        public string Name { get; } = "RANDOM MOVEMENT";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        private Vector2 previousForce = Vector2.Zero;

        public RandomMovement(float desiredSpeed)
        {
            DesiredSpeed = desiredSpeed;
        }

        public int Update(Organism self, float deltaTime)
        {
            Cost = 0.1f;
            var speed = self.Velocity.Length();

            if (speed > DesiredSpeed)
            {
                this.Log($"RANDOM MOVEMENT; {speed} > {DesiredSpeed}");
                return 0;
            }

            timeToChange -= deltaTime;
            Cost = 1f;

            if (timeToChange > 0f)
            {
                self.Force += previousForce;
                this.Log($"RANDOM MOVEMENT {previousForce}");
                return 0;
            }
            timeToChange = changeRate;

            var direction = new Vector2((float)Game1.Random.NextDouble()*2f - 1f,(float)Game1.Random.NextDouble()*2f - 1f);
            direction.Normalize();
            direction *= DesiredSpeed * deltaTime;
            var forceAdd = (direction/deltaTime)*self.Mass;
            forceAdd = forceAdd * 0.25f + previousForce * 0.75f;
            previousForce = forceAdd;
            self.Force += forceAdd;
            this.Log($"RANDOM MOVEMENT {forceAdd}");
            Cost = 2f;

            return 0;
        }

        private Dictionary<int,string> _string = new Dictionary<int, string>();
        public string ToString(int level = 0)
        {
            if (!_string.ContainsKey(level))
                _string.Add(level, $"{this.Indent(level)}RandomMovement[{DesiredSpeed}]");

            return _string[level];
        }
    }
}
