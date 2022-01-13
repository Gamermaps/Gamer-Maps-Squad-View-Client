namespace GamerMapsSquadViewClient.Helpers
{
	using System;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Drawing.Imaging;

	internal static class ImageHelpers
	{
		private const Int32 _targetWidth = 1920;
		private const Int32 _targetHeight = 1080;

		internal static void ProcessImage(String originalFilePath, String targetFilePath)
		{
			ImageCodecInfo imageCodecInfo = GetEncoder(ImageFormat.Jpeg);

			using (Bitmap originalBitmap = new Bitmap(originalFilePath))
			using (Bitmap targetBitmap = new Bitmap(_targetWidth, _targetHeight))
			using (Graphics graphics = Graphics.FromImage(targetBitmap))
			using (EncoderParameters myEncoderParameters = new EncoderParameters())
			using (EncoderParameter myEncoderParameter = new EncoderParameter(Encoder.Quality, 30L))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
				myEncoderParameters.Param[0] = myEncoderParameter;

				Single widthRatio = (Single)_targetWidth / originalBitmap.Width;
				Single heightRatio = (Single)_targetHeight / originalBitmap.Height;
				Single ratio = widthRatio < heightRatio ? widthRatio : heightRatio;
				Int32 insideWidth = (Int32)(originalBitmap.Width * ratio);
				Int32 insideHeight = (Int32)(originalBitmap.Height * ratio);

				Rectangle targetRetangle = new Rectangle((_targetWidth / 2) - (insideWidth / 2), (_targetHeight / 2) - (insideHeight / 2), insideWidth, insideHeight);

				graphics.DrawImage(originalBitmap, targetRetangle);
				targetBitmap.Save(targetFilePath, imageCodecInfo, myEncoderParameters);
			}
		}

		private static ImageCodecInfo GetEncoder(ImageFormat format)
		{
			ImageCodecInfo[] encoders = ImageCodecInfo.GetImageDecoders();

			foreach (ImageCodecInfo codec in encoders)
			{
				if (codec.FormatID == format.Guid)
				{
					return codec;
				}
			}

			return null;
		}
	}
}
