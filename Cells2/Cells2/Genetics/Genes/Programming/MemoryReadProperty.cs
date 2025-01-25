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
        public class Maker : GeneMaker
        {
            public Maker()
                : base(0xFE,0xFF, 4)
            {
            }

            public override IAmAGene Make(byte[] fragment)
            {
                if (fragment.Length < Size)
                    throw new GenomeTooShortException();

                return new MemoryReadProperty(
                    memoryLocation: fragment[1].AsByte(0xFF),
                    targetMemoryLocation: fragment[2].AsByte(0xFF),
                    property: fragment[3].AsByte(0xFF)
                    );
            }
        }
        private readonly GameObject.ReadableProperties _property;
        private readonly byte _memoryLocation;
        private readonly byte _targetMemoryLocation;
        public float Cost { get; private set; } = 1f;
        public string Name { get; } = "READ";
        public List<string> Log { get; } = new List<string>();
        public int LogIndentLevel { get; set; } = 0;

        private static List<GameObject.ReadableProperties> _readableProperties = Enum.GetValues(typeof(GameObject.ReadableProperties)).Cast<GameObject.ReadableProperties>().ToList();

        public MemoryReadProperty(byte memoryLocation, byte targetMemoryLocation, byte property)
        {
            _memoryLocation = memoryLocation;
            _targetMemoryLocation = targetMemoryLocation;

            var propertyIndex = property % _readableProperties.Count;
            _property = _readableProperties[propertyIndex];
        }

        public int Update(Organism self, float deltaTime)
        {
            this.Log(ToString(), 1);
            var target = self.Remember<GameObject>(_targetMemoryLocation);

            if (target == null)
            {
                this.Log($"no object at [{_targetMemoryLocation:X2}x0]");
                return 0;
            }

            switch (_property)
            {
                case GameObject.ReadableProperties.Position_X:
                    self.Remember(_memoryLocation, target.Position.X);
                    this.Log($"value: {target.Position.X}");
                    break;

                case GameObject.ReadableProperties.Position_Y:
                    self.Remember(_memoryLocation, target.Position.Y);
                    this.Log($"value: {target.Position.Y}");
                    break;

                case GameObject.ReadableProperties.Age:
                    self.Remember(_memoryLocation, target.Age);
                    this.Log($"value: {target.Age}");
                    break;

                case GameObject.ReadableProperties.Mass:
                    self.Remember(_memoryLocation, target.Mass);
                    this.Log($"value: {target.Mass}");
                    break;

                case GameObject.ReadableProperties.Alive:
                    self.Remember(_memoryLocation, target.Alive);
                    this.Log($"value: {target.Alive}");
                    break;

                case GameObject.ReadableProperties.Radius:
                    if (target is Organism) {
                        self.Remember(_memoryLocation, (target as Organism).Radius);
                        this.Log($"value: {(target as Organism).Radius}");
                    }
                    else
                        this.Log($"{target.GetType().Name} does not have this property");
                    break;

                case GameObject.ReadableProperties.Color_R:
                    if (target is Organism) {
                        self.Remember(_memoryLocation, (target as Organism).Color.R);
                        this.Log($"value: {(target as Organism).Color.R:X2}x0");
                    }
                    else
                        this.Log($"{target.GetType().Name} does not have this property");
                    break;
                case GameObject.ReadableProperties.Color_G:
                    if (target is Organism) {
                        self.Remember(_memoryLocation, (target as Organism).Color.G);
                        this.Log($"value: {(target as Organism).Color.G:X2}x0");
                    }
                    else
                        this.Log($"{target.GetType().Name} does not have this property");
                    break;
                case GameObject.ReadableProperties.Color_B:
                    if (target is Organism) {
                        self.Remember(_memoryLocation, (target as Organism).Color.B);
                        this.Log($"value: {(target as Organism).Color.B:X2}x0");
                    }
                    else
                        this.Log($"{target.GetType().Name} does not have this property");
                    break;
            }

            return 0;
        }

        private string _string;
        public override string ToString()
        {
            if (_string == null)
                _string = $"{Name} [{_memoryLocation:X2}x0] = [{_memoryLocation:X2}x0].{_property}";

            return _string;
        }
    }
}
