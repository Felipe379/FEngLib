using System.Collections.ObjectModel;

namespace FEngViewer.WPF.UIHelpers;

public abstract class TreeFolder<TChild>
{
    protected TreeFolder(string name, ObservableCollection<TChild> children)
    {
        Name = name;
        Children = children;
    }

    public string Name { get; }

    public ObservableCollection<TChild> Children { get; }
}