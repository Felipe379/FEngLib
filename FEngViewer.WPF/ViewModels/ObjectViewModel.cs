using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using FEngLib.Objects;
using FEngViewer.WPF.UIHelpers;
using MahApps.Metro.IconPacks;

namespace FEngViewer.WPF.ViewModels;

public abstract class ObjectFacade : ObservableObject, INamedObject
{
    protected abstract IObject<ObjectData> Object { get; }

    public string Name
    {
        get => string.IsNullOrEmpty(Object.Name) ? $"0x{Object.NameHash:X8}" : Object.Name;
        set => throw new NotImplementedException();
    }

    public ObjectType Type
    {
        get => Object.GetObjectType();
    }

    public bool IsNameExplicit
    {
        get => !string.IsNullOrEmpty(Object.Name);
    }
}

public class SimpleImageFacade : ObjectFacade
{
    public SimpleImageFacade(SimpleImage o)
    {
        Object = o;
    }

    protected override SimpleImage Object { get; }
}

public class TextFacade : ObjectFacade
{
    public TextFacade(Text o)
    {
        Object = o;
    }

    protected override Text Object { get; }
}

public class GroupFacade : ObjectFacade
{
    public GroupFacade(Group o)
    {
        Object = o;
    }

    protected override Group Object { get; }
}

public class ObjectViewModel : ObservableObject
{
    public ObjectViewModel(IObject<ObjectData> o)
    {
        TreeNodes = new CompositeCollection
        {
            new CollectionContainer
            {
                Collection = new List<object>()
            }
        };

        Facade = o switch
        {
            SimpleImage si => new SimpleImageFacade(si),
            Text t => new TextFacade(t),
            Group g => new GroupFacade(g),
            _ => throw new Exception($"Cannot create facade for object of type {o.GetType()}")
        };
    }

    public PackIconFontAwesomeKind TreeIcon =>
        Facade.Type switch
        {
            ObjectType.Image => PackIconFontAwesomeKind.ImageSolid,
            ObjectType.SimpleImage => PackIconFontAwesomeKind.ShapesSolid,
            ObjectType.Movie => PackIconFontAwesomeKind.FileVideoSolid,
            ObjectType.ColoredImage => PackIconFontAwesomeKind.PaintRollerSolid,
            ObjectType.String => PackIconFontAwesomeKind.HeadingSolid,
            ObjectType.MultiImage => PackIconFontAwesomeKind.ImagesSolid,
            _ => PackIconFontAwesomeKind.QuestionCircleSolid
        };

    public ObjectFacade Facade { get; }
    public CompositeCollection TreeNodes { get; }
}

public class GroupViewModel : ObjectViewModel
{
    public ObservableCollection<ObjectViewModel> Children { get; }

    public GroupViewModel(Group group, IEnumerable<ObjectViewModel> children) : base(group)
    {
        Children = new ObservableCollection<ObjectViewModel>(children);
        TreeNodes.Add(new CollectionContainer
        {
            Collection = Children
        });
    }
}