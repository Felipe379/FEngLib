using FEngLib;
using FEngLib.Scripts;
using FEngLib.Utils;
using FEngViewer.TypeConverters;
using JetBrains.Annotations;
using System.ComponentModel;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace FEngViewer.Wrappers;

public abstract class ScriptViewWrapper<TScript> where TScript : Script
{
	protected ScriptViewWrapper(TScript wrappedScript, HashResolver hashResolver)
	{
		WrappedScript = wrappedScript;
		HashResolver = hashResolver;
	}

	[NotNull] protected TScript WrappedScript { get; }
	protected HashResolver HashResolver { get; }

	[Category("Meta")]
	[RefreshProperties(RefreshProperties.Repaint)]
	[ReadOnly(true)]
	public string Name
	{
		get => WrappedScript.Name;
		set
		{
			WrappedScript.Id = value.BinHash();
			WrappedScript.Name = value;
			HashResolver.AddUserKey(value, WrappedScript.Id);
		}
	}

	[Category("Meta")]
	[TypeConverter(typeof(HexTypeConverter))]
	[RefreshProperties(RefreshProperties.Repaint)]
	[ReadOnly(true)]
	[DisplayName("Hash (hex)")]
	public uint HashHex
	{
		get => WrappedScript.Id;
		set
		{
			WrappedScript.Id = value;
			WrappedScript.Name = HashResolver.ResolveNameHash(WrappedScript.Name, value);
		}
	}

	[Category("Meta")]
	[ReadOnly(true)]
	[DisplayName("Hash (dec)")]
	[RefreshProperties(RefreshProperties.Repaint)]
	public uint HashDec
	{
		get => WrappedScript.Id;
		set
		{
			WrappedScript.Id = value;
			WrappedScript.Name = HashResolver.ResolveNameHash(WrappedScript.Name, value);
		}
	}

	[Category("Meta")]
	[RefreshProperties(RefreshProperties.Repaint)]
	public string ChainName
	{
		get => WrappedScript.ChainedId.HasValue ? HashResolver.ResolveNameHash(null, WrappedScript.ChainedId.Value) : null;
		set
		{
			if (value is not null)
			{
				var valueBinHash = value.BinHash();
				HashResolver.AddUserKey(value, valueBinHash);
				WrappedScript.ChainedId = valueBinHash;
			}
			else
			{
				WrappedScript.ChainedId = null;
			}
		}
	}

	[Category("Meta")]
	[TypeConverter(typeof(HexTypeConverter))]
	[RefreshProperties(RefreshProperties.Repaint)]
	[DisplayName("Chain (hash)")]
	public uint? ChainedIdHash
	{
		get => WrappedScript.ChainedId;
		set => WrappedScript.ChainedId = value;
	}

	[Category("Meta")]
	[RefreshProperties(RefreshProperties.Repaint)]
	[DisplayName("Chain (dec)")]
	public uint? ChainedIdDec
	{
		get => WrappedScript.ChainedId;
		set => WrappedScript.ChainedId = value;
	}
}

public class DefaultScriptViewWrapper : ScriptViewWrapper<Script>
{
	public DefaultScriptViewWrapper(Script wrappedScript, HashResolver hashResolver) : base(wrappedScript, hashResolver)
	{
	}
}
