using CommandLine;
using FEngLib;
using FEngLib.Objects;
using FEngLib.Packages;
using FEngLib.Scripts;
using FEngLib.Structures;
using FEngRender.Data;
using FEngViewer.Properties;
using FEngViewer.Wrappers;
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
		SetFormDetails();

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
		imageList.Images.Add("TreeItem_SimpleImage", Resources.TreeItem_SimpleImage);
		imageList.Images.Add("TreeItem_Model", Resources.TreeItem_Model);
		imageList.Images.Add("TreeItem_Effect", Resources.TreeItem_Effect);
		imageList.Images.Add("TreeItem_AnimatedImage", Resources.TreeItem_AnimatedImage);
		imageList.Images.Add("TreeItem_List", Resources.TreeItem_List);
		imageList.Images.Add("TreeItem_CodeList", Resources.TreeItem_CodeList);
		imageList.Images.Add("TreeItem_GenericObject", Resources.TreeItem_GenericObject);
		treeView1.ImageList = imageList;
	}

	private void PackageView_Load(object sender, EventArgs e)
	{
		var args = Environment.GetCommandLineArgs();

		var opts = Parser.Default.ParseArguments<Options>(args);

		opts.WithParsed(parsed => LoadFile(parsed.InputFile, false))
			.WithNotParsed(err => Application.Exit());
	}

	[UsedImplicitly]
	private class Options
	{
		[Option('i', "input")] public string InputFile { get; [UsedImplicitly] set; }
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
			resourceRequestNode.Tag = resourceRequest;

			resourceRequestNode.ImageKey = resourceRequestNode.SelectedImageKey = resourceRequest.Type switch
			{
				ResourceType.Image => "TreeItem_Image",
				ResourceType.MultiImage => "TreeItem_MultiImage",
				ResourceType.Movie => "TreeItem_Movie",
				ResourceType.Font => "TreeItem_Font",
				ResourceType.Model => "TreeItem_Model",
				ResourceType.AnimatedImage => "TreeItem_AnimatedImage",
				ResourceType.Effect => "TreeItem_Effect",
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
			ObjectType.AnimImage => "TreeItem_AnimatedImage",
			ObjectType.Effect => "TreeItem_Effect",
			ObjectType.Model => "TreeItem_Model",
			ObjectType.SimpleImage => "TreeItem_SimpleImage",
			ObjectType.List => "TreeItem_List",
			ObjectType.CodeList => "TreeItem_CodeList",
			_ => "TreeItem_GenericObject"
		};

		var nodeText = PackageViewExtensions.GetObjectText(feObj);
		var objTreeNode = collection.Add(nodeText);
		objTreeNode.Tag = viewNode;
		objTreeNode.Name = PackageViewExtensions.GetObjectTreeKey(feObj);
		if (nodeImageKey != null)
			objTreeNode.ImageKey = objTreeNode.SelectedImageKey = nodeImageKey;

		foreach (var script in feObj.GetScripts())
			CreateScriptTreeNode(objTreeNode.Nodes, script);

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
			eventNode.Tag = scriptEvent;
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
		trackNode.Tag = track;

		void AddNodeKey<T>(int time, object value, T tag) where T : TrackNode
		{
			var node = trackNode.Nodes.Add($"T={time}: {value}");
			node.ImageKey = node.SelectedImageKey = "TreeItem_Keyframe";
			node.Tag = tag;
		}

		switch (track)
		{
			case Vector2Track vector2Track:
				foreach (var deltaKey in vector2Track.DeltaKeys)
					AddNodeKey(deltaKey.Time, deltaKey.Val, deltaKey);
				break;
			case Vector3Track vector3Track:
				foreach (var deltaKey in vector3Track.DeltaKeys)
					AddNodeKey(deltaKey.Time, deltaKey.Val, deltaKey);
				break;
			case QuaternionTrack quaternionTrack:
				foreach (var deltaKey in quaternionTrack.DeltaKeys)
					AddNodeKey(deltaKey.Time, deltaKey.Val, deltaKey);
				break;
			case ColorTrack colorTrack:
				foreach (var deltaKey in colorTrack.DeltaKeys)
					AddNodeKey(deltaKey.Time, deltaKey.Val, deltaKey);
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
			LblItemIndex.Text = "Index: " + _currentPackage.Objects.IndexOf(wrappedObject).ToString("D4");
			objectPropertyGrid.SelectedObject = wrappedObject switch
			{
				Text t => new TextObjectViewWrapper(t, AppService.Instance.HashResolver),
				Image i => new ImageObjectViewWrapper(i, AppService.Instance.HashResolver),
				ColoredImage ci => new ColoredImageObjectViewWrapper(ci, AppService.Instance.HashResolver),
				_ => new DefaultObjectViewWrapper(wrappedObject, AppService.Instance.HashResolver)
			};
		}
#if DEBUG
		else if (e.Node?.Tag is Script script)
		{
			objectPropertyGrid.SelectedObject = new DefaultScriptViewWrapper(script, AppService.Instance.HashResolver);
		}
		else if (e.Node?.Tag is Track track)
		{
			objectPropertyGrid.SelectedObject = track;//new DefaultScriptViewWrapper(script, AppService.Instance.HashResolver);
		}
		else if (e.Node?.Tag is TrackNode trackNode)
		{
			objectPropertyGrid.SelectedObject = trackNode;//new DefaultScriptViewWrapper(script, AppService.Instance.HashResolver);
		}
		else if (e.Node?.Tag is Event scriptEvent)
		{
			objectPropertyGrid.SelectedObject = scriptEvent;//new DefaultScriptViewWrapper(script, AppService.Instance.HashResolver);
		}
#endif
		else
		{

#if DEBUG
			objectPropertyGrid.SelectedObject = e.Node?.Tag;
#else
			objectPropertyGrid.SelectedObject = null;
#endif
		LblItemIndex.Text = null;
		}
		Render();
	}

	private void viewOutput_MouseMove(object sender, MouseEventArgs e)
	{
		labelCoordDisplay.Text = $"FE: ({e.X - 320,4:D}, {e.Y - 240,4:D}) | Real: ({e.X,4:D}, {e.Y,4:D})";
	}

	private void viewOutput_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Right)
		{
			var ctxPoint = new Point(e.X, e.Y);
			viewerContextMenu.Show(viewOutput, ctxPoint);
			return;
		}


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
			var key = PackageViewExtensions.GetObjectTreeKey(feObj);
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

	private void FindNode(string text, int direction)
	{
		if (string.IsNullOrEmpty(text))
			return;

		var objectsFound = _currentPackage.Objects
			.Where(o => new[] { o.Name, $"0x{o.NameHash.ToString("X")}" }.Any(t => t != null && t.Contains(text, StringComparison.CurrentCultureIgnoreCase)))
			.ToArray();

		if (objectsFound.Any())
		{
			_packageViewExtensions.SearchIndex += direction;
			if (_packageViewExtensions.SearchIndex > objectsFound.Length - 1)
				_packageViewExtensions.SearchIndex = 0;
			else if (_packageViewExtensions.SearchIndex < 0)
				_packageViewExtensions.SearchIndex = objectsFound.Length - 1;

			var objectFound = objectsFound.ElementAtOrDefault(_packageViewExtensions.SearchIndex);

			if (objectFound is not null)
			{
				var treeKey = PackageViewExtensions.GetObjectTreeKey(objectFound);
				var foundNodes = treeView1.Nodes.Find(treeKey, true);
				if (foundNodes.Any())
				{
					treeView1.SelectedNode = foundNodes.First();
					treeView1.Focus();
				}
			}
		}
	}

	private void CurrentPackageWasModified(string treeKey = null)
	{
		_currentRenderTree = RenderTree.Create(_currentPackage);
		PopulateTreeView(_currentPackage, _currentRenderTree);

		if (!string.IsNullOrWhiteSpace(treeKey))
		{
			var foundNodes = treeView1.Nodes.Find(treeKey, true);
			treeView1.SelectedNode = foundNodes.First();
			treeView1.SelectedNode.Expand();
			treeView1.Focus();
		}
		else
		{
			viewOutput.SelectedNode = null;
		}
		Render();

		// window title
		SetFormDetails();
	}

	private void treeView1_MouseDown(object sender, MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Right)
			return;

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

			toggleScriptMenuItem.Text = ReferenceEquals(viewNode.GetCurrentScript(), script) ? "Stop" : "Start";

			scriptContextMenu.Show(treeView1, ctxPoint);
		}
	}

	private void toggleScriptMenuItem_Click(object sender, EventArgs e)
	{
		if (treeView1.SelectedNode?.Tag is Script script)
		{
			var viewNode = (RenderTreeNode)treeView1.SelectedNode.Parent.Tag;
			viewNode.SetCurrentScript(ReferenceEquals(viewNode.GetCurrentScript(), script) ? null : script.Id);
		}
	}

	private void renameScriptMenuItem_Click(object sender, EventArgs e)
	{
		if (treeView1.SelectedNode?.Tag is not Script script)
			return;

		var viewNode = (RenderTreeNode)treeView1.SelectedNode.Parent.Tag;
		var objectData = viewNode.GetObject();
		var scripts = objectData.GetScripts();

		var objectInput = _packageViewExtensions.ObjectInput(script.Name, false);

		if (string.IsNullOrWhiteSpace(objectInput.Name) || objectInput.Name == script.Name || _packageViewExtensions.NameAlreadyExists(objectInput.Name, objectInput.NameHash, objectData, script))
			return;

		foreach (var chainedScript in scripts)
		{
			if (chainedScript.ChainedId == script.Id)
				chainedScript.ChainedId = objectInput.NameHash;
		}

		script.Name = objectInput.Name;
		script.Id = objectInput.NameHash;

		CurrentPackageWasModified(PackageViewExtensions.GetObjectTreeKey(objectData));

	}

	private void backgroundColorMenuItem_Click(object sender, EventArgs e)
	{
		var colorDialog = new ColorDialog();
		if (colorDialog.ShowDialog() != DialogResult.OK)
			return;

		viewOutput.BackgroundColor = new Color4(colorDialog.Color.B, colorDialog.Color.G, colorDialog.Color.R, colorDialog.Color.A);

		Render();
	}

	private void renameToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (treeView1.SelectedNode?.Tag is not RenderTreeNode node)
			return;

		var selectedObject = node.GetObject();

		if (selectedObject is null)
			return;

		var objectInput = _packageViewExtensions.ObjectInput(selectedObject.Name, false);

		if (string.IsNullOrWhiteSpace(objectInput.Name) || objectInput.Name == selectedObject.Name || _packageViewExtensions.NameAlreadyExists(objectInput.Name, objectInput.NameHash, selectedObject))
			return;

		selectedObject.Name = objectInput.Name;
		selectedObject.NameHash = objectInput.NameHash;
		AppService.Instance.HashResolver.AddUserKey(selectedObject.Name, selectedObject.NameHash);

		CurrentPackageWasModified(PackageViewExtensions.GetObjectTreeKey(selectedObject));
	}

	private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (treeView1.SelectedNode?.Tag is not RenderTreeNode node)
			return;

		var selectedObject = node.GetObject();

		if (selectedObject is null)
			return;

		var parentKey = PackageViewExtensions.GetObjectTreeKey(selectedObject.Parent);
		_packageViewExtensions.DeleteObject(selectedObject);
		CurrentPackageWasModified(parentKey);
	}

	private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (treeView1.SelectedNode?.Tag is not RenderTreeNode node)
			return;

		var selectedObject = node.GetObject();

		if (selectedObject is null)
			return;

		var lastChild = _packageViewExtensions.FindLastChild(selectedObject);
		var position = _currentPackage.Objects.IndexOf(lastChild) + 1;
		var newObject = _packageViewExtensions.CopyObject(selectedObject, selectedObject, position);

		if (newObject is null)
			return;

		AppService.Instance.HashResolver.AddUserKey(newObject.Name, newObject.NameHash);

		CurrentPackageWasModified(PackageViewExtensions.GetObjectTreeKey(newObject));
	}

	private void cutToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (treeView1.SelectedNode?.Tag is not RenderTreeNode node)
			return;

		_packageViewExtensions.ObjectSelected = node.GetObject();
		_packageViewExtensions.ShouldCopyObject = false;
	}

	private void copyToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (treeView1.SelectedNode?.Tag is not RenderTreeNode node)
			return;

		_packageViewExtensions.ObjectSelected = node.GetObject();
		_packageViewExtensions.ShouldCopyObject = true;
	}

	private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (_packageViewExtensions.ObjectSelected is null)
			return;

		if (treeView1.SelectedNode?.Tag is not RenderTreeNode node)
			return;

		var selectedObject = node.GetObject();

		if (selectedObject is null)
			return;

		var treeKey = PackageViewExtensions.GetObjectTreeKey(selectedObject);

		if (_packageViewExtensions.ShouldCopyObject)
		{
			var lastChild = _packageViewExtensions.FindLastChild(selectedObject);
			var position = _currentPackage.Objects.IndexOf(lastChild) + 1;
			var newObject = _packageViewExtensions.CopyObject(_packageViewExtensions.ObjectSelected, selectedObject, position);

			if (newObject is null)
				return;

			AppService.Instance.HashResolver.AddUserKey(newObject.Name, newObject.NameHash);
			treeKey = PackageViewExtensions.GetObjectTreeKey(newObject);
		}
		else
		{
			if (_packageViewExtensions.ObjectSelected == selectedObject)
			{
				_packageViewExtensions.ObjectSelected = null;
				return;
			}

			treeKey = PackageViewExtensions.GetObjectTreeKey(_packageViewExtensions.ObjectSelected);
			var lastChild = _packageViewExtensions.FindLastChild(selectedObject);
			var position = _currentPackage.Objects.IndexOf(lastChild);
			var oldIndex = _currentPackage.Objects.IndexOf(_packageViewExtensions.ObjectSelected);

			var indexMove = oldIndex > position ? 1 : 0;
			position += indexMove;

			if (oldIndex == position)
				return;

			_packageViewExtensions.ObjectSelected.Parent = selectedObject.Parent;
			_packageViewExtensions.MoveItem(oldIndex, position);
			_packageViewExtensions.MoveChildren(_packageViewExtensions.ObjectSelected, indexMove);
		}

		CurrentPackageWasModified(treeKey);
	}


	private void BtnPrevious_Click(object sender, EventArgs e)
	{
		FindNode(TxtSearch.Text, -1);
	}

	private void BtnNext_Click(object sender, EventArgs e)
	{
		FindNode(TxtSearch.Text, +1);
	}

	private void TxtSearch_Leave(object sender, EventArgs e)
	{
		if (_packageViewExtensions.SearchText != TxtSearch.Text)
		{
			_packageViewExtensions.SearchText = TxtSearch.Text;
			_packageViewExtensions.SearchIndex = -1;
		}
	}

	private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (treeView1.SelectedNode?.Tag is not RenderTreeNode node)
			return;

		var selectedObject = node.GetObject();

		if (selectedObject is null)
			return;

		var parents = _currentPackage.Objects.FindAll(x => x.Parent?.NameHash == selectedObject.Parent?.NameHash && x.Parent?.Guid == selectedObject.Parent?.Guid);
		var indexInParents = parents.IndexOf(selectedObject);
		var newIndex = indexInParents - 1;

		if (newIndex > parents.Count - 1 || newIndex < 0)
			return;

		var lastChild = parents.ElementAt(newIndex);

		var position = _currentPackage.Objects.IndexOf(lastChild);
		var oldIndex = _currentPackage.Objects.IndexOf(selectedObject);

		_packageViewExtensions.MoveItem(oldIndex, position);
		_packageViewExtensions.MoveChildren(selectedObject, 1);

		CurrentPackageWasModified(PackageViewExtensions.GetObjectTreeKey(selectedObject));
	}

	private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (treeView1.SelectedNode?.Tag is not RenderTreeNode node)
			return;

		var selectedObject = node.GetObject();

		if (selectedObject is null)
			return;

		var parents = _currentPackage.Objects.FindAll(x => x.Parent?.NameHash == selectedObject.Parent?.NameHash && x.Parent?.Guid == selectedObject.Parent?.Guid);
		var indexInParents = parents.IndexOf(selectedObject);
		var newIndex = indexInParents + 1;

		if (newIndex > parents.Count - 1 || newIndex < 0)
			return;

		var objectInNewIndex = parents.ElementAt(newIndex);
		var lastChild = _packageViewExtensions.FindLastChild(objectInNewIndex);

		var position = _currentPackage.Objects.IndexOf(lastChild);
		var oldIndex = _currentPackage.Objects.IndexOf(selectedObject);

		_packageViewExtensions.MoveItem(oldIndex, position);
		_packageViewExtensions.MoveChildren(selectedObject, 0);

		CurrentPackageWasModified(PackageViewExtensions.GetObjectTreeKey(selectedObject));
	}

	private void moveUpScriptMenuItem_Click(object sender, EventArgs e)
	{
		MoveScript(-1);
	}

	private void moveDownScriptMenuItem_Click(object sender, EventArgs e)
	{
		MoveScript(1);
	}


	private void MoveScript(int position)
	{
		if (treeView1.SelectedNode?.Tag is not Script script)
			return;

		var viewNode = (RenderTreeNode)treeView1.SelectedNode.Parent.Tag;
		var selectedObject = viewNode.GetObject();

		if (selectedObject is null || selectedObject is not BaseObject baseObject)
			return;

		var currentItemIndex = baseObject.Scripts.IndexOf(script as BaseObjectScript);
		var currentItemNewIndex = currentItemIndex + position;
		if (currentItemNewIndex > baseObject.Scripts.Count - 1 || currentItemNewIndex < 0)
			return;

		var unrelatedScript = baseObject.Scripts.ElementAt(currentItemNewIndex);

		baseObject.Scripts[currentItemNewIndex] = script as BaseObjectScript;
		baseObject.Scripts[currentItemIndex] = unrelatedScript;

		CurrentPackageWasModified(PackageViewExtensions.GetObjectTreeKey(selectedObject));
	}

	private void SetFormDetails()
	{
		var details = $"{DateTimeOffset.Now:dd/MM/yyyy HH:mm:ss}";
		if (!string.IsNullOrWhiteSpace(AppService.Instance.CurrentFileLoaded))
			details += $" | {AppService.Instance.CurrentFileLoaded}";

		LblDetails.Text = details;

		this.Text = $"FEngViewer";
		if (_currentPackage is not null)
			this.Text += $" - {_currentPackage.Name}";

#if DEBUG
		Text += $" - Debug {DateTime.Today:dd/MM/yyyy}";
#endif
	}
}
