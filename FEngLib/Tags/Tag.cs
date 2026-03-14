using System;
using System.IO;

namespace FEngLib.Tags;

public abstract class Tag : ICloneable
{
    public abstract void Read(BinaryReader br, ushort id, ushort length);
    public abstract object Clone();
}