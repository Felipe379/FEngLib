using CommandLine;
using FEngLib;
using FEngLib.Objects;
using FEngLib.Packages;
using FEngLib.Scripts;
using FEngLib.Structures;
using FEngLib.Utils;
using FEngRender.Data;
using FEngViewer.Properties;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Image = FEngLib.Objects.Image;

namespace FEngViewer;

public partial class PackageView : Form
{
	internal Package _currentPackage;
	internal RenderTree _currentRenderTree;
	private PackageViewExtensions _packageViewExtensions;

	private TreeNode _rootNode;

	public PackageView()
	{
		InitializeComponent();
		_packageViewExtensions = new PackageViewExtensions(this);
		LblDetails.Text = string.Empty;
		//SetLabelDetails();

		var imageList = new ImageList();
		imageList.Images.Add("TreeItem_Package", Resources.TreeItem_Package);
		imageList.Images.Add("TreeItem_String", Resources.TreeItem_String);
		imageList.Images.Add("TreeItem_Group", Resources.TreeItem_Group);
		imageList.Images.Add("TreeItem_Image", Resources.TreeItem_Image);
		imageList.Images.Add("TreeItem_Script", Resources.TreeItem_Script);
		imageList.Images.Add("TreeItem_ScriptTrack", Resources.TreeItem_ScriptTrack);
		imageList.Images.Add("TreeItem_ScriptEvent", Resources.TreeItem_ScriptEvent);
		imageList.Images.Add("TreeItem_Movie", Resources.TreeItem_Movie);
		imageList.Images.Add("TreeItem_ColoredImage", Resources.TreeItem_ColoredImage);
		imageList.Images.Add("TreeItem_MultiImage", Resources.TreeItem_MultiImage);
		imageList.Images.Add("TreeItem_ObjectList", Resources.TreeItem_ObjectList);
		imageList.Images.Add("TreeItem_ResourceList", Resources.TreeItem_ResourceList);
		imageList.Images.Add("TreeItem_GenericResource", Resources.TreeItem_GenericResource);
		imageList.Images.Add("TreeItem_Font", Resources.TreeItem_Font);
		imageList.Images.Add("TreeItem_Keyframe", Resources.TreeItem_Keyframe);
		treeView1.ImageList = imageList;
	}

	private void PackageView_Load(object sender, EventArgs e)
	{
		var args = Environment.GetCommandLineArgs();

		var opts = Parser.Default.ParseArguments<Options>(args);

		opts.WithParsed(parsed => LoadFile(parsed.InputFile, false))
			.WithNotParsed(err => Application.Exit());
	}

	private void Render()
	{
		viewOutput.Render(_currentRenderTree);
	}


	private void PopulateTreeView(Package package, IEnumerable<RenderTreeNode> feObjectNodes)
	{
		// map group guid to children guids
		treeView1.BeginUpdate();
		treeView1.Nodes.Clear();

		_rootNode = treeView1.Nodes.Add(package.Name);

		var resourceListNode = _rootNode.Nodes.Add("Resources");
		resourceListNode.ImageKey = resourceListNode.SelectedImageKey = "TreeItem_ResourceList";

		foreach (var resourceRequest in package.ResourceRequests)
		{
			var resourceRequestNode = resourceListNode.Nodes.Add(resourceRequest.Name);

			resourceRequestNode.ImageKey = resourceRequestNode.SelectedImageKey = resourceRequest.Type switch
			{
				ResourceType.Image => "TreeItem_Image",
				ResourceType.MultiImage => "TreeItem_MultiImage",
				ResourceType.Movie => "TreeItem_Movie",
				ResourceType.Font => "TreeItem_Font",
				_ => "TreeItem_GenericResource"
			};
		}

		//var objectListNode = rootNode.Nodes.Add("Objects");
		//objectListNode.ImageKey = "TreeItem_ObjectList";
		ApplyObjectsToTreeNodes(feObjectNodes, _rootNode.Nodes);

		_rootNode.Expand();
		// treeView1.ExpandAll();
		treeView1.EndUpdate();

		treeView1.SelectedNode = _rootNode;
	}

	private static void ApplyObjectsToTreeNodes(IEnumerable<RenderTreeNode> objectNodes,
		TreeNodeCollection treeNodes)
	{
		foreach (var feObjectNode in objectNodes)
		{
			var objTreeNode = CreateObjectTreeNode(treeNodes, feObjectNode);
			if (feObjectNode is RenderTreeGroup grp)
				ApplyObjectsToTreeNodes(grp, objTreeNode.Nodes);
		}
	}

	private static TreeNode CreateObjectTreeNode(TreeNodeCollection collection, RenderTreeNode viewNode)
	{
		var feObj = viewNode.GetObject();
		var nodeImageKey = feObj.Type switch
		{
			ObjectType.String => "TreeItem_String",
			ObjectType.Image => "TreeItem_Image",
			ObjectType.Group => "TreeItem_Group",
			ObjectType.Movie => "TreeItem_Movie",
			ObjectType.ColoredImage => "TreeItem_ColoredImage",
			ObjectType.MultiImage => "TreeItem_MultiImage",
			_ => null
		};

		var nodeText = $"{feObj.Name ?? feObj.NameHash.ToString("X")}";

		if (nodeImageKey == null)
		{
			nodeText = feObj.Type + " " + nodeText;
		}

		var objTreeNode = collection.Add(nodeText);
		objTreeNode.Tag = viewNode;
		objTreeNode.Name = nodeText;
		if (nodeImageKey != null) objTreeNode.ImageKey = objTreeNode.SelectedImageKey = nodeImageKey;

		foreach (var script in feObj.GetScripts()) CreateScriptTreeNode(objTreeNode.Nodes, script);

		return objTreeNode;
	}

	private static void CreateScriptTreeNode(TreeNodeCollection collection, Script script)
	{
		var node = collection.Add(script.Name ?? $"0x{script.Id:X}");
		// ReSharper disable once LocalizableElement
		node.ImageKey = node.SelectedImageKey = "TreeItem_Script";
		node.Tag = script;

		foreach (var scriptEvent in script.Events)
		{
			var eventNode =
				node.Nodes.Add($"0x{scriptEvent.EventId:X} -> {scriptEvent.Target:X} @ T={scriptEvent.Time}");
			eventNode.ImageKey = eventNode.SelectedImageKey = "TreeItem_ScriptEvent";
		}

		var scriptTracks = script.GetTracks();

		CreateTrackNode(node, scriptTracks.Color, "Color");
		CreateTrackNode(node, scriptTracks.Pivot, "Pivot");
		CreateTrackNode(node, scriptTracks.Position, "Position");
		CreateTrackNode(node, scriptTracks.Rotation, "Rotation");
		CreateTrackNode(node, scriptTracks.Size, "Size");

		if (scriptTracks is ImageScriptTracks imageScriptTracks)
		{
			CreateTrackNode(node, imageScriptTracks.UpperLeft, "UpperLeft");
			CreateTrackNode(node, imageScriptTracks.LowerRight, "LowerRight");

			if (imageScriptTracks is MultiImageScriptTracks multiImageScriptTracks)
			{
				CreateTrackNode(node, multiImageScriptTracks.TopLeft1, "TopLeft1");
				CreateTrackNode(node, multiImageScriptTracks.TopLeft2, "TopLeft2");
				CreateTrackNode(node, multiImageScriptTracks.TopLeft3, "TopLeft3");
				CreateTrackNode(node, multiImageScriptTracks.BottomRight1, "BottomRight1");
				CreateTrackNode(node, multiImageScriptTracks.BottomRight2, "BottomRight2");
				CreateTrackNode(node, multiImageScriptTracks.BottomRight3, "BottomRight3");
				CreateTrackNode(node, multiImageScriptTracks.PivotRotation, "PivotRotation");
			}
		}
	}

	private static void CreateTrackNode(TreeNode scriptNode, Track track, string name)
	{
		if (track == null)
			return;

		var trackNode = scriptNode.Nodes.Add(name);
		trackNode.ImageKey = trackNode.SelectedImageKey = "TreeItem_ScriptTrack";

		void AddNodeKey(int time, object value)
		{
			var node = trackNode.Nodes.Add($"T={time}: {value}");
			node.ImageKey = node.SelectedImageKey = "TreeItem_Keyframe";
			;
		}

		switch (track)
		{
			case Vector2Track vector2Track:
				foreach (var deltaKey in vector2Track.DeltaKeys) AddNodeKey(deltaKey.Time, deltaKey.Val);
				break;
			case Vector3Track vector3Track:
				foreach (var deltaKey in vector3Track.DeltaKeys) AddNodeKey(deltaKey.Time, deltaKey.Val);
				break;
			case QuaternionTrack quaternionTrack:
				foreach (var deltaKey in quaternionTrack.DeltaKeys) AddNodeKey(deltaKey.Time, deltaKey.Val);
				break;
			case ColorTrack colorTrack:
				foreach (var deltaKey in colorTrack.DeltaKeys) AddNodeKey(deltaKey.Time, deltaKey.Val);
				break;
			default:
				throw new NotImplementedException($"Unsupported: {track.GetType()}");
		}
	}

	private void SavePackageToChunk(string path)
	{
		using var bw = new BinaryWriter(File.Create(path));

		using var tms = new MemoryStream();
		using var tbw = new BinaryWriter(tms);
		new FrontendChunkWriter(_currentPackage).Write(tbw);
		tms.Position = 0;

		bw.Write(0x30203);
		bw.Write((uint)tms.Length);
		tms.CopyTo(bw.BaseStream);
	}

	private void SavePackageToJson(string path)
	{
		File.WriteAllText(path, JsonConvert.SerializeObject(_currentPackage, new JsonSerializerSettings
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
		}));
	}

	private void OpenRecentlySaved(string path, bool fileIsJson)
	{
		var message = fileIsJson ? $"Do you want to open the file?{Environment.NewLine}{Path.GetFileName(path)}" : $"Do you want to open the folder?{Environment.NewLine}{Path.GetDirectoryName(path)}";
		if (MessageBox.Show(message, "Success", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
		{
			var processStartInfo = new ProcessStartInfo
			{
				FileName = path,
				UseShellExecute = true
			};
			if (!fileIsJson)
			{
				processStartInfo.FileName = Path.GetDirectoryName(path);
				processStartInfo.Verb = "open";
			}

			Process.Start(processStartInfo);
		}
	}

	private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
	{
		if (e.Node?.Tag is RenderTreeNode viewNode)
		{
			viewOutput.SelectedNode = viewNode;
			var wrappedObject = viewNode.GetObject();
			objectPropertyGrid.SelectedObject = wrappedObject switch
			{
				Text t => new TextObjectViewWrapper(t),
				Image i => new ImageObjectViewWrapper(i),
				ColoredImage ci => new ColoredImageObjectViewWrapper(ci),
				_ => new DefaultObjectViewWrapper(wrappedObject)
			};
			Render();
		}
	}

	private void viewOutput_MouseMove(object sender, MouseEventArgs e)
	{
		labelCoordDisplay.Text = $"FE: ({e.X - 320,4:D}, {e.Y - 240,4:D}) | Real: ({e.X,4:D}, {e.Y,4:D})";
	}

	private void viewOutput_MouseClick(object sender, MouseEventArgs e)
	{
		bool WithinBounds(RenderTreeNode node, float x, float y)
		{
			var extents = node.Get2DExtents();

			return extents.HasValue && extents.Value.Contains((int)x, (int)y);
		}

		var feX = e.X - 320;
		var feY = e.Y - 240;

		// get highest Z rendertreenode with the click location in bounds
		var renderTree = RenderTree.GetAllTreeNodesForRendering(_currentRenderTree);
		try
		{
			var candidates = renderTree
				.Where(node => WithinBounds(node, feX, feY))
				//.OrderByDescending(n => n.GetZ());
				.OrderBy(node => // smallest area first => most "specific" candidate wins
				{
					var sz = node.Get2DExtents().Value.Size;
					return sz.Height * sz.Width; // area 
				});

			var top = candidates.First();

			var feObj = top.GetObject();
			var key = $"{feObj.Name ?? feObj.NameHash.ToString("X")}";
			var foundNodes = treeView1.Nodes.Find(key, true);
			treeView1.SelectedNode = foundNodes[0];
			treeView1.Focus();
		}
		catch (Exception)
		{
			// if linq stuff didn't find anything -
			// ignored
		}
	}

	private void OpenFileMenuItem_Click(object sender, EventArgs e)
	{
		var ofd = new OpenFileDialog();
		ofd.Filter = "FNG Files (*.fng, *.bin)|*.fng;*.bin|JSON Files (*.json)|*.json";
		ofd.CheckFileExists = true;
		if (ofd.ShowDialog() == DialogResult.OK)
		{
			switch (ofd.FilterIndex)
			{
				case 1:
					LoadFile(ofd.FileName, false);
					break;
				case 2:
					LoadFile(ofd.FileName, true);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(ofd.FilterIndex));
			}
			_packageViewExtensions.ObjectSelected = null;
			_packageViewExtensions.ShouldCopyObject = false;
		}
	}

	private void LoadFile(string path = null, bool fileIsJson = false)
	{
		Package package;
		if (string.IsNullOrWhiteSpace(path))
		{
			if (_currentPackage is null)
				return;

			package = AppService.Instance.ReloadFile();
		}
		else
		{
			if (fileIsJson)
				package = AppService.Instance.LoadJson(path);
			else
				package = AppService.Instance.LoadFile(path);
		}

		viewOutput.Init(Path.Combine(Path.GetDirectoryName(path) ?? "", "textures"));

		_currentPackage = package;
		CurrentPackageWasModified();
	}

	private void SaveFileMenuItem_Click(object sender, EventArgs e)
	{
		if (_currentPackage is null)
			return;

		var sfd = new SaveFileDialog();
		sfd.Filter = "FNG Files (*.fng)|*.fng|BIN Files (*.bin)|*.bin|JSON Files (*.json)|*.json";
		sfd.FileName = Path.GetFileNameWithoutExtension(_currentPackage?.Name);
		sfd.AddExtension = true;
		if (sfd.ShowDialog() == DialogResult.OK)
		{
			switch (sfd.FilterIndex)
			{
				case 1:
				case 2:
					SaveFile(sfd.FileName, false);
					break;
				case 3:
					SaveFile(sfd.FileName, true);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(sfd.FilterIndex));
			}
		}
	}

	private void ReloadFileMenuItem_Click(object sender, EventArgs e)
	{
		LoadFile();
	}

	private void SaveFile(string path, bool fileIsJson)
	{
		if (string.IsNullOrWhiteSpace(path))
			return;

		if (fileIsJson)
			SavePackageToJson(path);
		else
			SavePackageToChunk(path);

		OpenRecentlySaved(path, fileIsJson);
	}


	private void CurrentPackageWasModified(uint? key = null, uint? guid = null)
	{
		_currentRenderTree = RenderTree.Create(_currentPackage);
		PopulateTreeView(_currentPackage, _currentRenderTree);

		if (key.HasValue && guid.HasValue)
		{
			// How the fuck do I select the correct object using key and guid if there are two or more with the same key?
			var renderTree = RenderTree.GetAllTreeNodesForRendering(_currentRenderTree);
			var renderTreeNode = renderTree.Where(d =>
			{
				var frontendObject = d.GetObject();
				return frontendObject.NameHash == key && frontendObject.Guid == guid;
			}).FirstOrDefault();

			if (renderTreeNode is not null)
			{
				var feObj = renderTreeNode.GetObject();
				var treeKey = $"{feObj.Name ?? feObj.NameHash.ToString("X")}";
				var foundNodes = treeView1.Nodes.Find(treeKey, true);
				treeView1.SelectedNode = foundNodes.LastOrDefault();
				treeView1.Focus();
			}
			else
			{
				viewOutput.SelectedNode = null;
			}
		}
		else
		{
			viewOutput.SelectedNode = null;
		}
		Render();

		// window title
		Text = _currentPackage.Name;
	}

	private void treeView1_MouseDown(object sender, MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Right) return;

		TreeNode hit_node = treeView1.GetNodeAt(e.X, e.Y);
		treeView1.SelectedNode = hit_node;
		var ctxPoint = new Point(e.X, e.Y);

		if (hit_node?.Tag is RenderTreeNode)
		{
			objectContextMenu.Show(treeView1, ctxPoint);
		}
		else if (hit_node?.Tag is Script script)
		{
			var viewNode = (RenderTreeNode)hit_node.Parent.Tag;

			toggleScriptItem.Text = ReferenceEquals(viewNode.GetCurrentScript(), script) ? "Stop" : "Start";

			scriptContextMenu.Show(treeView1, ctxPoint);
		}
	}

	private void toggleScriptItem_Click(object sender, EventArgs e)
	{
		if (treeView1.SelectedNode?.Tag is Script script)
		{
			var viewNode = (RenderTreeNode)treeView1.SelectedNode.Parent.Tag;
			viewNode.SetCurrentScript(ReferenceEquals(viewNode.GetCurrentScript(), script) ? null : script.Id);
		}
	}

	private void radioBgBlack_CheckedChanged(object sender, EventArgs e)
	{
		if (sender == radioBgBlack)
		{
			viewOutput.BackgroundColor = new Color4(0, 0, 0, 255);
		}
		else if (sender == radioBgGreen)
		{
			viewOutput.BackgroundColor = new Color4(0, 255, 0, 255);
		}

		Render();
	}

	private void renameToolStripMenuItem_Click(object sender, EventArgs e)
	{
		var selectedObject = _packageViewExtensions.GetObject(false);
		if (selectedObject is null)
			return;

		var input = _packageViewExtensions.ObjectInput(selectedObject.Name, selectedObject is Group);

		if (string.IsNullOrWhiteSpace(input))
			return;

		selectedObject.Name = input;
		selectedObject.NameHash = input.BinHash();
		AppService.Instance.HashResolver.AddUserKey(selectedObject.Name, selectedObject.NameHash);

		CurrentPackageWasModified(selectedObject.NameHash, selectedObject.Guid);
	}

	private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		var selectedObject = _packageViewExtensions.GetObject(false);
		if (selectedObject is null)
			return;

		_currentPackage.Objects.Remove(selectedObject);
		CurrentPackageWasModified();
	}

	private void cloneToolStripMenuItem_Click(object sender, EventArgs e)
	{
		var selectedObject = _packageViewExtensions.GetObject(false);
		var newObject = _packageViewExtensions.CopyObject(selectedObject, selectedObject.Parent);

		if (newObject is null)
			return;

		AppService.Instance.HashResolver.AddUserKey(newObject.Name, newObject.NameHash);

		_currentPackage.ResourceRequests.Add(newObject.ResourceRequest);
		_currentPackage.Objects.Add(newObject);

		CurrentPackageWasModified(newObject.NameHash, newObject.Guid);
	}

	private void cutToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_packageViewExtensions.ObjectSelected = _packageViewExtensions.GetObject(false);
		_packageViewExtensions.ShouldCopyObject = false;
	}

	private void copyToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_packageViewExtensions.ObjectSelected = _packageViewExtensions.GetObject(false);
		_packageViewExtensions.ShouldCopyObject = true;
	}

	private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (_packageViewExtensions.ObjectSelected is null)
			return;

		var selectedGroup = _packageViewExtensions.GetObject(true);

		if (selectedGroup is null)
			return;

		uint? key = selectedGroup.NameHash;
		uint? guid = selectedGroup.Guid;

		if (_packageViewExtensions.ShouldCopyObject)
		{
			var newObject = _packageViewExtensions.CopyObject(_packageViewExtensions.ObjectSelected, selectedGroup);

			if (newObject is null)
				return;

			AppService.Instance.HashResolver.AddUserKey(newObject.Name, newObject.NameHash);
			key = newObject.NameHash;
			guid = newObject.Guid;
			_currentPackage.ResourceRequests.Add(newObject.ResourceRequest);
			_currentPackage.Objects.Add(newObject);
		}
		else
		{
			_packageViewExtensions.ObjectSelected.Parent = selectedGroup;
			_packageViewExtensions.ObjectSelected = null;
		}

		CurrentPackageWasModified(key, guid);
	}

	private void SetLabelDetails()
	{
		var details = $"{DateTimeOffset.Now:dd/MM/yyyy HH:mm:ss}";
		if (!string.IsNullOrWhiteSpace(AppService.Instance.CurrentFileLoaded))
			details += $"| {AppService.Instance.CurrentFileLoaded}";

		LblDetails.Text = details;
	}

	[UsedImplicitly]
	private class Options
	{
		[Option('i', "input")] public string InputFile { get; [UsedImplicitly] set; }
	}
}
