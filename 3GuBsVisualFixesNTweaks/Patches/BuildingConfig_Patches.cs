using _3GuBsVisualFixesNTweaks.Scripts;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace _3GuBsVisualFixesNTweaks.Patches
{
	class BuildingConfig_Patches
	{
		[HarmonyPatch]
		public static class AddTintableToBuildings
		{
			[HarmonyPrefix]
			public static void Prefix(GameObject go)
			{
				go.AddOrGet<ContentTintable>();
			}
			[HarmonyTargetMethods]
			internal static IEnumerable<MethodBase> TargetMethods()
			{
				const string name = nameof(IBuildingConfig.DoPostConfigureComplete);
				yield return typeof(LiquidConditionerConfig).GetMethod(name);
				yield return typeof(LiquidPumpConfig).GetMethod(name);
				yield return typeof(LiquidMiniPumpConfig).GetMethod(name);
			}
		}
		[HarmonyPatch]
		public static class ExcludeBuildingsFromFloorVis
		{
			[HarmonyPrefix]
			public static void Prefix(GameObject go)
			{
				go.AddTag(ModAssets.ModTags.PlacementVisualizerExcluded);
			}
			[HarmonyTargetMethods]
			internal static IEnumerable<MethodBase> TargetMethods()
			{
				const string name = nameof(IBuildingConfig.DoPostConfigureComplete);
				yield return typeof(SteamTurbineConfig2).GetMethod(name);
				yield return typeof(AutoMinerConfig).GetMethod(name);
				yield return typeof(VerticalWindTunnelConfig).GetMethod(name);

				yield return typeof(RocketInteriorGasInputConfig).GetMethod(name);
				yield return typeof(RocketInteriorGasOutputConfig).GetMethod(name);
				yield return typeof(RocketInteriorLiquidInputConfig).GetMethod(name);
				yield return typeof(RocketInteriorLiquidOutputConfig).GetMethod(name);
				yield return typeof(RocketInteriorSolidOutputConfig).GetMethod(name);
				yield return typeof(RocketInteriorSolidInputConfig).GetMethod(name);
				yield return typeof(RocketInteriorPowerPlugConfig).GetMethod(name);
			}
		}

		[HarmonyPatch(typeof(WallToiletConfig), nameof(WallToiletConfig.DoPostConfigureComplete))]
		public class WallToiletConfig_DoPostConfigureComplete_Patch
		{
			public static void Prefix(GameObject go)
			{
				go.AddTag(ModAssets.ModTags.PlacementVisualizerExcludedVertical);
			}
		}

		/// <summary>
		/// Register SOC
		/// </summary>
		[HarmonyPatch(typeof(ElectrobankChargerConfig), nameof(ElectrobankChargerConfig.DoPostConfigureComplete))]
		public class ElectrobankChargerConfig_DoPostConfigureComplete_Patch
		{
			public static void Postfix(GameObject go)
			{
				SymbolOverrideControllerUtil.AddToPrefab(go);
			}
		}

		[HarmonyPatch(typeof(WaterPurifierConfig), nameof(WaterPurifierConfig.DoPostConfigureComplete))]
		public class WaterPurifierConfig_DoPostConfigureComplete_Patch
		{
			public static void Postfix(GameObject go)
			{

				StateMachineController stateMachineController = go.AddOrGet<StateMachineController>();

				SgtLogger.l("removing PoweredActiveController from WaterPurifier");
				stateMachineController.cmpdef.defs.RemoveAll(def => def.GetStateMachineType() == typeof(PoweredActiveController));
			}
		}

		[HarmonyPatch(typeof(MethaneGeneratorConfig), nameof(MethaneGeneratorConfig.DoPostConfigureComplete))]
		public class MethaneGeneratorConfig_DoPostConfigureComplete_Patch
		{
			[HarmonyPriority(Priority.Low)]
			public static void Postfix(GameObject go)
			{
				//go.GetComponent<EnergyGenerator>().formula.inputs = [new EnergyGenerator.InputItem(GameTags.CombustibleGas, 0.09f, 0.9f)];
				ModAssets.AddGeneratorTint(go);
			}
		}

		[HarmonyPatch(typeof(LaunchPadConfig), nameof(LaunchPadConfig.CreateBuildingDef))]
		public class LaunchPadConfig_CreateBuildingDef_Patch
		{
			static bool Prepare() => Config.Instance.RocketPlatformRenderChange;

			public static void Postfix(BuildingDef __result)
			{
				if (!Config.Instance.RocketPlatformRenderChange)
					return;
				var anim = Assets.GetAnim("rocket_launchpad_fg_kanim");
				if (anim == null)
					return;

				__result.AnimFiles = [anim];
				SoundUtils.CopySoundsToAnim("rocket_launchpad_fg_kanim", "rocket_launchpad_kanim"); //anim has no sound, cloning them from original				
			}
		}

		[HarmonyPatch(typeof(PolymerizerConfig), nameof(PolymerizerConfig.DoPostConfigureComplete))]
		public class PolymerizerConfig_DoPostConfigureComplete_Patch
		{
			public static void Postfix(GameObject go)
			{
				ModAssets.AddElementConverterTint(go).TintPolymerizer = true;
			}
		}
		[HarmonyPatch(typeof(PetroleumGeneratorConfig), nameof(PetroleumGeneratorConfig.DoPostConfigureComplete))]
		public class PetroleumGeneratorConfig_DoPostConfigureComplete_Patch
		{
			public static void Postfix(GameObject go)
			{
				ModAssets.AddGeneratorTint(go);
			}
		}

		/// <summary>
		/// inject an extra storage that stores refinery products while the conveyor anim plays
		/// </summary>
		[HarmonyPatch(typeof(MetalRefineryConfig), nameof(MetalRefineryConfig.DoPostConfigureComplete))]
		public class MetalRefineryConfig_DoPostConfigureComplete_Patch
		{
			public static void Postfix(GameObject go)
			{
				go.AddOrGet<MetalRefineryTint>().ProductStorage = go.AddComponent<Storage>();
			}
		}

		[HarmonyPatch(typeof(LiquidHeaterConfig), nameof(LiquidHeaterConfig.DoPostConfigureComplete))]
		public class LiquidHeaterConfig_DoPostConfigureComplete_Patch
		{
			public static void Postfix(GameObject go)
			{
				go.AddOrGet<TintableByExterior>();
			}
		}

		[HarmonyPatch(typeof(CampfireConfig), nameof(CampfireConfig.ConfigureBuildingTemplate))]
		public class CampfireConfig_ConfigureBuildingTemplate_Patch
		{
			public static void Postfix(GameObject go)
			{
				go.AddOrGet<StorageMeter>();
			}
		}

		[HarmonyPatch(typeof(OxysconceConfig), nameof(OxysconceConfig.ConfigureBuildingTemplate))]
		public class OxysconceConfig_ConfigureBuildingTemplate_Patch
		{
			public static void Postfix(GameObject go)
			{
				go.AddOrGet<SconceAnimator>();
			}
		}
	}
}
