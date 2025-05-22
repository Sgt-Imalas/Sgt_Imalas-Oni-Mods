using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkiTrueTiles_SkinSelectorAddon.Patches
{
    class SimCellOccupierPatch
    {
		[HarmonyPatch(typeof(SimCellOccupier), "OnCleanUp")]
		public class SimCellOccupier_OnCleanup_Patch
		{
			public static void Prefix(SimCellOccupier __instance)
			{
				if (!__instance.HasTag(ModAssets.Tags.texturedTile))
					return;

				ModAssets.RemoveAll(Grid.PosToCell(__instance));
			}
		}
		[HarmonyPatch(typeof(SimCellOccupier), "OnSpawn")]
		public class SimCellOccupier_OnSpawn_Patch
		{
			public static void Postfix(SimCellOccupier __instance)
			{
				if (!__instance.HasTag(ModAssets.Tags.texturedTile))
					return;

				var cell = Grid.PosToCell(__instance);

				if (__instance.GetComponent<PrimaryElement>() is PrimaryElement primaryElement)
					ModAssets.TT_Add(cell, primaryElement.ElementID);

				// tiles like airflow tiles need a frame delay to update
				if (!__instance.doReplaceElement)
					GameScheduler.Instance.ScheduleNextFrame("refresh cell", obj => RefreshCell(cell));
			}

			private static void RefreshCell(int cell)
			{
				TileVisualizer.RefreshCell(cell, ObjectLayer.FoundationTile, ObjectLayer.ReplacementTile);
			}
		}
	}
}
