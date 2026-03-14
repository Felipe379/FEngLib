using System.IO;
using FEngLib.Tags;

namespace FEngLib.Messaging.Tags;

public class ResponseTargetTag : Tag
{
    public uint Target { get; set; }

    public override object Clone()
    {
        return new ResponseTargetTag() { Target = this.Target };
    }

    public override void Read(BinaryReader br, ushort id,
        ushort length)
    {
        Target = br.ReadUInt32();
    }
}