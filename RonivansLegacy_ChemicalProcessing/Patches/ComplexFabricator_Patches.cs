using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.ComplexFabricatorsRandom;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.CustomComplexFabricators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class ComplexFabricator_Patches
    {

        [HarmonyPatch(typeof(ComplexFabricator), nameof(ComplexFabricator.CancelWorkingOrder))]
        public class ComplexFabricator_CancelWorkingOrder_Patch
        {
            public static void Prefix(ComplexFabricator __instance)
            {
                if(__instance is ComplexFabricatorRandomOutput fabricatorRandomOutput)
				{
                    fabricatorRandomOutput.DestroyFragileIngredientsOnCancel();
				}
			}
        }

        [HarmonyPatch(typeof(ComplexFabricator), nameof(ComplexFabricator.DropExcessIngredients))]
        public class ComplexFabricator_DropExcessIngredients_Patch
        {
            public static bool Prefix(ComplexFabricator __instance, Storage storage)
            {
                if(__instance is CustomComplexFabricatorBase p)

				{
                    p.DropExcessIngredients(storage);
                    return false;
                }
                return true;
            }
        }
    }
}
