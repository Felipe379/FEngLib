using System.Collections.ObjectModel;
using FEngViewer.WPF.UIHelpers;

namespace FEngViewer.WPF.ViewModels;

public class MessageResponsesFolder : TreeFolder<MessageResponseViewModel>
{
    public MessageResponsesFolder(ObservableCollection<MessageResponseViewModel> children) : base("Message Responses", children)
    { }
}