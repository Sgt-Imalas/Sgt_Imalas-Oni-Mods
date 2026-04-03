using HarmonyLib;
using MassMoveTo.Tools.SweepByType;
using System;
using System.Collections.Generic;
using System.Text;

namespace MassMoveTo.Patches
{
	internal class ToolMenu_Patches
	{
		/// <summary>
		/// add mass move tool to toolbox
		/// </summary>
		[HarmonyPatch(typeof(ToolMenu), nameof(ToolMenu.CreateBasicTools))]
        public class ToolMenu_CreateBasicTools_Patch
        {
			public static void Prefix(ToolMenu __instance)
			{
				__instance.basicTools.Add(ToolMenu.CreateToolCollection(
						STRINGS.UI.TOOLS.MOVETOSELECTTOOL.NAME,
						ModAssets.MassMoveToolIcon.name,
						ModAssets.Actions.MassMoveTool_Open.GetKAction(),
						typeof(FilteredMoveSelectTool).Name,
						string.Format(STRINGS.UI.TOOLS.MOVETOSELECTTOOL.TOOLTIP, "{Hotkey}"),
						false
					));

			}
		}

	}
}
