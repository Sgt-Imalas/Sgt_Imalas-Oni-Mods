using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rockets_TinyYetBig.Patches
{
	internal class BuildingFacadeCustomData_Patches
	{

        [HarmonyPatch(typeof(BuildingFacadeCustomData), nameof(BuildingFacadeCustomData.ApplyCustomData))]
        public class BuildingFacadeCustomData_ApplyCustomData_Patch
        {
            public static void Postfix(Building building)
            {
                if (!building.TryGetComponent<RocketModuleCluster>(out _) || !building.TryGetComponent<VerticalModuleTiler>(out var tiler))
                    return;
				int cellTop = tiler.GetCellTop();
				int cellBottom = tiler.GetCellBottom();
				if (Grid.IsValidCell(cellTop))
				{
					var above = Grid.Objects[cellTop, (int) tiler.objectLayer];
					if (above != null && above.TryGetComponent<VerticalModuleTiler>(out var tilerAbove))
						tilerAbove.OnNeighbourCellsUpdated(null);
				}
				if (Grid.IsValidCell(cellBottom))
				{
					var below = Grid.Objects[cellBottom, (int)tiler.objectLayer];
					if (below != null && below.TryGetComponent<VerticalModuleTiler>(out var tilerBelow))
						tilerBelow.OnNeighbourCellsUpdated(null);
				}
			}
        }
	}
}
