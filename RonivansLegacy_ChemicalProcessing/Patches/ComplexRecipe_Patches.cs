using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class ComplexRecipe_Patches
	{

		[HarmonyPatch(typeof(ComplexRecipe), nameof(ComplexRecipe.IsAnyProductDeprecated))]
		public class ComplexRecipe_IsAnyProductDeprecated_Patch
		{
			public static void Postfix(ComplexRecipe __instance, ref bool __result)
			{
				if (__result)
					return;

				if (!__instance.fabricators.Any())
					return;

				var fabricatorId = __instance.fabricators.First();

				if (BuildingManager.DisabledBuildingIDs.Contains(fabricatorId.ToString()))
				{
					__result = true;
					return;
				}
				var ingredients = __instance.ingredients;
				if (ingredients != null && ingredients.Any())
				{
					for (int i = 0; i < __instance.ingredients.Length; i++)
					{
						var ingredient = ingredients[i].material;
						GameObject prefab = Assets.TryGetPrefab(ingredient);
						if (prefab != null && prefab.HasTag(GameTags.DeprecatedContent))
						{
							__result = true;
							return;
						}
					}
				}
			}
		}
	}
}
