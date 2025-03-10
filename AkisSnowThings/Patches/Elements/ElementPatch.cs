using AkisSnowThings.Content.Scripts.Elements;
using ElementUtilNamespace;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkisSnowThings.Patches.Elements
{
    internal class ElementPatch
    {
        /// <summary>
        /// Credit: akis beached 
        /// </summary>
        [HarmonyPatch(typeof(ElementLoader))]
        [HarmonyPatch(nameof(ElementLoader.Load))]
        public class ElementLoader_Load_Patch
        {
            public static void Prefix(Dictionary<string, SubstanceTable> substanceTablesByDlc)
            {
                var list = substanceTablesByDlc[DlcManager.VANILLA_ID].GetList();
                SnowModElements.RegisterSubstances(list);
            }
		}

		/// <summary>
		/// Required for Simhashes conversion to string to include the modded elements
		/// </summary>
		// Credit: Heinermann (Blood mod)
		public static class EnumPatch
		{
			[HarmonyPatch(typeof(Enum), "ToString", new Type[] { })]
			public class SimHashes_ToString_Patch
			{
				public static bool Prefix(ref Enum __instance, ref string __result) => SgtElementUtil.SimHashToString_EnumPatch(__instance, ref __result);
			}

			[HarmonyPatch(typeof(Enum), nameof(Enum.Parse), new Type[] { typeof(Type), typeof(string), typeof(bool) })]
			private class SimHashes_Parse_Patch
			{
				private static bool Prefix(Type enumType, string value, ref object __result) => SgtElementUtil.SimhashParse_EnumPatch(enumType, value, ref __result);
			}
		}
	}
}
