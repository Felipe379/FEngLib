using System.IO;
using FEngLib.Scripts;
using FEngLib.Structures;
using FEngLib.Utils;

namespace FEngLib.Objects;

public sealed class ColoredImageData : BaseImageData
{
    public Color4 TopLeft { get; set; }
    public Color4 TopRight { get; set; }
    public Color4 BottomRight { get; set; }
    public Color4 BottomLeft { get; set; }

    public override void Read(BinaryReader br)
    {
        base.Read(br);
        TopLeft = br.ReadColor();
        TopRight = br.ReadColor();
        BottomRight = br.ReadColor();
        BottomLeft = br.ReadColor();
    }

    public override void Write(BinaryWriter bw)
    {
        base.Write(bw);
        bw.Write(TopLeft);
        bw.Write(TopRight);
        bw.Write(BottomRight);
        bw.Write(BottomLeft);
    }
}

public sealed class ColoredImageScript : BaseImageScript, IScript<ColoredImageScript>
{
    public Track<TTrackValue> GetTrack<TTrackValue>(TrackId<ColoredImageScript, TTrackValue> id) where TTrackValue : struct
    {
        return GetTrackInternal<TTrackValue>(id);
    }

    public void SetTrack<TTrackValue>(TrackId<ColoredImageScript, TTrackValue> id, Track<TTrackValue> track) where TTrackValue : struct
    {
        SetTrackInternal(id, track);
    }

    public void RemoveTrack<TTrackValue>(TrackId<ColoredImageScript, TTrackValue> id) where TTrackValue : struct
    {
        RemoveTrackInternal(id);
    }
}

public sealed class ColoredImage : BaseImage<ColoredImageData, ColoredImageScript>
{
    public ColoredImage(ColoredImageData data) : base(data)
    {
    }

    public override ObjectType GetObjectType()
    {
        return ObjectType.ColoredImage;
    }

    public override void InitializeData()
    {
        Data = new ColoredImageData();
    }
}