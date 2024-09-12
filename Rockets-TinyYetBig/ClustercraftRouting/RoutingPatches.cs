using HarmonyLib;
using Rockets_TinyYetBig.Docking;

namespace Rockets_TinyYetBig.ClustercraftRouting
{
	internal class RoutingPatches
	{
		//[HarmonyPatch(typeof(ClustercraftConfig))]
		//[HarmonyPatch(nameof(ClustercraftConfig.CreatePrefab))]
		//public static class SwapOutClusterDestinationSelector
		//{
		//    public static void Postfix(GameObject __result)
		//    {
		//        if (__result.TryGetComponent<RocketClusterDestinationSelector>(out var ToRemove))
		//        {
		//            UnityEngine.Object.Destroy(ToRemove);
		//            var replacement = __result.AddOrGet<ExtendedRocketClusterDestinationSelector>();

		//            replacement.requireLaunchPadOnAsteroidDestination = true;
		//            replacement.assignable = true;
		//            replacement.shouldPointTowardsPath = true;
		//        }
		//    }
		//}

		//[HarmonyPatch(typeof(RocketClusterDestinationSelector))]
		//[HarmonyPatch(nameof(RocketClusterDestinationSelector.SetUpReturnTrip))]
		//public static class PreventRoundTripCancelationOnRocketDock
		//{
		//    public static bool Prefix(RocketClusterDestinationSelector __instance)
		//    {
		//        if (__instance.TryGetComponent<DockingSpacecraftHandler>(out DockingSpacecraftHandler handler))
		//        {
		//            return !handler.IsLoading;
		//        }
		//        if(__instance is ExtendedRocketClusterDestinationSelector extended)
		//        {
		//            extended.ProceedToNextTarget();
		//            return false;
		//        }

		//        return true;
		//    }
		//}

		[HarmonyPatch(typeof(RocketClusterDestinationSelector))]
		[HarmonyPatch(nameof(RocketClusterDestinationSelector.CanRocketHarvest))]
		public static class PreventRoundTripCancelationOnRocketDock3
		{
			public static void Postfix(RocketClusterDestinationSelector __instance, ref bool __result)
			{
				if (!__result && __instance.TryGetComponent<DockingSpacecraftHandler>(out DockingSpacecraftHandler handler))
				{
					__result = handler.IsLoading;
				}
			}
		}

	}
}
