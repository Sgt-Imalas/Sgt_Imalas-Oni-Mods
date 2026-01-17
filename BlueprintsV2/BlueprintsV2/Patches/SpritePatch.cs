
using HarmonyLib;
using UtilLibs;

namespace BlueprintsV2.Patches
{
	internal class SpritePatch
	{
		public static string createblueprint_button = "createblueprint_button";
		public static string apply_settings_sprite = "apply_blueprint_settings";
		public static string createblueprint_visualizer = "createblueprint_visualizer";
		public static string snapshot_button = "snapshot_button";
		public static string snapshot_visualizer = "snapshot_visualizer";
		public static string useblueprint_button = "useblueprint_button";
		public static string useblueprint_visualizer = "useblueprint_visualizer";
		public static string gas_placer_icon = "BPV2_GasPlacer";
		public static string liquid_placer_icon = "BPV2_LiquidPlacer";
		public static string solid_placer_icon = "BPV2_SolidPlacer";


		[HarmonyPatch(typeof(Assets), "OnPrefabInit")]
		public class Assets_OnPrefabInit_Patch
		{
			[HarmonyPriority(Priority.LowerThanNormal)]
			public static void Prefix(Assets __instance)
			{
				ModAssets.BLUEPRINTS_APPLY_SETTINGS_SPRITE = InjectionMethods.AddSpriteToAssets(__instance, apply_settings_sprite);
				ModAssets.BLUEPRINTS_CREATE_ICON_SPRITE = InjectionMethods.AddSpriteToAssets(__instance, createblueprint_button);
				ModAssets.BLUEPRINTS_CREATE_VISUALIZER_SPRITE = InjectionMethods.AddSpriteToAssets(__instance, createblueprint_visualizer);

				ModAssets.BLUEPRINTS_SNAPSHOT_ICON_SPRITE = InjectionMethods.AddSpriteToAssets(__instance, snapshot_button);
				ModAssets.BLUEPRINTS_SNAPSHOT_VISUALIZER_SPRITE = InjectionMethods.AddSpriteToAssets(__instance, snapshot_visualizer);

				ModAssets.BLUEPRINTS_USE_ICON_SPRITE = InjectionMethods.AddSpriteToAssets(__instance, useblueprint_button);
				ModAssets.BLUEPRINTS_USE_VISUALIZER_SPRITE = InjectionMethods.AddSpriteToAssets(__instance, useblueprint_visualizer);


				ModAssets.Gas_Placer_Sprite = InjectionMethods.AddSpriteToAssets(__instance, gas_placer_icon);
				ModAssets.Liquid_Placer_Sprite = InjectionMethods.AddSpriteToAssets(__instance, liquid_placer_icon);
				ModAssets.Solid_Placer_Sprite = InjectionMethods.AddSpriteToAssets(__instance, solid_placer_icon);


				ModAssets.PlanningToolPreview_Square = InjectionMethods.AddSpriteToAssets(__instance, "BPV2_PlantoolPlacer_Square");
				ModAssets.PlanningToolPreview_Circle = InjectionMethods.AddSpriteToAssets(__instance, "BPV2_PlantoolPlacer_Circle");
				ModAssets.PlanningToolPreview_Diamond= InjectionMethods.AddSpriteToAssets(__instance, "BPV2_PlantoolPlacer_Diamond");

			}
		}
	}
}
