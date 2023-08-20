using System.IO;

namespace FEngLib.Objects.Tags;

public class StringBufferLabelTag : ObjectTag
{
    public StringBufferLabelTag(IObject<ObjectData> frontendObject) : base(frontendObject)
    {
    }

	public override object Clone()
	{
		var result = new StringBufferLabelTag(null);

		result.InternalClone(this);

		result.Label = this.Label;

		return result;
	}

	public string Label { get; set; }

    public override void Read(BinaryReader br,
        ushort id,
        ushort length)
    {
        Label = new string(br.ReadChars(length)).Trim('\x00');
    }
}
