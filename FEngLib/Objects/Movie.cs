namespace FEngLib.Objects;

public class Movie : BaseObject
{
    public Movie(CommonObjectData data) : base(data)
    {
    }

    public override object Clone()
    {
        var result = new Movie(null);

        result.InternalClone(this);

        return result;
    }

    public override ObjectType GetObjectType()
    {
        return ObjectType.Movie;
    }

    public override void InitializeData()
    {
        Data = new CommonObjectData();
    }
}