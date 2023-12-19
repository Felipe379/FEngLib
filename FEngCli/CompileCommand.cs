using CommandLine;
using FEngLib;
using FEngLib.Objects;
using FEngLib.Packages;
using FEngLib.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FEngCli;

[Verb("compile")]
public class CompileCommand : BaseCommand
{
	public struct Counter
	{
		public Counter(string name, int count)
		{
			this.Name = name;
			this.Count = count;
		}

		public string Name;
		public int Count;
	}

	[Option('i')] public IEnumerable<string> InputPath { get; set; }

	[Option('o')] public string OutputPath { get; set; }

	private static void AssignGUID(Package package, IObject<ObjectData> @object, uint oldGUID)
	{
		var guid = oldGUID;

		while (package.Objects.Find(x => x.Guid == guid) is not null)
		{
			guid++;
		}

		Console.WriteLine($"Assigning object GUID {guid}.");

		@object.Guid = guid;
	}

	private static (IObject<ObjectData>, int) FindLastObject(List<IObject<ObjectData>> objects, string name)
	{
		IObject<ObjectData> result = null;
		var j = 1;

		Debug.Assert(objects.Find(x => x.NameHash == $"{name}0".BinHash()) is null);

		while (true)
		{
			var @object = objects.Find(x => x.NameHash == $"{name}{j}".BinHash());

			if (@object is not null)
			{
				j++;
				result = @object;
			}

			else
			{
				j--;
				break;
			}
		}

		Console.WriteLine($"Highest source object for {name} was {name}{j}.");

		return (result, j);
	}

	private static void AddObjectToMessageTargetLists(Package package, IObject<ObjectData> oldObject, IObject<ObjectData> newObject)
	{
		foreach (var targetList in package.MessageTargetLists)
		{
			if (targetList.Targets.Contains(oldObject.Guid))
			{
				Console.WriteLine($"Adding object {newObject.NameHash} to MessageTargetList for {targetList.MsgId}...");

				targetList.Targets.Add(newObject.Guid);
			}
		}
	}

	private static void AssignNameHash(IObject<ObjectData> @object, int i)
	{
		switch (@object.NameHash)
		{
			case 0xE18FA36D: // CHECK_MARK_70
				@object.NameHash = $"CHECK_MARK_{i}".BinHash();
				return;

			case 0x79AD2624: // LOCKED_FABRICATOR_NEW_ICON_70
				@object.NameHash = $"LOCKED_FABRICATOR_NEW_ICON_{i}".BinHash();
				return;
		}

		var key = @object.NameHash;

		Console.WriteLine($"Unhandled child key {key}.");

		var ii = 0;
	}

	private static void RemoveStrings(Package[] packages)
	{
		foreach (var package in packages)
		{
			foreach (var @object in package.Objects)
			{
				var text = @object as Text;

				if (text is not null && text.Value is not null && text.Value != string.Empty)
				{
					Console.WriteLine($"Removing Text object string {text.Value}...");

					text.Value = string.Empty;
				}
			}
		}
	}

	private static void CopyObjectsCustomize(Package package)
	{
		if (package is not null && package.Name == "FeCustomizeParts.fng")
		{
			var name = "SUB_OPTION_";

			(var oldSubOption, var index) = FindLastObject(package.Objects, name);
			var children = package.Objects.FindAll(x => x.Parent == oldSubOption);

			Debug.Assert(children.Count == 6);

			for (var i = index + 1; i < 150; ++i)
			{
				var indexedName = $"{name}{i}";
				var indexedKey = indexedName.BinHash();

				// ensure object doesn't exist
				var @object = package.Objects.Find(x => x.NameHash == indexedKey);

				Debug.Assert(@object is null);

				// create object
				Console.WriteLine($"Creating object {indexedName}...");

				// allocate
				var newSubOption = oldSubOption.Clone() as IObject<ObjectData>;

				// set key
				newSubOption.NameHash = indexedKey;

				// set guid
				AssignGUID(package, newSubOption, oldSubOption.Guid);

				// add to package
				package.ResourceRequests.Add(newSubOption.ResourceRequest);
				package.Objects.Add(newSubOption);

				// add to message target lists
				AddObjectToMessageTargetLists(package, oldSubOption, newSubOption);

				// copy all children
				foreach (var oldChild in children)
				{
					// ensure object has no children
					Debug.Assert(package.Objects.FindAll(x => x.Parent == oldChild).Count == 0);

					// create object
					Console.WriteLine($"Creating child object of {indexedName} with key {oldChild.NameHash}...");

					// allocate
					var newChild = oldChild.Clone() as IObject<ObjectData>;

					// set key
					AssignNameHash(newChild, i);

					// set parent
					newChild.Parent = newSubOption;

					// set guid
					AssignGUID(package, newChild, oldChild.Guid);

					// add to message target lists
					AddObjectToMessageTargetLists(package, oldChild, newChild);

					// add to package
					package.ResourceRequests.Add(newChild.ResourceRequest);
					package.Objects.Add(newChild);
				}
			}

			// parents of SUB_OPTION_ are correct
			var parentA = package.Objects.Find(x => x.NameHash == Hashing.BinHash("SUB_OPTION_1")); // 993
			var parentB = package.Objects.Find(x => x.NameHash == Hashing.BinHash("SUB_OPTION_2")); // 994
			var parentC = package.Objects.Find(x => x.NameHash == Hashing.BinHash("SUB_OPTION_71")); // 728

			// parents of CHECK_MARK_
			var checkMarkA = package.Objects.Find(x => x.NameHash == Hashing.BinHash("CHECK_MARK_1")); // 993
			var checkMarkB = package.Objects.Find(x => x.NameHash == Hashing.BinHash("CHECK_MARK_2")); // 994
			var checkMarkC = package.Objects.Find(x => x.NameHash == Hashing.BinHash("CHECK_MARK_71")); // 728

			var ii = 0;
		}
	}

	private static void CopyObjectsWorldMap(Package package)
	{
		if (package is not null && package.Name == "WorldMapMain.fng")
		{
			foreach (var counter in new[]
			{
				new Counter("RANDOM_DESTINATION_", 25), // encounter, 50 destinations, 25 at a time
				new Counter("MMICON_PUR_KNOCKOUT_", 10), // pursuit knockout, 5 in vanilla
				new Counter("MMICON_PUR_TAG_", 10), // pursuit tag, ^
				new Counter("MMICON_CANYON_", 20), // canyon duel and sprint, 10 + 10 in vanilla
				new Counter("MMICON_CANYONDRIFT_", 10), // canyon drift, 10 in vanilla
				new Counter("MMICON_DRIFT_", 30), // drift, 12 in vanilla
				new Counter("MMICON_SPEEDTRAP_", 30), // speedtrap, 10 in vanilla
				new Counter("MMICON_TOLLBOOTH_", 30), // checkpoint, 10 in vanilla
				new Counter("MMICON_KNOCKOUT_", 10), // knockout, 6 in vailla
				new Counter("MMICON_CIRCUIT_", 50), // circuit, 24 in vanilla
				new Counter("MMICON_SPRINT_", 50), // sprint, 29 in vanilla
				new Counter("SAFEHOUSEINDICATOR_", 10), // safe houses, 4 in vanilla
				new Counter("CAR_LOT_", 10), // car lots, 0 in vanilla?
				new Counter("HIDING_SPOT_", 100), // hiding spots, 32 in vanilla
				new Counter("PURSUIT_BREAKER_", 100), // pursuit breakers, 34 in vanilla
			})
			{
				IObject<ObjectData> lastObject = null;

				var j = 0;

				while (lastObject is null)
				{
					var name = $"{counter.Name}{j}";
					var key = name.BinHash();

					lastObject = package.Objects.Find(x => x.NameHash == key);

					if (lastObject is null)
					{
						j++;
					}
				}

				Console.WriteLine($"Highest source object for {counter.Name} was {j}.");

				for (var i = 0; i < counter.Count; ++i)
				{
					var name = $"{counter.Name}{i}";
					var key = name.BinHash();

					var @object = package.Objects.Find(x => x.NameHash == key);

					if (@object is null)
					{
						Console.WriteLine($"Creating FEObject {name}...");

						var newObject = lastObject.Clone() as IObject<ObjectData>;

						// no parent change required.
						newObject.NameHash = key;

						AssignGUID(package, newObject, lastObject.Guid);

						package.ResourceRequests.Add(newObject.ResourceRequest);
						package.Objects.Add(newObject);
					}

					else
					{
						Console.WriteLine($"{name} already exists, continuing...");
					}

					var newColor = @object.Data.Color;

					if (counter.Name is not "HIDING_SPOT_" and not "PURSUIT_BREAKER_" and not "RANDOM_DESTINATION_")
					{
						Console.WriteLine($"{name} is not special, setting alpha to 0...");

						newColor.Alpha = 0;
					}

					@object.Data.Color = newColor;
				}
			}
		}
	}

	public override int Execute()
	{
		var packages = InputPath.Select(path => JsonConvert.DeserializeObject<Package>(File.ReadAllText(path), new JsonSerializerSettings
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
		})).ToArray();

		//NFSCO specific
		//RemoveStrings(packages);
		//CopyObjectsCustomize(Array.Find(packages, x => x.Name == "FeCustomizeParts.fng"));
		//CopyObjectsWorldMap(Array.Find(packages, x => x.Name == "WorldMapMain.fng"));

		using var bw = new BinaryWriter(File.Create(OutputPath));

		foreach (var package in packages)
		{
			using var tms = new MemoryStream();
			using var tbw = new BinaryWriter(tms);
			new FrontendChunkWriter(package).Write(tbw);
			tms.Position = 0;

			bw.Write(0x30203);
			bw.Write((uint)tms.Length);
			tms.CopyTo(bw.BaseStream);
		}

		return 0;
	}
}
