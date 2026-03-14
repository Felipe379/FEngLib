namespace FEngLib.Messaging.Commands;

public class PostMessageToGame : MessageCommand
{
    public PostMessageToGame(uint messageHash) : base(messageHash)
    {
    }

    public override uint GetId()
    {
        return (uint)ResponseHelpers.FEMessageResponseCommands.MR_PostMessageToGame;
    }

    public override string GetCommandName()
    {
        return "PostMessageToGame";
    }

    public override string ToString()
    {
        return $"{GetCommandName()}(0x{MessageHash:X8})";
    }

    public override object Clone()
    {
        var result = new PostMessageToGame(default);

        result.InternalClone(this);

        return result;
    }
}