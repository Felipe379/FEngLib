using System.IO;
using FEngLib.Tags;

namespace FEngLib.Messaging.Tags;

public class ResponseStringParamTag : Tag
{
    public string Param { get; set; }

    public override object Clone()
    {
        return new ResponseStringParamTag() { Param = this.Param };
    }

    public override void Read(BinaryReader br, ushort id,
        ushort length)
    {
        Param = new string(br.ReadChars(length)).Trim('\x00');
    }
}