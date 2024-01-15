using System.Collections.ObjectModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using FEngLib.Messaging;
using FEngLib.Objects;
using FEngLib.Packages;
using FEngViewer.WPF.UIHelpers;

namespace FEngViewer.WPF.ViewModels.PackageView;

public class ResourceRequestFolder : TreeFolder<ResourceRequestViewModel>
{
    public ResourceRequestFolder(ObservableCollection<ResourceRequestViewModel> children) : base("Resources", children)
    {
    }
}

public class MessageDefinitionsFolder : TreeFolder<MessageDefinitionViewModel>
{
    public MessageDefinitionsFolder(ObservableCollection<MessageDefinitionViewModel> children) : base("Message Definitions", children)
    {
    }
}

public class MessageResponsesFolder : TreeFolder<MessageResponseViewModel>
{
    public MessageResponsesFolder(ObservableCollection<MessageResponseViewModel> children) : base("Message Responses", children)
    { }
}

public class PackageFacade : ObservableObject
{
    private Package _package;

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

    public PackageFacade(Package package)
    {
        _package = package;
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
        Objects = new ObservableCollection<ObjectViewModel>(
            package.Objects
                .Where(o => o.Parent is null)
                .Select(o => ObjectToViewModel(package, o)));

        TreeNodes = new CompositeCollection
        {
            new CollectionContainer
            {
                Collection = new object[]
                {
                    new ResourceRequestFolder(ResourceRequests),
                    new MessageDefinitionsFolder(MessageDefinitions),
                    new MessageResponsesFolder(MessageResponses)
                }
            },
            new CollectionContainer { Collection = Objects }
        };
        //TreeNodes = new ObservableCollection<object>(treeNodes);
    }

    private ObjectViewModel ObjectToViewModel(Package package, IObject<ObjectData> obj)
    {
        return obj switch
        {
            Group group => new GroupViewModel(
                group,
                package.Objects
                    .FindAll(o => ReferenceEquals(o.Parent, obj))
                    .Select(o => ObjectToViewModel(package, o))),
            _ => new ObjectViewModel(obj)
        };
    }
}