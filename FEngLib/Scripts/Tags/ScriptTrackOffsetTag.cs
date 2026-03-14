using System.IO;
using FEngLib.Objects;

namespace FEngLib.Scripts.Tags;

public class ScriptTrackOffsetTag : ScriptTag
{
    public ScriptTrackOffsetTag(IObject<BaseObjectData> frontendObject, ScriptProcessingContext scriptProcessingContext) :
        base(frontendObject,
            scriptProcessingContext)
    {
    }

    public uint Offset { get; set; }

    public override object Clone()
    {
        var result = new ScriptTrackOffsetTag(null, null);

        result.InternalClone(this);

        result.Offset = this.Offset;

        return result;
    }

    public override void Read(BinaryReader br,
        ushort id,
        ushort length)
    {
        Offset = br.ReadUInt32();
    }
}