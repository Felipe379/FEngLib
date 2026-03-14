using System;
using System.Collections.Generic;

namespace FEngLib.Packages;

public class MessageTargets : ICloneable
{
    public MessageTargets()
    {
        Targets = new List<uint>();
    }

    public MessageTargets(uint msgId, List<uint> targets)
    {
        MsgId = msgId;
        Targets = targets;
    }

    public object Clone()
    {
        var result = new MessageTargets();

        result.MsgId = this.MsgId;

        result.Targets.AddRange(this.Targets);

        return result;
    }

    public uint MsgId { get; set; }
    public List<uint> Targets { get; }
}