using HarmonyLib;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.Patches.StationDerelictPatches
{
	internal class ArtifactPOIClusterGridEntity_Patches
	{
		/// <summary>
		/// Hides the derelict poi when the derelict station is uncovered
		/// </summary>
		[HarmonyPatch(typeof(ArtifactPOIClusterGridEntity), nameof(ArtifactPOIClusterGridEntity.IsVisible), MethodType.Getter)]
		public static class ArtifactPOIClusterGridEntity_ReplaceOnReveal
		{
			[HarmonyPrepare]
			static bool Prepare() => false; //disabled because derelict stations arent an official feature yet
			public static void Postfix(ArtifactPOIClusterGridEntity __instance, ref bool __result)
			{
				if (__instance.TryGetComponent<LoreBearer>(out var loreBearer))
				{
					if (SpaceStationManager.IsSpaceStationAt(__instance.Location))
						__result = !loreBearer.BeenClicked;
				}

			}
		}
	}
}
