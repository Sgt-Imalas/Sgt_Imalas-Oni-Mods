using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace UtilLibs
{
	public static class SanitationUtils
	{
		static HashSet<char> ForbiddenCharacters;

		/// <summary>
		/// bad things happen on windows if you name a file like that, regardless of the file type
		/// </summary>
		static HashSet<string> ForbiddenNames = new HashSet<string>()
		{
			"CON", "PRN", "AUX", "NUL","COM1"
			,"COM2" ,"COM3" , "COM4", "COM5" , "COM6","COM7" , "COM8", "COM9" , "LPT1"
			,"LPT2" ,"LPT3" ,"LPT4" ,"LPT5" ,"LPT6" ,"LPT7" ,"LPT8" , "LPT9"
		};
		public static string SanitizeName(string name)
		{
			string returnString = string.Empty;
			if (ForbiddenCharacters == null)
			{
				ForbiddenCharacters = new HashSet<char>();
				ForbiddenCharacters.UnionWith(Path.GetInvalidFileNameChars());
				ForbiddenCharacters.UnionWith(Path.GetInvalidFileNameChars());
				ForbiddenCharacters.UnionWith(Path.GetInvalidPathChars());
			}
			for (int i = 0; i < name.Length; ++i)
			{
				char character = name[i];
				returnString += ForbiddenCharacters.Contains(character) ? '_' : character;
			}
			returnString = returnString.Trim(' ');
			if (ForbiddenNames.Contains(returnString))
			{
				returnString = Path.GetRandomFileName();
			}
			return returnString;
		}
		public static string SanitizeModName(string modName)
		{
			var reg = new Regex("(\\/\\/.*)");
			var match = reg.Match(modName);
			if (match.Success)
			{
				return modName.Substring(0, match.Index).TrimEnd(' ');
			}
			return modName;
		}

		public static string GetSanitizedNamePath(string source)
		{
			//SgtLogger.l("Sanitizing...");
			//SgtLogger.l(source, "1");
			source = Path.GetFileName(source);
			//SgtLogger.l(source, "2");
			source = ReplaceInvalidChars(source);
			//SgtLogger.l(source, "3");

			if (ForbiddenNames.Contains(source.ToUpperInvariant()))
			{
				SgtLogger.l("file name was one of the forbidden ones, replacing..");
				source = Path.GetRandomFileName();
				//SgtLogger.l(source, "4");
			}

			return source;
		}

		public static string ReplaceInvalidChars(string filename)
		{
			return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
		}
	}
}
