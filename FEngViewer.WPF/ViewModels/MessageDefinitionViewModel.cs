using CommunityToolkit.Mvvm.ComponentModel;
using FEngLib.Packages;

namespace FEngViewer.WPF.ViewModels;

public class MessageDefinitionViewModel : ObservableObject
{
    public MessageDefinitionViewModel(Package.MessageDefinition messageDefinition)
    {
        MessageDefinition = messageDefinition;
    }

    public Package.MessageDefinition MessageDefinition { get; }

    public string Name
    {
        get => MessageDefinition.Name;
        set => SetProperty(MessageDefinition.Name, value, MessageDefinition, (md, name) => md.Name = name);
    }

    public string Category
    {
        get => MessageDefinition.Category;
        set => SetProperty(MessageDefinition.Category, value, MessageDefinition, (md, cat) => md.Category = cat);
    }
}