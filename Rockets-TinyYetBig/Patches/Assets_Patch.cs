using HarmonyLib;
using Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
	internal class Assets_Patch
	{
		[HarmonyPatch(typeof(Assets), nameof(Assets.OnPrefabInit))]
		public class Assets_OnPrefabInit_Patch
		{
			public static void Prefix(Assets __instance)
			{
				InjectionMethods.AddSpriteToAssets(__instance, "rtb_dlc_banner");
				InjectionMethods.AddSpriteToAssets(__instance, "research_type_deep_space_icon");
				InjectionMethods.AddSpriteToAssets(__instance, "research_type_deep_space_icon_unlock");
				InjectionMethods.AddSpriteToAssets(__instance, "RTB_CrashedUFOStoryTrait_icon");
				InjectionMethods.AddSpriteToAssets(__instance, "RTB_CrashedUFOStoryTrait_image");
			}
		}

		[HarmonyPatch(typeof(Assets), nameof(Assets.AddBuildingDef))]
		public class Assets_AddBuildingDef_Patch
		{
			public static void Prefix(BuildingDef def)
			{
				if (def.BuildingComplete.TryGetComponent<RocketEngineCluster>(out _))
				{
					def.BuildingComplete.AddOrGet<RocketEngineIndicator>();
					def.BuildingUnderConstruction.AddOrGet<RocketEngineIndicator>();
				}
			}
		}
	}
}
