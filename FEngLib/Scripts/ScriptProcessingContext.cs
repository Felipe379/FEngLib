using System;
using System.Collections.Generic;

namespace FEngLib.Scripts;

public class TempTrack : ICloneable
{
    private Track m_track;

    public TempTrack(Track track)
    {
        Track = track;
    }

    public object Clone()
    {
        return new TempTrack(null)
        {
            m_track = this.m_track?.Clone() as Track,
            Offset = this.Offset
        };
    }

    public Track Track 
    {
        get => m_track;
        private set => m_track = value;
    }
    public uint? Offset { get; set; }
}

public class ScriptProcessingContext : ICloneable
{
    private Script m_script;

    public ScriptProcessingContext(Script script)
    {
        Script = script;
        Tracks = new List<TempTrack>();
    }

    public object Clone()
    {
        var result = new ScriptProcessingContext(null)
        {
            Script = this.m_script?.Clone() as Script,
            CurrentTrack = this.CurrentTrack?.Clone() as TempTrack
        };

        foreach (var @track in Tracks)
        {
            result.Tracks.Add(@track?.Clone() as TempTrack);
        }

        return result;
    }

    public Script Script
    {
        get => m_script;
        private set => m_script = value;
    }
    public TempTrack CurrentTrack { get; set; }
    public List<TempTrack> Tracks { get; }
}