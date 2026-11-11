using HarmonyLib;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
	internal class ClusterGrid_Patches
	{
		/// <summary>
		/// Causes ClusterGrid.GetVisibleEntityOfLayerAtCell to skip over space stations in its results (these share the poi layer)
		/// this is needed if a method checks for mineable/collectible pois
		/// always set it to false again after the calling method has finished (prefix on, postfix off)
		/// </summary>
		/// <param name="skip">if spacestations should be skipped.</param>
		public static void SkipStations(bool skip) => SkipSpaceStationsNextCalls = skip;
		private static bool SkipSpaceStationsNextCalls = false;

		[HarmonyPatch(typeof(ClusterGrid), nameof(ClusterGrid.GetVisibleEntityOfLayerAtCell))]
		public class ClusterGrid_GetVisibleEntityOfLayerAtCell_Patch
		{
			public static void Postfix(ClusterGrid __instance, AxialI cell, EntityLayer entityLayer, ref ClusterGridEntity __result)
			{
				if (SkipSpaceStationsNextCalls == false || entityLayer != SpaceStation.ENTITY_LAYER || __result == null || __result is not SpaceStation)
					return;
				SgtLogger.l("Skipping Station on entitylayer");
				__result = null;
				foreach (ClusterGridEntity entityOfLayerAtCell in __instance.cellContents[cell])
				{
					if (entityOfLayerAtCell.IsVisible && entityOfLayerAtCell.Layer == entityLayer && entityOfLayerAtCell is not SpaceStation)
					{
						__result = entityOfLayerAtCell;
						break;
					}
				}
			}
		}
	}
}
