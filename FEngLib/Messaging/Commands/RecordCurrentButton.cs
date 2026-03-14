namespace FEngLib.Messaging.Commands;

public class RecordCurrentButton : NonParameterizedCommand
{
    public override uint GetId()
    {
        return (uint)ResponseHelpers.FEMessageResponseCommands.MR_RecordCurrentButton;
    }

    public override string GetCommandName()
    {
        return "RecordCurrentButton";
    }

    public override string ToString()
    {
        return $"{GetCommandName()}()";
    }

    public override object Clone()
    {
        var result = new RecordCurrentButton();

        result.InternalClone(this);

        return result;
    }
}