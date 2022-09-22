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
        [HarmonyPatch(typeof(Clustercraft))]
        [HarmonyPatch("OnSpawn")]
        public class AddDockingManager_Patch
        { 
            public static void Postfix(Clustercraft __instance)
            {
                //__instance.FindOrAdd<DockingManager>();
            }
        }
        [HarmonyPatch(typeof(Clustercraft))]
        [HarmonyPatch("OnCleanUp")]
        public class RemoveDockingManager_Patch
        {
            public static void Postfix(Clustercraft __instance)
            {
                //__instance.FindOrAdd<DockingManager>().DeleteObject();
            }
        }

        [HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
        public static class CustomSideScreenPatch_DailyReset
        {
            public static void Postfix(List<DetailsScreen.SideScreenRef> ___sideScreens)
            {
                UIUtils.AddClonedSideScreen<DockingSidescreen>("DockingSidescreen", "LogicBroadcastChannelSideScreen", typeof(LogicBroadcastChannelSideScreen));
            }
        }
    }
}
