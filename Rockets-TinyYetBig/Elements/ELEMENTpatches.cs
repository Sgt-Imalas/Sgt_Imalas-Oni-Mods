using ElementUtilNamespace;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Rockets_TinyYetBig.Elements
{

	public class ELEMENTpatches
	{
		/// <summary>
		/// from akis beached 
		/// </summary>
		[HarmonyPatch(typeof(ElementLoader))]
		[HarmonyPatch(nameof(ElementLoader.Load))]
		public class ElementLoader_Load_Patch
		{
			public static void Prefix(Dictionary<string, SubstanceTable> substanceTablesByDlc, ref Hashtable substanceList)
			{
				// Add my new elements
				var list = substanceTablesByDlc[DlcManager.VANILLA_ID].GetList();
				ModElements.RegisterSubstances(list, ref substanceList);
			}
		}


		[HarmonyPatch(typeof(ElementLoader), "CollectElementsFromYAML")]
		public class MakeNeutroniumFullyRadBlocking_Patch
		{
			public static void Postfix(ref List<ElementLoader.ElementEntry> __result)
			{
				foreach (var elem in __result)
				{
					if (elem.elementId == SimHashes.Unobtanium.ToString())
					{
						elem.radiationAbsorptionFactor = 1.26f;
					}
				}
			}
		}

		// Credit: Heinermann (Blood mod)
		public static class EnumPatch
		{
			[HarmonyPatch(typeof(Enum), "ToString", new Type[] { })]
			private class SimHashes_ToString_Patch
			{
				private static bool Prefix(ref Enum __instance, ref string __result)
				{
					if (__instance is SimHashes hashes)
					{
						return !SgtElementUtil.SimHashNameLookup.TryGetValue(hashes, out __result);
					}

					return true;
				}
			}

			[HarmonyPatch(typeof(Enum), nameof(Enum.Parse), new Type[] { typeof(Type), typeof(string), typeof(bool) })]
			private class SimHashes_Parse_Patch
			{
				private static bool Prefix(Type enumType, string value, ref object __result)
				{
					if (enumType.Equals(typeof(SimHashes)))
					{
						return !SgtElementUtil.ReverseSimHashNameLookup.TryGetValue(value, out __result);
					}

					return true;
				}
			}
		}
	}
}
