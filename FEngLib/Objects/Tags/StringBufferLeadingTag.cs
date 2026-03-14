using System.IO;

namespace FEngLib.Objects.Tags;

public class StringBufferLeadingTag : ObjectTag
{
    public StringBufferLeadingTag(IObject<BaseObjectData> frontendObject) : base(frontendObject)
    {
    }

    public int Leading { get; set; }


    public override object Clone()
    {
        var result = new StringBufferLeadingTag(null);

        result.InternalClone(this);

        result.Leading = this.Leading;

        return result;
    }

    public override void Read(BinaryReader br,
        ushort id,
        ushort length)
    {
        Leading = br.ReadInt32();
    }
}