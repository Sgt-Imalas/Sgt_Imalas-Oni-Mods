using HarmonyLib;
using System;
using UnityEngine;

namespace OniRetroEdition.ModPatches
{
	internal class PipeConnectionPatches
	{
		//[HarmonyPatch(typeof(BuildingCellVisualizerResources), nameof(BuildingCellVisualizerResources.Initialize))]
		//public class BuildingCellVisualizerResources_Init_Patch
		//{
		//    public static void Postfix(BuildingCellVisualizerResources __instance)
		//    {

		//        Color bluu = UIUtils.rgb(90, 168, 211);

		//        __instance.gasIOColours
		//            = new BuildingCellVisualizerResources.IOColours()
		//            {
		//                input = new BuildingCellVisualizerResources.ConnectedDisconnectedColours()
		//                {
		//                    connected = __instance.gasIOColours.input.connected,
		//                    disconnected = UIUtils.Darken(__instance.gasIOColours.input.connected, 50)
		//                },
		//                output = new BuildingCellVisualizerResources.ConnectedDisconnectedColours()
		//                {
		//                    connected = __instance.gasIOColours.output.connected,
		//                    disconnected = UIUtils.Darken(__instance.gasIOColours.output.connected, 50)
		//                }
		//            };
		//        __instance.liquidIOColours
		//            = new BuildingCellVisualizerResources.IOColours()
		//            {
		//                input = new BuildingCellVisualizerResources.ConnectedDisconnectedColours()
		//                {
		//                    connected = __instance.liquidIOColours.input.connected,
		//                    disconnected = UIUtils.Darken(__instance.liquidIOColours.input.connected, 50)
		//                },
		//                output = new BuildingCellVisualizerResources.ConnectedDisconnectedColours()
		//                {
		//                    connected = __instance.liquidIOColours.output.connected,
		//                    disconnected = UIUtils.Darken(__instance.liquidIOColours.output.connected, 50)
		//                }
		//            };

		//    }
		//}


		static HashedString IconMode;
		[HarmonyPatch(typeof(EntityCellVisualizer), nameof(EntityCellVisualizer.DrawIcons))]
		public class Assets_OnPrefabInit_Patch
		{
			public static void Prefix(HashedString mode)
			{
				IconMode = mode;
			}
		}
		[HarmonyPatch(typeof(EntityCellVisualizer),
			nameof(EntityCellVisualizer.DrawUtilityIcon),
			new Type[] { typeof(int), typeof(Sprite), typeof(GameObject), typeof(Color), typeof(float), typeof(bool) },
			new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal })]
		public class BuildingCellVisualizerResources_Init
		{
			[HarmonyPrepare]
			static bool Prep() => Config.Instance.oldPipeIcons;
			public static void Prefix(EntityCellVisualizer __instance,
				int cell,
				ref Sprite icon_img)
			{

				if (__instance is not BuildingCellVisualizer visualizer)
					return;


				if (IconMode != OverlayModes.GasConduits.ID && IconMode != OverlayModes.LiquidConduits.ID)
					return;

				if (IconMode == OverlayModes.GasConduits.ID)
				{
					bool connected = Grid.Objects[cell, (int)ObjectLayer.GasConduit] != null;

					icon_img = connected ? Assets.GetSprite(SpritePatch.Gas_Input_Connected) : Assets.GetSprite(SpritePatch.Gas_Input_Disconnected);
					//if (icon_img == __instance.resources.gasInputIcon)
					//{
					//    bool connected = Grid.Objects[cell, (int)ObjectLayer.LiquidConduit]!=null;

					//    icon_img = connected ? Assets.GetSprite(SpritePatch.Gas_Input_Connected) : Assets.GetSprite(SpritePatch.Gas_Input_Disconnected);
					//}
					//else if (icon_img == __instance.resources.gasOutputIcon)
					//{
					//    bool connected = Grid.Objects[cell, (int)ObjectLayer.LiquidConduit] != null;

					//    icon_img = connected ? Assets.GetSprite(SpritePatch.Gas_Input_Connected) : Assets.GetSprite(SpritePatch.Gas_Input_Disconnected);
					//}
				}
				if (IconMode == OverlayModes.LiquidConduits.ID)
				{
					bool connected = Grid.Objects[cell, (int)ObjectLayer.LiquidConduit] != null;

					icon_img = connected ? Assets.GetSprite(SpritePatch.Liquid_Input_Connected) : Assets.GetSprite(SpritePatch.Liquid_Input_Disconnected);
					//if (icon_img == __instance.resources.liquidInputIcon)
					//{
					//    if (tint.Equals(__instance.resources.liquidIOColours.input.connected))
					//    {
					//        icon_img = Assets.GetSprite(SpritePatch.Liquid_Input_Connected);

					//    }
					//    else
					//    {
					//        icon_img = Assets.GetSprite(SpritePatch.Liquid_Input_Disconnected);
					//    }
					//}
					//else if (icon_img == __instance.resources.liquidOutputIcon)
					//{
					//    if (tint.Equals(__instance.resources.liquidIOColours.output.connected))
					//    {
					//        icon_img = Assets.GetSprite(SpritePatch.Liquid_Input_Connected);
					//    }
					//    else
					//    {
					//        icon_img = Assets.GetSprite(SpritePatch.Liquid_Input_Disconnected);
					//    }
					//}
				}
			}
		}

	}
}
