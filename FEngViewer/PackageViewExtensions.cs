using FEngLib.Objects;
using FEngLib.Scripts;
using FEngLib.Utils;
using FEngViewer.Prompt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FEngViewer
{
	public class PackageViewExtensions
	{
		private readonly PackageView _packageView;

		public PackageViewExtensions(PackageView packageView)
		{
			_packageView = packageView;
		}

		public IObject<ObjectData> ObjectSelected { get; set; }
		public bool ShouldCopyObject { get; set; }

		public string SearchText { get; set; }
		public int SearchIndex { get; set; }

		public static string GetObjectTreeKey(IObject<ObjectData> objectData) => objectData != null ? $"{GetObjectText(objectData)}:{objectData.Guid:X}" : null;
		public static string GetObjectText(IObject<ObjectData> objectData) => objectData != null ? GetHashText(objectData.Name, objectData.NameHash) : null;

		public static string ResolveName(string name, uint hash) => name ?? AppService.Instance.HashResolver.ResolveNameHash(null, hash);
		public static string GetHashText(string name, uint hash) => name ?? $"{hash} (0x{hash:X}) {ResolveName(null, hash) ?? "-----"}";

		public static void AddUserKey(string name, uint hash)
		{
			if (!string.IsNullOrEmpty(name))
				AppService.Instance.HashResolver.AddUserKey(name, hash);
		}

		public IObject<ObjectData> CopyObject(IObject<ObjectData> existingObject, IObject<ObjectData> selectedObject, int? index = null, string objectName = null, string objectNumber = null, int? objectCount = null)
		{
			if (existingObject is null)
				return null;

			var fullObjectName = objectName;
			var doesItEndWithANumber = !string.IsNullOrWhiteSpace(objectNumber);
			if (doesItEndWithANumber)
				fullObjectName += FormatNumber(objectCount, objectNumber);

			var fullOjbectNameExists = !string.IsNullOrWhiteSpace(fullObjectName);

			var objectInput = fullOjbectNameExists
				? (fullObjectName, fullObjectName.BinHash(), existingObject is Group)
				: ObjectInput(ResolveName(existingObject.Name, existingObject.NameHash), existingObject is Group);

			if (!objectInput.NameHash.HasValue)
				return null;

			if (doesItEndWithANumber && NameAlreadyExists(objectInput.Name, objectInput.NameHash.Value, existingObject, null, fullOjbectNameExists))
				return null;

			var parent = selectedObject;

			var newObject = CreateNewObject(existingObject, parent, objectInput.Name, objectInput.NameHash.Value);

			if (newObject is null)
				return null;

			var validIndex = index ?? _packageView._currentPackage.Objects.Count;
			var newObjectDictionary = new Dictionary<int, IObject<ObjectData>>();

			if (objectInput.CreateChildren && newObject is Group)
			{
				newObjectDictionary = CreateChildren(existingObject, newObject, validIndex, objectNumber, objectCount);
			}

			newObjectDictionary.Add(validIndex, newObject);

			foreach (var pair in newObjectDictionary.OrderBy(p => p.Key))
			{
				_packageView._currentPackage.Objects.Insert(pair.Key, FixClonedObject(pair.Value));
				AddUserKey(pair.Value.Name, pair.Value.NameHash);
			}

			return newObject;
		}

		public IObject<ObjectData> CreateNewObject(IObject<ObjectData> existingObject, IObject<ObjectData> parent, string name, uint nameHash)
		{
			var newObject = existingObject.Clone() as IObject<ObjectData>;
			newObject.Name = name;
			newObject.NameHash = nameHash;
			newObject.Parent = parent;

			return newObject;
		}

		public IObject<ObjectData> FixClonedObject(IObject<ObjectData> newObject)
		{
			var oldGuid = newObject.Guid;

			while (_packageView._currentPackage.Objects.Find(x => x.Guid == newObject.Guid) is not null)
			{
				newObject.Guid++;
			}

			foreach (var targetList in _packageView._currentPackage.MessageTargetLists)
			{
				if (targetList.Targets.Contains(oldGuid))
					targetList.Targets.Add(newObject.Guid);
			}

			return newObject;
		}

		private static string FormatNumber(int? number, string originalNumber) => number?.ToString().PadLeft(originalNumber?.Length ?? 0, '0');

		public static bool IsSelfOrAncestor(IObject<ObjectData> ancestor, IObject<ObjectData> objectData)
		{
			for (var current = objectData; current is not null; current = current.Parent)
			{
				if (current == ancestor)
					return true;
			}

			return false;
		}

		public static (string Name, string Number) SplitTrailingNumber(string input)
		{
			int index = input.Length;

			while (index > 0 && char.IsDigit(input[index - 1]))
				index--;

			return index == input.Length
				? (input, string.Empty)
				: (input[..index], input[index..]);
		}

		public Dictionary<int, IObject<ObjectData>> CreateChildren(IObject<ObjectData> objectData, IObject<ObjectData> newObjectData, int index, string objectNumber = null, int? objectCount = null)
		{
			var children = _packageView._currentPackage.Objects.FindAll(x => x.Parent?.NameHash == objectData.NameHash && x.Parent?.Guid == objectData.Guid);
			var newChildren = new Dictionary<int, IObject<ObjectData>>();
			foreach (var child in children)
			{
				index++;
				var childName = child.Name;
				var childNameHash = child.NameHash;
				var resolvedChildName = ResolveName(child.Name, child.NameHash);
				if (!string.IsNullOrWhiteSpace(resolvedChildName) && objectCount.HasValue && !string.IsNullOrEmpty(objectNumber))
				{
					var splitName = SplitTrailingNumber(resolvedChildName);
					if (splitName.Number == objectNumber)
					{
						childName = $"{splitName.Name}{FormatNumber(objectCount, objectNumber)}";
						childNameHash = childName.BinHash();
					}
				}

				var newChild = CreateNewObject(child, newObjectData, childName, childNameHash);
				newChildren.Add(index, newChild);

				if (newChild is Group)
				{
					var groupChildren = CreateChildren(child, newChild, index, objectNumber, objectCount);
					foreach (var groupChild in groupChildren)
						newChildren.Add(groupChild.Key, groupChild.Value);

					if (groupChildren.Count != 0)
						index = groupChildren.Keys.Max();
				}
			}

			return newChildren;
		}

		public void DeleteObject(IObject<ObjectData> objectData)
		{
			if (objectData is Group)
			{
				foreach (var child in _packageView._currentPackage.Objects.FindAll(x => x.Parent == objectData))
					DeleteObject(child);
			}

			if (ObjectSelected == objectData)
				ObjectSelected = null;

			foreach (var targetList in _packageView._currentPackage.MessageTargetLists)
				targetList.Targets.RemoveAll(guid => guid == objectData.Guid);

			_packageView._currentPackage.Objects.Remove(objectData);
		}

		public IObject<ObjectData> FindLastChild(IObject<ObjectData> objectData)
		{
			if (objectData is null)
				return null;

			if (objectData is not Group)
				return objectData;

			var children = _packageView._currentPackage.Objects.FindAll(x => x.Parent?.NameHash == objectData.NameHash && x.Parent?.Guid == objectData.Guid);
			var lastChild = objectData;

			foreach (var child in children)
			{
				lastChild = child;

				if (child is Group)
					lastChild = FindLastChild(child);
			}

			return lastChild;
		}

		public int MoveChildren(IObject<ObjectData> parent)
		{
			var index = _packageView._currentPackage.Objects.IndexOf(parent) + 1;
			var children = _packageView._currentPackage.Objects.FindAll(x => x.Parent?.NameHash == parent.NameHash && x.Parent?.Guid == parent.Guid);

			foreach (var child in children)
			{
				var childIndex = _packageView._currentPackage.Objects.IndexOf(child);
				MoveItem(childIndex, index);

				index += childIndex > index ? 1 : 0;

				if (child is Group)
					index = MoveChildren(child);
			}

			return index;
		}

		public void MoveItem(int oldIndex, int newIndex)
		{
			var item = _packageView._currentPackage.Objects[oldIndex];

			_packageView._currentPackage.Objects.RemoveAt(oldIndex);

			if (newIndex > oldIndex)
				newIndex--;

			_packageView._currentPackage.Objects.Insert(newIndex, item);
		}

		public (string Name, uint? NameHash, bool CreateChildren) ObjectInput(string defaultInput, bool isGroup)
		{
			using var inputForm = new InputForm(CharacterCasing.Upper, isGroup);
			inputForm.Input = defaultInput;
			inputForm.CreateChildren = isGroup;
			if (inputForm.ShowDialog() != DialogResult.OK)
				return (null, null, false);

			var input = string.Empty;
			uint inputHash = 0;
			if (inputForm.Input.IsHash(16))
			{
				inputHash = Convert.ToUInt32(inputForm.Input, 16);
				input = ResolveName(null, inputHash);
			}
			else if (!string.IsNullOrEmpty(inputForm.Input))
			{
				input = inputForm.Input;
				inputHash = inputForm.Input.BinHash();
			}
			else
			{
				return (null, null, false);
			}

			return (input, inputHash, inputForm.CreateChildren);
		}

		public bool NameAlreadyExists(string name, uint hash, IObject<ObjectData> data, object tag = null, bool skipExistingNames = false)
		{
			if (tag is Script collection)
			{
				var scripts = data.GetScripts();

				if (scripts.Any(s => s.Id == hash || (!string.IsNullOrEmpty(name) && s.Name == name)))
				{
					if (skipExistingNames)
						return true;

					var result = MessageBox.Show($"A script with the name {name} or hash 0x{hash:x8} already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return true;
				}
			}
			else
			{
				if (_packageView._currentPackage.Objects.Any(x => x.NameHash == hash || (!string.IsNullOrEmpty(name) && x.Name == name)))
				{
					if (skipExistingNames)
						return true;

					var result = MessageBox.Show($"An object with the name {name} or hash 0x{hash:x8} already exists. Do you want to create it anyway?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
					return result != DialogResult.Yes;
				}
			}

			return false;
		}
	}
}
