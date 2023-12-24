using System;
using System.Windows.Forms;

namespace FEngViewer.Prompt
{
	public partial class InputForm : Form
	{
		public string Input
		{
			get { return TxtInput.Text; }
			set { TxtInput.Text = value; }
		}

		public bool CreateChildren
		{
			get { return ChkChildren.Checked; }
			set { ChkChildren.Checked = value; }
		}

		public InputForm()
		{
			InitializeComponent();
			BtnOk.Enabled = !string.IsNullOrWhiteSpace(TxtInput.Text);
			ChkChildren.Visible = false;
		}

		public InputForm(CharacterCasing characterCasing, bool createChildren) : this()
		{
			TxtInput.CharacterCasing = characterCasing;
			ChkChildren.Visible = createChildren;
		}

		private void BtnOk_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Input = TxtInput.Text;
		}

		private void BtnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Input = null;
		}

		private void TxtInput_TextChanged(object sender, EventArgs e)
		{
			BtnOk.Enabled = !string.IsNullOrWhiteSpace(TxtInput.Text);
		}
	}
}
