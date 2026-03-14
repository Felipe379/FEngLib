using System.IO;

namespace FEngLib.Objects.Tags;

public class StringBufferMaxWidthTag : ObjectTag
{
    public StringBufferMaxWidthTag(IObject<BaseObjectData> frontendObject) : base(frontendObject)
    {
    }

    public uint MaxWidth { get; set; }

    public override object Clone()
    {
        var result = new StringBufferMaxWidthTag(null);

        result.InternalClone(this);

        result.MaxWidth = this.MaxWidth;

        return result;
    }

    public override void Read(BinaryReader br,
        ushort id,
        ushort length)
    {
        MaxWidth = br.ReadUInt32();
    }
}