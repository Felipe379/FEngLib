using System.IO;
using System.Numerics;
using FEngLib.Scripts;
using FEngLib.Utils;

namespace FEngLib.Objects;

public class ImageData : BaseObjectData
{
    public Vector2 UpperLeft { get; set; }
    public Vector2 LowerRight { get; set; }

    public override void Read(BinaryReader br)
    {
        base.Read(br);

        UpperLeft = br.ReadVector2();
        LowerRight = br.ReadVector2();
    }

    public override void Write(BinaryWriter bw)
    {
        base.Write(bw);

        bw.Write(UpperLeft);
        bw.Write(LowerRight);
    }
}

//public class ImageScriptTracks : BaseScriptTracks
//{
//    public Vector2Track UpperLeft { get; set; }
//    public Vector2Track LowerRight { get; set; }
//}

public class ImageScript : Script, IScript<ImageScript>
{
    public Track<TTrackValue> GetTrack<TTrackValue>(TrackId<ImageScript, TTrackValue> id) where TTrackValue : struct
    {
        return GetTrackInternal<TTrackValue>(id);
    }

    public void SetTrack<TTrackValue>(TrackId<ImageScript, TTrackValue> id, Track<TTrackValue> track) where TTrackValue : struct
    {
        SetTrackInternal(id, track);
    }

    public void RemoveTrack<TTrackValue>(TrackId<ImageScript, TTrackValue> id) where TTrackValue : struct
    {
        RemoveTrackInternal(id);
    }
}

public class Image : Image<ImageData, ImageScript>
{
    public Image(ImageData data) : base(data)
    {
    }

    public override ObjectType GetObjectType()
    {
        return ObjectType.Image;
    }
}

public interface IImage<out TData> : IObject<TData> where TData : ImageData
{
    uint ImageFlags { get; set; }
}

public abstract class Image<TData, TScript> : BaseObject<TData, TScript>, IImage<TData>
    where TData : ImageData, new() where TScript : Script, new()
{
    protected Image(TData data) : base(data)
    {
        Data = data;
    }

    public override void InitializeData()
    {
        Data = new TData();
    }

    public uint ImageFlags { get; set; }
}