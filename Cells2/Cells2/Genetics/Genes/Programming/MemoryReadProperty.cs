using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cells.GameObjects;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes.Programming
{
    public class MemoryReadProperty: ICanUpdate
    {
        public class Maker : GeneMaker<MemoryReadProperty>
        {
            private static List<GameObject.ReadableProperty> _readableProperties = Enum.GetValues(typeof(GameObject.ReadableProperty)).Cast<GameObject.ReadableProperty>().ToList();
            public Maker(byte markerFrom, byte markerTo)
                : base(markerFrom, markerTo, 4)
            {
            }

            public override MemoryReadProperty Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                var propertyIndex = fragment[3].AsByte(0xFF) % _readableProperties.Count;
                return new MemoryReadProperty(
                    address: fragment[1].AsByte(0xFF, 0x11),
                    targetAddress: fragment[2].AsByte(0x11),
                    property: _readableProperties[propertyIndex]
                    );
            }

            public override byte[] MakeFragment(MemoryReadProperty gene)
            {
                var fragment = base.MakeFragment(gene);
                fragment[1] = gene.Address;
                fragment[2] = gene.TargetAddress;
                fragment[3] = (byte)_readableProperties.IndexOf(gene.Property);
                return fragment;
            }
        }
        public readonly GameObject.ReadableProperty Property;
        public readonly byte Address;
        public readonly byte TargetAddress;
        public float Cost { get; private set; } = 1f;
        public string Name { get; } = "READ";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        public MemoryReadProperty(byte address, byte targetAddress, GameObject.ReadableProperty property)
        {
            Address = address;
            TargetAddress = targetAddress;
            Property = property;
        }

        public int Update(Organism self, float deltaTime)
        {
            var target = TargetAddress == 11 ? self : self.Remember<GameObject>(TargetAddress);

            if (target == null)
            {
                this.Log($"no object at [0x{TargetAddress:X2}]");
                return 0;
            }

            switch (Property)
            {
                case GameObject.ReadableProperty.Position_X:
                    self.Remember(Address, target.Position.X);
                    this.Log($"value: {target.Position.X}");
                    break;

                case GameObject.ReadableProperty.Position_Y:
                    self.Remember(Address, target.Position.Y);
                    this.Log($"value: {target.Position.Y}");
                    break;

                case GameObject.ReadableProperty.Relative_X:
                    self.Remember(Address, target.Position.X - self.Position.X);
                    this.Log($"value: {target.Position.X - self.Position.X}");
                    break;

                case GameObject.ReadableProperty.Relative_Y:
                    self.Remember(Address, target.Position.Y - self.Position.Y);
                    this.Log($"value: {target.Position.Y - self.Position.Y}");
                    break;

                case GameObject.ReadableProperty.Age:
                    self.Remember(Address, target.Age);
                    this.Log($"value: {target.Age}");
                    break;

                case GameObject.ReadableProperty.Mass:
                    self.Remember(Address, target.Mass);
                    this.Log($"value: {target.Mass}");
                    break;

                case GameObject.ReadableProperty.Alive:
                    self.Remember(Address, target.Alive);
                    this.Log($"value: {target.Alive}");
                    break;

                case GameObject.ReadableProperty.Radius:
                    if (target is Organism) {
                        self.Remember(Address, (target as Organism).Radius);
                        this.Log($"value: {(target as Organism).Radius}");
                    }
                    else
                        this.Log($"{target.GetType().Name} does not have this property");
                    break;

                case GameObject.ReadableProperty.Color_R:
                    if (target is Organism) {
                        self.Remember(Address, (target as Organism).Color.R);
                        this.Log($"value: 0x{(target as Organism).Color.R:X2}");
                    }
                    else
                        this.Log($"{target.GetType().Name} does not have this property");
                    break;
                case GameObject.ReadableProperty.Color_G:
                    if (target is Organism) {
                        self.Remember(Address, (target as Organism).Color.G);
                        this.Log($"value: 0x{(target as Organism).Color.G:X2}");
                    }
                    else
                        this.Log($"{target.GetType().Name} does not have this property");
                    break;
                case GameObject.ReadableProperty.Color_B:
                    if (target is Organism) {
                        self.Remember(Address, (target as Organism).Color.B);
                        this.Log($"value: 0x{(target as Organism).Color.B:X2}");
                    }
                    else
                        this.Log($"{target.GetType().Name} does not have this property");
                    break;
                case GameObject.ReadableProperty.Energy:
                    if (target == self)
                    {
                        self.Remember(Address, self.Energy);
                        this.Log($"value: {self.Energy:0.0}");
                    }
                    else
                        this.Log($"Energy is only readable by self");
                    break;
                case GameObject.ReadableProperty.RelatedPercent:
                    if (target is Organism) {
                        var relation = GetRelatedPercent(self, target as Organism);
                        self.Remember(Address, relation);
                        this.Log($"relation: {relation * 100f:0.0}%");
                    }
                    else
                        this.Log($"{target.GetType().Name} does not have this property");
                    break;
                case GameObject.ReadableProperty.Fitness:
                    if (target is Organism oTarget) {
                        self.Remember(Address, oTarget.Fitness);
                        this.Log($"fitness: {oTarget.Fitness:0.0}");
                    }
                    else
                        this.Log($"{target.GetType().Name} does not have this property");
                    break;
            }

            return 0;
        }

        private Dictionary<Organism, float> _relationCache = new Dictionary<Organism, float>();
        private float GetRelatedPercent(Organism self, Organism organism)
        {
            if (!_relationCache.ContainsKey(organism))
                _relationCache.Add(organism, self.DNA.RelatedPercent(organism.DNA));

            return _relationCache[organism];
        }

        private string _string;
        public override string ToString()
        {
            if (_string == null)
                _string = $"{Name} [0x{Address:X2}] = [0x{TargetAddress:X2}].{Property}";

            return _string;
        }
    }
}
