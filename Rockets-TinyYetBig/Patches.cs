using HarmonyLib;
using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.Buildings.CargoBays;
using Rockets_TinyYetBig.Content.Scripts.UI.Sidescreens;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig
{
    class OldPatches
	{
		/// <summary>
		/// Extend rocket backwall to all wall buildings in rocket (glass, ports)
		/// </summary>
		[HarmonyPatch(typeof(WorldContainer))]
		[HarmonyPatch(nameof(WorldContainer.PlaceInteriorTemplate))]
		public static class RocketBackgroundGeneration_Patch
		{
			public static bool BackgroundExtension(string id, string rwtName)
			{
				return (id == rwtName
					|| id == "RocketEnvelopeWindowTile"
					|| id == "RocketInteriorLiquidOutputPort"
					|| id == "RocketInteriorLiquidInputPort"
					);
			}

			private static readonly MethodInfo BackgroundHelper = AccessTools.Method(
			   typeof(RocketBackgroundGeneration_Patch),
			   nameof(BackgroundExtension)
			);


			private static readonly MethodInfo SuitableMethodInfo = AccessTools.Method(
					typeof(System.String),
				   "op_Equality"
			   );

			static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
			{
				var code = instructions.ToList();
				var insertionIndex = code.FindIndex(ci => ci.operand is MethodInfo f && f == SuitableMethodInfo);


				if (insertionIndex != -1)
				{
					code[insertionIndex] = new CodeInstruction(OpCodes.Call, BackgroundHelper);
				}
				else
					Debug.LogError("TRANSPILER BROKE: RocketBackgroundGeneration");

				return code;
			}
		}


		///// <summary>
		///// Fridge Access Hatch accessible food should count towards tracker
		///// </summary>
		//[HarmonyPatch(typeof(RationTracker))]
		//[HarmonyPatch(nameof(RationTracker.CountRations))]
		//public static class CalorieCounterForRocketModules
		//{
		//    public static void Postfix(WorldInventory inventory,ref float __result)
		//    {
		//        var modules = ModAssets.FridgeModuleGrabbers.GetWorldItems(inventory.worldId);
		//        if(modules.Count> 0)
		//        {
		//            __result += modules.First().TotalKCAL;
		//        }
		//    }
		//}

		//[HarmonyPatch(typeof(HarvestablePOIConfig))]
		//[HarmonyPatch("GenerateConfigs")]
		//[HarmonyPatch(new Type[] { typeof(List<HarvestablePOIConfig.HarvestablePOIParams>) })]
		//public static class FixForMissingCarbonFieldAnim
		//{
		//    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
		//    {
		//        var code = instructions.ToList();
		//        var insertionIndex = code.FindIndex(ci => ci.opcode == OpCodes.Ldstr && (string)ci.operand == "cloud");

		//        if (insertionIndex != -1)
		//        {
		//            code[insertionIndex].operand = "carbon_asteroid_field";
		//        }
		//        return code;
		//    }
		//}


		/// <summary>
		/// Reduces width of rocket modules over 5 width (mainly engines) to 5 width if the config option for it is enabled
		/// </summary>
		[HarmonyPatch(typeof(BuildingTemplates), nameof(BuildingTemplates.CreateRocketBuildingDef))]
		public static class RocketEngineWidthIsReduced
		{
			public static void Postfix(BuildingDef def)
			{

				if (Config.Instance.SlimLargeEngines && def.WidthInCells > 5)
				{
					SgtLogger.l("squishing module " + def.name);
					def.WidthInCells = 5;
					def.GenerateOffsets();
				}
			}
		}


		/// <summary>
		/// Adjusts Critter storage to go with units instead of kg (would be 100kg/critter)
		/// </summary>
		[HarmonyPatch(typeof(Storage), "MassStored")]
		public static class KGToUnitsPatch
		{
			public static bool Prefix(Storage __instance, ref float __result)
			{
				if (__instance.storageFilters != null && __instance.storageFilters.Count > 0)
				{

					if (__instance.storageFilters.All(CritterContainmentModuleConfigOLD.GetCritterTags().Contains))
					{
						__result = __instance.UnitsStored();
						return false;
					}
				}
				return true;
			}
		}


		/// <summary>
		/// Adds Scanner Module Telescope function
		/// </summary>
		[HarmonyPatch(typeof(ScannerModuleConfig), "DoPostConfigureComplete")]
		public static class BuffScannerModule
		{
			public static void Postfix(GameObject go)
			{
				go.AddOrGetDef<ExplorerModuleTelescope.Def>();
			}
		}
		/// <summary>
		/// TelescopeTarget add fallback selfdestroy
		/// </summary>
		[HarmonyPatch(typeof(ClusterGridEntity), "OnSpawn")]
		public static class DestroyFinishedTelescopeTargets
		{
			public static void Postfix(ClusterGridEntity __instance)
			{
				if (__instance is TelescopeTarget)
				{
					__instance.gameObject.AddOrGet<TelescopeSelfDestruct>();
				}
			}
		}

		/// <summary>
		/// Add Custom Sidescreen for laser drillcone
		/// </summary>
		[HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
		public static class CustomSideScreenPatch_LaserDrillcone
		{
			public static void Postfix(List<DetailsScreen.SideScreenRef> ___sideScreens)
			{
				UIUtils.AddClonedSideScreen<ResourceHarvestModuleHEPSideScreen>("HarvestModuleHEPSideScreen", "HarvestModuleSideScreen", typeof(HarvestModuleSideScreen));
			}
		}

		/// <summary>
		/// add direction selection sidescreen to rad battery
		/// </summary>
		[HarmonyPatch(typeof(HighEnergyParticleDirectionSideScreen))]
		[HarmonyPatch(nameof(HighEnergyParticleDirectionSideScreen.IsValidForTarget))]
		public static class AddTargetValidityForRadBattery
		{
			public static bool Prefix(GameObject target, ref bool __result)
			{
				if (target.TryGetComponent<RadiationBatteryOutputHandler>(out _))
				{
					__result = true;
					return false;
				}
				return true;
			}
		}



		/// <summary>
		/// More than 16 Rockets allowed simultaniously
		/// </summary>
		[HarmonyPatch(typeof(ClusterManager))]
		[HarmonyPatch("OnPrefabInit")]
		public static class RocketCount_Patch
		{
			public static void Postfix()
			{
				if (Config.Instance.CompressInteriors)
					ClusterManager.MAX_ROCKET_INTERIOR_COUNT = 100;
			}
		}


		/// <summary>
		/// Add launch_pst anim to normal modules
		/// </summary>
		[HarmonyPatch(typeof(RocketModuleCluster))]
		[HarmonyPatch("UpdateAnimations")]
		public static class AddPstLaunchAnim
		{
			public static bool Prefix(RocketModuleCluster __instance)
			{
				if (__instance.TryGetComponent<ExtendedClusterModuleAnimator>(out _))
				{
					return false;
				}
				return true;

			}
		}



		/// <summary>
		/// Patch to decrease interior size from 32x32 to dynamic value per habitat template
		/// </summary>
		[HarmonyPatch(typeof(ClusterManager))]
		[HarmonyPatch("CreateRocketInteriorWorld")]
		public class ClusterManager_CreateRocketInteriorWorld_Patch
		{

			public static Vector2I ConditionForSize(Vector2I original, string templateString)
			{
				if (Config.Instance.CompressInteriors)
				{
					switch (templateString)
					{
						case "interiors/habitat_medium_compressed":
							//case "interiors/habitat_medium_radiator":
							original = new Vector2I(18, 15);
							break;
						case "interiors/habitat_small_compressed":
							original = new Vector2I(14, 13);
							break;
						case "interiors/habitat_small_expanded":
							original = new Vector2I(14, 15);
							break;
						case "interiors/habitat_medium_expanded":
							original = new Vector2I(20, 19);
							break;
						case "interiors/habitat_medium_stargazer":
							original = new Vector2I(18, 13);
							break;
						case "interiors/habitat_nosecone_plated":
							original = new Vector2I(18, 18);
							break;
					}
				}
				return original;
			}

			private static readonly MethodInfo InteriorSizeHelper = AccessTools.Method(
			   typeof(ClusterManager_CreateRocketInteriorWorld_Patch),
			   nameof(ConditionForSize)
			);


			private static readonly FieldInfo SizeFieldInfo = AccessTools.Field(
				typeof(TUNING.ROCKETRY),
				"ROCKET_INTERIOR_SIZE"
			);

			static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
			{
				var code = instructions.ToList();
				var insertionIndex = code.FindIndex(ci => ci.operand is FieldInfo f && f == SizeFieldInfo);

				if (insertionIndex != -1)
				{
					code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Ldarg_2));
					code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, InteriorSizeHelper));
				}

				return code;
			}
		}
	}
}
