using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace HoverPipetteTool.Patches
{
	internal class ToolMenu_Patches
	{
		[HarmonyPatch(typeof(ToolMenu), nameof(ToolMenu.OnKeyUp))]
		public class ToolMenu_OnKeyUp_Patch
		{
			public static void Postfix(ToolMenu __instance, KButtonEvent e)
			{
				if (e.Consumed)
					return;

				if (DetailsScreen.Instance?.isEditing ?? false)
					return;

				if (e.IsAction(ModAssets.PickHoveredBuilding.GetKAction()))
				{
					ModAssets.SelectNextBuilding();
				}
			}
		}

	}
}
