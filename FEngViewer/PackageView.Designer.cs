
using System.Windows.Forms;

namespace FEngViewer
{
	partial class PackageView
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.viewOutput = new GLRenderControl();
			this.labelCoordDisplay = new Label();
			this.groupBgColor = new GroupBox();
			this.radioBgGreen = new RadioButton();
			this.radioBgBlack = new RadioButton();
			this.colorDialog1 = new ColorDialog();
			this.menuStrip1 = new MenuStrip();
			this.FileMenuItem = new ToolStripMenuItem();
			this.OpenFileMenuItem = new ToolStripMenuItem();
			this.ReloadFileMenuItem = new ToolStripMenuItem();
			this.SaveFileMenuItem = new ToolStripMenuItem();
			this.objectContextMenu = new ContextMenuStrip(this.components);
			this.renameToolStripMenuItem = new ToolStripMenuItem();
			this.moveUpToolStripMenuItem = new ToolStripMenuItem();
			this.moveDownToolStripMenuItem = new ToolStripMenuItem();
			this.duplicateToolStripMenuItem = new ToolStripMenuItem();
			this.copyToolStripMenuItem = new ToolStripMenuItem();
			this.cutToolStripMenuItem = new ToolStripMenuItem();
			this.pasteToolStripMenuItem = new ToolStripMenuItem();
			this.deleteToolStripMenuItem = new ToolStripMenuItem();
			this.scriptContextMenu = new ContextMenuStrip(this.components);
			this.toggleScriptItem = new ToolStripMenuItem();
			this.LblDetails = new Label();
			this.tableLayoutPanel1 = new TableLayoutPanel();
			this.tableLayoutPanel2 = new TableLayoutPanel();
			this.BtnNext = new Button();
			this.BtnPrevious = new Button();
			this.TxtSearch = new TextBox();
			this.objectPropertyGrid = new PropertyGrid();
			this.treeView1 = new TreeView();
			this.groupBgColor.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.objectContextMenu.SuspendLayout();
			this.scriptContextMenu.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// viewOutput
			// 
			this.viewOutput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.viewOutput.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.viewOutput.BackColor = System.Drawing.Color.Black;
			this.tableLayoutPanel1.SetColumnSpan(this.viewOutput, 2);
			this.viewOutput.Location = new System.Drawing.Point(410, 2);
			this.viewOutput.Margin = new Padding(3, 2, 3, 2);
			this.viewOutput.MaximumSize = new System.Drawing.Size(640, 480);
			this.viewOutput.MinimumSize = new System.Drawing.Size(640, 480);
			this.viewOutput.Name = "viewOutput";
			this.tableLayoutPanel1.SetRowSpan(this.viewOutput, 2);
			this.viewOutput.SelectedNode = null;
			this.viewOutput.Size = new System.Drawing.Size(640, 480);
			this.viewOutput.TabIndex = 0;
			this.viewOutput.TabStop = false;
			this.viewOutput.MouseClick += this.viewOutput_MouseClick;
			this.viewOutput.MouseMove += this.viewOutput_MouseMove;
			// 
			// labelCoordDisplay
			// 
			this.labelCoordDisplay.AutoSize = true;
			this.labelCoordDisplay.Location = new System.Drawing.Point(410, 484);
			this.labelCoordDisplay.Name = "labelCoordDisplay";
			this.labelCoordDisplay.Size = new System.Drawing.Size(72, 15);
			this.labelCoordDisplay.TabIndex = 2;
			this.labelCoordDisplay.Text = "X:    0   Y:    0";
			// 
			// groupBgColor
			// 
			this.groupBgColor.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			this.groupBgColor.Controls.Add(this.radioBgGreen);
			this.groupBgColor.Controls.Add(this.radioBgBlack);
			this.groupBgColor.Location = new System.Drawing.Point(896, 487);
			this.groupBgColor.Name = "groupBgColor";
			this.groupBgColor.Size = new System.Drawing.Size(154, 58);
			this.groupBgColor.TabIndex = 3;
			this.groupBgColor.TabStop = false;
			this.groupBgColor.Text = "Background";
			// 
			// radioBgGreen
			// 
			this.radioBgGreen.AutoSize = true;
			this.radioBgGreen.Location = new System.Drawing.Point(77, 26);
			this.radioBgGreen.Name = "radioBgGreen";
			this.radioBgGreen.Size = new System.Drawing.Size(56, 19);
			this.radioBgGreen.TabIndex = 1;
			this.radioBgGreen.Text = "Green";
			this.radioBgGreen.UseVisualStyleBackColor = true;
			this.radioBgGreen.CheckedChanged += this.radioBgBlack_CheckedChanged;
			// 
			// radioBgBlack
			// 
			this.radioBgBlack.AutoSize = true;
			this.radioBgBlack.Checked = true;
			this.radioBgBlack.Location = new System.Drawing.Point(6, 26);
			this.radioBgBlack.Name = "radioBgBlack";
			this.radioBgBlack.Size = new System.Drawing.Size(53, 19);
			this.radioBgBlack.TabIndex = 0;
			this.radioBgBlack.TabStop = true;
			this.radioBgBlack.Text = "Black";
			this.radioBgBlack.UseVisualStyleBackColor = true;
			this.radioBgBlack.CheckedChanged += this.radioBgBlack_CheckedChanged;
			// 
			// menuStrip1
			// 
			this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.menuStrip1.Items.AddRange(new ToolStripItem[] { this.FileMenuItem });
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(1484, 24);
			this.menuStrip1.TabIndex = 4;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// FileMenuItem
			// 
			this.FileMenuItem.DropDownItems.AddRange(new ToolStripItem[] { this.OpenFileMenuItem, this.ReloadFileMenuItem, this.SaveFileMenuItem });
			this.FileMenuItem.Name = "FileMenuItem";
			this.FileMenuItem.Size = new System.Drawing.Size(37, 20);
			this.FileMenuItem.Text = "File";
			// 
			// OpenFileMenuItem
			// 
			this.OpenFileMenuItem.Name = "OpenFileMenuItem";
			this.OpenFileMenuItem.ShortcutKeys = Keys.Control | Keys.O;
			this.OpenFileMenuItem.Size = new System.Drawing.Size(183, 22);
			this.OpenFileMenuItem.Text = "Open";
			this.OpenFileMenuItem.Click += this.OpenFileMenuItem_Click;
			// 
			// ReloadFileMenuItem
			// 
			this.ReloadFileMenuItem.Name = "ReloadFileMenuItem";
			this.ReloadFileMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.R;
			this.ReloadFileMenuItem.Size = new System.Drawing.Size(183, 22);
			this.ReloadFileMenuItem.Text = "Reload";
			this.ReloadFileMenuItem.Click += this.ReloadFileMenuItem_Click;
			// 
			// SaveFileMenuItem
			// 
			this.SaveFileMenuItem.Name = "SaveFileMenuItem";
			this.SaveFileMenuItem.ShortcutKeys = Keys.Control | Keys.S;
			this.SaveFileMenuItem.Size = new System.Drawing.Size(183, 22);
			this.SaveFileMenuItem.Text = "Save";
			this.SaveFileMenuItem.Click += this.SaveFileMenuItem_Click;
			// 
			// objectContextMenu
			// 
			this.objectContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.objectContextMenu.Items.AddRange(new ToolStripItem[] { this.renameToolStripMenuItem, this.moveUpToolStripMenuItem, this.moveDownToolStripMenuItem, this.duplicateToolStripMenuItem, this.copyToolStripMenuItem, this.cutToolStripMenuItem, this.pasteToolStripMenuItem, this.deleteToolStripMenuItem });
			this.objectContextMenu.Name = "objectContextMenu";
			this.objectContextMenu.Size = new System.Drawing.Size(203, 180);
			// 
			// renameToolStripMenuItem
			// 
			this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
			this.renameToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.R;
			this.renameToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
			this.renameToolStripMenuItem.Text = "Rename";
			this.renameToolStripMenuItem.Click += this.renameToolStripMenuItem_Click;
			// 
			// moveUpToolStripMenuItem
			// 
			this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
			this.moveUpToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Up;
			this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
			this.moveUpToolStripMenuItem.Text = "Move up";
			this.moveUpToolStripMenuItem.Click += this.moveUpToolStripMenuItem_Click;
			// 
			// moveDownToolStripMenuItem
			// 
			this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
			this.moveDownToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Down;
			this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
			this.moveDownToolStripMenuItem.Text = "Move down";
			this.moveDownToolStripMenuItem.Click += this.moveDownToolStripMenuItem_Click;
			// 
			// duplicateToolStripMenuItem
			// 
			this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
			this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
			this.duplicateToolStripMenuItem.Text = "Duplicate";
			this.duplicateToolStripMenuItem.Click += this.duplicateToolStripMenuItem_Click;
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.C;
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
			this.copyToolStripMenuItem.Text = "Copy";
			this.copyToolStripMenuItem.Click += this.copyToolStripMenuItem_Click;
			// 
			// cutToolStripMenuItem
			// 
			this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
			this.cutToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.X;
			this.cutToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
			this.cutToolStripMenuItem.Text = "Cut";
			this.cutToolStripMenuItem.Click += this.cutToolStripMenuItem_Click;
			// 
			// pasteToolStripMenuItem
			// 
			this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
			this.pasteToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.V;
			this.pasteToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
			this.pasteToolStripMenuItem.Text = "Paste";
			this.pasteToolStripMenuItem.Click += this.pasteToolStripMenuItem_Click;
			// 
			// deleteToolStripMenuItem
			// 
			this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			this.deleteToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.D;
			this.deleteToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
			this.deleteToolStripMenuItem.Text = "Delete";
			this.deleteToolStripMenuItem.Click += this.deleteToolStripMenuItem_Click;
			// 
			// scriptContextMenu
			// 
			this.scriptContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.scriptContextMenu.Items.AddRange(new ToolStripItem[] { this.toggleScriptItem });
			this.scriptContextMenu.Name = "scriptContextMenu";
			this.scriptContextMenu.Size = new System.Drawing.Size(181, 26);
			// 
			// toggleScriptItem
			// 
			this.toggleScriptItem.Name = "toggleScriptItem";
			this.toggleScriptItem.Size = new System.Drawing.Size(180, 22);
			this.toggleScriptItem.Text = "toolStripMenuItem1";
			this.toggleScriptItem.Click += this.toggleScriptItem_Click;
			// 
			// LblDetails
			// 
			this.LblDetails.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.LblDetails, 4);
			this.LblDetails.Location = new System.Drawing.Point(3, 805);
			this.LblDetails.Name = "LblDetails";
			this.LblDetails.Size = new System.Drawing.Size(136, 15);
			this.LblDetails.TabIndex = 5;
			this.LblDetails.Text = "dd/MM/yyyy HH:mm:ss";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.ColumnCount = 4;
			this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.viewOutput, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.labelCoordDisplay, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.LblDetails, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.objectPropertyGrid, 3, 0);
			this.tableLayoutPanel1.Controls.Add(this.groupBgColor, 2, 2);
			this.tableLayoutPanel1.Controls.Add(this.treeView1, 0, 1);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 27);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 4;
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1460, 820);
			this.tableLayoutPanel1.TabIndex = 6;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.tableLayoutPanel2.AutoSize = true;
			this.tableLayoutPanel2.ColumnCount = 2;
			this.tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			this.tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			this.tableLayoutPanel2.Controls.Add(this.BtnNext, 1, 1);
			this.tableLayoutPanel2.Controls.Add(this.BtnPrevious, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this.TxtSearch, 0, 0);
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 2;
			this.tableLayoutPanel2.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel2.Size = new System.Drawing.Size(401, 58);
			this.tableLayoutPanel2.TabIndex = 8;
			// 
			// BtnNext
			// 
			this.BtnNext.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.BtnNext.Location = new System.Drawing.Point(203, 32);
			this.BtnNext.Name = "BtnNext";
			this.BtnNext.Size = new System.Drawing.Size(195, 23);
			this.BtnNext.TabIndex = 7;
			this.BtnNext.Text = "Next";
			this.BtnNext.UseVisualStyleBackColor = true;
			this.BtnNext.Click += this.BtnNext_Click;
			// 
			// BtnPrevious
			// 
			this.BtnPrevious.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.BtnPrevious.Location = new System.Drawing.Point(3, 32);
			this.BtnPrevious.Name = "BtnPrevious";
			this.BtnPrevious.Size = new System.Drawing.Size(194, 23);
			this.BtnPrevious.TabIndex = 7;
			this.BtnPrevious.Text = "Previous";
			this.BtnPrevious.UseVisualStyleBackColor = true;
			this.BtnPrevious.Click += this.BtnPrevious_Click;
			// 
			// TxtSearch
			// 
			this.TxtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.tableLayoutPanel2.SetColumnSpan(this.TxtSearch, 2);
			this.TxtSearch.Location = new System.Drawing.Point(3, 3);
			this.TxtSearch.Name = "TxtSearch";
			this.TxtSearch.PlaceholderText = "Search";
			this.TxtSearch.Size = new System.Drawing.Size(395, 23);
			this.TxtSearch.TabIndex = 7;
			this.TxtSearch.Leave += this.TxtSearch_Leave;
			// 
			// objectPropertyGrid
			// 
			this.objectPropertyGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.objectPropertyGrid.Location = new System.Drawing.Point(1056, 3);
			this.objectPropertyGrid.Name = "objectPropertyGrid";
			this.tableLayoutPanel1.SetRowSpan(this.objectPropertyGrid, 3);
			this.objectPropertyGrid.Size = new System.Drawing.Size(401, 799);
			this.objectPropertyGrid.TabIndex = 5;
			// 
			// treeView1
			// 
			this.treeView1.Dock = DockStyle.Fill;
			this.treeView1.Location = new System.Drawing.Point(3, 67);
			this.treeView1.Name = "treeView1";
			this.tableLayoutPanel1.SetRowSpan(this.treeView1, 2);
			this.treeView1.Size = new System.Drawing.Size(401, 735);
			this.treeView1.TabIndex = 2;
			this.treeView1.AfterSelect += this.treeView1_AfterSelect;
			this.treeView1.MouseDown += this.treeView1_MouseDown;
			// 
			// PackageView
			// 
			this.AutoScaleMode = AutoScaleMode.Inherit;
			this.AutoScroll = true;
			this.AutoScrollMinSize = new System.Drawing.Size(1000, 600);
			this.ClientSize = new System.Drawing.Size(1484, 861);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.menuStrip1);
			this.Name = "PackageView";
			this.Text = "FEngViewer";
			this.Load += this.PackageView_Load;
			this.groupBgColor.ResumeLayout(false);
			this.groupBgColor.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.objectContextMenu.ResumeLayout(false);
			this.scriptContextMenu.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		#endregion
		private GLRenderControl viewOutput;
		private System.Windows.Forms.ColorDialog colorDialog1;
		private System.Windows.Forms.Label labelCoordDisplay;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem OpenFileMenuItem;
		private System.Windows.Forms.ToolStripMenuItem SaveFileMenuItem;
		private System.Windows.Forms.ContextMenuStrip objectContextMenu;
		private System.Windows.Forms.ContextMenuStrip scriptContextMenu;
		private System.Windows.Forms.ToolStripMenuItem toggleScriptItem;
		private System.Windows.Forms.GroupBox groupBgColor;
		private System.Windows.Forms.RadioButton radioBgGreen;
		private System.Windows.Forms.RadioButton radioBgBlack;
		private ToolStripMenuItem deleteToolStripMenuItem;
		private ToolStripMenuItem FileMenuItem;
		private ToolStripMenuItem duplicateToolStripMenuItem;
		private ToolStripMenuItem copyToolStripMenuItem;
		private ToolStripMenuItem pasteToolStripMenuItem;
		private ToolStripMenuItem cutToolStripMenuItem;
		private ToolStripMenuItem renameToolStripMenuItem;
		private ToolStripMenuItem ReloadFileMenuItem;
		private Label LblDetails;
		private TableLayoutPanel tableLayoutPanel1;
		internal TreeView treeView1;
		private PropertyGrid objectPropertyGrid;
		private TextBox TxtSearch;
		private TableLayoutPanel tableLayoutPanel2;
		private Button BtnNext;
		private Button BtnPrevious;
		private ToolStripMenuItem moveUpToolStripMenuItem;
		private ToolStripMenuItem moveDownToolStripMenuItem;
	}
}
