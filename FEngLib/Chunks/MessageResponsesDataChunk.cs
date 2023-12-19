using System.IO;
using FEngLib.Messaging;
using FEngLib.Objects;
using FEngLib.Packages;
using FEngLib.Tags;

namespace FEngLib.Chunks;

public class MessageResponsesDataChunk : FrontendObjectChunk
{
    public MessageResponsesDataChunk(IObject<ObjectData> frontendObject, HashResolver hashResolver) : base(frontendObject, hashResolver)
    {
    }

	private string _logReference = "Message";

	public override IObject<ObjectData> Read(Package package, ObjectReaderState readerState, BinaryReader reader)
    {
        var tagProcessor = new MessageResponseTagProcessor<IObject<ObjectData>>();
        TagStream tagStream = new MessageTagStream(reader,
            readerState.CurrentChunkBlock.Size);

        while (tagStream.HasTag())
        {
            var tag = tagStream.NextTag();
            tagProcessor.ProcessTag(FrontendObject, tag);
			FrontendObject.Name = HashResolver.ResolveNameHash(FrontendObject.Name, FrontendObject.NameHash, _logReference);
        }

		return FrontendObject;
    }

    public override FrontendChunkType GetChunkType()
    {
        return FrontendChunkType.MessageResponses;
    }
}
