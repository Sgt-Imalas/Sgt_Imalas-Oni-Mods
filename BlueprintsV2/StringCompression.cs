using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace BlueprintsV2
{

	internal class StringCompression
	{

		/// <summary>
		/// Compresses the string.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns></returns>
		public static string CompressString(string text)
		{
			//return "```" + text + "```";
			byte[] buffer = Encoding.UTF8.GetBytes(text);
			var memoryStream = new MemoryStream();
			using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
			{
				gZipStream.Write(buffer, 0, buffer.Length);
			}

			memoryStream.Position = 0;

			var compressedData = new byte[memoryStream.Length];
			memoryStream.Read(compressedData, 0, compressedData.Length);

			var gZipBuffer = new byte[compressedData.Length + 4];
			Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
			Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
			return Convert.ToBase64String(gZipBuffer);
		}

		/// <summary>
		/// Decompresses the string.
		/// </summary>
		/// <param name="compressedText">The compressed text.</param>
		/// <returns></returns>
		public static string DecompressString(string compressedText)
		{
			try
			{
				//return compressedText.Trim('`');
				byte[] gZipBuffer = Convert.FromBase64String(compressedText);
				using (var memoryStream = new MemoryStream())
				{
					int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
					memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

					var buffer = new byte[dataLength];

					memoryStream.Position = 0;
					using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
					{
						gZipStream.Read(buffer, 0, buffer.Length);
					}

					return Encoding.UTF8.GetString(buffer);
				}
			}
			catch (Exception ex) 
			{
				return string.Empty;
			}
		}
	}
}
