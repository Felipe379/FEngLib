using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FEngLib.Messaging;
using FEngLib.Objects;
using FEngLib.Scripts;
using FEngLib.Structures;
using FEngLib.Utils;
using FEngRender.Data;
using FEngViewer.WPF.UIHelpers;
using MahApps.Metro.IconPacks;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace FEngViewer.WPF.ViewModels;

public class ScriptEventViewModel : ObservableObject, INamedEntity
{
    private Event Event { get; }

    public ScriptEventViewModel(Event @event)
    {
        Event = @event;
    }

    public uint EventId
    {
        get => Event.EventId;
        set
        {
            SetProperty(Event.EventId, value, Event, (e, id) => e.EventId = id);
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(IsNameExplicit));
        }
    }

    public uint Target
    {
        get => Event.Target;
        set => SetProperty(Event.Target, value, Event, (e, target) => e.Target = target);
    }

    public uint Time
    {
        get => Event.Time;
        set => SetProperty(Event.Time, value, Event, (e, time) => e.Time = time);
    }

    //public string DisplayText
    //{
    //    get
    //    {
    //        return $"0x{EventId:X8} -> 0x{Target:X} @ T={Time}";
    //    }
    //}

    public Event GetEvent()
    {
        return Event;
    }

    public string Name
    {
        get
        {
            // TODO: Implement hash resolution
            return $"0x{EventId:X}";
        }
        set
        {
            throw new NotImplementedException();
        }
    }

    public bool IsNameExplicit
    {
        get
        {
            // TODO: Implement hash resolution
            return false;
        }
    }

    public string TargetName
    {
        get
        {
            // TODO: Implement GUID resolution
            return $"0x{Target:X}";
        }
        set
        {
            throw new NotImplementedException();
        }
    }

    public bool IsTargetNameExplicit
    {
        get
        {
            // TODO: Implement GUID resolution
            return false;
        }
    }
}

public class ScriptEventsFolder : TreeFolder<ScriptEventViewModel>
{
    public ScriptEventsFolder(ObservableCollection<ScriptEventViewModel> children) : base("Events", children)
    {
    }
}

public abstract class ScriptTrackViewModel : ObservableObject, ICommandable, IEditable
{
    private List<CommandableOption> _commands;

    protected ScriptTrackViewModel(TrackId id, Track track)
    {
        TrackId = id;
        Track = track;

        _commands = new List<CommandableOption>
        {
            new CommandOption("Add Key", PackIconFontAwesomeKind.StopwatchSolid, new RelayCommand(() =>
            {
                Debugger.Break();
            }, () => Length > 0)),
            new CommandOption("Delete", PackIconFontAwesomeKind.TrashSolid, new RelayCommand(() =>
            {
                Debugger.Break();
            }))
        };

        Children = new CompositeCollection();
    }

    public string Name
    {
        get
        {
            return TrackId.Name;
        }
    }

    public TrackInterpolationMethod InterpType
    {
        get => Track.InterpType;
        set
        {
            Track.InterpType = value;
            OnPropertyChanged();
        }
    }

    public TrackParamType ParamType => Track.GetParamType();

    public uint Length
    {
        get => Track.Length;
        set => SetTrackLength(value);
    }

    private TrackId TrackId { get; }
    private Track Track { get; }

    public ICollection Commands
    {
        get
        {
            return _commands;
        }
    }

    public ICollection Children { get; }

    public abstract PropertyDefinitionCollection GetPropertyDefinitions();

    protected abstract void SetTrackLength(uint length);
}

public abstract class ScriptTrackViewModel<TValue, TTrack> : ScriptTrackViewModel where TTrack : Track<TValue> where TValue : struct
{
    protected TTrack Track { get; }

    protected ScriptTrackViewModel(TrackId id, TTrack track) : base(id, track)
    {
        Track = track;
    }

    protected override void SetTrackLength(uint length)
    {
        if (length < Track.DeltaKeys.Max(dk => dk.Time))
        {
            throw new ArgumentException("Track must not end before a key begins", nameof(length));
        }

        Track.Length = length;
    }
}

public class Vector2TrackViewModel : ScriptTrackViewModel<Vector2, Vector2Track>
{
    public Vector2Wrapper BaseKey { get; }

    public Vector2TrackViewModel(TrackId<Vector2> id, Vector2Track track) : base(id, track)
    {
        BaseKey = new Vector2Wrapper(track.BaseKey, (vec) => track.BaseKey = vec);
    }

    public override PropertyDefinitionCollection GetPropertyDefinitions()
    {
        return EditorPropertyDefinitions.Vector2ScriptTrackProperties;
    }
}

public class Vector3TrackViewModel : ScriptTrackViewModel<Vector3, Vector3Track>
{
    public Vector3Wrapper BaseKey { get; }

    public Vector3TrackViewModel(TrackId<Vector3> id, Vector3Track track) : base(id, track)
    {
        BaseKey = new Vector3Wrapper(track.BaseKey, (vec) => track.BaseKey = vec);
    }

    public override PropertyDefinitionCollection GetPropertyDefinitions()
    {
        return EditorPropertyDefinitions.Vector3ScriptTrackProperties;
    }
}

public class QuaternionTrackViewModel : ScriptTrackViewModel<Quaternion, QuaternionTrack>
{
    public QuaternionWrapper BaseKey { get; }

    public QuaternionTrackViewModel(TrackId<Quaternion> id, QuaternionTrack track) : base(id, track)
    {
        BaseKey = new QuaternionWrapper(track.BaseKey, (quat) => track.BaseKey = quat);
    }

    public override PropertyDefinitionCollection GetPropertyDefinitions()
    {
        return EditorPropertyDefinitions.QuaternionScriptTrackProperties;
    }
}

public class ColorTrackViewModel : ScriptTrackViewModel<Color4, ColorTrack>
{
    public Color BaseKey
    {
        get
        {
            var color = Track.BaseKey;
            if (color is not { Alpha: >= 0 and <= 255, Blue: >= 0 and <= 255, Green: >= 0 and <= 255, Red: >= 0 and <= 255 })
            {
                throw new Exception("Invalid color: " + color);
            }
            return Color.FromArgb((byte)color.Alpha, (byte)color.Red, (byte)color.Green, (byte)color.Blue);
        }
        set
        {
            Track.BaseKey = new Color4(value.B, value.G, value.R, value.A);
            OnPropertyChanged();
        }
    }

    public ColorTrackViewModel(TrackId<Color4> id, ColorTrack track) : base(id, track)
    {
    }

    public override PropertyDefinitionCollection GetPropertyDefinitions()
    {
        return EditorPropertyDefinitions.ColorScriptTrackProperties;
    }
}

public class AddTrackOption : CommandOption
{
    public AddTrackOption(string label, PackIconFontAwesomeKind icon, RelayCommand command) : base(label, icon, command)
    {
    }
}

public abstract class ScriptViewModel : ObservableObject, INamedEntity, ICommandable, IEditable
{
    private CommandOption _deleteCommand;
    protected ObservableCollection<AddTrackOption> AddTrackCommands { get; }

    private readonly CompositeCollection _commands;

    private Script Script { get; }

    public string Name
    {
        get
        {
            // TODO: Implement hash resolution
            return IsNameExplicit ? Script.Name : $"0x{Script.Id:X8}";
        }
        set => throw new NotImplementedException();
    }

    public bool IsNameExplicit
    {
        get
        {
            return Script.Name is { Length: > 0 };
        }
    }

    private ObservableCollection<ScriptEventViewModel> Events
    {
        get;
    }

    public CompositeCollection Children { get; }

    public ICollection Commands
    {
        get
        {
            return _commands;
        }
    }

    public uint NameHash
    {
        get
        {
            return Script.Id;
        }
    }

    public bool LoopingEnabled
    {
        get => (Script.Flags & 1) == 1;
        set
        {
            if (value)
            {
                SetProperty(
                    Script.Flags,
                    Script.Flags | 1u,
                    Script,
                    (s, f) => s.Flags = f);
            }
            else
            {
                SetProperty(
                    Script.Flags,
                    Script.Flags & ~1u,
                    Script,
                    (s, f) => s.Flags = f);
            }
        }
    }

    public uint Length
    {
        get => Script.Length;
        set
        {
            if (value < Script.Tracks.Values.Max(t => t.Length))
            {
                throw new ArgumentException("Script cannot be shorter than longest track", nameof(value));
            }

            SetProperty(Script.Length, value, Script, (s, l) => s.Length = l);
        }
    }

    protected ScriptViewModel(Script script)
    {
        Script = script;
        Events = new SyncingObservableCollection<Event, ScriptEventViewModel>(
            script.Events,
            ev => new ScriptEventViewModel(ev),
            sevm => sevm.GetEvent());
        Children = new CompositeCollection
        {
            new ScriptEventsFolder(Events)
        };

        AddTrackCommands = new ObservableCollection<AddTrackOption>();
        _deleteCommand = new CommandOption("Delete", PackIconFontAwesomeKind.TrashSolid, new RelayCommand(ExecuteDeleteCommand, CanExecuteDeleteCommand));

        _commands = new CompositeCollection
        {
            new MultiCommandOption("Add", PackIconFontAwesomeKind.PlusSolid, new CommandableOption[]
            {
                new MultiCommandOption("Track", PackIconFontAwesomeKind.SlidersHSolid, AddTrackCommands),
                new CommandOption("Event", PackIconFontAwesomeKind.BoltSolid, new RelayCommand(() =>
                {
                    Debugger.Break();
                }))
            }),
            _deleteCommand
        };
    }

    public PropertyDefinitionCollection GetPropertyDefinitions()
    {
        return EditorPropertyDefinitions.ScriptProperties;
    }

    private void ExecuteDeleteCommand()
    {

    }

    private bool CanExecuteDeleteCommand()
    {
        return Script.Id != Hashing.BinHash("INIT");
    }
}

public abstract class ScriptViewModel<TScript> : ScriptViewModel where TScript : Script
{
    private ObservableCollection<ScriptTrackViewModel> Tracks
    {
        get;
    }

    public TScript Script { get; }

    protected ScriptViewModel(TScript script) : base(script)
    {
        Script = script;
        Tracks = new ObservableCollection<ScriptTrackViewModel>(TrackHelpers.GetAllTracks(script)
            .Select(CreateScriptTrackViewModel));
        Children.Add(new CollectionContainer { Collection = Tracks });

        AddTrackCommands.Add(CreateAddTrackOption(Script as Script, BaseScriptTrackIds.Color, () => new ColorTrack()));
        AddTrackCommands.Add(CreateAddTrackOption(Script as Script, BaseScriptTrackIds.Pivot, () => new Vector3Track()));
        AddTrackCommands.Add(CreateAddTrackOption(Script as Script, BaseScriptTrackIds.Position, () => new Vector3Track()));
        AddTrackCommands.Add(CreateAddTrackOption(Script as Script, BaseScriptTrackIds.Rotation, () => new QuaternionTrack()));
        AddTrackCommands.Add(CreateAddTrackOption(Script as Script, BaseScriptTrackIds.Size, () => new Vector3Track()));
    }

    protected AddTrackOption CreateAddTrackOption<TScriptBase, TValue>(
        TScriptBase script, TrackId<TValue> trackId, Func<Track<TValue>> trackConstructor)
        where TValue : struct
        where TScriptBase : Script, IScript<TScriptBase>
    {
        if (trackId is not TrackId<TScriptBase, TValue> upcastedTrackId)
            throw new ArgumentException("Invalid track ID for script/key type", nameof(trackId));

        return new AddTrackOption(
            trackId.Name,
            PackIconFontAwesomeKind.SlidersHSolid,
            new RelayCommand(
                () =>
                {
                    var track = trackConstructor();
                    script.SetTrack(upcastedTrackId, track);
                    Tracks.Add(CreateScriptTrackViewModel(new TrackEntry(trackId, track)));
                    foreach (var addTrackCommand in AddTrackCommands)
                    {
                        addTrackCommand.Command.NotifyCanExecuteChanged();
                    }
                },
                () => script.GetTrack(upcastedTrackId) is null));
    }

    private static ScriptTrackViewModel CreateScriptTrackViewModel(TrackEntry trackEntry)
    {
        return (trackEntry.Id, trackEntry.Track) switch
        {
            (TrackId<Vector2> id, Vector2Track track) => new Vector2TrackViewModel(id, track),
            (TrackId<Vector3> id, Vector3Track track) => new Vector3TrackViewModel(id, track),
            (TrackId<Quaternion> id, QuaternionTrack track) => new QuaternionTrackViewModel(id, track),
            (TrackId<Color4> id, ColorTrack track) => new ColorTrackViewModel(id, track),
            var pair => throw new Exception($"Cannot create track ViewModel for: {pair}")
        };
    }
}

public class CommonScriptViewModel : ScriptViewModel<CommonScript>
{
    public CommonScriptViewModel(CommonScript script) : base(script)
    {
    }
}

public abstract class BaseImageScriptViewModel<TScript> : ScriptViewModel<TScript>
    where TScript : BaseImageScript
{
    protected BaseImageScriptViewModel(TScript script) : base(script)
    {
        AddTrackCommands.Add(CreateAddTrackOption(Script as BaseImageScript, ImageScriptTrackIds.UpperLeft, () => new Vector2Track()));
        AddTrackCommands.Add(CreateAddTrackOption(Script as BaseImageScript, ImageScriptTrackIds.LowerRight, () => new Vector2Track()));
    }
}

public class ImageScriptViewModel : BaseImageScriptViewModel<ImageScript>
{
    public ImageScriptViewModel(ImageScript script) : base(script)
    {
    }
}

public class MultiImageScriptViewModel : BaseImageScriptViewModel<MultiImageScript>
{
    public MultiImageScriptViewModel(MultiImageScript script) : base(script)
    {
        AddTrackCommands.Add(CreateAddTrackOption(Script, MultiImageScriptTrackIds.TopLeft1, () => new Vector2Track()));
        AddTrackCommands.Add(CreateAddTrackOption(Script, MultiImageScriptTrackIds.TopLeft2, () => new Vector2Track()));
        AddTrackCommands.Add(CreateAddTrackOption(Script, MultiImageScriptTrackIds.TopLeft3, () => new Vector2Track()));
        AddTrackCommands.Add(CreateAddTrackOption(Script, MultiImageScriptTrackIds.BottomRight1, () => new Vector2Track()));
        AddTrackCommands.Add(CreateAddTrackOption(Script, MultiImageScriptTrackIds.BottomRight2, () => new Vector2Track()));
        AddTrackCommands.Add(CreateAddTrackOption(Script, MultiImageScriptTrackIds.BottomRight3, () => new Vector2Track()));
        AddTrackCommands.Add(CreateAddTrackOption(Script, MultiImageScriptTrackIds.PivotRotation, () => new Vector3Track()));
    }
}

public class ColoredImageScriptViewModel : BaseImageScriptViewModel<ColoredImageScript>
{
    public ColoredImageScriptViewModel(ColoredImageScript script) : base(script)
    {
        AddTrackCommands.Add(CreateAddTrackOption(Script, ColoredImageScriptTrackIds.TopLeft, () => new ColorTrack()));
        AddTrackCommands.Add(CreateAddTrackOption(Script, ColoredImageScriptTrackIds.TopRight, () => new ColorTrack()));
        AddTrackCommands.Add(CreateAddTrackOption(Script, ColoredImageScriptTrackIds.BottomRight, () => new ColorTrack()));
        AddTrackCommands.Add(CreateAddTrackOption(Script, ColoredImageScriptTrackIds.BottomLeft, () => new ColorTrack()));
    }
}

public class ScriptsFolder : TreeFolder<ScriptViewModel>
{
    public ScriptsFolder(ObservableCollection<ScriptViewModel> children) : base("Scripts", children)
    {
    }
}

public static class EditorPropertyDefinitions
{
    public static readonly PropertyDefinitionCollection BaseObjectProperties;
    public static readonly PropertyDefinitionCollection ScriptProperties;

    public static readonly PropertyDefinitionCollection Vector2ScriptTrackProperties;
    public static readonly PropertyDefinitionCollection Vector3ScriptTrackProperties;
    public static readonly PropertyDefinitionCollection QuaternionScriptTrackProperties;
    public static readonly PropertyDefinitionCollection ColorScriptTrackProperties;

    public static readonly PropertyDefinitionCollection BaseImageObjectProperties;
    public static readonly PropertyDefinitionCollection ColoredImageObjectProperties;
    public static readonly PropertyDefinitionCollection MultiImageObjectProperties;

    static EditorPropertyDefinitions()
    {
        BaseObjectProperties = new PropertyDefinitionCollection
        {
            new()
            {
                Category = "Meta",
                DisplayName = "GUID",
                TargetProperties = { nameof(ObjectViewModel.Guid) }
            },
            new()
            {
                Category = "Meta",
                DisplayName = "Name Hash",
                TargetProperties = { nameof(ObjectViewModel.NameHash) }
            },
            new()
            {
                Category = "Object Data",
                DisplayName = "Color",
                TargetProperties = { nameof(ObjectViewModel.Color) },
            },
            new()
            {
                Category = "Object Data",
                DisplayName = "Pivot",
                TargetProperties = { nameof(ObjectViewModel.Pivot) },
                IsExpandable = true
            },
            new()
            {
                Category = "Object Data",
                DisplayName = "Position",
                TargetProperties = { nameof(ObjectViewModel.Position) },
                IsExpandable = true
            },
            new()
            {
                Category = "Object Data",
                DisplayName = "Rotation",
                TargetProperties = { nameof(ObjectViewModel.Rotation) },
                IsExpandable = true
            },
            new()
            {
                Category = "Object Data",
                DisplayName = "Size",
                TargetProperties = { nameof(ObjectViewModel.Size) },
                IsExpandable = true
            }
        };

        BaseImageObjectProperties = MergeCollections(BaseObjectProperties, new PropertyDefinitionCollection
        {
            new()
            {
                Category = "Object Data - Image",
                DisplayName = "Upper Left UV",
                TargetProperties = { nameof(ImageViewModel.UpperLeft) },
                IsExpandable = true
            },
            new()
            {
                Category = "Object Data - Image",
                DisplayName = "Lower Right UV",
                TargetProperties = { nameof(ImageViewModel.LowerRight) },
                IsExpandable = true
            },
        });

        ColoredImageObjectProperties = MergeCollections(BaseImageObjectProperties, new PropertyDefinitionCollection
        {
            new()
            {
                Category = "Object Data - Image (Colored)",
                DisplayName = "Top Left Color",
                TargetProperties = { nameof(ColoredImageViewModel.TopLeft)}
            },
            new()
            {
                Category = "Object Data - Image (Colored)",
                DisplayName = "Top Right Color",
                TargetProperties = { nameof(ColoredImageViewModel.TopRight)}
            },
            new()
            {
                Category = "Object Data - Image (Colored)",
                DisplayName = "Bottom Right Color",
                TargetProperties = { nameof(ColoredImageViewModel.BottomRight)}
            },
            new()
            {
                Category = "Object Data - Image (Colored)",
                DisplayName = "Bottom Left Color",
                TargetProperties = { nameof(ColoredImageViewModel.BottomLeft)}
            },
        });

        MultiImageObjectProperties = MergeCollections(BaseImageObjectProperties, new PropertyDefinitionCollection
        {
            new()
            {
                Category = "Object Data - Image (Multi-Textured)",
                DisplayName = "Texture 1: Top Left UV",
                TargetProperties = { nameof(MultiImageViewModel.TopLeft1) },
                IsExpandable = true
            },
            new()
            {
                Category = "Object Data - Image (Multi-Textured)",
                DisplayName = "Texture 1: Bottom Right UV",
                TargetProperties = { nameof(MultiImageViewModel.BottomRight1) },
                IsExpandable = true
            },
            new()
            {
                Category = "Object Data - Image (Multi-Textured)",
                DisplayName = "Texture 2: Top Left UV",
                TargetProperties = { nameof(MultiImageViewModel.TopLeft2) },
                IsExpandable = true
            },
            new()
            {
                Category = "Object Data - Image (Multi-Textured)",
                DisplayName = "Texture 2: Bottom Right UV",
                TargetProperties = { nameof(MultiImageViewModel.BottomRight2) },
                IsExpandable = true
            },
            new()
            {
                Category = "Object Data - Image (Multi-Textured)",
                DisplayName = "Texture 3: Top Left UV",
                TargetProperties = { nameof(MultiImageViewModel.TopLeft3) },
                IsExpandable = true
            },
            new()
            {
                Category = "Object Data - Image (Multi-Textured)",
                DisplayName = "Texture 3: Bottom Right UV",
                TargetProperties = { nameof(MultiImageViewModel.BottomRight3) },
                IsExpandable = true
            },
            new()
            {
                Category = "Object Data - Image (Multi-Textured)",
                DisplayName = "Pivot Rotation",
                TargetProperties = { nameof(MultiImageViewModel.PivotRotation) },
                IsExpandable = true
            },
        });

        ScriptProperties = new PropertyDefinitionCollection
        {
            new()
            {
                Category = "Meta",
                DisplayName = "Name Hash",
                TargetProperties = { nameof(ScriptViewModel.NameHash) }
            },
            new()
            {
                Category = "Script Data",
                DisplayName = "Length",
                Description = "The length of the script, in milliseconds.",
                TargetProperties = { nameof(ScriptViewModel.Length) }
            },
            new()
            {
                Category = "Script Data",
                DisplayName = "Looping",
                Description = "Whether the script should restart immediately after finishing.",
                TargetProperties = { nameof(ScriptViewModel.LoopingEnabled) }
            }
        };

        var baseScriptTrackProperties = new PropertyDefinitionCollection
        {
            new()
            {
                Category = "Meta",
                DisplayName = "Parameter Type",
                TargetProperties = { nameof(ScriptTrackViewModel.ParamType) }
            },
            new()
            {
                Category = "Track Data",
                DisplayName = "Interpolation Method",
                Description = "The interpolation strategy to use for the track. Unless you have a REALLY GOOD REASON to change this, LEAVE IT ALONE.",
                TargetProperties = { nameof(ScriptTrackViewModel.InterpType) }
            },
            new()
            {
                Category = "Track Data",
                DisplayName = "Length",
                Description = "The length of the track, in milliseconds.",
                TargetProperties = { nameof(ScriptTrackViewModel.Length) }
            },
        };

        QuaternionScriptTrackProperties = MergeCollections(baseScriptTrackProperties, new PropertyDefinitionCollection
        {
            new()
            {
                Category = "Track Data",
                DisplayName = "Base Key",
                Description = "The initial value of the property controlled by the track.",
                IsExpandable = true,
                TargetProperties = { nameof(QuaternionTrackViewModel.BaseKey) },
            }
        });

        Vector2ScriptTrackProperties = MergeCollections(baseScriptTrackProperties, new PropertyDefinitionCollection
        {
            new()
            {
                Category = "Track Data",
                DisplayName = "Base Key",
                Description = "The initial value of the property controlled by the track.",
                IsExpandable = true,
                TargetProperties = { nameof(Vector2TrackViewModel.BaseKey) },
            }
        });

        Vector3ScriptTrackProperties = MergeCollections(baseScriptTrackProperties, new PropertyDefinitionCollection
        {
            new()
            {
                Category = "Track Data",
                DisplayName = "Base Key",
                Description = "The initial value of the property controlled by the track.",
                IsExpandable = true,
                TargetProperties = { nameof(Vector3TrackViewModel.BaseKey) },
            }
        });

        ColorScriptTrackProperties = MergeCollections(baseScriptTrackProperties, new PropertyDefinitionCollection
        {
            new()
            {
                Category = "Track Data",
                DisplayName = "Base Key",
                Description = "The initial value of the property controlled by the track.",
                TargetProperties = { nameof(ColorTrackViewModel.BaseKey) },
            }
        });
    }

    private static PropertyDefinitionCollection MergeCollections(params PropertyDefinitionCollection[] collections)
    {
        var merged = new PropertyDefinitionCollection();
        foreach (var propertyDefinition in collections.SelectMany(c => c))
        {
            merged.Add(propertyDefinition);
        }

        return merged;
    }
}

public class ObjectFlagsWrapper : ObservableObject
{
    private readonly IObject<BaseObjectData> _obj;

    public bool Invisible
    {
        get => GetFlag(ObjectFlags.Invisible);
        set => UpdateFlag(ObjectFlags.Invisible, value);
    }

    public bool PCOnly
    {
        get => GetFlag(ObjectFlags.PCOnly);
        set => UpdateFlag(ObjectFlags.PCOnly, value);
    }

    public bool ConsoleOnly
    {
        get => GetFlag(ObjectFlags.ConsoleOnly);
        set => UpdateFlag(ObjectFlags.ConsoleOnly, value);
    }

    public bool MouseObject
    {
        get => GetFlag(ObjectFlags.MouseObject);
        set => UpdateFlag(ObjectFlags.MouseObject, value);
    }

    public bool SaveStaticTracks
    {
        get => GetFlag(ObjectFlags.SaveStaticTracks);
        set => UpdateFlag(ObjectFlags.SaveStaticTracks, value);
    }

    public bool DontNavigate
    {
        get => GetFlag(ObjectFlags.DontNavigate);
        set => UpdateFlag(ObjectFlags.DontNavigate, value);
    }

    public bool UsesLibraryObject
    {
        get => GetFlag(ObjectFlags.UsesLibraryObject);
        set => UpdateFlag(ObjectFlags.UsesLibraryObject, value);
    }

    public bool CodeSuppliedResource
    {
        get => GetFlag(ObjectFlags.CodeSuppliedResource);
        set => UpdateFlag(ObjectFlags.CodeSuppliedResource, value);
    }

    public bool IgnoreButton
    {
        get => GetFlag(ObjectFlags.IgnoreButton);
        set => UpdateFlag(ObjectFlags.IgnoreButton, value);
    }

    public bool ObjectLocked
    {
        get => GetFlag(ObjectFlags.ObjectLocked);
        set => UpdateFlag(ObjectFlags.ObjectLocked, value);
    }

    public bool HideInEdit
    {
        get => GetFlag(ObjectFlags.HideInEdit);
        set => UpdateFlag(ObjectFlags.HideInEdit, value);
    }

    public bool IsButton
    {
        get => GetFlag(ObjectFlags.IsButton);
        set => UpdateFlag(ObjectFlags.IsButton, value);
    }

    public bool PerspectiveProjection
    {
        get => GetFlag(ObjectFlags.PerspectiveProjection);
        set => UpdateFlag(ObjectFlags.PerspectiveProjection, value);
    }

    public bool AffectAllScripts
    {
        get => GetFlag(ObjectFlags.AffectAllScripts);
        set => UpdateFlag(ObjectFlags.AffectAllScripts, value);
    }

    public ObjectFlagsWrapper(IObject<BaseObjectData> obj)
    {
        _obj = obj;
    }

    private bool GetFlag(ObjectFlags flag)
    {
        return (_obj.Flags & flag) == flag;
    }

    private void UpdateFlag(ObjectFlags flag, bool value, [CallerMemberName] string? flagName = null)
    {
        Debug.Assert(flagName != null);
        var newFlags = _obj.Flags;
        if (value)
            newFlags |= flag;
        else
            newFlags &= ~flag;
        SetProperty(_obj.Flags, newFlags, _obj, (o, f) => o.Flags = f, flagName);
    }

    public override string ToString()
    {
        return _obj.Flags.ToString();
    }
}

public class Vector2Wrapper : ObservableObject
{
    private Vector2 _vector;
    private readonly Action<Vector2> _setter;

    public float X
    {
        get => _vector.X;
        set
        {
            _vector.X = value;
            _setter(_vector);
            OnPropertyChanged();
        }
    }

    public float Y
    {
        get => _vector.Y;
        set
        {
            _vector.Y = value;
            _setter(_vector);
            OnPropertyChanged();
        }
    }

    public Vector2Wrapper(Vector2 vector, Action<Vector2> setter)
    {
        _vector = vector;
        _setter = setter;
    }

    public override string ToString()
    {
        return _vector.ToString();
    }
}

public class Vector3Wrapper : ObservableObject
{
    private Vector3 _vector;
    private readonly Action<Vector3> _setter;

    public float X
    {
        get => _vector.X;
        set
        {
            _vector.X = value;
            _setter(_vector);
            OnPropertyChanged();
        }
    }

    public float Y
    {
        get => _vector.Y;
        set
        {
            _vector.Y = value;
            _setter(_vector);
            OnPropertyChanged();
        }
    }

    public float Z
    {
        get => _vector.Z;
        set
        {
            _vector.Z = value;
            _setter(_vector);
            OnPropertyChanged();
        }
    }

    public Vector3Wrapper(Vector3 vector, Action<Vector3> setter)
    {
        _vector = vector;
        _setter = setter;
    }

    public override string ToString()
    {
        return _vector.ToString();
    }
}

public class QuaternionWrapper : ObservableObject
{
    private Quaternion _quaternion;
    private readonly Action<Quaternion> _setter;

    public float X
    {
        get => _quaternion.X;
        set
        {
            _quaternion.X = value;
            _setter(_quaternion);
            OnPropertyChanged();
        }
    }

    public float Y
    {
        get => _quaternion.Y;
        set
        {
            _quaternion.Y = value;
            _setter(_quaternion);
            OnPropertyChanged();
        }
    }

    public float Z
    {
        get => _quaternion.Z;
        set
        {
            _quaternion.Z = value;
            _setter(_quaternion);
            OnPropertyChanged();
        }
    }

    public float W
    {
        get => _quaternion.W;
        set
        {
            _quaternion.W = value;
            _setter(_quaternion);
            OnPropertyChanged();
        }
    }

    public QuaternionWrapper(Quaternion quaternion, Action<Quaternion> setter)
    {
        _quaternion = quaternion;
        _setter = setter;
    }

    public override string ToString()
    {
        return _quaternion.ToString();
    }
}

public class ColorWrapper : ObservableObject
{
    private Color4 _color;
    private readonly Action<Color4> _setter;

    public int Red
    {
        get => _color.Red;
        set
        {
            _color.Red = value;
            _setter(_color);
            OnPropertyChanged();
        }
    }

    public int Green
    {
        get => _color.Green;
        set
        {
            _color.Green = value;
            _setter(_color);
            OnPropertyChanged();
        }
    }

    public int Blue
    {
        get => _color.Blue;
        set
        {
            _color.Blue = value;
            _setter(_color);
            OnPropertyChanged();
        }
    }

    public int Alpha
    {
        get => _color.Alpha;
        set
        {
            _color.Alpha = value;
            _setter(_color);
            OnPropertyChanged();
        }
    }

    public ColorWrapper(Color4 color, Action<Color4> setter)
    {
        _color = color;
        _setter = setter;
    }

    public override string ToString()
    {
        return _color.ToString();
    }
}

public static class ColorHelper
{
    public static Color FngColorToMediaColor(Color4 color)
    {
        if (color is not { Alpha: >= 0 and <= 255, Blue: >= 0 and <= 255, Green: >= 0 and <= 255, Red: >= 0 and <= 255 })
        {
            throw new Exception("Invalid color: " + color);
        }
        return Color.FromArgb((byte)color.Alpha, (byte)color.Red, (byte)color.Green, (byte)color.Blue);
    }

    public static Color4 MediaColorToFngColor(Color color)
    {
        return new Color4(color.B, color.G, color.R, color.A);
    }
}

public abstract class ObjectViewModel : ObservableObject, INamedEntity, IEditable
{
    //private RenderTreeNode RenderTreeNode { get; }

    private IObject<BaseObjectData> Object { get; }

    //[Browsable(false)]
    public CompositeCollection Children { get; }

    //[Browsable(false)]
    public ObservableCollection<MessageResponseViewModel> MessageResponses { get; }

    //[Browsable(false)]
    public abstract PackIconFontAwesomeKind Icon { get; }

    //[Browsable(false)]
    public string Name
    {
        get
        {
            // TODO: Implement hash resolution
            return IsNameExplicit ? Object.Name : $"0x{Object.NameHash:X8}";
        }
        set => throw new NotImplementedException();
    }

    //[Category("Meta")]
    //[DisplayName("Name Hash")]
    public uint NameHash
    {
        get
        {
            return Object.NameHash;
        }
    }

    //[Browsable(false)]
    public bool IsNameExplicit
    {
        get
        {
            return Object.Name is { Length: > 0 };
        }
    }

    //[Category("Meta")]
    //[DisplayName("GUID")]
    public uint Guid
    {
        get => Object.Guid;
        set => SetProperty(Object.Guid, value, Object, (o, g) => o.Guid = g);
    }

    //[Category("Meta")]
    //[DisplayName("Flags")]
    //[ExpandableObject]
    //[TypeConverter(typeof(ObjectFlagsConverter))]
    public ObjectFlagsWrapper Flags { get; }

    #region ObjectData Properties

    public Color Color
    {
        get => ColorHelper.FngColorToMediaColor(Object.Data.Color);
        set => SetProperty(Object.Data.Color, ColorHelper.MediaColorToFngColor(value), Object.Data, (od, col) => od.Color = col);
    }

    public Vector3Wrapper Pivot { get; }

    public Vector3Wrapper Position { get; }

    public QuaternionWrapper Rotation { get; }

    public Vector3Wrapper Size { get; }

    #endregion

    protected ObjectViewModel(RenderTreeNode renderTreeNode)
    {
        // here in case we need it later...
        //RenderTreeNode = renderTreeNode;
        Object = renderTreeNode.GetObject();
        MessageResponses = new SyncingObservableCollection<MessageResponse, MessageResponseViewModel>(
            Object.MessageResponses,
            mr => new MessageResponseViewModel(mr),
            mrvm => mrvm.MessageResponse
        );

        Children = new CompositeCollection
        {
            new MessageResponsesFolder(MessageResponses)
        };

        Flags = new ObjectFlagsWrapper(Object);
        Pivot = new Vector3Wrapper(Object.Data.Pivot, vec =>
        {
            Object.Data.Pivot = vec;
            OnPropertyChanged(nameof(Pivot));
        });
        Position = new Vector3Wrapper(Object.Data.Position, vec =>
        {
            Object.Data.Position = vec;
            OnPropertyChanged(nameof(Position));
        });
        Rotation = new QuaternionWrapper(Object.Data.Rotation, quaternion =>
        {
            Object.Data.Rotation = quaternion;
            OnPropertyChanged(nameof(Rotation));
        });
        Size = new Vector3Wrapper(Object.Data.Size, vec =>
        {
            Object.Data.Size = vec;
            OnPropertyChanged(nameof(Size));
        });
    }

    public virtual PropertyDefinitionCollection GetPropertyDefinitions()
    {
        return EditorPropertyDefinitions.BaseObjectProperties;
    }
}

public abstract class ObjectViewModel<TObject, TScript> : ObjectViewModel
where TObject : IObject<BaseObjectData>, IScriptedObject<TScript>
where TScript : Script
{
    protected RenderTreeNode<TObject, TScript> RenderTreeNode { get; }

    protected TObject Object { get; }

    private ObservableCollection<ScriptViewModel> Scripts
    {
        get;
    }

    protected ObjectViewModel(RenderTreeNode<TObject, TScript> renderTreeNode, IList<TScript> scriptList) : base(renderTreeNode)
    {
        RenderTreeNode = renderTreeNode;
        Object = renderTreeNode.FrontendObject;

        // this is kind of a stupid setup, but if it works, it works... I guess?
        Scripts = new SyncingObservableCollection<TScript, ScriptViewModel>(
            scriptList,
            CreateScriptViewModel,
            svm => ((ScriptViewModel<TScript>)svm).Script);
        Children.Add(new ScriptsFolder(Scripts));
    }

    protected abstract ScriptViewModel<TScript> CreateScriptViewModel(TScript script);
}

public abstract class BaseObjectViewModel<TObject> : ObjectViewModel<TObject, CommonScript>
where TObject : BaseObject<CommonObjectData, CommonScript>
{
    protected BaseObjectViewModel(RenderTreeNode<TObject, CommonScript> obj) : base(obj, obj.FrontendObject.Scripts)
    {
    }

    protected override ScriptViewModel<CommonScript> CreateScriptViewModel(CommonScript script)
    {
        return new CommonScriptViewModel(script);
    }
}

public abstract class BaseImageViewModel<TObject, TObjectData, TScript> : ObjectViewModel<TObject, TScript>
    where TObject : BaseImage<TObjectData, TScript>
    where TObjectData : BaseImageData, new()
    where TScript : Script, new()
{
    public Vector2Wrapper UpperLeft { get; }
    public Vector2Wrapper LowerRight { get; }

    protected BaseImageViewModel(RenderTreeNode<TObject, TScript> obj) : base(obj, obj.FrontendObject.Scripts)
    {
        UpperLeft = new Vector2Wrapper(Object.Data.UpperLeft, vec =>
        {
            Object.Data.UpperLeft = vec;
            OnPropertyChanged(nameof(UpperLeft));
        });
        LowerRight = new Vector2Wrapper(Object.Data.LowerRight, vec =>
        {
            Object.Data.LowerRight = vec;
            OnPropertyChanged(nameof(LowerRight));
        });
    }

    public override PropertyDefinitionCollection GetPropertyDefinitions()
    {
        return EditorPropertyDefinitions.BaseImageObjectProperties;
    }
}

public class TextViewModel : BaseObjectViewModel<Text>
{
    public TextViewModel(RenderTreeText renderTreeText) : base(renderTreeText)
    {
    }

    public override PackIconFontAwesomeKind Icon => PackIconFontAwesomeKind.HeadingSolid;
}

public class SimpleImageViewModel : BaseObjectViewModel<SimpleImage>
{
    public SimpleImageViewModel(RenderTreeSimpleImage renderTreeSimpleImage) : base(renderTreeSimpleImage)
    {
    }

    public override PackIconFontAwesomeKind Icon => PackIconFontAwesomeKind.ShapesSolid;
}

public class ImageViewModel : BaseImageViewModel<Image, ImageData, ImageScript>
{
    public ImageViewModel(RenderTreeImage renderTreeImage) : base(renderTreeImage)
    {
    }

    protected override ScriptViewModel<ImageScript> CreateScriptViewModel(ImageScript script)
    {
        return new ImageScriptViewModel(script);
    }

    public override PackIconFontAwesomeKind Icon => PackIconFontAwesomeKind.ImageSolid;
}

public class GroupViewModel : BaseObjectViewModel<Group>
{
    public ObservableCollection<ObjectViewModel> ChildObjects { get; }

    public GroupViewModel(RenderTreeGroup renderTreeGroup, IEnumerable<ObjectViewModel> children) : base(renderTreeGroup)
    {
        ChildObjects = new ObservableCollection<ObjectViewModel>(children);
        Children.Add(new CollectionContainer
        {
            Collection = ChildObjects
        });
    }

    public override PackIconFontAwesomeKind Icon => PackIconFontAwesomeKind.LayerGroupSolid;
}

public class MultiImageViewModel : BaseImageViewModel<MultiImage, MultiImageData, MultiImageScript>
{
    public Vector2Wrapper TopLeft1 { get; }
    public Vector2Wrapper TopLeft2 { get; }
    public Vector2Wrapper TopLeft3 { get; }
    public Vector2Wrapper BottomRight1 { get; }
    public Vector2Wrapper BottomRight2 { get; }
    public Vector2Wrapper BottomRight3 { get; }
    public Vector3Wrapper PivotRotation { get; }

    public MultiImageViewModel(RenderTreeNode<MultiImage, MultiImageScript> obj) : base(obj)
    {
        TopLeft1 = new Vector2Wrapper(Object.Data.TopLeft1, vec =>
        {
            Object.Data.TopLeft1 = vec;
            OnPropertyChanged(nameof(TopLeft1));
        });
        TopLeft2 = new Vector2Wrapper(Object.Data.TopLeft2, vec =>
        {
            Object.Data.TopLeft2 = vec;
            OnPropertyChanged(nameof(TopLeft2));
        });
        TopLeft3 = new Vector2Wrapper(Object.Data.TopLeft3, vec =>
        {
            Object.Data.TopLeft3 = vec;
            OnPropertyChanged(nameof(TopLeft3));
        });
        BottomRight1 = new Vector2Wrapper(Object.Data.BottomRight1, vec =>
        {
            Object.Data.BottomRight1 = vec;
            OnPropertyChanged(nameof(BottomRight1));
        });
        BottomRight2 = new Vector2Wrapper(Object.Data.BottomRight2, vec =>
        {
            Object.Data.BottomRight2 = vec;
            OnPropertyChanged(nameof(BottomRight2));
        });
        BottomRight3 = new Vector2Wrapper(Object.Data.BottomRight3, vec =>
        {
            Object.Data.BottomRight3 = vec;
            OnPropertyChanged(nameof(BottomRight3));
        });
        PivotRotation = new Vector3Wrapper(Object.Data.PivotRotation, vec =>
        {
            Object.Data.PivotRotation = vec;
            OnPropertyChanged(nameof(PivotRotation));
        });
    }

    public override PackIconFontAwesomeKind Icon => PackIconFontAwesomeKind.ImagesSolid;
    protected override ScriptViewModel<MultiImageScript> CreateScriptViewModel(MultiImageScript script)
    {
        return new MultiImageScriptViewModel(script);
    }

    public override PropertyDefinitionCollection GetPropertyDefinitions()
    {
        return EditorPropertyDefinitions.MultiImageObjectProperties;
    }
}

public class ColoredImageViewModel : BaseImageViewModel<ColoredImage, ColoredImageData, ColoredImageScript>
{
    public Color TopLeft
    {
        get => ColorHelper.FngColorToMediaColor(Object.Data.TopLeft);
        set => SetProperty(Object.Data.TopLeft, ColorHelper.MediaColorToFngColor(value), Object.Data, (od, col) => od.TopLeft = col);
    }
    public Color TopRight
    {
        get => ColorHelper.FngColorToMediaColor(Object.Data.TopRight);
        set => SetProperty(Object.Data.TopRight, ColorHelper.MediaColorToFngColor(value), Object.Data, (od, col) => od.TopRight = col);
    }
    public Color BottomRight
    {
        get => ColorHelper.FngColorToMediaColor(Object.Data.BottomRight);
        set => SetProperty(Object.Data.BottomRight, ColorHelper.MediaColorToFngColor(value), Object.Data, (od, col) => od.BottomRight = col);
    }
    public Color BottomLeft
    {
        get => ColorHelper.FngColorToMediaColor(Object.Data.BottomLeft);
        set => SetProperty(Object.Data.BottomLeft, ColorHelper.MediaColorToFngColor(value), Object.Data, (od, col) => od.BottomLeft = col);
    }

    public ColoredImageViewModel(RenderTreeNode<ColoredImage, ColoredImageScript> obj) : base(obj)
    {
    }

    public override PackIconFontAwesomeKind Icon => PackIconFontAwesomeKind.PaintRollerSolid;
    protected override ScriptViewModel<ColoredImageScript> CreateScriptViewModel(ColoredImageScript script)
    {
        return new ColoredImageScriptViewModel(script);
    }

    public override PropertyDefinitionCollection GetPropertyDefinitions()
    {
        return EditorPropertyDefinitions.ColoredImageObjectProperties;
    }
}

public class MovieViewModel : BaseObjectViewModel<Movie>
{
    public MovieViewModel(RenderTreeNode<Movie, CommonScript> obj) : base(obj)
    {
    }

    public override PackIconFontAwesomeKind Icon => PackIconFontAwesomeKind.FileVideoSolid;
}