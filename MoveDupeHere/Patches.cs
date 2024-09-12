using HarmonyLib;
using LogicSatellites.Buildings;
using UnityEngine;
using UtilLibs;

namespace MoveDupeHere
{
	internal class Patches
	{
		/// <summary>
		/// add buildings to plan screen
		/// </summary>
		[HarmonyPatch(typeof(GeneratedBuildings))]
		[HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
		public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
		{

			public static void Prefix()
			{
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Automation, GoHereTileConfig.ID, FloorSwitchConfig.ID);
			}
		}
		[HarmonyPatch(typeof(MorbRoverConfig), nameof(MorbRoverConfig.CreatePrefab))]
		public class MorbRoverConfig_CreatePrefab_Patch
		{
			public static void Postfix(GameObject __result)
			{
				__result.AddOrGet<UserNameable>();
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

		[HarmonyPatch(typeof(Db))]
		[HarmonyPatch("Initialize")]
		public class Db_Initialize_Patch
		{
			public static void Postfix(Db __instance)
			{
				//add buildings to technology tree
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Computers.Computing, GoHereTileConfig.ID);

			}
		}
	}
}
