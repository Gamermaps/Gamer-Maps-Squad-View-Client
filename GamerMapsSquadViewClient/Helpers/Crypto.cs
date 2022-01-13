namespace GamerMapsSquadViewClient.Helpers
{
	using System;
	using System.Security.Cryptography;
	using System.Text;

	internal static class Crypto
	{
		private static readonly Byte[] _Key = new Byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
		private static readonly Byte[] _Iv = new Byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };

		internal static String Crypt(this String text)
		{
			using (SymmetricAlgorithm algorithm = DES.Create())
			using (ICryptoTransform transform = algorithm.CreateEncryptor(_Key, _Iv))
			{
				Byte[] inputBuffer = Encoding.Unicode.GetBytes(text);
				Byte[] outputBuffer = transform.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
				return Convert.ToBase64String(outputBuffer);
			}
		}

		internal static String Decrypt(this String text)
		{
			using (SymmetricAlgorithm algorithm = DES.Create())
			using (ICryptoTransform transform = algorithm.CreateDecryptor(_Key, _Iv))
			{
				Byte[] inputBuffer = Convert.FromBase64String(text);
				Byte[] outputBuffer = transform.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
				return Encoding.Unicode.GetString(outputBuffer);
			}
		}
	}
}
