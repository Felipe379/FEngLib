using System;

namespace FEngLib.Scripts;

public abstract class TrackNode : ICloneable
{
    public int Time { get; set; }

    protected void InternalClone(TrackNode @object)
    {
        this.Time = @object.Time;
    }

    public abstract object GetValue();
    public abstract object Clone();
}

public class TrackNode<TValue> : TrackNode where TValue : struct
{
    public TrackNode()
    {
        Val = default;
    }

    protected void InternalClone(TrackNode<TValue> @object)
    {
        base.InternalClone(@object);

        this.Val = @object.Val;
    }

    public override object Clone()
    {
        var result = new TrackNode<TValue>();

        result.InternalClone(this);

        return result;
    }

    public TValue Val { get; set; }

    public override object GetValue()
    {
        return Val;
    }
}