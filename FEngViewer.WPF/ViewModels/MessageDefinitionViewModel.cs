using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FEngLib.Packages;

namespace FEngViewer.WPF.ViewModels;

public class MessageDefinitionViewModel : ObservableObject
{
    public MessageDefinitionViewModel(Package.MessageDefinition messageDefinition)
    {
        MessageDefinition = messageDefinition;
        TestCommand = new RelayCommand(ExecuteTestCommand);
        TestCommand2 = new RelayCommand(ExecuteTestCommand2, () => false);
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

    public ICommand TestCommand
    {
        get;
    }

    public ICommand TestCommand2
    {
        get;
    }

    private void ExecuteTestCommand()
    {
        MessageBox.Show("Hello!");
    }

    private void ExecuteTestCommand2()
    {
        MessageBox.Show("Hello!");
    }
}