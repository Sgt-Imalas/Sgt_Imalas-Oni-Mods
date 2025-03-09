using Database;
using HarmonyLib;
using System.Collections.Generic;
using UtilLibs;
using static InventoryOrganization;

namespace BathTub
{
	internal class SkinPatches
	{
		static List<string> BathtubSkins = new List<string>(16);
		public class AddNewSkins
		{
			// manually patching, because referencing BuildingFacades class will load strings too early
			public static void Patch(Harmony harmony)
			{
				SgtLogger.l("init Patch 2");
				var targetType = AccessTools.TypeByName("Database.BuildingFacades");
				var target = AccessTools.Constructor(targetType, new[] { typeof(ResourceSet) });
				var postfix = AccessTools.Method(typeof(InitFacadePatchForRockets), "PostfixPatch");
				harmony.Patch(target, postfix: new HarmonyMethod(postfix));
			}

			public class InitFacadePatchForRockets
			{
				public static void PostfixPatch(object __instance)
				{
					SgtLogger.l("Start Skin Patch");
					var resource = (ResourceSet<BuildingFacadeResource>)__instance;
					AddFacade(resource, "HandyRetroBathtub", STRINGS.BUILDINGS.PREFABS.SGTIMALAS_BATHTUB.FACADES.HANDY_RETRO_TUB.NAME, STRINGS.BUILDINGS.PREFABS.SGTIMALAS_BATHTUB.FACADES.HANDY_RETRO_TUB.DESC, PermitRarity.Universal, BathTubConfig.ID, "bathtub_handy_kanim");
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
					set.resources.Add(new BuildingFacadeResource(id, name, description, rarity, prefabId, animFile, workables, null,null));
					BathtubSkins.Add(id);
				}
			}
		}

		[HarmonyPatch(typeof(Db), "Initialize")]
		public static class Db_Initialize_Patch_Skins
		{
			public static void Prefix()
			{
				AddNewSkins.Patch(Mod.haromy);
				SublevelCategoryPatch.Patch(Mod.haromy);
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
				SupplyClosetUtils.AddItemsToSubcategory(PermitSubcategories.BUILDINGS_WASHROOM, BathtubSkins.ToArray());
			}
		}

		/// <summary>
		/// Init. auto translation
		/// </summary>
		[HarmonyPatch(typeof(Localization), "Initialize")]
		public static class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				LocalisationUtil.Translate(typeof(STRINGS), true);
			}
		}
	}
}
