namespace FEngLib.Messaging.Commands;

public class EndIf : NonParameterizedCommand
{
    public override uint GetId()
    {
        return (uint)ResponseHelpers.FEMessageResponseCommands.MR_EndIf;
    }

    public override string GetCommandName()
    {
        return "EndIf";
    }

    public override string ToString()
    {
        return GetCommandName();
    }

    public override object Clone()
    {
        var result = new EndIf();

        result.InternalClone(this);

        return result;
    }
}