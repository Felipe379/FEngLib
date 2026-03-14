using System.IO;
using FEngLib.Utils;

namespace FEngLib.Objects.Tags;

public class ObjectNameTag : ObjectTag
{
    public ObjectNameTag(IObject<BaseObjectData> frontendObject) : base(frontendObject)
    {
    }

    public string Name { get; set; }
    public uint NameHash { get; set; }


    public override object Clone()
    {
        var result = new ObjectNameTag(null);

        result.InternalClone(this);

        result.Name = this.Name;
        result.NameHash = this.NameHash;

        return result;
    }

    public override void Read(BinaryReader br,
        ushort id,
        ushort length)
    {
        Name = new string(br.ReadChars(length)).Trim('\x00');
        NameHash = Hashing.BinHash(Name.ToUpper());
    }
}