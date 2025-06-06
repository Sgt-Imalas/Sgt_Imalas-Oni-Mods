using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UtilLibs.BuildingPortUtils
{
    public static class ConduitDisplayPortPatching
	{
		private static HashSet<string> buildings = new HashSet<string>();
		internal static bool HasBuilding(string name)
		{
			return buildings.Contains(name);
		}

		// Add a building to the cache
		internal static void AddBuilding(string ID)
		{
			buildings.Add(ID);
		}

		public static void PatchAll(Harmony harmony)
		{
			var target = AccessTools.Method(typeof(EntityCellVisualizer), nameof(EntityCellVisualizer.DrawIcons));
			harmony.Patch(target, new HarmonyMethod(typeof(ConduitDisplayPortPatching), nameof(PortDrawPrefix)));

			var target2 = AccessTools.Method(typeof(BuildingDef), nameof(BuildingDef.MarkArea));
			harmony.Patch(target2, null,new HarmonyMethod(typeof(ConduitDisplayPortPatching), nameof(MarkAreaPostfix)));

			var target3 = AccessTools.Method(typeof(BuildingDef), nameof(BuildingDef.AreConduitPortsInValidPositions));
			harmony.Patch(target3, null, new HarmonyMethod(typeof(ConduitDisplayPortPatching), nameof(AreConduitPortsInValidPositionsPostfix)));
		}

		public static bool PortDrawPrefix(EntityCellVisualizer __instance, HashedString mode)
		{
			if (__instance is BuildingCellVisualizer bcVis && buildings.Contains(bcVis.building.Def.PrefabID))
			{
				UnityEngine.GameObject go = bcVis.building.gameObject;
				PortDisplayController controller = go.GetComponent<PortDisplayController>();
				if (controller != null)
				{
					return controller.Draw(bcVis, mode, go);
				}
			}
			return true;
		}

		public static void MarkAreaPostfix(BuildingDef __instance, int cell, Orientation orientation, ObjectLayer layer, GameObject go)
		{
			foreach (PortDisplay2 portDisplay in __instance.BuildingComplete.GetComponents<PortDisplay2>())
			{
				ConduitType secondaryConduitType2 = portDisplay.type;
				ObjectLayer objectLayerForConduitType4 = Grid.GetObjectLayerForConduitType(secondaryConduitType2);
				CellOffset rotatedCellOffset8 = Rotatable.GetRotatedCellOffset(portDisplay.offset, orientation);
				int cell11 = Grid.OffsetCell(cell, rotatedCellOffset8);
				__instance.MarkOverlappingPorts(Grid.Objects[cell11, (int)objectLayerForConduitType4], go);
				Grid.Objects[cell11, (int)objectLayerForConduitType4] = go;
			}
		}
		public static void AreConduitPortsInValidPositionsPostfix(BuildingDef __instance, ref bool __result, GameObject source_go, int cell, Orientation orientation, ref string fail_reason)
		{
			if (__result)
			{
				foreach (PortDisplay2 portDisplay in __instance.BuildingComplete.GetComponents<PortDisplay2>())
				{
					CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(portDisplay.offset, orientation);
					int utility_cell = Grid.OffsetCell(cell, rotatedCellOffset);
					__result = __instance.IsValidConduitConnection(source_go, portDisplay.type, utility_cell, ref fail_reason);
					if (!__result)
					{
						return;
					}
				}
			}
		}
	}
}
