using HarmonyLib;
using Klei;
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
				CodexUtils.CollectModdedCodexEntries(folder,__result, true);
			}
		}
	}
}
