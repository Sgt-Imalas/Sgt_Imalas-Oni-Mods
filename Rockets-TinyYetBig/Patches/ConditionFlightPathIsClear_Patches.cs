using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
	internal class ConditionFlightPathIsClear_Patches
	{

		[HarmonyPatch(typeof(ConditionFlightPathIsClear), nameof(ConditionFlightPathIsClear.GetStatusTooltip))]
		public class ConditionFlightPathIsClear_GetStatusTooltip_Patch
		{
			public static void Postfix(ConditionFlightPathIsClear __instance, ProcessCondition.Status status, ref string __result)
			{
				ModAssets.ClearBlockedBy_Flight();

				if (status == ProcessCondition.Status.Ready)
					return;

				if (__instance.moduleInterface == null || __instance.moduleInterface.CurrentPad == null)
					return;


				List<Building> modules = [.. __instance.moduleInterface.ClusterModules.Select(m => m.Get().GetComponent<Building>()).OrderByDescending(b => b.Def.WidthInCells)];

				if (!modules.Any()) return;

				int y = (int)modules.First().GetMyWorld().maximumBounds.y;


				//HashSet<int> obstructingCells = new HashSet<int>();
				string blockingName = string.Empty;
				int cellBlock = Grid.InvalidCell;

				for (int i = 0; i < modules.Count; i++)
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
							cellBlock = upwardsCell;
							//obstructingCells.Add(upwardsCell);
							breakAfter = true;
							break;
						}
					}
					if (breakAfter)
						break;
				}
				//skip double mentions
				ModAssets.SetBlocker(cellBlock, false, out blockingName);
				string blockedBy = "\n\n" + STRINGS.UI_MOD.RTB_ROCKETBLOCKEDBY + " " + blockingName + " " + STRINGS.UI_MOD.RTB_CLICKTOSELECT;

				//foreach (var cell in obstructingCells)
				//{
				//	var building = Grid.Objects[cell, (int)ObjectLayer.Building];
				//	if (building == null) //gantries have their own layer
				//		building = Grid.Objects[cell, (int)ObjectLayer.Gantry];

				//	if (building != null)
				//	{
				//		if (!ModAssets.LastBlockingBuildings.Contains(building))
				//		{
				//			ModAssets.LastBlockingBuildings.Add(building);
				//			blockedBy += "\n• ";
				//			blockedBy += UIUtils.StripAllFormatting(building.GetProperName());
				//		}
				//	}
				//	else
				//	{
				//		ModAssets.LastBlockingCells.Add(cell);
				//		blockedBy += "\n• ";
				//		blockedBy += Grid.Element[cell].name;
				//	}
				//}
				//blockedBy += "\n" + STRINGS.UI_MOD.RTB_CLICKTOSELECT;
				__result += blockedBy;
			}
		}
	}
}
