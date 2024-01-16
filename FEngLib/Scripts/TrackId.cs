using System;
using System.Numerics;
using FEngLib.Objects;
using FEngLib.Structures;

namespace FEngLib.Scripts;

public abstract class TrackId : IComparable<TrackId>
{
    internal uint Id { get; }

    public string Name { get; }

    /// <summary>
    /// Internal constructor to prevent external tomfoolery
    /// </summary>
    internal TrackId(uint id, string name)
    {
        Id = id;
        Name = name;
    }

    public int CompareTo(TrackId other)
    {
        return Id.CompareTo(other.Id);
    }
}

public abstract class TrackId<TTrackValue> : TrackId where TTrackValue : struct
{
    /// <summary>
    /// Internal constructor to prevent external tomfoolery
    /// </summary>
    internal TrackId(uint id, string name) : base(id, name) {}
}

public sealed class TrackId<TScript, TTrackValue> : TrackId<TTrackValue> where TScript : Script where TTrackValue : struct
{
    /// <summary>
    /// Internal constructor to prevent external tomfoolery
    /// </summary>
    internal TrackId(uint id, string name) : base(id, name) { }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is TrackId<TScript, TTrackValue> other && Id == other.Id;
    }

    public override int GetHashCode()
    {
        return (int)Id;
    }
}

public static class BaseScriptTrackIds
{
    public static readonly TrackId<Script, Color4> Color = new(0, nameof(Color));
    public static readonly TrackId<Script, Vector3> Pivot = new(4, nameof(Pivot));
    public static readonly TrackId<Script, Vector3> Position = new(7, nameof(Position));
    public static readonly TrackId<Script, Quaternion> Rotation = new(10, nameof(Rotation));
    public static readonly TrackId<Script, Vector3> Size = new(14, nameof(Size));
}

public static class ImageScriptTrackIds
{
    public static readonly TrackId<ImageScript, Vector2> UpperLeft = new(17, nameof(UpperLeft));
    public static readonly TrackId<ImageScript, Vector2> LowerRight = new(19, nameof(LowerRight));
}

public static class MultiImageScriptTrackIds
{
    public static readonly TrackId<MultiImageScript, Vector2> TopLeft1 = new(21, nameof(TopLeft1));
    public static readonly TrackId<MultiImageScript, Vector2> TopLeft2 = new(23, nameof(TopLeft2));
    public static readonly TrackId<MultiImageScript, Vector2> TopLeft3 = new(25, nameof(TopLeft3));
    public static readonly TrackId<MultiImageScript, Vector2> BottomRight1 = new(27, nameof(BottomRight1));
    public static readonly TrackId<MultiImageScript, Vector2> BottomRight2 = new(29, nameof(BottomRight2));
    public static readonly TrackId<MultiImageScript, Vector2> BottomRight3 = new(31, nameof(BottomRight3));
    public static readonly TrackId<MultiImageScript, Vector3> PivotRotation = new(33, nameof(PivotRotation));
}

public static class ColoredImageScriptTrackIds
{
    public static readonly TrackId<ColoredImageScript, Color4> TopLeft = new(21, nameof(TopLeft));
    public static readonly TrackId<ColoredImageScript, Color4> TopRight = new(25, nameof(TopRight));
    public static readonly TrackId<ColoredImageScript, Color4> BottomRight = new(29, nameof(BottomRight));
    public static readonly TrackId<ColoredImageScript, Color4> BottomLeft = new(33, nameof(BottomLeft));
}