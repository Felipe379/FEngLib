using FEngLib;
using FEngLib.Packages;
using JetBrains.Annotations;
using System.ComponentModel;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace FEngViewer.Wrappers;

public abstract class ResourceRequestWrapper<TResourceRequest> where TResourceRequest : ResourceRequest
{
	protected ResourceRequestWrapper(TResourceRequest wrappedScript)
	{
		WrappedResourceRequest = wrappedScript;
	}

	[NotNull] protected TResourceRequest WrappedResourceRequest { get; }
	protected HashResolver HashResolver { get; }

	[Category("Meta")]
	[ReadOnly(true)]
	public uint Id
	{
		get => WrappedResourceRequest.ID;
		set => WrappedResourceRequest.ID = value;
	}

}

public class DefaultResourceRequestWrapper : ResourceRequestWrapper<ResourceRequest>
{
	public DefaultResourceRequestWrapper(ResourceRequest wrappedScript) : base(wrappedScript)
	{
	}
}
