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

			var input = ObjectInput(existingObject.Name, existingObject is Group);

			if (string.IsNullOrWhiteSpace(input))
				return null;

			var newObject = existingObject.Clone() as IObject<ObjectData>;
			newObject.Name = input;
			newObject.NameHash = input.BinHash();
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

		public string ObjectInput(string defaultInput, bool isGroup)
		{
			var inputForm = new InputForm(CharacterCasing.Upper);
			inputForm.Input = defaultInput;
			if (inputForm.ShowDialog() != DialogResult.OK)
				return null;

			var inputHash = inputForm.Input.BinHash();

			if (_packageView._currentPackage.Objects.Any(x => x.Name == inputForm.Input || x.NameHash == inputHash))
			{
				if (isGroup)
				{
					var result = MessageBox.Show($"A group with the name {inputForm.Input} or hash 0x{inputHash:x8} already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return null;
				}
				else
				{
					var result = MessageBox.Show($"An object with the name {inputForm.Input} or hash 0x{inputHash:x8} already exists. Do you want to create it anyway?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
					if (result != DialogResult.Yes)
						return null;
				}
			}

			return inputForm.Input;
		}
	}
}
