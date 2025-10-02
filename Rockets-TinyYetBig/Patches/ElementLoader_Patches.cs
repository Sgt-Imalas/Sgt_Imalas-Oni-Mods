using ElementUtilNamespace;
using HarmonyLib;
using Rockets_TinyYetBig.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.Patches.ElementPatches
{
	internal class ElementLoader_Patches
	{
		[HarmonyPatch(typeof(ElementLoader), nameof(ElementLoader.Load))]
		public class ElementLoader_Load_Patch
		{
			/// <summary>
			/// Adds the new mod elements to the substance list
			/// </summary>
			public static void Prefix(Dictionary<string, SubstanceTable> substanceTablesByDlc, ref Hashtable substanceList) => ModElements.RegisterSubstances(substanceTablesByDlc[DlcManager.VANILLA_ID].GetList());

			/// <summary>
			/// Register additional substance tags after all elements are loaded
			/// </summary>
			[HarmonyPriority(Priority.Low)]
			public static void Postfix() => ModElements.RegisterAdditionalSubstanceTags();
		}

		/// <summary>
		/// make neutronium fully block radiation since it is used in the plated nosecone walls
		/// </summary>
		[HarmonyPatch(typeof(ElementLoader), nameof(ElementLoader.CollectElementsFromYAML))]
		public class MakeNeutroniumFullyRadBlocking_Patch
		{
			public static void Postfix(ref List<ElementLoader.ElementEntry> __result)
			{
				ModElements.ModifyExistingElements(__result);
			}
		}
	}
}
