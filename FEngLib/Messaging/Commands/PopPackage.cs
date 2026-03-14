namespace FEngLib.Messaging.Commands;

public class PopPackage : NonParameterizedCommand
{
    public override uint GetId()
    {
        return (uint)ResponseHelpers.FEMessageResponseCommands.MR_PopPackage;
    }

    public override string GetCommandName()
    {
        return "PopPackage";
    }

    public override object Clone()
    {
        var result = new PopPackage();

        result.InternalClone(this);

        return result;
    }
}