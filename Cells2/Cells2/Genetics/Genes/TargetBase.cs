using System;
using System.Collections.Generic;
using System.Linq;
using Cells;
using Cells.GameObjects;
using Cells.Genetics;
using Cells.Genetics.GeneTypes;

namespace Cells.Genetics.Genes;

public abstract class TargetBase<T> : ICanUpdate where T : GameObject
{
    public readonly float Range;
    public readonly byte TargetAddress;
    public readonly byte Capacity;
    public float Cost { get; private set; }
    public string Name => $"TARGET {typeof(T).Name.ToUpperInvariant()}";
    public List<string> Log { get; } = new List<string>();
    public int LogIndentLevel { get; set; } = 0;
    public float _lastTrackAge = 0f;
    public float TrackingInterval { get; private set; }

    protected abstract float maxTrackingInterval { get; }

    public TargetBase(float targetingRange, byte trackingCapacity, byte targetAddress, float trackingInterval)
    {
        Range = targetingRange;
        Capacity = trackingCapacity;
        TargetAddress = targetAddress;
        TrackingInterval = trackingInterval;
    }

    private List<T> lastResult = new List<T>();

    public int Update(Organism self, float deltaTime)
    {
        Cost = 0;
        var timeSinceLastTrack = self.Age - _lastTrackAge;
        if (timeSinceLastTrack < TrackingInterval)
        {
            this.Log($"charging: {timeSinceLastTrack:0.0} < {TrackingInterval:0.0}");
            return lastResult.Count == 0 ? 1 : 0;
        }

        _lastTrackAge = self.Age;
        var actualRange = self.Radius + Range;
        Cost = (actualRange / 800f) * (maxTrackingInterval - TrackingInterval);
        var inRange = ObjectManager.Instance.GetObjectsWithinRange<T>(self, actualRange)
            .OrderBy(self.Distance).ToList();
        lastResult = inRange;

        if (inRange.Count < 1)
        {
            this.Log($"nothing in range");
            return 1;
        }

        this.Log($"in range: {inRange.Count}");

        var address = TargetAddress;

        for (int i = 0; i < Capacity; i++)
        {
            if (i >= inRange.Count)
                break;

            self.Remember(address++, inRange[i]);
        }

        return 0;
    }

    private string _string = null;
    public override string ToString()
    {
        if (_string == null)
            _string = $"{Name} [R: {Range:0.} M:0x{TargetAddress:X2} C:{Capacity} I: {TrackingInterval:0.0}]";

        return _string;
    }
}
