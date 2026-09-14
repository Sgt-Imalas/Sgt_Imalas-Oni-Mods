using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rockets_TinyYetBig.Patches
{
	internal class ConditionFlightPathIsClear_Patches
	{

		[HarmonyPatch(typeof(ConditionFlightPathIsClear), nameof(ConditionFlightPathIsClear.GetStatusTooltip))]
		public class ConditionFlightPathIsClear_GetStatusTooltip_Patch
		{
			public static void Postfix(ConditionFlightPathIsClear __instance, ProcessCondition.Status status, ref string __result)
			{
				if (status == ProcessCondition.Status.Ready)
					return;

				if (__instance.moduleInterface == null || __instance.moduleInterface.CurrentPad == null)
					return;


				List<Building> modules = [.. __instance.moduleInterface.ClusterModules.Select(m => m.Get().GetComponent<Building>()).OrderByDescending(b => b.Def.WidthInCells)];

				if(!modules.Any()) return;

				int y = (int)modules.First().GetMyWorld().maximumBounds.y;


				HashSet<int> obstructingCells = new HashSet<int>();

				for (int i = 0; i< modules.Count; i++)
				{
					var module = modules[i];

					if (!module.TryGetComponent<Building>(out var building))
						continue;
					Extents extents = building.GetExtents();
					int bottomLeftCell = Grid.XYToCell(extents.x, extents.y);
					bool breakAfter = false;

					for (int x = 0; x < extents.width; ++x)
					{
						int upwardsCell = Grid.OffsetCell(bottomLeftCell, new CellOffset(x, 0));
						while (!Grid.IsSolidCell(upwardsCell) && Grid.CellToXY(upwardsCell).y < y)
							upwardsCell = Grid.CellAbove(upwardsCell);
						if (Grid.IsSolidCell(upwardsCell) || Grid.CellToXY(upwardsCell).y != y)
						{
							obstructingCells.Add(upwardsCell);
							breakAfter = true;
						}
					}
					if(breakAfter)
						break;
				}
				string blockedBy = "\n\n" + STRINGS.UI_MOD.RTB_ROCKETBLOCKEDBY;
				foreach(var cell in obstructingCells)
				{
					var building = Grid.Objects[cell, (int)ObjectLayer.Building];
					if(building == null) 
						building = Grid.Objects[cell, (int)ObjectLayer.Gantry];

					if (building != null)
					{
						blockedBy += "\n• ";
						blockedBy += building.GetProperName();
					}
					else
					{
						blockedBy += "\n• ";
						blockedBy += Grid.Element[cell].name;
					}
				}
				__result += blockedBy;
			}
		}
	}
}
