using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using Cells.GameObjects;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes
{
    public class Attack : ICanUpdate, ITrait
    {
        public class Maker : GeneMaker<Attack>
        {
            public Maker(byte markerFrom = 0x20, byte markerTo = 0x21)
                : base(markerFrom, markerTo, 6)
            {
            }

            public override Attack Make(byte[] fragment)
            {
                return new Attack(
                    targetAddress: fragment[1].AsByte(0x20),
                    dnaSampleSize: fragment[2].AsFloat(0.01f, 0.99f),
                    relationThreshold: fragment[3].AsFloat(0.01f, 1f),
                    attackForce: fragment[4].AsFloat(0.01f, 0.10f),
                    attackInterval: fragment[5].AsFloat(0.5f, 5f)
                );
            }

            public override byte[] MakeFragment(Attack gene)
            {
                var fragment = base.MakeFragment(gene);
                fragment[1] = gene.TargetAddress.AsGeneByte(0x20);
                fragment[2] = gene.DnaSampleSize.AsGeneByte(0.01f, 0.99f);
                fragment[3] = gene.RelationThreshold.AsGeneByte(0.01f, 1f);
                fragment[4] = gene.AttackForce.AsGeneByte(0.01f, 0.10f);
                fragment[5] = gene.AttackInterval.AsGeneByte(0.5f, 5f);
                return fragment;
            }
        }

        public string Name { get; } = "ATTACK";

        public float Cost { get; private set; }

        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public readonly byte TargetAddress;
        public readonly float DnaSampleSize;
        public readonly float RelationThreshold;
        public readonly float AttackForce;

        private float _timeSinceLastAttack = 10f;
        public float AttackInterval = 1f;

        private Dictionary<Organism, float> _relativismMap = new Dictionary<Organism, float>();

        public Attack(byte targetAddress, float dnaSampleSize, float relationThreshold, float attackForce, float attackInterval)
        {
            TargetAddress = targetAddress;
            DnaSampleSize = dnaSampleSize;
            RelationThreshold = relationThreshold;
            AttackForce = attackForce;
            AttackInterval = attackInterval;
        }

        private float GetRelativism(Organism self, Organism prey)
        {
            if (!_relativismMap.ContainsKey(prey))
                _relativismMap.Add(prey, self.DNA.RelatedPercent(prey.DNA));

            return _relativismMap[prey];
        }

        public void Apply(Organism self, List<IAmAGene> genes)
        {
            self.AddTexture(Game1.Virus, 1);
        }

        public int Update(Organism self, float deltaTime)
        {
            _timeSinceLastAttack += deltaTime;
            Cost = 0f;

            if (_timeSinceLastAttack < AttackInterval)
            {
                this.Log($"can't attack right now {_timeSinceLastAttack:0.0} < {AttackInterval:0.0}");
                return 1;
            }

            var prey = self.Remember<Organism>(TargetAddress);
            Cost += 1f;

            if (prey == null)
            {
                this.Log($"no target");
                return 1;
            }

            if (prey.Dead)
            {
                this.Log($"prey is dead");
                return 2;
            }

            var distance = (self.Position - prey.Position).Length();

            if (distance > (self.Radius + prey.Radius))
            {
                this.Log($"too far");
                return 1;
            }

            Cost += 1f;

            if (GetRelativism(self, prey) > RelationThreshold)
                this.Log($"looks like me, not attacking");
            else
            {
                _timeSinceLastAttack = 0f;
                var result = self.Attack(prey, AttackForce);
                this.Log($"attacking ({result.Damage})");

                if (prey.Dead)
                {
                    this.Log($"target died!");
                    return 2;
                }
                else
                    return 0;
            }

            return 1;
        }
        private string _string = null;
        public override string ToString()
        {
            if (_string == null)
                _string = $"{Name} [F: {AttackForce:0.00} I: {AttackInterval:0.00}s]";

            return _string;
        }
    }
}
