using HarmonyLib;
using Rockets_TinyYetBig.Derelicts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.Patches.StationDerelictPatches
{
	internal class LoreBearer_Patch
	{
		/// <summary>
		/// Create a derelict station when a lore bearer is read, aka "on inspection" of the derelict poi
		/// </summary>
		[HarmonyPatch(typeof(LoreBearer), nameof(LoreBearer.OnClickRead))]
		public static class RevealDerelictOnLoreRead
		{
			[HarmonyPrepare]
			static bool Prepare() => false;
			public static void Postfix(LoreBearer __instance)
			{
				ClusterManager.Instance.Trigger(1943181844, (object)"lorebearer revealed");
				if (__instance.TryGetComponent<ArtifactPOIClusterGridEntity>(out var artifact))
				{
					DerelictStation.SpawnNewDerelictStation(artifact);
				}

			}
		}
	}
}
