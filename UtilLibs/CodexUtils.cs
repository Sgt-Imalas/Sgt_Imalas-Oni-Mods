using Klei;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilLibs
{
	public static class CodexUtils
	{
		public static void PostProcess(List<CodexEntry> resultList)
		{
			foreach (CodexEntry codexEntry in resultList)
			{
				if (string.IsNullOrEmpty(codexEntry.sortString))
					codexEntry.sortString = (string)Strings.Get(codexEntry.title);
			}
			resultList.Sort((x, y) => x.sortString.CompareTo(y.sortString));
		}

		static string ModBasePath = Path.Combine(IO_Utils.ModPath, "codex");

		public static void CollectModdedCodexEntries(string category, List<CodexEntry> result, bool overrideExisting = false)
		{
			string SearchPath = category == "" ? ModBasePath : System.IO.Path.Combine(ModBasePath, category);
			if (!Directory.Exists(SearchPath))
			{
				return;
			}

			string[] yamlFiles = null;
			try
			{
				yamlFiles = Directory.GetFiles(SearchPath, "*.yaml");
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex);
			}
			if (yamlFiles == null || !yamlFiles.Any()) return;


			string upper = category.ToUpper();
			foreach (string codexFile in yamlFiles)
			{
				SgtLogger.l("CodexEntry: " + codexFile);

				if (!CodexCache.IsSubEntryAtPath(codexFile))
				{
					try
					{
						CodexEntry codexEntry = YamlIO.LoadFile<CodexEntry>(codexFile, new YamlIO.ErrorHandler(CodexCache.YamlParseErrorCB), CodexCache.widgetTagMappings);
						if (codexEntry != null)
						{
							codexEntry.category = upper;
							if (codexEntry.sortString.IsNullOrWhiteSpace())
								codexEntry.sortString = Strings.Get(codexEntry.title);

							if (overrideExisting)
							{
								result.RemoveAll(entry => entry.id == codexEntry.id);
							}

							result.Add(codexEntry);
							SgtLogger.l("added modded codex entry: " + codexEntry.id + " to category " + category);
						}
					}
					catch (Exception ex)
					{
						DebugUtil.DevLogErrorFormat("CodexCache.CollectEntries failed to load [{0}]: {1}", codexFile, ex.ToString());
					}
				}
			}
			CodexUtils.PostProcess(result);
		}
	}
}
