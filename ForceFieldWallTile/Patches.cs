using ForceFieldWallTile.Content.Defs.Buildings;
using ForceFieldWallTile.Content.ModDb;
using ForceFieldWallTile.Content.Scripts;
using HarmonyLib;
using UtilLibs;

namespace ForceFieldWallTile
{
	internal class Patches
	{
		[HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
		public class Db_Initialize_Patch
		{
			public static void Postfix(Db __instance)
			{
				ModStatusItems.CreateStatusItems();
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Liquids.Projectiles, ForceFieldTileConfig.ID);
				ModEffects.Register();
			}
		}

		[HarmonyPatch(typeof(Navigator), nameof(Navigator.OnSpawn))]
		public class Navigator_OnSpawn_Patch
		{
			public static void Postfix(Navigator __instance)
			{
				__instance.gameObject.AddOrGet<NavigatorForceFieldInteractions>();
			}
		}

		[HarmonyPatch(typeof(Comet), nameof(Comet.OnSpawn))]
		public class Comet_OnSpawn_Patch
		{
			public static void Postfix(Comet __instance)
			{
				__instance.gameObject.AddOrGet<CometShieldImpactor>();
			}
		}

		[HarmonyPatch(typeof(Game), nameof(Game.OnLoadLevel))]
		public class Game_OnLoadLevel_Patch
		{
			public static void Postfix() => ForceFieldTile.ClearAll();
		}

		/// <summary>
		/// add buildings to plan screen
		/// </summary>
		[HarmonyPatch(typeof(GeneratedBuildings))]
		[HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
		public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
		{
			public static void Prefix()
			{
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Utilities, ForceFieldTileConfig.ID, ThermalBlockConfig.ID);
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

		[HarmonyPatch(typeof(UnstableGroundManager), nameof(UnstableGroundManager.Update))]
		public class UnstableGroundManager_Update_Patch
		{
			public static void Postfix(UnstableGroundManager __instance)
			{
				if (App.isLoading)
					return;
				var fallingSnads = __instance.fallingObjects;
				for(int i = fallingSnads.Count - 1; i >= 0; i--)
				{
					var snad = fallingSnads[i];
					int cell = Grid.PosToCell(snad);
					if(ForceFieldTile.ForceFieldAt(cell, out var ff) && ff.ShieldActive && snad.TryGetComponent<UnstableGround>(out var snadInfo))
					{
						ff.HandleFallingSnad(snadInfo);
						fallingSnads.RemoveAt(i);
						__instance.ReleaseGO(snad);
					}
				}
			}
		}
	}
}
