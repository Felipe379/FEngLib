using System.IO;

namespace FEngLib.Objects.Tags;

public class ImageInfoTag : ObjectTag
{
    public ImageInfoTag(IObject<ObjectData> frontendObject) : base(frontendObject)
    {
    }

	public override object Clone()
	{
		var result = new ImageInfoTag(null);

		result.InternalClone(this);

		result.ImageFlags = this.ImageFlags;

		return result;
	}

	public uint ImageFlags { get; set; }

    public override void Read(BinaryReader br,
        ushort id,
        ushort length)
    {
        ImageFlags = br.ReadUInt32();
    }
}
