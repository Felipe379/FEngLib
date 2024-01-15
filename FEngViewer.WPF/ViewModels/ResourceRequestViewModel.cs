using CommunityToolkit.Mvvm.ComponentModel;
using FEngLib.Packages;
using MahApps.Metro.IconPacks;

namespace FEngViewer.WPF.ViewModels;

public class ResourceRequestViewModel : ObservableObject
{
    public ResourceRequest ResourceRequest { get; }

    public uint ID
    {
        get => ResourceRequest.ID;
        set => SetProperty(ResourceRequest.ID, value, ResourceRequest, (rr, id) => rr.ID = id);
    }

    public string Name
    {
        get => ResourceRequest.Name;
        set => SetProperty(ResourceRequest.Name, value, ResourceRequest, (rr, name) => rr.Name = name);
    }

    public ResourceType Type
    {
        get => ResourceRequest.Type;
        set
        {
            SetProperty(ResourceRequest.Type, value, ResourceRequest, (rr, type) => rr.Type = type);
            OnPropertyChanged(nameof(Icon));
        }
    }

    public PackIconFontAwesomeKind Icon
    {
        get
        {
            switch (Type)
            {
                case ResourceType.Image:
                    return PackIconFontAwesomeKind.ImageSolid;
                case ResourceType.Font:
                    return PackIconFontAwesomeKind.FontSolid;
                case ResourceType.Movie:
                    return PackIconFontAwesomeKind.FileVideoSolid;
                case ResourceType.MultiImage:
                    return PackIconFontAwesomeKind.ImagesSolid;
                default:
                    return PackIconFontAwesomeKind.FileSolid;
            }
        }
    }

    // <iconPacks:PackIconFontAwesome Kind="FontSolid" />


    public ResourceRequestViewModel(ResourceRequest resourceRequest)
    {
        ResourceRequest = resourceRequest;
    }
}