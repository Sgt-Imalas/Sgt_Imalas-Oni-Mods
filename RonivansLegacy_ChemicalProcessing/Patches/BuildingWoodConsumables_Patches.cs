using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class BuildingWoodConsumables_Patches
	{
		public class BuildingConfig_ConfigureBuildingTemplate_Patch
		{
			[HarmonyTargetMethods]
			internal static IEnumerable<MethodBase> TargetMethods()
			{
				yield return typeof(EthanolDistilleryConfig).GetMethod(nameof(EthanolDistilleryConfig.ConfigureBuildingTemplate));
				yield return typeof(CampfireConfig).GetMethod(nameof(CampfireConfig.ConfigureBuildingTemplate));
				yield return typeof(WoodGasGeneratorConfig).GetMethod(nameof(WoodGasGeneratorConfig.DoPostConfigureComplete));
			}
			[HarmonyPostfix]
			public static void Postfix(GameObject go)
			{
				var o = go.AddOrGet<SolidDeliverySelection>();
				o.Options = [.. RefinementRecipeHelper.GetWoods().Select(sh => sh.CreateTag())];
				o.AnyTag = GameTags.BuildingWood;
			}
		}
	}
}
