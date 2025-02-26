using System;
using System.Collections.Generic;

namespace Cells2.Events;

public class Event
{
    public EventType Type { get; }
    public Dictionary<string, object> Properties { get; }

    public Event(EventType type, Dictionary<string, object> properties)
    {
        Type = type;
        Properties = properties;
        EventCollector.Collect(this);
    }
}
