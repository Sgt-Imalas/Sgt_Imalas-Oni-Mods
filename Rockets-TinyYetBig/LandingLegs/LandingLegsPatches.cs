using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    return false;

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
    }
}
