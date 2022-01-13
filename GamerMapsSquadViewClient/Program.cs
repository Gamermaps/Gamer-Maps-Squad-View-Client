namespace GamerMapsSquadViewClient
{
	using System;
	using System.Threading;
	using System.Windows.Forms;
	using GamerMapsSquadViewClient.Forms;

	public class Program
	{
		[STAThread]
		private static void Main()
		{
			using (Mutex mutex = new Mutex(false, $@"Global\{nameof(GamerMapsSquadViewClient)}"))
			{
				if (!mutex.WaitOne(0, false))
				{
					MessageBox.Show("Program already running");
					return;
				}

				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);

				if (String.IsNullOrEmpty(Properties.Settings.Default.ScreenshotPath))
				{
					using (Form formPathSelector = new FrmPathSelector())
					{
						formPathSelector.ShowDialog();
					}
				}

				if (String.IsNullOrEmpty(Properties.Settings.Default.ScreenshotPath))
				{
					return;
				}

				using (Form frm = new FrmMain())
				{
					Application.Run(frm);
				}
			}
		}
	}
}
