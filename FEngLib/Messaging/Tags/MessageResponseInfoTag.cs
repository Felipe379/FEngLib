using System.IO;
using FEngLib.Tags;

namespace FEngLib.Messaging.Tags;

public class MessageResponseInfoTag : Tag
{
    public uint Hash { get; set; }

    public override object Clone()
    {
        return new MessageResponseInfoTag() { Hash = this.Hash };
    }

    public override void Read(BinaryReader br,
        ushort id,
        ushort length)
    {
        Hash = br.ReadUInt32();
    }
}