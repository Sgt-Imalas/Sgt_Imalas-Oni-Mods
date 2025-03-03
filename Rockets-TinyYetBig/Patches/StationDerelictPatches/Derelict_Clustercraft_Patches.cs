using HarmonyLib;
using Rockets_TinyYetBig.Derelicts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.Patches.StationDerelictPatches
{
	internal class Derelict_Clustercraft_Patches
	{
		[HarmonyPatch(typeof(Clustercraft), nameof(Clustercraft.GetPOIAtCurrentLocation))]
		public static class Clustercraft_GetPOIAtCurrentLocation_Redirect_DerelictStation
		{
			public static void Postfix(ref ClusterGridEntity __result)
			{
				if (__result == null) return;

				if (__result.TryGetComponent<DerelictStation>(out var derelictStation))
				{
					foreach (var poi in ClusterGrid.Instance.GetEntitiesOfLayerAtCell(derelictStation.Location, EntityLayer.POI))
					{
						if (poi.TryGetComponent<ArtifactPOIConfigurator>(out _))
						{
							__result = poi;
							return;
						}
					}
				}
			}
		}
	}
}
