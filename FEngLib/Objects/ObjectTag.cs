using FEngLib.Tags;

namespace FEngLib.Objects;

public abstract class ObjectTag : Tag
{
    private IObject<BaseObjectData> m_frontendObject;

    protected ObjectTag(IObject<BaseObjectData> frontendObject)
    {
        FrontendObject = frontendObject;
    }

    protected void InternalClone(ObjectTag tag)
    {
        this.FrontendObject = tag.m_frontendObject?.Clone() as IObject<BaseObjectData>;
    }

    protected IObject<BaseObjectData> FrontendObject
    { 
        get => m_frontendObject;
        private set => m_frontendObject = value;
    }
}