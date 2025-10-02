using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.Patches
{
	internal class ConditionRobotPilotReady_Patches
	{

        [HarmonyPatch(typeof(ConditionRobotPilotReady), nameof(ConditionRobotPilotReady.EvaluateCondition))]
        public class ConditionRobotPilotReady_EvaluateCondition_Patch
        {
            /// <summary>
            /// Prevents robo pilots from launching by logic if they only have a single trip worth of databanks
            /// </summary>
            /// <param name="__instance"></param>
            /// <param name="__result"></param>

            public static void Postfix(ConditionRobotPilotReady __instance, ref ProcessCondition.Status __result)
            {
                ///warning status is problematic, ignore everything else
                if (__result != ProcessCondition.Status.Warning)
                    return;

                ///rocket can only strand if it doesnt have a spacefarer module, ignore otherwise
                if (__instance.RocketHasDupeControlStation())
                    return;

                if (__instance.craftInterface == null)
                    return;

                int distance = __instance.craftInterface.m_clustercraft.m_clusterTraveler.RemainingTravelNodes();

                bool canRoundTrip = __instance.module.HasResourcesToMove(distance * 2);

                if(!canRoundTrip)
                {
                    __result = ProcessCondition.Status.Failure;
                }
			}
        }
	}
}
