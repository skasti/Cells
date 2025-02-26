using System;
using System.Reflection.Metadata.Ecma335;

namespace Cells2.Events;

public class EventType
{
    public string Name { get; }

    public EventType(string name)
    {
        Name = name;
    }

    public override bool Equals(object obj)
    {
        if (obj is EventType other)
            return Name == other.Name;

        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}
