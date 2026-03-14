using System;

namespace FEngLib.Packages;

public enum ResourceType
{
    None = 0x0,
    Image = 0x1,
    Font = 0x2,
    Model = 0x3,
    Movie = 0x4,
    Effect = 0x5,
    AnimatedImage = 0x6,
    MultiImage = 0x7,
}

public class ResourceRequest : ICloneable
{
    public uint ID { get; set; }
    public ResourceType Type { get; set; }
    public string Name { get; set; }

    public object Clone()
    {
        return new ResourceRequest()
        {
            ID = this.ID,
            Type = this.Type,
            Name = this.Name
        };
    }

    public override bool Equals(object obj)
    {
        if (object.ReferenceEquals(this, obj)) return true;

        if (obj is ResourceRequest request)
        {
            return this.ID == request.ID && this.Type == request.Type && this.Name == request.Name;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return (this.ID, this.Type, this.Name).GetHashCode();
    }
}