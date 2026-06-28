using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class CodexElementCategoryList_Patches
	{

        [HarmonyPatch(typeof(CodexElementCategoryList), nameof(CodexElementCategoryList.Configure))]
        public class CodexElementCategoryList_Configure_Patch
        {
			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{
				var m_GetPrefabsWithTag = AccessTools.Method(typeof(Assets), nameof(Assets.GetPrefabsWithTag), [typeof(Tag)]);

				foreach (var ci in orig)
				{
					yield return ci;
					if (ci.Calls(m_GetPrefabsWithTag))
					{
						yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CodexElementCategoryList_Configure_Patch), nameof(RemoveDisabledEntries)));
					}
				}
			}

			private static List<GameObject> RemoveDisabledEntries(List<GameObject> originalList)
			{
				originalList.RemoveAll(entry => entry.HasTag(GameTags.HideFromCodex));
				return originalList;
			}
		}
	}
}
