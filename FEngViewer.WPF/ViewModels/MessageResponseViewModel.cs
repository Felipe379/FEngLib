using CommunityToolkit.Mvvm.ComponentModel;
using FEngLib.Messaging;
using FEngViewer.WPF.UIHelpers;

namespace FEngViewer.WPF.ViewModels;

public class MessageResponseViewModel : ObservableObject, INamedEntity
{
    public MessageResponseViewModel(MessageResponse messageResponse)
    {
        MessageResponse = messageResponse;
    }

    public MessageResponse MessageResponse { get; }

    public string Name
    {
        get
        {
            return $"0x{MessageResponse.Id:X}";
        }
        set
        {
            throw new NotImplementedException("Setting message names is currently not supported");
        }
    }

    public bool IsNameExplicit
    {
        get
        {
            return false;
        }
    }
}