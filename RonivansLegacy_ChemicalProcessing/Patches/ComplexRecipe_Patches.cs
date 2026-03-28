using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
					__result = true;
            }
        }
	}
}
