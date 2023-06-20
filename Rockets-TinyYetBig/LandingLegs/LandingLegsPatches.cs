using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static LaunchConditionManager;

namespace Rockets_TinyYetBig.LandingLegs
{
    class LandingLegsPatches
    {
        [HarmonyPatch(typeof(LaunchPad), "Sim1000ms")]
        public static class NoLogicUpdate1
        {
            public static bool Prefix(LaunchPad __instance)
            {
                if (__instance is RTB_LaunchPadWithoutLogic)
                {
                    __instance.CheckLandedRocketPassengerModuleStatus();
                    return false;
                }

                return true;
            }

        }
        [HarmonyPatch(typeof(LaunchPad), "IsLogicInputConnected")]
        public static class NoLogicUpdate2
        {
            public static bool Prefix(LaunchPad __instance)
            {
                if (__instance is RTB_LaunchPadWithoutLogic)
                    return false;

                return true;
            }

        }
        [HarmonyPatch(typeof(LaunchPad), "RebuildLaunchTowerHeight")]
        public static class NoLogicUpdate3
        {
            public static bool Prefix(LaunchPad __instance)
            {
                if (__instance is RTB_LaunchPadWithoutLogic)
                    return false;

                return true;
            }
            
        }
        [HarmonyPatch(typeof(SimpleInfoScreen), nameof(SimpleInfoScreen.RefreshProcessConditionsForType))]
        public static class NoLogicUpdate4
        {
            public static bool Prefix(SimpleInfoScreen __instance, ProcessCondition.ProcessConditionType conditionType)
            {


                if(__instance.selectedTarget == null || __instance.selectedTarget.GetComponent<IProcessConditionSet>() == null)
                    return false;


                if(__instance.selectedTarget.TryGetComponent<RTB_LaunchPadWithoutLogic>(out _))
                    return false;

                return true;
            }

        }
    }
}
