using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using FEngLib.Messaging;
using FEngLib.Objects;
using FEngLib.Packages;
using FEngRender.Data;
using FEngViewer.WPF.UIHelpers;

namespace FEngViewer.WPF.ViewModels;

public class ResourceRequestFolder : TreeFolder<ResourceRequestViewModel>
{
    public ResourceRequestFolder(ObservableCollection<ResourceRequestViewModel> children) : base("Resources", children)
    {
    }
}

public class MessageDefinitionsFolder : TreeFolder<MessageDefinitionViewModel>
{
    public ICollectionView ChildrenViewSource
    {
        get;
    }

    public MessageDefinitionsFolder(ObservableCollection<MessageDefinitionViewModel> children) : base("Message Definitions", children)
    {
        ChildrenViewSource = CollectionViewSource.GetDefaultView(children);
        ChildrenViewSource.GroupDescriptions.Add(new PropertyGroupDescription(nameof(MessageDefinitionViewModel.Category)));
    }
}

public class RenderPackageFacade : ObservableObject
{
    private readonly Package _package;
    public RenderTree RenderTree { get; }

    public ObservableCollection<ResourceRequestViewModel> ResourceRequests
    {
        get;
    }

    public ObservableCollection<MessageDefinitionViewModel> MessageDefinitions
    {
        get;
    }

    public ObservableCollection<MessageResponseViewModel> MessageResponses
    {
        get;
    }

    public ObservableCollection<ObjectViewModel> Objects
    {
        get;
    }

    public string Name
    {
        get => _package.Name;
        set => SetProperty(_package.Name, value, _package, (p, n) => p.Name = n);
    }

    public string Filename
    {
        get => _package.Filename;
        set => SetProperty(_package.Filename, value, _package, (p, fn) => p.Filename = fn);
    }

    public CompositeCollection TreeNodes { get; }

    public RenderPackageFacade(Package package, RenderTree renderTree)
    {
        _package = package;
        RenderTree = renderTree;

        ResourceRequests = new SyncingObservableCollection<ResourceRequest, ResourceRequestViewModel>(
            package.ResourceRequests,
            rr => new ResourceRequestViewModel(rr),
            rrvm => rrvm.ResourceRequest);
        MessageDefinitions = new SyncingObservableCollection<Package.MessageDefinition, MessageDefinitionViewModel>(
            package.MessageDefinitions,
            md => new MessageDefinitionViewModel(md),
            mdvm => mdvm.MessageDefinition);
        MessageResponses = new SyncingObservableCollection<MessageResponse, MessageResponseViewModel>(
            package.MessageResponses,
            mr => new MessageResponseViewModel(mr),
            mrvm => mrvm.MessageResponse);

        // TODO: We can use a SyncingObservableCollection once we create a proper tree API in FEngLib.
        //Objects = new SyncingObservableCollection<IObject<ObjectData>, ObjectViewModel>(
        //    package.Objects,
        //    obj => ObjectToViewModel(package, obj),
        //    ovm => ovm.Facade.Object);
        //Objects = new ObservableCollection<ObjectViewModel>(
        //    package.Objects
        //        .Where(o => o.Parent is null)
        //        .Select(o => ObjectToViewModel(package, o)));
        Objects = new ObservableCollection<ObjectViewModel>(renderTree.Select(RenderNodeToViewModel));

        TreeNodes = new CompositeCollection
        {
            new ResourceRequestFolder(ResourceRequests),
            new MessageDefinitionsFolder(MessageDefinitions),
            new MessageResponsesFolder(MessageResponses),
            new CollectionContainer { Collection = Objects }
        };
        //TreeNodes = new ObservableCollection<object>(treeNodes);
    }

    //private static ObjectViewModel ObjectToViewModel(Package package, IObject<BaseObjectData> obj)
    //{
    //    return obj switch
    //    {
    //        Group group => new GroupViewModel(
    //            group,
    //            package.Objects
    //                .FindAll(o => ReferenceEquals(o.Parent, obj))
    //                .Select(o => ObjectToViewModel(package, o))),
    //        Text text => new TextViewModel(text),
    //        SimpleImage simpleImage => new SimpleImageViewModel(simpleImage),
    //        Image image => new ImageViewModel(image),
    //        _ => throw new Exception($"Can't create view model for object of type: {obj.GetType()}")
    //    };
    //}

    private static ObjectViewModel RenderNodeToViewModel(RenderTreeNode node)
    {
        return node switch
        {
            RenderTreeGroup group => new GroupViewModel(group, group.Select(RenderNodeToViewModel)),
            RenderTreeText text => new TextViewModel(text),
            RenderTreeSimpleImage simpleImage => new SimpleImageViewModel(simpleImage),
            RenderTreeImage image => new ImageViewModel(image),
            _ => throw new Exception($"Can't create view model for node of type: {node.GetType()}")
        };
    }
}