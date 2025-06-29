using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.ComplexFabricatorsRandom;
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
    }
}
