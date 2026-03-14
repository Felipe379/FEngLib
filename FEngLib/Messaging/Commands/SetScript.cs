namespace FEngLib.Messaging.Commands;

public class SetScript : ScriptCommand
{
    public SetScript(uint scriptHash) : base(scriptHash)
    {
    }

    public override uint GetId()
    {
        return (uint)ResponseHelpers.FEMessageResponseCommands.MR_SetScript;
    }

    public override string GetCommandName()
    {
        return "SetScript";
    }

    public override string ToString()
    {
        return $"{GetCommandName()}(0x{ScriptHash:X8})";
    }


    public override object Clone()
    {
        var result = new SetScript(default);

        result.InternalClone(this);

        return result;
    }
}