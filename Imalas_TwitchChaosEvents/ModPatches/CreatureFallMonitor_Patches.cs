using HarmonyLib;
using Imalas_TwitchChaosEvents.Events;

namespace Imalas_TwitchChaosEvents.ModPatches
{
	internal class CreatureFallMonitor_Patches
	{

		[HarmonyPatch(typeof(CreatureFallMonitor.Instance), nameof(CreatureFallMonitor.Instance.CanSwimAtCurrentLocation))]
		public class CreatureFallMonitor_Instance_CanSwimAtCurrentLocation_Patch
		{
			public static void Postfix(CreatureFallMonitor.Instance __instance, ref bool __result)
			{
				if (!FlyingFishEvent.EventActive)
					return;

				if (__instance.def.canSwim)
				{
					__result = true;
				}
			}
		}
	}
}
