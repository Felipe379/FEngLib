using System.ComponentModel;
using System.Drawing.Design;
using System.Numerics;
using FEngLib;
using FEngLib.Objects;
using FEngLib.Packages;
using FEngLib.Structures;
using FEngLib.Utils;
using FEngViewer.TypeConverters;
using FEngViewer.UIEditors;
using JetBrains.Annotations;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace FEngViewer;

public abstract class ObjectViewWrapper<TObject> where TObject : class, IObject<ObjectData>
{
    protected ObjectViewWrapper(TObject wrappedObject, HashResolver hashResolver)
    {
        WrappedObject = wrappedObject;
		HashResolver = hashResolver;
    }

    [NotNull] protected TObject WrappedObject { get; }
	protected HashResolver HashResolver { get; }

	#region Basic object properties

	[Category("Meta")]
	[ReadOnly(true)]
	public string Name
	{
		get => WrappedObject.Name;
		set => WrappedObject.Name = value;
	}

	[Category("Meta")]
	[TypeConverter(typeof(HexTypeConverter))]
	[ReadOnly(true)]
	[DisplayName("Hash (hex)")]
	public uint HashHex
	{
		get => WrappedObject.NameHash;
		set => WrappedObject.NameHash = value;
	}

	[Category("Meta")]
	[ReadOnly(true)]
	[DisplayName("Hash (dec)")]
	public uint HashDec
	{
		get => WrappedObject.NameHash;
		set => WrappedObject.NameHash = value;
	}

	[Category("Meta")]
    [TypeConverter(typeof(HexTypeConverter))]
	[ReadOnly(true)]
	public uint Guid
    {
        get => WrappedObject.Guid;
        set => WrappedObject.Guid = value;
    }

    [Editor(typeof(ResourceRequestEditor), typeof(UITypeEditor))]
    [TypeConverter(typeof(ResourceRequestTypeConverter))]
    public ResourceRequest Resource
    {
        get => WrappedObject.ResourceRequest;
        set => WrappedObject.ResourceRequest = value;
    }

    #endregion

    #region Common object data

    [Category("Data")]
    [TypeConverter(typeof(Color4TypeConverter))]
    [Editor(typeof(Color4Editor), typeof(UITypeEditor))]
    public Color4 Color
    {
        get => WrappedObject.Data.Color;
        set => WrappedObject.Data.Color = value;
    }

    [Category("Data")]
    [TypeConverter(typeof(Vector3TypeConverter))]
    public Vector3 Pivot
    {
        get => WrappedObject.Data.Pivot;
        set => WrappedObject.Data.Pivot = value;
    }

    [Category("Data")]
    [TypeConverter(typeof(Vector3TypeConverter))]
    public Vector3 Position
    {
        get => WrappedObject.Data.Position;
        set => WrappedObject.Data.Position = value;
    }

    [Category("Data")]
    [TypeConverter(typeof(QuaternionTypeConverter))]
    public Quaternion Rotation
    {
        get => WrappedObject.Data.Rotation;
        set => WrappedObject.Data.Rotation = value;
    }

    [Category("Data")]
    [TypeConverter(typeof(Vector3TypeConverter))]
    public Vector3 Size
    {
        get => WrappedObject.Data.Size;
        set => WrappedObject.Data.Size = value;
    }

    #endregion

    #region Flags

    [Category("Flags")]
    public bool Invisible
    {
        get => (WrappedObject.Flags & ObjectFlags.Invisible) != 0;
        set => WrappedObject.SetFlag(ObjectFlags.Invisible, value);
    }

    public enum EPlatformSpecific
    {
        PcOnly,
        ConsoleOnly,
        None
    }

    [Category("Flags")]
    [Description("Whether this object should only be shown on a specific platform.")]
    public EPlatformSpecific PlatformSpecific
    {
        get
        {
            switch (WrappedObject.Flags & (ObjectFlags.PCOnly | ObjectFlags.ConsoleOnly))
            {
                case ObjectFlags.PCOnly:
                    return EPlatformSpecific.PcOnly;
                case ObjectFlags.ConsoleOnly:
                    return EPlatformSpecific.ConsoleOnly;
                case ObjectFlags.PCOnly | ObjectFlags.ConsoleOnly:
                    // this is invalid state, so we do not allow setting it.
                    // however, technically this is possible, so at least display it if that's the case.
                    return EPlatformSpecific.PcOnly | EPlatformSpecific.ConsoleOnly;
                default:
                    return EPlatformSpecific.None;
            }
        }
        set
        {
            switch (value)
            {
                case EPlatformSpecific.PcOnly:
                    WrappedObject.SetFlag(ObjectFlags.PCOnly, true);
                    WrappedObject.SetFlag(ObjectFlags.ConsoleOnly, false);
                    break;
                case EPlatformSpecific.ConsoleOnly:
                    WrappedObject.SetFlag(ObjectFlags.PCOnly, false);
                    WrappedObject.SetFlag(ObjectFlags.ConsoleOnly, true);
                    break;
                case EPlatformSpecific.None:
                    WrappedObject.SetFlag(ObjectFlags.PCOnly, false);
                    WrappedObject.SetFlag(ObjectFlags.ConsoleOnly, false);
                    break;
            }
        }
    }

    [Category("Flags")]
    public bool MouseObject
    {
        get => (WrappedObject.Flags & ObjectFlags.MouseObject) != 0;
        set => WrappedObject.SetFlag(ObjectFlags.MouseObject, value);
    }

    [Category("Flags")]
    public bool SaveStaticTracks
    {
        get => (WrappedObject.Flags & ObjectFlags.SaveStaticTracks) != 0;
        set => WrappedObject.SetFlag(ObjectFlags.SaveStaticTracks, value);
    }

    [Category("Flags")]
    [Description("Button related. TODO")]
    public bool DontNavigate
    {
        get => (WrappedObject.Flags & ObjectFlags.DontNavigate) != 0;
        set => WrappedObject.SetFlag(ObjectFlags.DontNavigate, value);
    }

    [Category("Flags")]
    [Description(
        "If set, this object's GUID is used to look up an object from another package to use as a 'template' of sorts. " +
        "Not supported, setting this flag WILL make the game fail reading this package.")]
    public bool UsesLibraryObject
    {
        get => (WrappedObject.Flags & ObjectFlags.UsesLibraryObject) != 0;
        set => WrappedObject.SetFlag(ObjectFlags.UsesLibraryObject, value);
    }

    [Category("Flags")]
    public bool CodeSuppliedResource
    {
        get => (WrappedObject.Flags & ObjectFlags.CodeSuppliedResource) != 0;
        set => WrappedObject.SetFlag(ObjectFlags.CodeSuppliedResource, value);
    }

    [Category("Flags")]
    public bool ObjectLocked
    {
        get => (WrappedObject.Flags & ObjectFlags.ObjectLocked) != 0;
        set => WrappedObject.SetFlag(ObjectFlags.ObjectLocked, value);
    }

    [Category("Flags")]
    [Description("Whether this object should be displayed in the preview.")]
    public bool HideInEdit
    {
        get => (WrappedObject.Flags & ObjectFlags.HideInEdit) != 0;
        set => WrappedObject.SetFlag(ObjectFlags.HideInEdit, value);
    }

    [Category("Flags")]
    [Description("Whether this object is a button. Buttons have a special role with regard to input/interaction. " +
                 "TODO: write up details on buttons somewhere")]
    public bool IsButton
    {
        get => (WrappedObject.Flags & ObjectFlags.IsButton) != 0;
        set => WrappedObject.SetFlag(ObjectFlags.IsButton, value);
    }

    [Category("Flags")]
    [Description("Whether this button should be ignored. Can only be set with IsButton." +
                 "TODO: write up details on buttons somewhere")]
    public bool IgnoreButton
    {
        get => (WrappedObject.Flags & ObjectFlags.IgnoreButton) != 0;
        set
        {
            if (value && IsButton) // only allow setting this flag on buttons
                WrappedObject.SetFlag(ObjectFlags.IgnoreButton, true);
            else // always allow clearing it
                WrappedObject.SetFlag(ObjectFlags.IgnoreButton, false);
        }
    }

    [Category("Flags")]
    public bool PerspectiveProjection
    {
        get => (WrappedObject.Flags & ObjectFlags.PerspectiveProjection) != 0;
        set => WrappedObject.SetFlag(ObjectFlags.PerspectiveProjection, value);
    }

    [Category("Flags")]
    public bool AffectAllScripts
    {
        get => (WrappedObject.Flags & ObjectFlags.AffectAllScripts) != 0;
        set => WrappedObject.SetFlag(ObjectFlags.AffectAllScripts, value);
    }

    #endregion
}

public class DefaultObjectViewWrapper : ObjectViewWrapper<IObject<ObjectData>>
{
    public DefaultObjectViewWrapper(IObject<ObjectData> wrappedObject, HashResolver hashResolver) : base(wrappedObject, hashResolver)
    {
    }

    [ReadOnly(true)]
    [Category("Meta")]
    [Description("The type of the object.")]
    public ObjectType Type => WrappedObject.Type;
}

public class TextObjectViewWrapper : ObjectViewWrapper<Text>
{
    public TextObjectViewWrapper(Text wrappedObject, HashResolver hashResolver) : base(wrappedObject, hashResolver)
    {
    }

    [Category("Text")]
    [Description("The amount of memory the game must allocate to store the localized version of the text.")]
    public uint BufferLength
    {
        get => WrappedObject.BufferLength;
        set => WrappedObject.BufferLength = value;
    }

    [Category("Text")]
    [Description("Placeholder text to be shown at design time.")]
    public string Placeholder
    {
        get => WrappedObject.Value;
        set => WrappedObject.Value = value;
    }

	[Category("Text")]
	[Description("The language label of the localized string to display in-game.")]
	[RefreshProperties(RefreshProperties.Repaint)]
	public string Label
	{
		get => WrappedObject.Label;
		set
		{
			WrappedObject.Label = value;
			WrappedObject.Hash = value.BinHash();
		}
	}

	[Category("Text")]
	[Description("The ID of the localized string to display in-game.")]
	[TypeConverter(typeof(HexTypeConverter))]
	[RefreshProperties(RefreshProperties.Repaint)]
	[DisplayName("LabelHash (hex)")]
	public uint LabelHashHex
	{
		get => WrappedObject.Hash;
		set
		{
			WrappedObject.Hash = value;
			WrappedObject.Label = HashResolver.ResolveNameHash(WrappedObject.Label, value);
		}
	}

	[Category("Text")]
	[Description("The ID of the localized string to display in-game.")]
	[TypeConverter(typeof(HexTypeConverter))]
	[RefreshProperties(RefreshProperties.Repaint)]
	[DisplayName("LabelHash (dec)")]
	public uint LabelHashDec
	{
		get => WrappedObject.Hash;
		set
		{
			WrappedObject.Hash = value;
			WrappedObject.Label = HashResolver.ResolveNameHash(WrappedObject.Label, value);
		}
	}

	[Category("Text")]
    public TextFormat Formatting
    {
        get => WrappedObject.Formatting;
        set => WrappedObject.Formatting = value;
    }

    [Category("Text")]
    public int Leading
    {
        get => WrappedObject.Leading;
        set => WrappedObject.Leading = value;
    }

    [Category("Text")]
    public uint MaxWidth
    {
        get => WrappedObject.MaxWidth;
        set => WrappedObject.MaxWidth = value;
    }
}

public abstract class ImageObjectViewWrapper<TImage> : ObjectViewWrapper<TImage> where TImage : class, IImage<ImageData>
{
    protected ImageObjectViewWrapper(TImage wrappedObject, HashResolver hashResolver) : base(wrappedObject, hashResolver)
    {
    }

    [Category("Image")]
    [DisplayName("Upper Left UV")]
    [TypeConverter(typeof(Vector2TypeConverter))]
    public Vector2 UpperLeft
    {
        get => WrappedObject.Data.UpperLeft;
        set => WrappedObject.Data.UpperLeft = value;
    }

    [Category("Image")]
    [DisplayName("Lower Right UV")]
    [TypeConverter(typeof(Vector2TypeConverter))]
    public Vector2 LowerRight
    {
        get => WrappedObject.Data.LowerRight;
        set => WrappedObject.Data.LowerRight = value;
    }

    [Category("Image")]
    [DisplayName("FE Image Flags")]
    public uint ImageFlags
    {
        get => WrappedObject.ImageFlags;
        set => WrappedObject.ImageFlags = value;
    }
}

public class ImageObjectViewWrapper : ImageObjectViewWrapper<Image>
{
    public ImageObjectViewWrapper(Image wrappedObject, HashResolver hashResolver) : base(wrappedObject, hashResolver)
    {
    }
}

public class ColoredImageObjectViewWrapper : ImageObjectViewWrapper<ColoredImage>
{
    public ColoredImageObjectViewWrapper(ColoredImage wrappedObject, HashResolver hashResolver) : base(wrappedObject, hashResolver)
    {
    }

    [Category("Image (Colored)")]
    [DisplayName("Top Left Color")]
    [TypeConverter(typeof(Color4TypeConverter))]
    [Editor(typeof(Color4Editor), typeof(UITypeEditor))]
    public Color4 TopLeft
    {
        get => WrappedObject.Data.TopLeft;
        set => WrappedObject.Data.TopLeft = value;
    }

    [Category("Image (Colored)")]
    [DisplayName("Top Right Color")]
    [TypeConverter(typeof(Color4TypeConverter))]
    [Editor(typeof(Color4Editor), typeof(UITypeEditor))]
    public Color4 TopRight
    {
        get => WrappedObject.Data.TopRight;
        set => WrappedObject.Data.TopRight = value;
    }

    [Category("Image (Colored)")]
    [DisplayName("Bottom Left Color")]
    [TypeConverter(typeof(Color4TypeConverter))]
    [Editor(typeof(Color4Editor), typeof(UITypeEditor))]
    public Color4 BottomLeft
    {
        get => WrappedObject.Data.BottomLeft;
        set => WrappedObject.Data.BottomLeft = value;
    }

    [Category("Image (Colored)")]
    [DisplayName("Bottom Right Color")]
    [TypeConverter(typeof(Color4TypeConverter))]
    [Editor(typeof(Color4Editor), typeof(UITypeEditor))]
    public Color4 BottomRight
    {
        get => WrappedObject.Data.BottomRight;
        set => WrappedObject.Data.BottomRight = value;
    }
}
