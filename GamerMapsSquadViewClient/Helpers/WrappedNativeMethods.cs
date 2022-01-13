namespace GamerMapsSquadViewClient.Helpers
{
	using System;
	using System.IO;
	using System.Runtime.InteropServices;

	internal class WrappedNativeMethods
	{
		internal static String GetMimeFromFile(String filename)
		{
			if (!File.Exists(filename))
			{
				throw new FileNotFoundException(filename + " not found");
			}

			Byte[] buffer = new Byte[256];
			using (FileStream fileStream = new FileStream(filename, FileMode.Open))
			{
				Int32 length = fileStream.Length < 256 ? (Int32)fileStream.Length : 256;
				fileStream.Read(buffer, 0, length);
			}

			try
			{
				UInt32 result = FindMimeFromData(IntPtr.Zero, null, buffer, 256, null, 0, out String mime, 0);
				switch (result)
				{
					case 0:
						return mime;

					case 0x80004005:
						return "Unspecified failure";

					case 0x8007000E:
						return "There is insufficient memory to complete the operation.";

					case 0x80070057:
						return "One or more arguments are not valid";

					default:
						return "unknown/unknown";
				}
			}
			catch
			{
				return "unknown/unknown";
			}
		}

		[DllImport("urlmon.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		private static extern UInt32 FindMimeFromData(
			IntPtr pBC,
			[MarshalAs(UnmanagedType.LPWStr)] String pwzUrl,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I1, SizeParamIndex = 3)] Byte[] pBuffer,
			UInt32 cbSize,
			[MarshalAs(UnmanagedType.LPWStr)] String pwzMimeProposed,
			Int32 dwMimeFlags,
			[MarshalAs(UnmanagedType.LPWStr)] out String ppwzMimeOut,
			Int32 dwReserved);
	}
}
