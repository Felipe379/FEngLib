using System;
using System.Collections.Generic;

namespace FEngLib.Scripts;

//public abstract class BaseScriptTracks
//{
//    public ColorTrack Color { get; set; }
//    public Vector3Track Pivot { get; set; }
//    public Vector3Track Position { get; set; }
//    public QuaternionTrack Rotation { get; set; }
//    public Vector3Track Size { get; set; }
//}

//public interface IScript
//{
//    //Track GetTrack(TrackId id);
//    IEnumerable<KeyValuePair<TrackId, Track>> EnumerateTracks();
//}

public interface IScript<TSelf>/* : IScript*/ where TSelf : Script, IScript<TSelf>
{
    Track<TTrackValue> GetTrack<TTrackValue>(TrackId<TSelf, TTrackValue> id) where TTrackValue : struct;
    void SetTrack<TTrackValue>(TrackId<TSelf, TTrackValue> id, Track<TTrackValue> track) where TTrackValue : struct;
    void RemoveTrack<TTrackValue>(TrackId<TSelf, TTrackValue> id) where TTrackValue : struct;
}

public abstract class Script : IScript<Script>
{
    private Dictionary<TrackId, Track> _tracks;

    public IReadOnlyDictionary<TrackId, Track> Tracks => _tracks;

    protected Script()
    {
        _tracks = new Dictionary<TrackId, Track>();
        // Tracks = new List<Track>();
        Events = new List<Event>();
    }

    public string Name { get; set; }
    public uint Id { get; set; }
    public uint? ChainedId { get; set; }
    public uint Length { get; set; }
    public uint Flags { get; set; }

    // public List<Track> Tracks { get; }
    public List<Event> Events { get; }

    //public abstract BaseScriptTracks GetTracks();
    public Track<TTrackValue> GetTrack<TTrackValue>(TrackId<Script, TTrackValue> id) where TTrackValue : struct
    {
        return GetTrackInternal<TTrackValue>(id);
    }

    public void SetTrack<TTrackValue>(TrackId<Script, TTrackValue> id, Track<TTrackValue> track) where TTrackValue : struct
    {
        SetTrackInternal(id, track);
    }

    public void RemoveTrack<TTrackValue>(TrackId<Script, TTrackValue> id) where TTrackValue : struct
    {
        RemoveTrackInternal(id);
    }

    protected Track<TTrackValue> GetTrackInternal<TTrackValue>(TrackId id) where TTrackValue : struct
    {
        var track = GetTrack(id);
        return track switch
        {
            null => null,
            Track<TTrackValue> castedTrack => castedTrack,
            _ => throw new Exception($"Track corruption detected... expected {typeof(Track<TTrackValue>)} but got {track.GetType()}")
        };
    }

    protected void SetTrackInternal<TTrackValue>(TrackId<TTrackValue> id, Track<TTrackValue> track) where TTrackValue : struct
    {
        if (_tracks.TryGetValue(id, out var existingTrack)
            && existingTrack is not Track<TTrackValue>)
        {
            throw new Exception(
                $"Inadvertent track corruption detected: existing track is of type {existingTrack.GetType()}, new track is of type {track.GetType()}");
        }

        _tracks[id] = track;
    }

    protected void RemoveTrackInternal(TrackId id)
    {
        _tracks.Remove(id);
    }

    protected Track GetTrack(TrackId id)
    {
        return _tracks.GetValueOrDefault(id);
    }
}

//public abstract class Script<TTracks> : Script where TTracks : BaseScriptTracks, new()
//{
//    protected Script()
//    {
//        Tracks = new TTracks();
//    }

//    public TTracks Tracks { get; protected init; }

//    public override BaseScriptTracks GetTracks()
//    {
//        return Tracks;
//    }
//}