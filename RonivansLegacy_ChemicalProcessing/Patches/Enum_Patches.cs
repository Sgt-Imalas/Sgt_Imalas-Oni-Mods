using ElementUtilNamespace;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class Enum_Patches
    {
		/// <summary>
		/// Required for Simhashes conversion to string to include the modded elements
		/// </summary>
		// Credit: Heinermann (Blood mod)
		public static class EnumPatch
		{
			[HarmonyPatch(typeof(Enum), "ToString", [])]
			public class SimHashes_ToString_Patch
			{
				public static bool Prefix(ref Enum __instance, ref string __result) => SgtElementUtil.SimHashToString_EnumPatch(__instance, ref __result);
			}

			[HarmonyPatch(typeof(Enum), nameof(Enum.Parse), [typeof(Type), typeof(string), typeof(bool)])]
			private class SimHashes_Parse_Patch
			{
				private static bool Prefix(Type enumType, string value, ref object __result) => SgtElementUtil.SimhashParse_EnumPatch(enumType, value, ref __result);
			}
		}
	}
}
