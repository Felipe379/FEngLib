namespace FEngLib.Messaging.Commands;

public class RecallRecordedButton : ObjectCommand
{
    public RecallRecordedButton(uint objectGuid) : base(objectGuid)
    {
    }

    public override uint GetId()
    {
        return (uint)ResponseHelpers.FEMessageResponseCommands.MR_RecallRecordedButton;
    }

    public override string GetCommandName()
    {
        return "RecallRecordedButton";
    }

    public override object Clone()
    {
        var result = new RecallRecordedButton(default);

        result.InternalClone(this);

        return result;
    }
}