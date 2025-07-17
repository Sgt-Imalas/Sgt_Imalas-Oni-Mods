using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	class SingleSliderSideScreen_Patches
	{
		public static void AddGeneratorToIgnore(string ID) { GeneratorsToIgnore.Add(ID); SgtLogger.l("Adding generator to screen ignore list: " + ID); }
		static HashSet<string> GeneratorsToIgnore = [];
		[HarmonyPatch(typeof(SingleSliderSideScreen), nameof(SingleSliderSideScreen.IsValidForTarget))]
		public class SingleSliderSideScreen_IsValidForTarget_Patch
		{
			public static void Postfix(GameObject target, ref bool __result)
			{
				if (!__result)
					return;

				if (!target.TryGetComponent<KPrefabID>(out var prefab))
					return;
				foreach (var gen in GeneratorsToIgnore)
				{
					if (prefab.IsPrefabID(gen))
					{
						__result = false;
						return;
					}
				}
			}
		}
	}
}
