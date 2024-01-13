namespace FEngViewer
{
    partial class NewKeyForm
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
            groupBox1 = new System.Windows.Forms.GroupBox();
            okButton = new System.Windows.Forms.Button();
            cancelButton = new System.Windows.Forms.Button();
            keyListBoxLabel = new System.Windows.Forms.Label();
            dupeKeyComboBox = new System.Windows.Forms.ComboBox();
            label1 = new System.Windows.Forms.Label();
            keyTimeTrackBar = new System.Windows.Forms.TrackBar();
            keyTimestampLabel = new System.Windows.Forms.Label();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)keyTimeTrackBar).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(okButton);
            groupBox1.Controls.Add(cancelButton);
            groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            groupBox1.Location = new System.Drawing.Point(0, 89);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(343, 48);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            // 
            // okButton
            // 
            okButton.Enabled = false;
            okButton.Location = new System.Drawing.Point(256, 18);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.TabIndex = 1;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += okButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Location = new System.Drawing.Point(12, 18);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.TabIndex = 0;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // keyListBoxLabel
            // 
            keyListBoxLabel.AutoSize = true;
            keyListBoxLabel.Location = new System.Drawing.Point(14, 19);
            keyListBoxLabel.Name = "keyListBoxLabel";
            keyListBoxLabel.Size = new System.Drawing.Size(82, 15);
            keyListBoxLabel.TabIndex = 1;
            keyListBoxLabel.Text = "Duplicate Key:";
            // 
            // dupeKeyComboBox
            // 
            dupeKeyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            dupeKeyComboBox.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dupeKeyComboBox.FormattingEnabled = true;
            dupeKeyComboBox.Location = new System.Drawing.Point(101, 16);
            dupeKeyComboBox.Name = "dupeKeyComboBox";
            dupeKeyComboBox.Size = new System.Drawing.Size(196, 21);
            dupeKeyComboBox.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(14, 45);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(58, 15);
            label1.TabIndex = 3;
            label1.Text = "Key Time:";
            // 
            // keyTimeTrackBar
            // 
            keyTimeTrackBar.LargeChange = 100;
            keyTimeTrackBar.Location = new System.Drawing.Point(94, 45);
            keyTimeTrackBar.Maximum = 100;
            keyTimeTrackBar.Name = "keyTimeTrackBar";
            keyTimeTrackBar.Size = new System.Drawing.Size(156, 45);
            keyTimeTrackBar.SmallChange = 20;
            keyTimeTrackBar.TabIndex = 4;
            keyTimeTrackBar.TickFrequency = 100;
            // 
            // keyTimestampLabel
            // 
            keyTimestampLabel.AutoSize = true;
            keyTimestampLabel.Location = new System.Drawing.Point(256, 45);
            keyTimestampLabel.Name = "keyTimestampLabel";
            keyTimestampLabel.Size = new System.Drawing.Size(41, 15);
            keyTimestampLabel.TabIndex = 5;
            keyTimestampLabel.Text = "100ms";
            // 
            // NewKeyForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(343, 137);
            Controls.Add(keyTimestampLabel);
            Controls.Add(keyTimeTrackBar);
            Controls.Add(label1);
            Controls.Add(dupeKeyComboBox);
            Controls.Add(keyListBoxLabel);
            Controls.Add(groupBox1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "NewKeyForm";
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Create Key";
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)keyTimeTrackBar).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label keyListBoxLabel;
        private System.Windows.Forms.ComboBox dupeKeyComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar keyTimeTrackBar;
        private System.Windows.Forms.Label keyTimestampLabel;
    }
}