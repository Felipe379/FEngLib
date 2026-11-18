using System;
using System.Collections.Generic;
using System.IO;
using FEngLib.Packages;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using FEngLib;

namespace FEngViewer;

public class AppService
{
    private static readonly Lazy<AppService> LazyInstance = new(() => new AppService());
    private Package _currentPackage;

	public bool IsJson;
	public string CurrentFileLoaded;
	public HashResolver HashResolver;

    private AppService()
    {
		HashResolver = new HashResolver();
    }

    public static AppService Instance => LazyInstance.Value;

	public Package Load(string path, bool isJson)
	{
		return isJson ? LoadJson(path) : LoadFile(path);
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

	private static Package LoadJson(string path)
	{
		return JsonConvert.DeserializeObject<Package>(File.ReadAllText(path), new JsonSerializerSettings
		{
			Formatting = Formatting.Indented,
			Converters = new List<JsonConverter>
			{
				new StringEnumConverter()
			},
			TypeNameHandling = TypeNameHandling.Auto,
			ReferenceLoopHandling = ReferenceLoopHandling.Error,
			PreserveReferencesHandling = PreserveReferencesHandling.Objects,
			NullValueHandling = NullValueHandling.Ignore
		}) ?? throw new InvalidDataException($"Invalid FEng JSON file: {path}");
	}

	public void SetCurrentPackage(Package package, string path, bool isJson)
	{
		CurrentFileLoaded = path;
		IsJson = isJson;
		_currentPackage = package;
	}

	public List<ResourceRequest> GetResourceRequests()
    {
        return _currentPackage?.ResourceRequests ??
               throw new NullReferenceException("Received a request for resource requests when no package is loaded!");
    }
}
