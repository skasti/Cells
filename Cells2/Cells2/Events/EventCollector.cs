using System;
using System.Collections.Generic;

namespace Cells2.Events;

public static class EventCollector
{
    public static readonly Dictionary<EventType, List<Event>> Events = new Dictionary<EventType, List<Event>>();
    public static void Collect(Event @event)
    {
        if (!Events.ContainsKey(@event.Type))
            Events.Add(@event.Type, new List<Event>());

        Events[@event.Type].Add(@event);
    }
}
