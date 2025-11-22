using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _SgtsModUpdater
{
    public class Paths
    {
		public static string GameDocumentsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Klei","OxygenNotIncluded");
		public static string ModsFolder = Path.Combine(GameDocumentsFolder, "mods");
		public static string LocalModsFolder = Path.Combine(ModsFolder, "Local");
		public static string DevModsFolder = Path.Combine(ModsFolder, "Dev");
		public static string SteamModsFolder = Path.Combine(ModsFolder, "Steam");
		
		
		public static string GetReadableFileSize(double len)
		{
			string[] sizes = { "B", "KB", "MB", "GB", "TB" };
			int order = 0;
			while (len >= 1024 && order < sizes.Length - 1)
			{
				order++;
				len = len / 1024;
			}
			return String.Format("{0:0.##} {1}", len, sizes[order]);
		}
		public static string StripFormatting(string toStrip)
		{
			return Regex.Replace(toStrip, "<[^>]+>", "");
		}
	}
}
