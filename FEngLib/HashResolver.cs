using FEngLib.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FEngLib
{
	public class HashResolver
	{
		private Dictionary<uint, HashSet<string>> Keys { get; } = new Dictionary<uint, HashSet<string>>();

		public HashResolver()
		{
			Keys = ReadKeys();
			LogWriteHashCollision();
		}

		public string ResolveNameHash(string name, uint hash, string logReference = null)
		{
			if (hash != 0)
			{
				if (string.IsNullOrEmpty(name) || Hashing.BinHash(name) != hash)
				{
					if (name != null && Hashing.BinHash(name.ToUpperInvariant()) == hash)
					{
						name = name.ToUpperInvariant();
					}
					else if (Keys.TryGetValue(hash, out var names))
					{
						name = string.Join(" / ", names);
					}
				}
			}

			return name;
		}

		private void LogWriteHashCollision()
		{
			var file = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\_Log_HashCollision.txt";

			StreamWriter unresolved;
			{
				unresolved = !File.Exists(file) ? new StreamWriter(file) : File.AppendText(file);

				foreach (var keyValue in Keys)
				{
					if (keyValue.Value.Count <= 1)
						continue;

					foreach (var value in keyValue.Value)
					{
						unresolved.WriteLine($"0x{keyValue.Key:x8} - {value}");
					}
				}
				unresolved.Close();
			}
		}


		private Dictionary<uint, HashSet<string>> ReadKeys()
		{
			var files = GetFiles($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\Keys");

			var keys = new HashSet<string>(ReadFiles(files));
			return FillDictionary(keys);
		}

		private IEnumerable<string> GetFiles(string directory)
		{
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			return Directory.GetFiles(directory, "*.txt", SearchOption.AllDirectories);
		}

		private HashSet<string> ReadFiles(IEnumerable<string> files)
		{
			var keys = new HashSet<string>();

			foreach (var file in files)
			{
				if (!File.Exists(file))
					continue;

				var lines = File.ReadAllLines(file);
				keys.UnionWith(lines);
			}

			keys.UnionWith(new HashSet<string>(keys.Select(c => c.ToUpperInvariant())));

			return keys;
		}

		private Dictionary<uint, HashSet<string>> FillDictionary(HashSet<string> keys)
		{
			var dictionary = new Dictionary<uint, HashSet<string>>();
			foreach (var key in keys)
			{
				AddNewKey(dictionary, key);
			}

			return dictionary;
		}

		private static void AddNewKey(Dictionary<uint, HashSet<string>> dictionary, string key)
		{
			var hash = Hashing.BinHash(key);
			if (!dictionary.ContainsKey(hash))
			{
				dictionary[hash] = new HashSet<string>();
			}

			dictionary[hash].Add(key);
		}

	}
}
