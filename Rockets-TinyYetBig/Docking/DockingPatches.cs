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

        [HarmonyPatch(typeof(RocketClusterDestinationSelector), nameof(RocketClusterDestinationSelector.OnClusterLocationChanged))]
        public static class AutoDockToSpaceStation
        {
            public static bool Prefix(RocketClusterDestinationSelector __instance, object data)
            {
                if (__instance.TryGetComponent<DockingManager>(out var mng))
                {
                    ClusterLocationChangedEvent locationChangedEvent = (ClusterLocationChangedEvent)data;
                    if (locationChangedEvent.newLocation != __instance.m_destination)
                        return true;

                    var AllDockerObjects = ClusterGrid.Instance.GetVisibleEntitiesAtCell(mng.clustercraft.Location).FindAll(e => e.TryGetComponent(out DockingManager manager));
                    var AllDockers = AllDockerObjects
                        .Select(e => e.GetComponent<DockingManager>())
                        .Where(t_mng => t_mng.HasDoors() && t_mng.GetCraftType == DockableType.SpaceStation || t_mng.GetCraftType == DockableType.Derelict)                        
                        .ToList();
                    SgtLogger.l(AllDockers.Count +"", "dockers");
                    if(AllDockers.Count()==0) 
                        return true;

                    SgtLogger.l(AllDockers.First().GetWorldId() + "", "firstworld");
                    SgtLogger.l(mng.GetWorldId() + "", "ownWorld");
                    mng.AddPendingDock(AllDockers.First().GetWorldId());

                    if(!__instance.m_repeat)
                        return false;


                    return false;
                }
                return true;
            }
        }
    }
}
