using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using FEngLib.Messaging;
using FEngLib.Packages;
using FEngLib.Scripts;
using FEngLib.Structures;
using FEngLib.Utils;

namespace FEngLib.Objects;

/// <summary>
/// This represents all common data found in an object's 'ObjD' chunk.
/// For objects where the ObjD chunk contains extra data (e.g. images),
/// inherit from this class to represent the extra values in that chunk. 
/// </summary>
public abstract class BaseObjectData : IBinaryAccess
{
    public Color4 Color { get; set; }
    public Vector3 Pivot { get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public Vector3 Size { get; set; }

    public virtual void Read(BinaryReader br)
    {
        Color = br.ReadColor();
        Pivot = br.ReadVector3();
        Position = br.ReadVector3();
        Rotation = br.ReadQuaternion();
        Size = br.ReadVector3();
    }

    public virtual void Write(BinaryWriter bw)
    {
        bw.Write(Color);
        bw.Write(Pivot);
        bw.Write(Position);
        bw.Write(Rotation);
        bw.Write(Size);
    }
}

/// <summary>
/// Common interface for all object types and their properties.
/// Extend this if your object type has extra ObjD attributes,
/// and if there are other object types that inherit from your object type (to ensure type safety).
/// </summary>
/// <typeparam name="TData">
/// A type inheriting from ObjectData,
/// representing the contents of an ObjD chunk for this object.
/// </typeparam>
public interface IObject<out TData> : IScriptedObject, IHaveMessageResponses where TData : BaseObjectData
{
    TData Data { get; }
    ObjectFlags Flags { get; set; }
    ResourceRequest ResourceRequest { get; set; }
    string Name { get; set; }
    uint NameHash { get; set; }
    uint Guid { get; set; }
    IObject<BaseObjectData> Parent { get; set; }
    ObjectType GetObjectType();

    void InitializeData();

    void SetFlag(ObjectFlags flag, bool value)
    {
        if (value)
            Flags |= flag;
        else
            Flags &= ~flag;
    }
}

public interface IScriptedObject
{
    IEnumerable<Script> GetScripts();

    Script CreateScript();

    Script FindScript(uint id);
}

public interface IScriptedObject<out TScript> : IScriptedObject where TScript : Script
{
    new IEnumerable<TScript> GetScripts();

    new TScript CreateScript();

    new TScript FindScript(uint id);
}

///// <summary>
///// A script track set containing just the standard parameter tracks.
///// </summary>
//public sealed class CommonScriptTracks : BaseScriptTracks
//{}

/// <summary>
/// A script containing just the standard parameter tracks.
/// </summary>
public sealed class CommonScript : Script
{
}

/// <summary>
/// An object data class containing just the standard parameters.
/// </summary>
public sealed class CommonObjectData : BaseObjectData {}

/// <summary>
/// Base class for objects that do not have any additional parameters.
/// </summary>
public abstract class BaseObject : BaseObject<CommonObjectData, CommonScript>
{
    protected BaseObject(CommonObjectData data) : base(data)
    {
    }
}

/// <summary>
/// Base class for objects that have additional parameters beyond the standard set (Color, Pivot, etc.)
/// </summary>
/// <typeparam name="TData">
/// The <see cref="BaseObjectData"/> type used by the object.
/// </typeparam>
/// <typeparam name="TScript"></typeparam>
[DebuggerDisplay("{GetObjectType()}: {Guid,h} (parent: {Parent?.Guid,h})")]
public abstract class BaseObject<TData, TScript> : IObject<TData>, IScriptedObject<TScript>
    where TData : BaseObjectData, new() where TScript : Script, new()
{
    protected BaseObject(TData data)
    {
        Scripts = new List<TScript>();
        MessageResponses = new List<MessageResponse>();
        Data = data;
    }

    public List<TScript> Scripts { get; }

    public TData Data { get; protected set; }
    public ObjectFlags Flags { get; set; }
    public ResourceRequest ResourceRequest { get; set; }
    public string Name { get; set; }
    public uint NameHash { get; set; }
    public uint Guid { get; set; }
    public IObject<BaseObjectData> Parent { get; set; }
    public List<MessageResponse> MessageResponses { get; }

    public abstract void InitializeData();
    public abstract ObjectType GetObjectType();

    Script IScriptedObject.FindScript(uint id)
    {
        return FindScript(id);
    }

    Script IScriptedObject.CreateScript()
    {
        return CreateScript();
    }

    IEnumerable<Script> IScriptedObject.GetScripts()
    {
        return GetScripts();
    }

    public IEnumerable<TScript> GetScripts()
    {
        return Scripts;
    }

    public TScript CreateScript()
    {
        var script = new TScript();
        Scripts.Add(script);
        return script;
    }

    public TScript FindScript(uint id)
    {
        return Scripts.Find(s => s.Id == id);
    }
}