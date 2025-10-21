using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ElementLoader;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	class ElementLoader_Patches
	{

		[HarmonyPatch(typeof(ElementLoader), nameof(ElementLoader.Load))]
		public class ElementLoader_Load_Patch
		{
			public static void Prefix(Dictionary<string, SubstanceTable> substanceTablesByDlc)
			{
				var list = substanceTablesByDlc[DlcManager.VANILLA_ID].GetList();
				ModElements.RegisterSubstances(list);
			}
		}

		[HarmonyPatch(typeof(ElementLoader), nameof(ElementLoader.CollectElementsFromYAML))]
		public class ElementLoader_CollectElementsFromYAML_Patch
		{
			public static void Postfix(ref List<ElementEntry> __result)
			{
				ModElements.ClearReenabledVanillaElementCodexTags(ref __result);
			}
		}

		[HarmonyPatch(typeof(ElementLoader), nameof(ElementLoader.FinaliseElementsTable))]
		public class ElementLoader_FinaliseElementsTable_Patch
		{
			public static void Postfix()
			{
				ModElements.ModifyExistingElements();
			}
		}
	}
}
