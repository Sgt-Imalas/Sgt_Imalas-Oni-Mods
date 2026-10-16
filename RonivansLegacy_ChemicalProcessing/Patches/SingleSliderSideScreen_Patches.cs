using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	class SingleSliderSideScreen_Patches
	{
		[HarmonyPatch(typeof(SingleSliderSideScreen), nameof(SingleSliderSideScreen.IsValidForTarget))]
		public class SingleSliderSideScreen_IsValidForTarget_Patch
		{
			public static void Postfix(GameObject target, ref bool __result)
			{
				if (!__result)
					return;

				if (!target.TryGetComponent<KPrefabID>(out var prefab))
					return;
				foreach (var gen in GeneratorList.GeneratorsToIgnore)
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
