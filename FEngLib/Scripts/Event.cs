using System;

namespace FEngLib.Scripts;

public class Event : ICloneable
{
    public object Clone()
    {
        return new Event()
        {
            EventId = this.EventId,
            Target = this.Target,
            Time = this.Time
        };
    }

    public uint EventId { get; set; }
    public uint Target { get; set; }
    public uint Time { get; set; }
}