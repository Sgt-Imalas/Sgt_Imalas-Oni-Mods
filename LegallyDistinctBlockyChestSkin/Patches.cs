using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static InventoryOrganization;

namespace LegallyDistinctBlockyChestSkin
{
	internal class Patches
	{
		public const string ChestSkin = "LDBCS_BlockyChest";
		public class AddNewSkins
		{
			// manually patching, because referencing BuildingFacades class will load strings too early
			public static void Patch(Harmony harmony)
			{
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
					AddFacade(resource, ChestSkin, STRINGS.BUILDINGS.PREFABS.LDBCS_BLOCKYCHEST.NAME, STRINGS.BUILDINGS.PREFABS.LDBCS_BLOCKYCHEST.DESC, PermitRarity.Universal, StorageLockerConfig.ID, "mc_chest_kanim");
					SoundUtils.CopySoundsToAnim("mc_chest_kanim", "storagelocker_kanim");
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
					set.resources.Add(new BuildingFacadeResource(id, name, description, rarity, prefabId, animFile, workables, null, null));
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
				SupplyClosetUtils.AddItemsToSubcategory(PermitSubcategories.BUILDINGS_STORAGE, [ChestSkin]);
			}
		}
	}
}
