using System.IO;

namespace FEngLib.Objects.Tags;

public class ObjectDataTag : ObjectTag
{
    public ObjectDataTag(IObject<BaseObjectData> frontendObject) : base(frontendObject)
    {
    }

    public override object Clone()
    {
        var result = new ObjectDataTag(null);

        result.InternalClone(this);

        return result;
    }

    public override void Read(BinaryReader br,
        ushort id,
        ushort length)
    {
        FrontendObject.InitializeData();
        FrontendObject.Data.Read(br);
    }
}