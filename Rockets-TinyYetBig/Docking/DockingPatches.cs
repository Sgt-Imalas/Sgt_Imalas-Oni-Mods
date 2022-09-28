using HarmonyLib;
using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.Docking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
    class DockingPatches
    {
        [HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
        public static class CustomSideScreenPatch_DailyReset
        {
            public static void Postfix(List<DetailsScreen.SideScreenRef> ___sideScreens)
            {
                UIUtils.AddClonedSideScreen<DockingSidescreen>("DockingSidescreen", "LogicBroadcastChannelSideScreen", typeof(LogicBroadcastChannelSideScreen));
            }
        }

        [HarmonyPatch(typeof(Clustercraft), "OnClusterDestinationChanged")]
        public static class UndockOnFlight
        {
            public static void Postfix(Clustercraft __instance)
            {
                if (RocketryUtils.IsRocketInFlight(__instance))
                {
                    if(__instance.TryGetComponent<DockingManager>(out var mng))
                    {
                        mng.UndockAll();
                    }
                }
            }
        }
    }
}
