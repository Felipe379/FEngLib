namespace FEngLib.Objects;

public class SimpleImage : BaseObject
{
    public SimpleImage(CommonObjectData data) : base(data)
    {
    }

    public override object Clone()
    {
        var result = new SimpleImage(null);

        result.InternalClone(this);

        return result;
    }

    public override ObjectType GetObjectType()
    {
        return ObjectType.SimpleImage;
    }

    public override void InitializeData()
    {
        Data = new CommonObjectData();
    }
}