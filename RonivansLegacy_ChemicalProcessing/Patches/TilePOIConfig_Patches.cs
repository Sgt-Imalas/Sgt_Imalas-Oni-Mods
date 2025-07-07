using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class TilePOIConfig_Patches
    {

        [HarmonyPatch(typeof(TilePOIConfig), nameof(TilePOIConfig.CreateBuildingDef))]
        public class TilePOIConfig_CreateBuildingDef_Patch
        {
            public static void Postfix(BuildingDef __result)
            {
                __result.ShowInBuildMenu = true;
				__result.DebugOnly = false;
				__result.Deprecated = false;
				__result.BaseDecor = 15;
				__result.BaseDecorRadius = 3;
				__result.MaterialCategory = MATERIALS.PRECIOUS_ROCKS;
			}
		}
		[HarmonyPatch(typeof(TilePOIConfig), nameof(TilePOIConfig.DoPostConfigureComplete))]
		public class TilePOIConfig_DoPostConfigureComplete_Patch
		{
			public static void Postfix(GameObject go)
			{
				KPrefabID component = go.GetComponent<KPrefabID>();
				component.AddTag(GameTags.FloorTiles);
			}
		}

		[HarmonyPatch(typeof(TilePOIConfig), nameof(TilePOIConfig.ConfigureBuildingTemplate))]	
		public class TilePOIConfig_ConfigureBuildingTemplate_Patch
		{
			public static void Postfix(GameObject go)
			{
				go.AddOrGet<SimCellOccupier>().movementSpeedMultiplier = 1.5f; //== DUPLICANTSTATS.MOVEMENT_MODIFIERS.BONUS_3;
			}
		}
	}
}
