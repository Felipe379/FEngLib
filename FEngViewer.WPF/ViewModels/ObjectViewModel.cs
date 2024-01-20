using System.Collections.ObjectModel;
using System.Numerics;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using FEngLib.Messaging;
using FEngLib.Objects;
using FEngLib.Scripts;
using FEngLib.Structures;
using FEngRender.Data;
using FEngViewer.WPF.UIHelpers;
using MahApps.Metro.IconPacks;

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

public abstract class ScriptTrackViewModel : ObservableObject
{
    protected ScriptTrackViewModel(TrackId id, Track track)
    {
        TrackId = id;
        Track = track;
    }

    public string Name
    {
        get
        {
            return TrackId.Name;
        }
    }

    private TrackId TrackId { get; }
    private Track Track { get; }
}

public abstract class ScriptTrackViewModel<TTrack> : ScriptTrackViewModel where TTrack : Track
{
    protected ScriptTrackViewModel(TrackId id, TTrack track) : base(id, track)
    {
    }
}

public class Vector2TrackViewModel : ScriptTrackViewModel<Vector2Track>
{
    public Vector2TrackViewModel(TrackId<Vector2> id, Vector2Track track) : base(id, track)
    {
    }
}

public class Vector3TrackViewModel : ScriptTrackViewModel<Vector3Track>
{
    public Vector3TrackViewModel(TrackId<Vector3> id, Vector3Track track) : base(id, track)
    {
    }
}

public class QuaternionTrackViewModel : ScriptTrackViewModel<QuaternionTrack>
{
    public QuaternionTrackViewModel(TrackId<Quaternion> id, QuaternionTrack track) : base(id, track)
    {
    }
}

public class ColorTrackViewModel : ScriptTrackViewModel<ColorTrack>
{
    public ColorTrackViewModel(TrackId<Color4> id, ColorTrack track) : base(id, track)
    {
    }
}

public abstract class ScriptViewModel : ObservableObject, INamedEntity
{
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
    }
}

public class ImageScriptViewModel : BaseImageScriptViewModel<ImageScript>
{
    public ImageScriptViewModel(ImageScript script) : base(script)
    {
    }
}

public class ScriptsFolder : TreeFolder<ScriptViewModel>
{
    public ScriptsFolder(ObservableCollection<ScriptViewModel> children) : base("Scripts", children)
    {
    }
}

public abstract class ObjectViewModel : ObservableObject, INamedEntity
{
    //private RenderTreeNode RenderTreeNode { get; }

    private IObject<BaseObjectData> Object { get; }

    public CompositeCollection Children { get; }

    public ObservableCollection<MessageResponseViewModel> MessageResponses { get; }

    public abstract PackIconFontAwesomeKind Icon { get; }

    public string Name
    {
        get
        {
            // TODO: Implement hash resolution
            return IsNameExplicit ? Object.Name : $"0x{Object.NameHash:X8}";
        }
        set => throw new NotImplementedException();
    }

    public bool IsNameExplicit
    {
        get
        {
            return Object.Name is { Length: > 0 };
        }
    }

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
    protected BaseImageViewModel(RenderTreeNode<TObject, TScript> obj) : base(obj, obj.FrontendObject.Scripts)
    {
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