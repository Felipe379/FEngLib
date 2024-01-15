using System.IO;

namespace FEngLib.Objects.Tags;

public class StringBufferLabelHashTag : ObjectTag
{
    public StringBufferLabelHashTag(IObject<BaseObjectData> frontendObject) : base(frontendObject)
    {
    }

    public uint Hash { get; set; }

    public override void Read(BinaryReader br,
        ushort id,
        ushort length)
    {
        Hash = br.ReadUInt32();
    }
}