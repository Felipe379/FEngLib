using System.IO;

namespace FEngLib.Objects.Tags;

public class ObjectHashTag : ObjectTag
{
    public ObjectHashTag(IObject<BaseObjectData> frontendObject) : base(frontendObject)
    {
    }


    public override object Clone()
    {
        var result = new ObjectHashTag(null);

        result.InternalClone(this);

        result.Hash = this.Hash;

        return result;
    }

    public uint Hash { get; set; }

    public override void Read(BinaryReader br,
        ushort id,
        ushort length)
    {
        Hash = br.ReadUInt32();
    }
}