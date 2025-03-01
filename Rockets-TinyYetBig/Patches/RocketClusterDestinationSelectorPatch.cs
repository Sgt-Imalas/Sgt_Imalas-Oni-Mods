using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
	internal class RocketClusterDestinationSelectorPatch
	{

   //     [HarmonyPatch(typeof(RocketClusterDestinationSelector), nameof(RocketClusterDestinationSelector.SetDestinationPad))]
   //     public class RocketClusterDestinationSelector_SetDestinationPad_Patch
   //     {
   //         public static void Postfix(RocketClusterDestinationSelector __instance)
   //         {
   //             SgtLogger.l("OnSetDestinationPad", "RocketClusterDestinationSelectorPatch");
   //             foreach (var pad in __instance.m_launchPad)
   //             {
   //                 SgtLogger.l("WorldID: " + pad.Key + " -> " + pad.Value.Get()?.GetProperName());
   //             }

   //         }
   //     }

   //     [HarmonyPatch(typeof(RocketClusterDestinationSelector), nameof(RocketClusterDestinationSelector.GetDestinationPad), [typeof(AxialI)])]
   //     public class RocketClusterDestinationSelector_GetDestinationPad_Patch
   //     {
   //         public static void Postfix(RocketClusterDestinationSelector __instance, LaunchPad __result, AxialI destination)
   //         {
   //             SgtLogger.l("OnGetDestinationPad", "RocketClusterDestinationSelectorPatch");
   //             foreach (var pad in __instance.m_launchPad)
   //             {
   //                 SgtLogger.l("WorldID: " + pad.Key + " -> " + pad.Value.Get()?.GetProperName());
   //             }
   //             if (__result != null)
   //                 SgtLogger.l("Selected Pad: " + __result?.GetProperName() + " at " + destination.Q+","+destination.R);
			//	else
			//		SgtLogger.l("No pad selected at " + destination.Q + "," + destination.R);

			//}
   //     }


        /// <summary>
        /// Debug testing for an issue with landing with overlapping asteroids
        /// </summary>

   //     [HarmonyPatch(typeof(Clustercraft), nameof(Clustercraft.CanLandAtAsteroid))]
   //     public class Clustercraft_CanLandAtAsteroid_Patch
   //     {
   //         public static void Prefix(Clustercraft __instance, AxialI location, bool mustLandImmediately)
   //         {
   //             SgtLogger.l("CanLandAtLocation check: "+__instance.GetProperName() + " at " + location.Q + "," + location.R + " mustLandImmediately: " + mustLandImmediately);
   //             SgtLogger.l("Target Destination: " + __instance.m_moduleInterface?.GetClusterDestinationSelector().GetDestination().Q + "," + __instance.m_moduleInterface?.GetClusterDestinationSelector().GetDestination().R);
			//	LaunchPad destinationPad = __instance.m_moduleInterface?.GetClusterDestinationSelector()?.GetDestinationPad();

   //             if(destinationPad == null)
   //                 SgtLogger.l("no rocket platform was selected as destination, Assert passed by destinationPad being null");
   //             else
			//	{
			//		var worldLocation = destinationPad.GetMyWorldLocation();
   //                 SgtLogger.l("Rockets target world location: " + location.Q + "," + location.R);
			//		SgtLogger.l("Selected rocket platform: " + destinationPad?.GetProperName() + ", worldLocation of that: " + worldLocation.Q + "," + worldLocation.R);
			//		SgtLogger.l("Testing Assert: destinationPad == null -> " + (destinationPad == null) + " || destinationPad.GetMyWorldLocation() == location -> " + (destinationPad.GetMyWorldLocation() == location));
			//	}
			//}
   //     }

    //    [HarmonyPatch(typeof(Clustercraft), nameof(Clustercraft.Speed), MethodType.Getter)]
    //    public class Clustercraft_Speed_Patch
    //    {
    //        public static void Postfix(ref float __result)
    //        {
    //            __result *= 100;
    //        }
    //    }
    }
}
