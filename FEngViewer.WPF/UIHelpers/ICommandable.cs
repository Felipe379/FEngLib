using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.IconPacks;

namespace FEngViewer.WPF.UIHelpers;

public abstract class CommandableOption
{
    protected CommandableOption(string label, PackIconFontAwesomeKind icon, IRelayCommand command)
    {
        Label = label;
        Icon = icon;
        Command = command;
    }

    public string Label { get; }

    public PackIconFontAwesomeKind Icon { get; }
    public IRelayCommand Command { get; }
}

public class CommandOption : CommandableOption
{

    public CommandOption(string label, PackIconFontAwesomeKind icon, IRelayCommand command) : base(label, icon, command)
    {
    }
}

public class MultiCommandOption : CommandableOption
{
    private static readonly IRelayCommand _noOpCommand = new RelayCommand(() => { });

    public ICollection SubCommands { get; }

    public MultiCommandOption(string label, PackIconFontAwesomeKind icon, ICollection subCommands) : base(label, icon, _noOpCommand)
    {
        SubCommands = subCommands;
    }
}

public class CommandableOptionTemplateSelector : DataTemplateSelector
{
    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        if (container is FrameworkElement frameworkElement && item is CommandableOption commandableOption)
        {
            
        }
        return base.SelectTemplate(item, container);
    }
}

public interface ICommandable
{
    ICollection Commands { get; }
}