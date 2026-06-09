using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class WoodGasGeneratorConfig_Patches
	{
		/// <summary>
		/// Adds ui to the wood generator to select the type of acceptable fuels
		/// </summary>
        [HarmonyPatch(typeof(WoodGasGeneratorConfig), nameof(WoodGasGeneratorConfig.DoPostConfigureComplete))]
        public class WoodGasGeneratorConfig_DoPostConfigureComplete_Patch
        {
            public static void Postfix(GameObject go)
			{
				var o = go.AddOrGet<SolidDeliverySelection>();
				o.Options = [.. RefinementRecipeHelper.GetWoods().Select(sh => sh.CreateTag())];
				o.AnyTag = GameTags.BuildingWood;
			}
        }
	}
}
