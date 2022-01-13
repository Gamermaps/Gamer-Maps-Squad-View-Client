namespace GamerMapsSquadViewClient.Helpers
{
	using System;
	using System.IO;

	internal static class FileHelper
	{
		internal static Boolean IsFileReady(String sFilename)
		{
			// If the file can be opened for exclusive access it means that the file
			// is no longer locked by another process.
			try
			{
				using (FileStream inputStream = File.Open(sFilename, FileMode.Open, FileAccess.Read, FileShare.None))
				{
					return inputStream.Length > 0;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
