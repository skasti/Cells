using System;
using System.Collections.Generic;
using Cells;
using Cells.GameObjects;
using Cells.Genetics;
using Cells.Genetics.Exceptions;
using Cells.Genetics.GeneTypes;
using Microsoft.Xna.Framework;

namespace Cells.Genetics.Genes.Programming;

public class AddForceMem : ICanUpdate
{
    public class Maker : GeneMaker<AddForceMem>
    {
        public Maker(byte markerFrom, byte? markerTo = null)
            : base(markerFrom, markerTo ?? markerFrom, 3)
        {
        }

        public override AddForceMem Make(byte[] fragment)
        {
            if (fragment.Length < Size)
                throw new GenomeTooShortException();

            return new AddForceMem(
                xAddress: fragment[1],
                yAddress: fragment[2]
            );
        }

        public override byte[] MakeFragment(AddForceMem gene)
        {
            var fragment = base.MakeFragment(gene);
            fragment[1] = gene.XAddress;
            fragment[2] = gene.YAddress;
            return fragment;
        }
    }

    public readonly byte XAddress;
    public readonly byte YAddress;

    public float Cost { get; private set; } = 0.5f;

    public string Name { get; } = "ADD FORCE";
    public List<string> Log { get; } = new List<string>();
    public int LogIndentLevel { get; set; } = 0;


    public AddForceMem(byte xAddress, byte yAddress)
    {
        XAddress = xAddress;
        YAddress = yAddress;
    }

    public int Update(Organism self, float deltaTime)
    {
        var xForce = self.Remember<float>(XAddress);
        var yForce = self.Remember<float>(YAddress);
        var force = new Vector2(xForce, yForce);
        this.Log($"adding force: {force.ToShortString()}");
        self.Force += force;
        return 0;
    }

    private string _string;
    public override string ToString()
    {
        if (_string == null)
            _string = $"{Name} [0x{XAddress:X2}, 0x{YAddress:X2}]";

        return _string;
    }
}
