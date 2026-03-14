using System.IO;
using FEngLib.Objects;

namespace FEngLib.Scripts.Tags;

public class ScriptChainTag : ScriptTag
{
    public ScriptChainTag(IObject<BaseObjectData> frontendObject, ScriptProcessingContext scriptProcessingContext) : base(
        frontendObject, scriptProcessingContext)
    {
    }

    public uint Id { get; set; }

    public override object Clone()
    {
        var result = new ScriptChainTag(null, null);

        result.InternalClone(this);

        result.Id = this.Id;

        return result;
    }

    public override void Read(BinaryReader br,
        ushort id,
        ushort length)
    {
        Id = br.ReadUInt32();
    }
}