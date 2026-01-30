using HarmonyLib;
using Klei;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	class CodexCache_Patches
	{

		[HarmonyPatch(typeof(CodexCache), nameof(CodexCache.CollectEntries))]
		public class CodexCache_CollectEntries_Patch
		{
			public static void Postfix(string folder, List<CodexEntry> __result)
			{
				SgtLogger.l("Collecting Codex Entries for " + folder);
				CodexUtils.CollectModdedCodexEntries(folder,__result, true);
			}
		}

		[HarmonyPatch(typeof(CodexCache), nameof(CodexCache.CollectSubEntries))]
		public class CodexCache_CollectSubEntries_Patch
		{
			public static void Postfix()
			{
				CodexDatabase.GenerateGuidanceDeviceEntries();
			}
		}
	}
}
