using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UtilLibs
{
	public static class Extensions
	{
		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			var known = new HashSet<TKey>();
			return source.Where(element => known.Add(keySelector(element)));
		}


		public static void AddRange(this IDictionary dict, IDictionary other)
		{
			foreach (DictionaryEntry item in other)
			{
				dict.Add(item.Key, item.Value);
			}
		}
		public static T CreateDelegate<T>(this MethodInfo method) where T : MulticastDelegate => (T)Delegate.CreateDelegate(typeof(T), method);




		/// <summary>
		/// Compresses a string.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns></returns>
		public static string CompressString(this string text)
		{
			//return "```" + text + "```";
			byte[] buffer = Encoding.UTF8.GetBytes(text);
			byte[] compressedData;
			using (var memoryStream = new MemoryStream())
			{
				using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))	
					gZipStream.Write(buffer, 0, buffer.Length);
				compressedData = memoryStream.ToArray();
			}
			var gZipBuffer = new byte[compressedData.Length + 4];
			Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
			Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
			return Convert.ToBase64String(gZipBuffer);
		}

		/// <summary>
		/// Decompresses a string.
		/// </summary>
		/// <param name="compressedText">The compressed text.</param>
		/// <returns></returns>
		public static string DecompressString(this string compressedText)
		{
			try
			{
				byte[] gZipBuffer = Convert.FromBase64String(compressedText);
				using var compressedStream = new MemoryStream(gZipBuffer, 4, gZipBuffer.Length - 4);
				using var gZipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
				using var uncompressedStream = new MemoryStream();
				gZipStream.CopyTo(uncompressedStream);
				return Encoding.UTF8.GetString(uncompressedStream.ToArray());
			}
			catch (Exception ex)
			{
				SgtLogger.error("Decompression Failure: " + ex.Message);
				return string.Empty;
			}
		}

	}
}
