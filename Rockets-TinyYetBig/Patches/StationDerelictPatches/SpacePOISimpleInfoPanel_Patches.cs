using HarmonyLib;
using Rockets_TinyYetBig.Derelicts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches.StationDerelictPatches
{
	internal class SpacePOISimpleInfoPanel_Patches
	{
		/// <summary>
		/// Redirecting the info panel from the derelict station to the artifact poi thats behind it (derelict stations exist temporarily on top of artifact pois when they are "revealed")
		/// </summary>
		[HarmonyPatch(typeof(SpacePOISimpleInfoPanel), nameof(SpacePOISimpleInfoPanel.Refresh))]
		public static class SpacePOISimpleInfoPanel_Redirect_DerelictStation
		{
			public static void Prefix(ref GameObject selectedTarget)
			{
				if (selectedTarget == null) return;

				if (selectedTarget.TryGetComponent<DerelictStation>(out var derelictStation))
				{
					foreach (var poi in ClusterGrid.Instance.GetEntitiesOfLayerAtCell(derelictStation.Location, EntityLayer.POI))
					{
						if (poi.TryGetComponent<ArtifactPOIConfigurator>(out _))
						{
							selectedTarget = poi.gameObject;
							return;
						}
					}
				}
			}
		}
	}
}
