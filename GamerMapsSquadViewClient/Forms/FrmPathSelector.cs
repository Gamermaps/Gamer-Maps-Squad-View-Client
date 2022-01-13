namespace GamerMapsSquadViewClient.Forms
{
	using System;
	using System.Windows.Forms;

	public partial class FrmPathSelector : Form
	{
		public FrmPathSelector()
		{
			InitializeComponent();
		}

		private void FrmPathSelector_Load(Object sender, EventArgs e)
		{
			lblSelected.Text = Properties.Settings.Default.ScreenshotPath;
		}

		private void FrmPathSelector_FormClosed(Object sender, FormClosedEventArgs e)
		{
			Properties.Settings.Default.ScreenshotPath = lblSelected.Text;
			Properties.Settings.Default.Save();
		}

		private void Button1_Click(Object sender, EventArgs e)
		{
			DialogResult result = chooseFolder.ShowDialog();

			if (String.IsNullOrEmpty(chooseFolder.SelectedPath))
			{
				return;
			}

			lblSelected.Text = chooseFolder.SelectedPath.ToString();
			Properties.Settings.Default.ScreenshotPath = lblSelected.Text;
			Properties.Settings.Default.Save();
		}

		private void BtnClose_Click(Object sender, EventArgs e)
		{
			Close();
		}
	}
}
