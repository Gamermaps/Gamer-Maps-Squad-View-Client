namespace GamerMapsSquadViewClient.Forms
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Windows.Forms;
	using GamerMapsSquadViewClient.Helpers;

	public partial class FrmMain : Form
	{
		private readonly String _tmpImage;
		private String _accessKey;
		private Int32 _screenshotsUploaded = 0;

		public FrmMain()
		{
			InitializeComponent();

			_tmpImage = Path.Combine(Path.GetTempPath(), $"{nameof(GamerMapsSquadViewClient)}.jpg");
		}

		private void FrmMain_Load(Object sender, EventArgs e)
		{
			if (!String.IsNullOrEmpty(Properties.Settings.Default.UserName))
			{
				txtUsername.Text = Crypto.Decrypt(Properties.Settings.Default.UserName);
			}

			if (!String.IsNullOrEmpty(Properties.Settings.Default.Password))
			{
				txtPassword.Text = Crypto.Decrypt(Properties.Settings.Default.Password);
			}
		}

		private void FrmMain_FormClosing(Object sender, FormClosingEventArgs e)
		{
			ntfyMenu.Icon.Dispose();
		}

		private void ExitToolStripMenuItem_Click(Object sender, EventArgs e)
		{
			Close();
		}

		private void BtnLogin_Click(Object sender, EventArgs e)
		{
			// Store username and password
			Properties.Settings.Default.UserName = Crypto.Crypt(txtUsername.Text);
			Properties.Settings.Default.Password = Crypto.Crypt(txtPassword.Text);
			Properties.Settings.Default.Save();

			if (String.IsNullOrEmpty(txtUsername.Text))
			{
				lblStatus.Text = "Please enter username";
				return;
			}

			if (String.IsNullOrEmpty(txtPassword.Text))
			{
				lblStatus.Text = "Please enter password";
				return;
			}

			HttpResult httpResult = HttpManager.ClientLogin(txtUsername.Text, txtPassword.Text);
			if (!httpResult.Success)
			{
				lblStatus.Text = "Invalid user account";
				return;
			}

			_accessKey = httpResult.Message;

			fileSystemWatcher1.Path = Properties.Settings.Default.ScreenshotPath;
			fileSystemWatcher1.EnableRaisingEvents = true;

			pnlLogin.Visible = false;
			lblInfo.Visible = true;
			lblStatus.Text = "Ready for action...";
		}

		private void TsmChooseScreenshot_Click(Object sender, EventArgs e)
		{
			fileSystemWatcher1.EnableRaisingEvents = false;

			using (Form frm = new FrmPathSelector())
			{
				frm.ShowDialog();
			}

			fileSystemWatcher1.Path = Properties.Settings.Default.ScreenshotPath;
			fileSystemWatcher1.EnableRaisingEvents = true;
		}

		private void LnkWebsite_LinkClicked(Object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://gamermaps.net/squadview");
		}

		private void FileSystemWatcher1_Created(Object sender, FileSystemEventArgs e)
		{
			try
			{
				// it fires twice for one file save.
				fileSystemWatcher1.EnableRaisingEvents = false;

				String sourceFilePath = Path.Combine(Properties.Settings.Default.ScreenshotPath, e.Name);

				// Check for file access..
				while (!FileHelper.IsFileReady(sourceFilePath))
				{
				}

				// Valid format?
				String mimeType = WrappedNativeMethods.GetMimeFromFile(sourceFilePath);
				if (!mimeType.Equals("image/x-png") && !mimeType.Equals("image/pjpeg"))
				{
					return;
				}

				ImageHelpers.ProcessImage(sourceFilePath, _tmpImage);

				HttpResult httpResult = HttpManager.UploadFile(_accessKey, _tmpImage);
				if (httpResult.Success)
				{
					_screenshotsUploaded++;
					lblStatus.Text = $"Uploaded {_screenshotsUploaded} screenshot{(_screenshotsUploaded == 1 ? String.Empty : "s")}";
				}
				else
				{
					lblStatus.Text = httpResult.Message;
				}

				try
				{
					File.Delete(_tmpImage);
				}
				catch
				{
				}
			}
			finally
			{
				fileSystemWatcher1.EnableRaisingEvents = true;
			}
		}
	}
}
