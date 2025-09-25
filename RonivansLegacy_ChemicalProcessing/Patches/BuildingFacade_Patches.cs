using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	class BuildingFacade_Patches
	{

		[HarmonyPatch(typeof(BuildingFacade), nameof(BuildingFacade.ChangeBuilding))]
		public class BuildingFacade_ChangeBuilding_Patch
		{
			public static void Postfix(BuildingFacade __instance)
			{
				__instance.gameObject.Trigger(ModAssets.OnBuildingFacadeChanged);
			}
		}

		[HarmonyPatch(typeof(BuildingFacade), nameof(BuildingFacade.OnSpawn))]
		public class BuildingFacade_OnSpawn_Patch
		{
			public static void Prefix(BuildingFacade __instance)
			{
				if (__instance.IsOriginal)
					return;

				if (__instance.HasTag(GameTags.FloorTiles))
				{
					SgtLogger.l(__instance.GetProperName() + " is a tile, but tried to have the skin: " + __instance.currentFacade);
					__instance.currentFacade = null;
				}
			}
		}
	}
}
