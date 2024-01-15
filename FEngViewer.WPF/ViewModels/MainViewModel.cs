using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using FEngLib.Packages;
using FEngViewer.WPF.Services;
using Microsoft.Win32;

namespace FEngViewer.WPF.ViewModels;

public class MainViewModel
{
    private readonly ITestService _testService = null!;
    private ICommand? _openCommand;

    public ICommand OpenCommand
    {
        get
        {
            return _openCommand ??= new RelayCommand(ExecuteOpenCommand);
        }
    }

    /// <summary>
    /// Design time view-model constructor
    /// </summary>
    public MainViewModel()
    {
        //
    }

    /// <summary>
    /// Runtime view-model constructor
    /// </summary>
    /// <param name="testService"></param>
    public MainViewModel(ITestService testService)
    {
        _testService = testService;
    }

    private void ExecuteOpenCommand()
    {
        var ofd = new OpenFileDialog
        {
            Filter = "FNG Files (*.fng)|*.fng|All files (*.*)|*.*",
            CheckFileExists = true,
        };

        if (ofd.ShowDialog() == true)
        {
            LoadPackage(ofd.FileName);
        }
    }

    // TODO: Move this to a service or something!
    private void LoadPackage(string packagePath)
    {
        using var fs = new FileStream(packagePath, FileMode.Open);
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
                throw new InvalidDataException($"Invalid FEng chunk file: {packagePath}");
        }

        using var ms = new MemoryStream();
        fs.CopyTo(ms);
        ms.Position = 0;

        using var mr = new BinaryReader(ms);

        Package package = new FrontendPackageLoader().Load(mr);
        Debugger.Break();
    }
}