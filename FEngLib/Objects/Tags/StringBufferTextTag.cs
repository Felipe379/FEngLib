using System.IO;
using System.Text;

namespace FEngLib.Objects.Tags;

public class StringBufferTextTag : ObjectTag
{
    public StringBufferTextTag(IObject<BaseObjectData> frontendObject) : base(frontendObject)
    {
    }

    public string Value { get; set; }


    public override object Clone()
    {
        var result = new StringBufferTextTag(null);

        result.InternalClone(this);

        result.Value = this.Value;

        return result;
    }

    public override void Read(BinaryReader br,
        ushort id,
        ushort length)
    {
        Value = Encoding.Unicode.GetString(br.ReadBytes(length)).Trim('\0');
    }
}