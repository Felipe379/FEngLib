using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using FEngLib.Objects;
using FEngLib.Packages;
using FEngLib.Utils;

namespace FEngLib;

public abstract class FrontendObjectChunk
{
    protected IObject<ObjectData> FrontendObject { get; }

	protected FrontendObjectChunk(IObject<ObjectData> frontendObject)
    {
        FrontendObject = frontendObject;
    }


	public abstract IObject<ObjectData> Read(Package package, ObjectReaderState readerState, BinaryReader reader);
    public abstract FrontendChunkType GetChunkType();
}
