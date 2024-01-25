using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FEngLib.Packages;
using FEngRender.Data;
using FEngViewer.WPF.UIHelpers;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace FEngViewer.WPF.ViewModels;

public class PackageViewModel : ObservableObject
{
    private Package? _package;
    private RenderPackageFacade _facade;
    private object? _currentEditingObject;
    private PropertyDefinitionCollection? _currentEditingPropertyDefinitions;

    public RenderPackageFacade Facade
    {
        get => _facade;
        set
        {
            SetProperty(ref _facade, value);
            OnPropertyChanged(nameof(TreeRoots));
        }
    }

    public ICommand SelectionChangedCommand { get; }

    public object? CurrentEditingObject
    {
        get => _currentEditingObject;
        set => SetProperty(ref _currentEditingObject, value);
    }

    public PropertyDefinitionCollection? CurrentEditingPropertyDefinitions
    {
        get => _currentEditingPropertyDefinitions;
        set => SetProperty(ref _currentEditingPropertyDefinitions, value);
    }

    public IList<object> TreeRoots => _facade != null ? new List<object> { _facade } : new List<object>();

    /// <summary>
    /// Design-time constructor
    /// </summary>
    public PackageViewModel()
    {
        SelectionChangedCommand = new RelayCommand<object>(ExecuteSelectionChange);
        if (/*DesignerProperties.GetIsInDesignMode(new DependencyObject())*/true)
        {
            Facade = new RenderPackageFacade(DummyData.TestPackage, DummyData.TestRenderTree);
        }
        else
        {
            Package package = LoadFile(@"D:\Games\NFS\Research\FNGs_ALL\mwfinal\BUSTED_OVERLAY.fng");
            Facade = new RenderPackageFacade(package, RenderTree.Create(package));
        }
    }

    private void ExecuteSelectionChange(object? obj)
    {
        if (obj is IEditable editable)
        {
            CurrentEditingObject = editable;
            CurrentEditingPropertyDefinitions = editable.GetPropertyDefinitions();
        }
        else
        {
            CurrentEditingObject = null;
        }
    }

    private static Package LoadFile(string path)
    {
        using var fs = new FileStream(path, FileMode.Open);
        using var fr = new BinaryReader(fs);
        var marker = fr.ReadUInt32();
        switch (marker)
        {
            case 0x30203:
                fs.Seek(0x10, SeekOrigin.Begin);
                break;
            case 0xE76E4546:
                fs.Seek(0x8, SeekOrigin.Begin);
                break;
            default:
                throw new InvalidDataException($"Invalid FEng chunk file: {path}");
        }

        using var ms = new MemoryStream();
        fs.CopyTo(ms);
        ms.Position = 0;

        using var mr = new BinaryReader(ms);
        return new FrontendPackageLoader().Load(mr);
    }
}