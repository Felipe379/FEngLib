using CommunityToolkit.Mvvm.ComponentModel;
using FEngLib.Messaging;

namespace FEngViewer.WPF.ViewModels;

public class MessageResponseViewModel : ObservableObject
{
    public MessageResponseViewModel(MessageResponse messageResponse)
    {
        MessageResponse = messageResponse;
    }

    public MessageResponse MessageResponse { get; }

    public string MessageName
    {
        // TODO: implement hash resolution
        get => $"0x{MessageResponse.Id:X}";
    }


}