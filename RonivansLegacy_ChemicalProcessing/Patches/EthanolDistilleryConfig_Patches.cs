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
	internal class EthanolDistilleryConfig_Patches
	{

		[HarmonyPatch(typeof(EthanolDistilleryConfig), nameof(EthanolDistilleryConfig.ConfigureBuildingTemplate))]
		public class EthanolDistilleryConfig_ConfigureBuildingTemplate_Patch
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
