namespace FEngLib.Messaging.Commands;

public class SetActiveButton : ObjectCommand
{
    public SetActiveButton(uint objectGuid) : base(objectGuid)
    {
    }

    public override uint GetId()
    {
        return (uint)ResponseHelpers.FEMessageResponseCommands.MR_SetActiveButton;
    }

    public override string GetCommandName()
    {
        return "SetActiveButton";
    }

    public override object Clone()
    {
        var result = new SetActiveButton(default);

        result.InternalClone(this);

        return result;
    }
}