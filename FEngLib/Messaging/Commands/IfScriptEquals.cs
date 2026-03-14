namespace FEngLib.Messaging.Commands;

public class IfScriptEquals : ScriptCommand
{
    public IfScriptEquals(uint scriptHash) : base(scriptHash)
    {
    }

    public override uint GetId()
    {
        return (uint)ResponseHelpers.FEMessageResponseCommands.MR_IfScriptEquals;
    }

    public override string GetCommandName()
    {
        return "IfScriptEquals";
    }

    public override object Clone()
    {
        var result = new IfScriptEquals(default);

        result.InternalClone(this);

        return result;
    }
}