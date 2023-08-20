using System;
using System.Collections.Generic;
using FEngLib.Messaging;
using FEngLib.Objects;

namespace FEngLib.Packages;

/// <summary>
///     Stores frontend objects, resource names, etc.
/// </summary>
public class Package : IHaveMessageResponses
{
    public Package()
    {
        ResourceRequests = new List<ResourceRequest>();
        Objects = new List<IObject<ObjectData>>();
        MessageResponses = new List<MessageResponse>();
        MessageTargetLists = new List<MessageTargets>();
        MessageDefinitions = new List<MessageDefinition>();
    }

    public string Name { get; set; }
    public string Filename { get; set; }
    public List<ResourceRequest> ResourceRequests { get; }
    public List<IObject<ObjectData>> Objects { get; }
    public List<MessageTargets> MessageTargetLists { get; }
    public List<MessageDefinition> MessageDefinitions { get; }
    public List<MessageResponse> MessageResponses { get; }

	public object Clone()
	{
		var result = new Package();

		result.Name = this.Name;
		result.Filename = this.Filename;

		foreach (var resource in this.ResourceRequests)
		{
			result.ResourceRequests.Add(resource?.Clone() as ResourceRequest);
		}

		foreach (var @object in this.Objects)
		{
			result.Objects.Add(@object?.Clone() as IObject<ObjectData>);
		}

		foreach (var target in this.MessageTargetLists)
		{
			result.MessageTargetLists.Add(target?.Clone() as MessageTargets);
		}

		foreach (var definition in this.MessageDefinitions)
		{
			result.MessageDefinitions.Add(definition?.Clone() as MessageDefinition);
		}

		foreach (var response in this.MessageResponses)
		{
			result.MessageResponses.Add(response?.Clone() as MessageResponse);
		}

		return result;
	}

    public IObject<ObjectData> FindObjectByGuid(uint guid)
    {
        return Objects.Find(o => o.Guid == guid) ??
               throw new KeyNotFoundException($"Could not find object with GUID: 0x{guid:X8}");
    }

    public IObject<ObjectData> FindObjectByHash(uint hash)
    {
        return Objects.Find(o => o.NameHash == hash) ??
               throw new KeyNotFoundException($"Could not find object with hash: 0x{hash:X8}");
    }

    public class MessageDefinition : ICloneable
    {
        public string Name { get; set; }
        public string Category { get; set; }

		public object Clone()
		{
			return new MessageDefinition
			{
				Name = this.Name,
				Category = this.Category
			};
		}
    }
}
