using System.IO;
using FEngLib.Objects;

namespace FEngLib.Scripts.Tags;

public class ScriptHeaderTag : ScriptTag
{
    public ScriptHeaderTag(IObject<BaseObjectData> frontendObject, ScriptProcessingContext scriptProcessingContext) : base(
        frontendObject, scriptProcessingContext)
    {
    }

    public uint Id { get; set; }
    public uint Length { get; set; }
    public uint Flags { get; set; }
    public uint TrackCount { get; set; }

    public override object Clone()
    {
        var result = new ScriptHeaderTag(null, null);

        result.InternalClone(this);

        result.Id = this.Id;
        result.Length = this.Length;
        result.Flags = this.Flags;
        result.TrackCount = this.TrackCount;

        return result;
    }

    public override void Read(BinaryReader br,
        ushort id,
        ushort length)
    {
        Id = br.ReadUInt32();
        Length = br.ReadUInt32();
        Flags = br.ReadUInt32();
        TrackCount = br.ReadUInt32();
    }
}