using System.Collections.Generic;

namespace FEngLib.Scripts;

public class TempTrack
{
    public TempTrack(Track track)
    {
        Track = track;
    }

    public Track Track { get; }
    public uint? Offset { get; set; }
}

public class ScriptProcessingContext
{
    public ScriptProcessingContext(Script script)
    {
        Script = script;
        Tracks = new List<TempTrack>();
    }

    public Script Script { get; }
    public TempTrack CurrentTrack { get; set; }
    public List<TempTrack> Tracks { get; }
}