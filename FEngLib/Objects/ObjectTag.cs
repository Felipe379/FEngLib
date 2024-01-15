using FEngLib.Tags;

namespace FEngLib.Objects;

public abstract class ObjectTag : Tag
{
    protected ObjectTag(IObject<BaseObjectData> frontendObject)
    {
        FrontendObject = frontendObject;
    }

    protected IObject<BaseObjectData> FrontendObject { get; }
}