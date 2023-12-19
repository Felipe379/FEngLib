
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
			this.splitContainer1 = new SplitContainer();
			this.treeView1 = new TreeView();
			this.splitContainer2 = new SplitContainer();
			this.viewOutput = new GLRenderControl();
			this.labelCoordDisplay = new Label();
			this.groupBgColor = new GroupBox();
			this.radioBgGreen = new RadioButton();
			this.radioBgBlack = new RadioButton();
			this.objectPropertyGrid = new PropertyGrid();
			this.colorDialog1 = new ColorDialog();
			this.menuStrip1 = new MenuStrip();
			this.FileMenuItem = new ToolStripMenuItem();
			this.OpenFileMenuItem = new ToolStripMenuItem();
			this.SaveFileMenuItem = new ToolStripMenuItem();
			this.JsonImportMenuItem = new ToolStripMenuItem();
			this.JsonExportMenuItem = new ToolStripMenuItem();
			this.objectContextMenu = new ContextMenuStrip(this.components);
			this.deleteToolStripMenuItem = new ToolStripMenuItem();
			this.scriptContextMenu = new ContextMenuStrip(this.components);
			this.toggleScriptItem = new ToolStripMenuItem();
			this.cloneToolStripMenuItem = new ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)this.splitContainer1).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.splitContainer2).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.groupBgColor.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.objectContextMenu.SuspendLayout();
			this.scriptContextMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.splitContainer1.Location = new System.Drawing.Point(0, 31);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.treeView1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size(1499, 963);
			this.splitContainer1.SplitterDistance = 429;
			this.splitContainer1.TabIndex = 0;
			// 
			// treeView1
			// 
			this.treeView1.Dock = DockStyle.Fill;
			this.treeView1.Location = new System.Drawing.Point(0, 0);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(429, 963);
			this.treeView1.TabIndex = 1;
			this.treeView1.AfterSelect += this.treeView1_AfterSelect;
			this.treeView1.MouseDown += this.treeView1_MouseDown;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.splitContainer2.FixedPanel = FixedPanel.Panel1;
			this.splitContainer2.Location = new System.Drawing.Point(3, 3);
			this.splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.viewOutput);
			this.splitContainer2.Panel1.Controls.Add(this.labelCoordDisplay);
			this.splitContainer2.Panel1.Controls.Add(this.groupBgColor);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.objectPropertyGrid);
			this.splitContainer2.Size = new System.Drawing.Size(1060, 960);
			this.splitContainer2.SplitterDistance = 649;
			this.splitContainer2.TabIndex = 5;
			// 
			// viewOutput
			// 
			this.viewOutput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.viewOutput.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.viewOutput.BackColor = System.Drawing.Color.Black;
			this.viewOutput.Location = new System.Drawing.Point(3, 2);
			this.viewOutput.Margin = new Padding(3, 2, 3, 2);
			this.viewOutput.MaximumSize = new System.Drawing.Size(640, 480);
			this.viewOutput.MinimumSize = new System.Drawing.Size(640, 480);
			this.viewOutput.Name = "viewOutput";
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
			this.labelCoordDisplay.Location = new System.Drawing.Point(3, 484);
			this.labelCoordDisplay.Name = "labelCoordDisplay";
			this.labelCoordDisplay.Size = new System.Drawing.Size(72, 15);
			this.labelCoordDisplay.TabIndex = 2;
			this.labelCoordDisplay.Text = "X:    0   Y:    0";
			// 
			// groupBgColor
			// 
			this.groupBgColor.Controls.Add(this.radioBgGreen);
			this.groupBgColor.Controls.Add(this.radioBgBlack);
			this.groupBgColor.Location = new System.Drawing.Point(489, 484);
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
			// objectPropertyGrid
			// 
			this.objectPropertyGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.objectPropertyGrid.Location = new System.Drawing.Point(3, 3);
			this.objectPropertyGrid.Name = "objectPropertyGrid";
			this.objectPropertyGrid.Size = new System.Drawing.Size(401, 953);
			this.objectPropertyGrid.TabIndex = 4;
			// 
			// menuStrip1
			// 
			this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.menuStrip1.Items.AddRange(new ToolStripItem[] { this.FileMenuItem });
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(1499, 24);
			this.menuStrip1.TabIndex = 4;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// FileMenuItem
			// 
			this.FileMenuItem.DropDownItems.AddRange(new ToolStripItem[] { this.OpenFileMenuItem, this.SaveFileMenuItem, this.JsonImportMenuItem, this.JsonExportMenuItem });
			this.FileMenuItem.Name = "FileMenuItem";
			this.FileMenuItem.Size = new System.Drawing.Size(37, 20);
			this.FileMenuItem.Text = "File";
			// 
			// OpenFileMenuItem
			// 
			this.OpenFileMenuItem.Name = "OpenFileMenuItem";
			this.OpenFileMenuItem.Size = new System.Drawing.Size(141, 22);
			this.OpenFileMenuItem.Text = "Open";
			this.OpenFileMenuItem.Click += this.OpenFileMenuItem_Click;
			// 
			// SaveFileMenuItem
			// 
			this.SaveFileMenuItem.Name = "SaveFileMenuItem";
			this.SaveFileMenuItem.Size = new System.Drawing.Size(141, 22);
			this.SaveFileMenuItem.Text = "Save";
			this.SaveFileMenuItem.Click += this.SaveFileMenuItem_Click;
			// 
			// JsonImportMenuItem
			// 
			this.JsonImportMenuItem.Name = "JsonImportMenuItem";
			this.JsonImportMenuItem.Size = new System.Drawing.Size(141, 22);
			this.JsonImportMenuItem.Text = "JSON Import";
			this.JsonImportMenuItem.Click += this.JsonImportMenuItem_Click;
			// 
			// JsonExportMenuItem
			// 
			this.JsonExportMenuItem.Name = "JsonExportMenuItem";
			this.JsonExportMenuItem.Size = new System.Drawing.Size(141, 22);
			this.JsonExportMenuItem.Text = "JSON Export";
			this.JsonExportMenuItem.Click += this.JsonExportMenuItem_Click;
			// 
			// objectContextMenu
			// 
			this.objectContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.objectContextMenu.Items.AddRange(new ToolStripItem[] { this.cloneToolStripMenuItem, this.deleteToolStripMenuItem });
			this.objectContextMenu.Name = "objectContextMenu";
			this.objectContextMenu.Size = new System.Drawing.Size(181, 70);
			// 
			// deleteToolStripMenuItem
			// 
			this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			this.deleteToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
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
			// cloneToolStripMenuItem
			// 
			this.cloneToolStripMenuItem.Name = "cloneToolStripMenuItem";
			this.cloneToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.cloneToolStripMenuItem.Text = "Clone";
			this.cloneToolStripMenuItem.Click += this.cloneToolStripMenuItem_Click;
			// 
			// PackageView
			// 
			this.AutoScaleMode = AutoScaleMode.Inherit;
			this.ClientSize = new System.Drawing.Size(1499, 1061);
			this.Controls.Add(this.menuStrip1);
			this.Controls.Add(this.splitContainer1);
			this.Name = "PackageView";
			this.Text = "FEngViewer";
			this.Load += this.PackageView_Load;
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.splitContainer1).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel1.PerformLayout();
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.splitContainer2).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.groupBgColor.ResumeLayout(false);
			this.groupBgColor.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.objectContextMenu.ResumeLayout(false);
			this.scriptContextMenu.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
        private GLRenderControl viewOutput;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Label labelCoordDisplay;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem OpenFileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveFileMenuItem;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ContextMenuStrip objectContextMenu;
        private System.Windows.Forms.ContextMenuStrip scriptContextMenu;
        private System.Windows.Forms.ToolStripMenuItem toggleScriptItem;
        private System.Windows.Forms.GroupBox groupBgColor;
        private System.Windows.Forms.RadioButton radioBgGreen;
        private System.Windows.Forms.RadioButton radioBgBlack;
        private System.Windows.Forms.PropertyGrid objectPropertyGrid;
        private SplitContainer splitContainer2;
        private ToolStripMenuItem deleteToolStripMenuItem;
		private ToolStripMenuItem FileMenuItem;
		private ToolStripMenuItem JsonExportMenuItem;
		private ToolStripMenuItem JsonImportMenuItem;
		private ToolStripMenuItem cloneToolStripMenuItem;
	}
}
