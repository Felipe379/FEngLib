using System.IO;
using FEngLib.Objects;
using FEngLib.Packages;

namespace FEngLib;

public abstract class FrontendObjectChunk
{
    protected IObject<BaseObjectData> FrontendObject { get; }

    protected FrontendObjectChunk(IObject<BaseObjectData> frontendObject)
    {
        FrontendObject = frontendObject;
    }

    public abstract IObject<BaseObjectData> Read(Package package, ObjectReaderState readerState, BinaryReader reader);
    public abstract FrontendChunkType GetChunkType();
}