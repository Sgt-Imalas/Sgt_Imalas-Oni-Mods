using ImageMagick;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _KAnimPackerExe
{
	internal class PNGConverter
	{
		public static string CreateNormalizedTempPng(string source)
		{
			string fileNameInput = Path.GetFileName(source);
			var targetDir = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
			Directory.CreateDirectory(targetDir);
			string temp = Path.Combine(targetDir, fileNameInput);
			Log.LogMessage("Temp file: " + temp);

			using var image = new MagickImage(source);

			image.ColorSpace = ColorSpace.sRGB;
			image.Strip();
			image.Write(temp, MagickFormat.Png);

			return temp;
		}
	}
}
