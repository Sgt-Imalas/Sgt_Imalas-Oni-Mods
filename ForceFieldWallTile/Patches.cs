using Database;
using ForceFieldWallTile.Content.Defs.Buildings;
using ForceFieldWallTile.Content.ModDb;
using ForceFieldWallTile.Content.Scripts;
using HarmonyLib;
using Klei.AI;
using PeterHan.PLib.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static ForceFieldWallTile.ModAssets;

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

		[HarmonyPatch(typeof(ModifierSet), nameof(ModifierSet.Initialize))]
		public class ModifierSet_Initialize_Patch
		{
			public static void Postfix(ModifierSet __instance)
			{
				ModEffects.Register(__instance);
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
				ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Utilities, ForceFieldTileConfig.ID);
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
