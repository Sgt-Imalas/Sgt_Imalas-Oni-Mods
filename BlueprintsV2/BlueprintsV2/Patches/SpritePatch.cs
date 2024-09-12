
using HarmonyLib;
using UtilLibs;

namespace BlueprintsV2.Patches
{
	internal class SpritePatch
	{
		public static string createblueprint_button = "createblueprint_button";
		public static string createblueprint_visualizer = "createblueprint_visualizer";
		public static string snapshot_button = "snapshot_button";
		public static string snapshot_visualizer = "snapshot_visualizer";
		public static string useblueprint_button = "useblueprint_button";
		public static string useblueprint_visualizer = "useblueprint_visualizer";


		[HarmonyPatch(typeof(Assets), "OnPrefabInit")]
		public class Assets_OnPrefabInit_Patch
		{
			[HarmonyPriority(Priority.LowerThanNormal)]
			public static void Prefix(Assets __instance)
			{

				ModAssets.BLUEPRINTS_CREATE_ICON_SPRITE = InjectionMethods.AddSpriteToAssets(__instance, createblueprint_button);
				ModAssets.BLUEPRINTS_CREATE_VISUALIZER_SPRITE = InjectionMethods.AddSpriteToAssets(__instance, createblueprint_visualizer);

				ModAssets.BLUEPRINTS_SNAPSHOT_ICON_SPRITE = InjectionMethods.AddSpriteToAssets(__instance, snapshot_button);
				ModAssets.BLUEPRINTS_SNAPSHOT_VISUALIZER_SPRITE = InjectionMethods.AddSpriteToAssets(__instance, snapshot_visualizer);

				ModAssets.BLUEPRINTS_USE_ICON_SPRITE = InjectionMethods.AddSpriteToAssets(__instance, useblueprint_button);
				ModAssets.BLUEPRINTS_USE_VISUALIZER_SPRITE = InjectionMethods.AddSpriteToAssets(__instance, useblueprint_visualizer);
			}
		}
	}
}
