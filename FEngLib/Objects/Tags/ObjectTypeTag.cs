using System.IO;
using FEngLib.Utils;

namespace FEngLib.Objects.Tags;

public class ObjectTypeTag : ObjectTag
{
    public ObjectTypeTag(IObject<ObjectData> frontendObject) : base(frontendObject)
    {
    }

	public override object Clone()
	{
		var result = new ObjectTypeTag(null);

		result.InternalClone(this);

		result.Type = this.Type;

		return result;
	}

	public ObjectType Type { get; set; }

    public override void Read(BinaryReader br,
        ushort id,
        ushort length)
    {
        Type = br.ReadEnum<ObjectType>();
    }
}
