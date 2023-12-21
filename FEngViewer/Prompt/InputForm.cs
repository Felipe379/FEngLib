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

		public InputForm()
		{
			InitializeComponent();
			BtnOk.Enabled = !string.IsNullOrWhiteSpace(TxtInput.Text);
		}

		public InputForm(CharacterCasing characterCasing)
		{
			InitializeComponent();

			TxtInput.CharacterCasing = CharacterCasing.Upper;
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
