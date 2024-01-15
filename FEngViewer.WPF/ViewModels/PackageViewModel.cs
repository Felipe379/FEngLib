using System.ComponentModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using FEngLib.Packages;
using FEngViewer.WPF.ViewModels.PackageView;

namespace FEngViewer.WPF.ViewModels;

public class PackageViewModel : ObservableObject
{
    private Package? _package;
    private PackageFacade? _facade;

    public Package? Package
    {
        get => _package;
        set
        {
            _package = value;
            Facade = value == null ? null : new PackageFacade(value);
        }
    }

    PackageFacade? Facade
    {
        get => _facade;
        set
        {
            SetProperty(ref _facade, value);
            OnPropertyChanged(nameof(TreeRoots));
        }
    }

    public IList<object> TreeRoots => _facade != null ? new List<object> { _facade } : new List<object>();

    /// <summary>
    /// Design-time constructor
    /// </summary>
    public PackageViewModel()
    {
        if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
        {
            Package = DummyData.TestPackage;
        }
    }
}