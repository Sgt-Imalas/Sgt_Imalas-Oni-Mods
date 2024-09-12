using Database;
using HarmonyLib;
using System.Collections.Generic;
using UtilLibs;
using static InventoryOrganization;

namespace SkinMeterTest
{
	internal class Patches
	{
		public static readonly string[] ReservoirSkins =
		{
			"ReservoirRetro_1",
			"ReservoirRetro_2",

		};
		public class AddNewSkins
		{
			// manually patching, because referencing BuildingFacades class will load strings too early
			public static void Patch(Harmony harmony)
			{
				SgtLogger.l("init Patch 2");
				var targetType = AccessTools.TypeByName("Database.BuildingFacades");
				var target = AccessTools.Constructor(targetType, new[] { typeof(ResourceSet) });
				var postfix = AccessTools.Method(typeof(InitFacadePatchForDrywalls), "PostfixPatch");


				harmony.Patch(target, postfix: new HarmonyMethod(postfix));
			}

			public class InitFacadePatchForDrywalls
			{
				public static void PostfixPatch(object __instance)
				{
					SgtLogger.l("Start Skin Patch");
					var resource = (ResourceSet<BuildingFacadeResource>)__instance;
					AddFacade(resource, ReservoirSkins[0], "Retro Reservoir A", "A blast from the past!", PermitRarity.Universal, LiquidReservoirConfig.ID, "old_liquidreservoir_kanim");
					AddFacade(resource, ReservoirSkins[1], "Retro Reservoir B", "A blast from the past!", PermitRarity.Universal, LiquidReservoirConfig.ID, "old_liquidreservoir2_kanim");

					SgtLogger.l("Patch Executed");
				}

				public static void AddFacade(
					ResourceSet<BuildingFacadeResource> set,
					string id,
					LocString name,
					LocString description,
					PermitRarity rarity,
					string prefabId,
					string animFile,
					Dictionary<string, string> workables = null)
				{
					set.resources.Add(new BuildingFacadeResource(id, name, description, rarity, prefabId, animFile, DlcManager.AVAILABLE_ALL_VERSIONS, workables));
				}
			}
		}

		[HarmonyPatch(typeof(Db), "Initialize")]
		public static class Assets_OnPrefabInit_Patch
		{
			public static void Prefix()
			{
				AddNewSkins.Patch(Mod.HarmonyInstance);
				SublevelCategoryPatch.Patch(Mod.HarmonyInstance);
			}
		}

		// [HarmonyPatch(typeof(InventoryOrganization), "GenerateSubcategories")]
		public static class SublevelCategoryPatch
		{
			public static void Patch(Harmony harmony)
			{
				SgtLogger.l("init category");
				var targetType = AccessTools.TypeByName("InventoryOrganization");
				var target = AccessTools.Method(targetType, "GenerateSubcategories");
				var postfix = AccessTools.Method(typeof(SublevelCategoryPatch), "PostfixMethod");


				harmony.Patch(target, postfix: new HarmonyMethod(postfix));
			}
			public static void PostfixMethod()
			{
				SupplyClosetUtils.AddItemsToSubcategory(PermitSubcategories.BUILDINGS_STORAGE, ReservoirSkins);
			}
		}
	}
}
