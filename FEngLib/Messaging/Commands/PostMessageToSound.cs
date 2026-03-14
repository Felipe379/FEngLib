using System;

namespace FEngLib.Messaging.Commands;

public class PostMessageToSound : MessageCommand, ITargetedCommand
{
    public PostMessageToSound(uint messageHash, uint target) : base(messageHash)
    {
        Target = target;
    }

    public override uint GetId()
    {
        return (uint)ResponseHelpers.FEMessageResponseCommands.MR_PostMessageToSound;
    }

    public override string GetCommandName()
    {
        return "PostMessageToSound";
    }

    public override string ToString()
    {
        return $"{GetCommandName()}(0x{Target:X}, 0x{MessageHash:X8})";
    }

    public uint Target
    {
        get;
        set;
    }

    public override object Clone()
    {
        var result = new PostMessageToSound(default, default);

        result.InternalClone(this);

        result.Target = this.Target;

        return result;
    }
}