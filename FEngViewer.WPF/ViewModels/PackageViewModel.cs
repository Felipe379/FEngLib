using System.ComponentModel;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using FEngLib.Packages;
using FEngRender.Data;

namespace FEngViewer.WPF.ViewModels;

public class PackageViewModel : ObservableObject
{
    private Package? _package;
    private RenderPackageFacade _facade;

    public RenderPackageFacade Facade
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