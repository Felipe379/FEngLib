namespace FEngLib.Objects;

public class Group : BaseObject
{
    public Group(CommonObjectData data) : base(data)
    {
    }

    public override object Clone()
    {
        var result = new Group(null);

        result.InternalClone(this);

        return result;
    }

    public override ObjectType GetObjectType()
    {
        return ObjectType.Group;
    }

    public override void InitializeData()
    {
        Data = new CommonObjectData();
    }
}