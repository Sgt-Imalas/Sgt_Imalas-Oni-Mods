using Epic.OnlineServices;
using HarmonyLib;
using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.Docking;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static Clustercraft;
using static STRINGS.UI.CLUSTERMAP.ROCKETS;
using static TUNING.ROCKETRY;

namespace Rockets_TinyYetBig.Patches
{
    class DockingPatches
    {

        public static class Rocket_Speed_Docking_Patch
        {
            [HarmonyPatch(typeof(Clustercraft))]
            [HarmonyPatch(nameof(Clustercraft.EnginePower))]
            [HarmonyPatch(MethodType.Getter)]
            public static class EnginePower_Patch
            {
                public static void Postfix(Clustercraft __instance, ref float __result)
                {
                    if (__instance.TryGetComponent<DockingManager>(out var manager))
                    {
                        foreach (var docked in manager.GetConnectedRockets())
                        {
                            if (ClusterManager.Instance.GetWorld(docked).TryGetComponent<Clustercraft>(out var craft))
                            {
                                var engine = craft.ModuleInterface.GetEngine();

                                if (engine != null && engine.TryGetComponent<RocketModuleCluster>(out var engineModule) && Mathf.RoundToInt(craft.ModuleInterface.Range) > 0)
                                {
                                    __result += engineModule.performanceStats.EnginePower;
                                }
                            }
                        }
                    }
                    //SgtLogger.l("EnginePower " + __result);
                }
            }

            [HarmonyPatch(typeof(Clustercraft))]
            [HarmonyPatch(nameof(Clustercraft.TotalBurden))]
            [HarmonyPatch(MethodType.Getter)]
            public static class TotalBurden_Patch
            {
                public static void Postfix(Clustercraft __instance, ref float __result)
                {
                    if (__instance.TryGetComponent<DockingManager>(out var manager))
                    {
                        foreach (var docked in manager.GetConnectedRockets())
                        {
                            if (ClusterManager.Instance.GetWorld(docked).TryGetComponent<Clustercraft>(out var craft))
                            {
                                foreach (var module in craft.ModuleInterface.ClusterModules)
                                {
                                    __result += module.Get().performanceStats.Burden;
                                }
                            }
                        }
                    }
                    //SgtLogger.l("TotalBurden " + __result);
                }
            }
            [HarmonyPatch(typeof(Clustercraft))]
            [HarmonyPatch(nameof(Clustercraft.OnSpawn))]
            public static class Pull_Empty_Rockets
            {
                public static void Postfix(Clustercraft __instance)
                {
                    __instance.m_clusterTraveler.onTravelCB += () =>
                    {
                        if (__instance.TryGetComponent<DockingManager>(out var manager))
                        {
                            foreach (var docked in manager.GetConnectedRockets())
                            {
                                if (ClusterManager.Instance.GetWorld(docked).TryGetComponent<Clustercraft>(out var craft) 
                                && (Mathf.RoundToInt(craft.ModuleInterface.Range) == 0 || manager.GetCraftType != DockableType.Rocket)
                                && craft.Location != __instance.Location)
                                {
                                    if((ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(__instance.Location, EntityLayer.Asteroid) == null))
                                    {
                                        SgtLogger.l("Pulled stranded rocket " + craft.Name + " to new tile with " + __instance.Name);
                                        craft.Location = __instance.Location;
                                    }
                                    else
                                    {
                                        SgtLogger.l("Disconnected " + craft.Name + " as stranded in orbit");
                                        craft.m_clusterTraveler.m_destinationSelector.SetDestination(craft.Location);
                                        //craft.m_clusterTraveler.m_destinationSelector.SetDestination(__instance.Location);
                                    }
                                }
                            }
                        }
                    };
                }
            }


            [HarmonyPatch(typeof(Clustercraft))]
            [HarmonyPatch(nameof(Clustercraft.CheckDesinationInRange))]
            public static class CMI_Range_Patch
            {
                public static void Postfix(Clustercraft __instance, ref bool __result)
                {
                    if (__result == true)
                        return;

                    if (__instance.m_clusterTraveler.CurrentPath == null)
                    {
                        return;
                    }

                    __result = Mathf.RoundToInt(__instance.Speed * __instance.m_clusterTraveler.TravelETA()) <= Mathf.RoundToInt(__instance.ModuleInterface.Range);

                    //if (__instance.TryGetComponent<DockingManager>(out var manager))
                    //{
                    //    SgtLogger.l("speed: " + __instance.Speed + ", eta" + __instance.m_clusterTraveler.TravelETA() + ", range" + __instance.ModuleInterface.Range);
                    //}
                }
            }



            [HarmonyPatch(typeof(Clustercraft))]
            [HarmonyPatch(nameof(Clustercraft.Speed))]
            [HarmonyPatch(MethodType.Getter)]
            public static class Speed_Patch
            {
                public static bool Prefix(Clustercraft __instance, ref float __result)
                {
                    if (__instance.TryGetComponent<DockingManager>(out var manager))
                    {
                        float unmodifiedSpeed = __instance.EnginePower / __instance.TotalBurden;
                        float totalAutoPilotMultiplier = __instance.AutoPilotMultiplier;
                        float totalPilotSkillMultiplier = __instance.PilotSkillMultiplier;
                        float totalControlStationBuffTimeRemaining = __instance.controlStationBuffTimeRemaining;
                        //float numberOfPilots = 1;


                        if (manager.GetConnectedRockets().Count == 0)
                            return true;


                        foreach (var docked in manager.GetConnectedRockets())
                        {
                            if (ClusterManager.Instance.GetWorld(docked).TryGetComponent<Clustercraft>(out var craft))
                            {
                                totalAutoPilotMultiplier = totalAutoPilotMultiplier < craft.AutoPilotMultiplier ? craft.AutoPilotMultiplier : totalAutoPilotMultiplier;
                                totalPilotSkillMultiplier = totalPilotSkillMultiplier < craft.PilotSkillMultiplier ? craft.PilotSkillMultiplier : totalPilotSkillMultiplier;
                                totalControlStationBuffTimeRemaining = totalControlStationBuffTimeRemaining < craft.controlStationBuffTimeRemaining ? craft.controlStationBuffTimeRemaining : totalControlStationBuffTimeRemaining;
                                //++numberOfPilots;
                            }
                        }
                        //totalAutoPilotMultiplier /= numberOfPilots;
                        //totalPilotSkillMultiplier /= numberOfPilots;
                        //totalControlStationBuffTimeRemaining /= numberOfPilots;


                        float finalValue = unmodifiedSpeed * totalAutoPilotMultiplier * totalPilotSkillMultiplier;
                        if (totalControlStationBuffTimeRemaining > 0f)
                        {
                            finalValue += unmodifiedSpeed * 0.2f;
                        }
                        __result = finalValue;

                        return false;
                    }
                    return true;
                }
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

        [HarmonyPatch(typeof(Clustercraft))]
        [HarmonyPatch(nameof(Clustercraft.OnClusterDestinationChanged))]
        public static class UndockOnFlight
        {
            public static void Postfix(Clustercraft __instance)
            {
                if (true
                    //RocketryUtils.IsRocketInFlight(__instance)
                    )
                {
                    if (__instance.TryGetComponent<DockingManager>(out var manager))
                    {
                        var myDestination = manager.clustercraft.ModuleInterface.GetClusterDestinationSelector().GetDestination();

                        foreach (var docked in manager.DockingDoors)
                        {
                            if (docked.Value == -1)
                                continue;

                            if (SpaceStationManager.WorldIsSpaceStationInterior(docked.Value) && RocketryUtils.IsRocketInFlight(__instance))
                            {
                                manager.UnDockFromTargetWorld(docked.Value);
                            }
                            else
                            {
                                var selector = docked.Key.GetConnec().dManager.clustercraft.ModuleInterface.GetClusterDestinationSelector();
                                if (selector.GetDestination() != myDestination)
                                {
                                    selector.SetDestination(myDestination);
                                }
                            }
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Clustercraft))]
        [HarmonyPatch(nameof(Clustercraft.SetCraftStatus))]
        public static class UndockOnLand
        {
            public static void Postfix(Clustercraft __instance, CraftStatus craft_status)
            {
                if (__instance.TryGetComponent<DockingManager>(out var manager) && craft_status != CraftStatus.InFlight)
                {
                    manager.UndockAll();
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
                    if (mng.clustercraft.status != Clustercraft.CraftStatus.InFlight)
                    {
                        mng.UndockAll(true);
                        return true;
                    }

                    ClusterLocationChangedEvent locationChangedEvent = (ClusterLocationChangedEvent)data;
                    if (locationChangedEvent.newLocation != __instance.m_destination)
                        return true;

                    var AllDockerObjects = ClusterGrid.Instance.GetVisibleEntitiesAtCell(mng.clustercraft.Location).FindAll(e => e.TryGetComponent(out DockingManager manager));
                    var AllDockers = AllDockerObjects
                        .Select(e => e.GetComponent<DockingManager>())
                        .Where(t_mng => t_mng.HasDoors() && t_mng.GetCraftType == DockableType.SpaceStation || t_mng.GetCraftType == DockableType.Derelict)
                        .ToList();
                    //SgtLogger.l(AllDockers.Count +"", "dockers");
                    if (AllDockers.Count() == 0)
                        return true;

                    //SgtLogger.l(AllDockers.First().GetWorldId() + "", "firstworld");
                    //SgtLogger.l(mng.GetWorldId() + "", "ownWorld");
                    int targetWorldID = AllDockers.First().WorldId;
                    mng.AddPendingDock(targetWorldID);

                    if (!__instance.m_repeat)
                        return false;

                    SetupAwaitRocketLoading(__instance, mng, targetWorldID);

                    return false;
                }
                return true;
            }
            static void SetupAwaitRocketLoading(RocketClusterDestinationSelector selector, DockingManager manager, int targetWorld)
            {
                manager.OnFinishedLoading += new System.Action(() =>
                {
                    manager.UnDockFromTargetWorld(targetWorld, OnFinishedUndock: () => InitReturn(selector, manager));
                });
            }
            static void InitReturn(RocketClusterDestinationSelector selector, DockingManager manager)
            {
                selector.SetUpReturnTrip();
                manager.OnFinishedLoading = null;
            }
        }
    }
}
