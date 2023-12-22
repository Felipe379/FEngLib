using FEngLib.Objects;
using FEngLib.Utils;
using FEngRender.Data;
using FEngViewer.Prompt;
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
		public static string GetObjectText(IObject<ObjectData> objectData) => objectData != null ? $"{objectData.Name ?? objectData.NameHash.ToString("X")}" : null;

		public IObject<ObjectData> GetCurrentSelectedObject(bool? objectReturnIsGroup = null)
		{
			if (_packageView.treeView1.SelectedNode?.Tag is not RenderTreeNode node)
				return null;

			var nodeObject = node.GetObject();

			if (objectReturnIsGroup.HasValue)
			{
				if (nodeObject is Group grp && !objectReturnIsGroup.Value)
					return null;
			}

			return _packageView._currentPackage.Objects.Find(x => x.NameHash == nodeObject.NameHash && x.Guid == nodeObject.Guid);
		}

		public IObject<ObjectData> CopyObject(IObject<ObjectData> existingObject, IObject<ObjectData> parent)
		{
			if (existingObject is null)
				return null;

			var objectInput = ObjectInput(existingObject.Name, existingObject is Group);

			if (string.IsNullOrWhiteSpace(objectInput.Input))
				return null;

			var newObject = CreateNewObject(existingObject, parent, objectInput.Input, objectInput.Input.BinHash());

			if (newObject is null)
				return null;

			_packageView._currentPackage.ResourceRequests.Add(newObject.ResourceRequest);
			_packageView._currentPackage.Objects.Add(newObject);

			if (objectInput.CreateChildren && newObject is Group)
			{
				CreateChildren(existingObject, newObject);
			}

			return newObject;
		}

		public IObject<ObjectData> CreateNewObject(IObject<ObjectData> existingObject, IObject<ObjectData> parent, string name, uint nameHash)
		{
			var newObject = existingObject.Clone() as IObject<ObjectData>;
			newObject.Name = name;
			newObject.NameHash = nameHash;
			newObject.Parent = parent;

			var guid = existingObject.Guid;

			while (_packageView._currentPackage.Objects.Find(x => x.Guid == newObject.Guid) is not null)
			{
				newObject.Guid = guid++;
			}

			foreach (var targetList in _packageView._currentPackage.MessageTargetLists)
			{
				if (targetList.Targets.Contains(existingObject.Guid))
					targetList.Targets.Add(newObject.Guid);
			}

			return newObject;
		}

		public void CreateChildren(IObject<ObjectData> objectData, IObject<ObjectData> parent)
		{
			var children = _packageView._currentPackage.Objects.FindAll(x => x.Parent == objectData);

			foreach (var child in children)
			{
				var newChild = CreateNewObject(child, parent, child.Name, child.NameHash);
				_packageView._currentPackage.ResourceRequests.Add(newChild.ResourceRequest);
				_packageView._currentPackage.Objects.Add(newChild);

				if (newChild is Group)
				{
					CreateChildren(child, newChild);
				}
			}
		}

		public void DeleteObject(IObject<ObjectData> objectData)
		{
			if (objectData is Group)
			{
				var children = _packageView._currentPackage.Objects.FindAll(x => x.Parent == objectData);

				foreach (var child in children)
				{
					if (child is Group)
						DeleteObject(child);
					else
					{
						if (ObjectSelected == child)
							ObjectSelected = null;
						_packageView._currentPackage.Objects.Remove(child);
					}
				}
			}

			if (ObjectSelected == objectData)
				ObjectSelected = null;

			_packageView._currentPackage.Objects.Remove(objectData);
		}

		public (string Input, bool CreateChildren) ObjectInput(string defaultInput, bool isGroup)
		{
			var inputForm = new InputForm(CharacterCasing.Upper, isGroup);
			inputForm.Input = defaultInput;
			inputForm.CreateChildren = isGroup;
			if (inputForm.ShowDialog() != DialogResult.OK)
				return (null, false);

			var inputHash = inputForm.Input.BinHash();

			if (_packageView._currentPackage.Objects.Any(x => x.Name == inputForm.Input || x.NameHash == inputHash))
			{
				var result = MessageBox.Show($"An object with the name {inputForm.Input} or hash 0x{inputHash:x8} already exists. Do you want to create it anyway?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				if (result != DialogResult.Yes)
					return (null, false);
			}

			return (inputForm.Input, inputForm.CreateChildren);
		}
	}
}
