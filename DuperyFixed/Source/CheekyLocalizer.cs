using System.Collections.Generic;
using System.IO;
using static Localization;

namespace Dupery
{
	class CheekyLocalizer
	{
		public Dictionary<string, string> LocalizedStrings;

		public CheekyLocalizer(string directoryName)
		{
			string stringsPath = Path.Combine(directoryName, "strings", GetLocale()?.Code + ".po");
			this.LocalizedStrings = LoadStrings(stringsPath);
		}

		public bool TryGet(string key, out string value)
		{
			bool stringFound = LocalizedStrings.TryGetValue(key, out value);

			if (!stringFound)
			{
				stringFound = Strings.TryGet(new StringKey(key), out StringEntry entry);
				if (stringFound)
					value = entry.ToString();
			}

			return stringFound;
		}

		private Dictionary<string, string> LoadStrings(string stringsPath)
		{
			Dictionary<string, string> localizedStrings = new Dictionary<string, string>();

			if (File.Exists(stringsPath))
				localizedStrings = LoadStringsFile(stringsPath, false);

			return localizedStrings;
		}
	}
}
