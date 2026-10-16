using HarmonyLib;
using Rockets_TinyYetBig.SpaceStations;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches.SpaceStationPatches
{
	internal class RocketInteriorSectionSideScreen_Patches
	{
		/// <summary>
		/// Hide habitat side screen for ai rockets
		/// </summary>
		[HarmonyPatch(typeof(RocketInteriorSectionSideScreen), nameof(RocketInteriorSectionSideScreen.IsValidForTarget))]
		public class RocketInteriorSectionSideScreen_IsValidForTarget_Patch
		{
			public static bool Prefix(GameObject target, ref bool __result)
			{
				if (target.TryGetComponent<SpaceStation>(out _))
				{
					__result = false;
					return false;
				}
				return true;
			}
		}
	}
}
