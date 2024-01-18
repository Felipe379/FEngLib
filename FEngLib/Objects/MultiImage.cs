using System.IO;
using System.Numerics;
using FEngLib.Scripts;
using FEngLib.Utils;

namespace FEngLib.Objects;

public sealed class MultiImageData : BaseImageData
{
    public Vector2 TopLeft1 { get; set; }
    public Vector2 TopLeft2 { get; set; }
    public Vector2 TopLeft3 { get; set; }
    public Vector2 BottomRight1 { get; set; }
    public Vector2 BottomRight2 { get; set; }
    public Vector2 BottomRight3 { get; set; }
    public Vector3 PivotRotation { get; set; }

    public override void Read(BinaryReader br)
    {
        base.Read(br);

        TopLeft1 = br.ReadVector2();
        TopLeft2 = br.ReadVector2();
        TopLeft3 = br.ReadVector2();
        BottomRight1 = br.ReadVector2();
        BottomRight2 = br.ReadVector2();
        BottomRight3 = br.ReadVector2();
        PivotRotation = br.ReadVector3();
    }

    public override void Write(BinaryWriter bw)
    {
        base.Write(bw);

        bw.Write(TopLeft1);
        bw.Write(TopLeft2);
        bw.Write(TopLeft3);
        bw.Write(BottomRight1);
        bw.Write(BottomRight2);
        bw.Write(BottomRight3);
        bw.Write(PivotRotation);
    }
}

public sealed class MultiImageScript : BaseImageScript, IScript<MultiImageScript>
{
    public Track<TTrackValue> GetTrack<TTrackValue>(TrackId<MultiImageScript, TTrackValue> id) where TTrackValue : struct
    {
        return GetTrackInternal<TTrackValue>(id);
    }

    public void SetTrack<TTrackValue>(TrackId<MultiImageScript, TTrackValue> id, Track<TTrackValue> track) where TTrackValue : struct
    {
        SetTrackInternal(id, track);
    }

    public void RemoveTrack<TTrackValue>(TrackId<MultiImageScript, TTrackValue> id) where TTrackValue : struct
    {
        RemoveTrackInternal(id);
    }
}

public sealed class MultiImage : BaseImage<MultiImageData, MultiImageScript>
{
    public MultiImage(MultiImageData data) : base(data)
    {
    }

    public uint Texture1 { get; set; }
    public uint TextureFlags1 { get; set; }
    public uint Texture2 { get; set; }
    public uint TextureFlags2 { get; set; }
    public uint Texture3 { get; set; }
    public uint TextureFlags3 { get; set; }

    public override ObjectType GetObjectType()
    {
        return ObjectType.MultiImage;
    }

    public override void InitializeData()
    {
        Data = new MultiImageData();
    }
}