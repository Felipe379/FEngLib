using System.IO;
using FEngLib.Utils;

namespace FEngLib.Objects.Tags;

public class ObjectReferenceTag : ObjectTag
{
    public ObjectReferenceTag(IObject<BaseObjectData> frontendObject) : base(frontendObject)
    {
    }

    public uint Guid { get; set; }
    public uint NameHash { get; set; }
    public ObjectFlags Flags { get; set; }
    public int ResourceIndex { get; set; }


    public override object Clone()
    {
        var result = new ObjectReferenceTag(null);

        result.InternalClone(this);

        result.Guid = this.Guid;
        result.NameHash = this.NameHash;
        result.Flags = this.Flags;
        result.ResourceIndex = this.ResourceIndex;

        return result;
    }

    public override void Read(BinaryReader br,
        ushort id,
        ushort length)
    {
        Guid = br.ReadUInt32();
        NameHash = br.ReadUInt32();
        Flags = br.ReadEnum<ObjectFlags>();
        ResourceIndex = br.ReadInt32();
    }
}