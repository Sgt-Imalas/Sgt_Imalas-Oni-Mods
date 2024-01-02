using Epic.OnlineServices;
using HarmonyLib;
using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.Buildings;
using Rockets_TinyYetBig.Docking;
using Rockets_TinyYetBig.SpaceStations;
using Rockets_TinyYetBig.TwitchEvents.SpaceSpice;
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
        //[HarmonyPatch(typeof(BuildingDef), 
        //    nameof(BuildingDef.IsValidBuildLocation), 
        //    new Type[] { typeof(GameObject), typeof(int), typeof(Orientation), typeof(bool), typeof(string) },
        //    new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out})]
        //public static class FixTagForInvalidBuildlocationString
        //{
        //    public static void Postfix(string fail_reason)
        //    {
        //        if (fail_reason == null)
        //            return;

        //        SgtLogger.l(fail_reason, "FAILREASON");
        //        if (fail_reason.Contains(ModAssets.Tags.AttachmentSlotDockingDoor.ToString())){
        //            fail_reason.Replace(ModAssets.Tags.AttachmentSlotDockingDoor.ToString(), STRINGS.MISC.TAGS.RTB_DOCKINGTUBEATTACHMENTSLOT);
        //        }
        //    }
        //}

        [HarmonyPatch(typeof(ClustercraftInteriorDoorConfig))]
        [HarmonyPatch(nameof(ClustercraftInteriorDoorConfig.OnSpawn))]
        public static class AddDockingTubeAttachmentSlot
        {
            public static void Postfix(GameObject inst)
            {
                inst.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
                {
                    new BuildingAttachPoint.HardPoint(new CellOffset(1, 0), ModAssets.Tags.AttachmentSlotDockingDoor, (AttachableBuilding) null)
                };
            }
        }

        [HarmonyPatch(typeof(SaveLoader), "Save", new Type[] { typeof(string), typeof(bool), typeof(bool) })]
        public class SaveLoader_Save_Patch
        {
            public static void Prefix()
            {
                DockingManagerSingleton.Instance.OnSaving();
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
                    if (__instance.TryGetComponent<DockingSpacecraftHandler>(out var manager))
                    {
                        var myDestination = manager.clustercraft.ModuleInterface.GetClusterDestinationSelector().GetDestination();

                        foreach (var docked in manager.WorldDockables)
                        {
                            if (!DockingManagerSingleton.Instance.TryGetDockableIfDocked(docked.Value.GUID, out var dockedDockable))
                                continue;

                            if (SpaceStationManager.WorldIsSpaceStationInterior(dockedDockable.WorldId) && RocketryUtils.IsRocketInFlight(__instance))
                            {
                                DockingManagerSingleton.Instance.AddPendingUndock(docked.Value.GUID, dockedDockable.GUID);
                            }
                            else
                            {
                                var selector = dockedDockable.spacecraftHandler.clustercraft.ModuleInterface.GetClusterDestinationSelector();
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

        //[HarmonyPatch(typeof(ClustercraftConfig))]
        //[HarmonyPatch(nameof(ClustercraftConfig.OnSpawn))]
        //public static class AddDockingManager
        //{
        //    public static void Postfix(ref GameObject inst)
        //    {
        //        inst.AddOrGet<DockingManager>();               
        //    }
        //}


        [HarmonyPatch(typeof(Clustercraft))]
        [HarmonyPatch(nameof(Clustercraft.SetCraftStatus))]
        public static class UndockOnLand
        {
            public static void Postfix(Clustercraft __instance, CraftStatus craft_status)
            {
                if (__instance != null && __instance.gameObject != null && __instance.TryGetComponent<DockingSpacecraftHandler>(out var manager) && !craft_status.Equals(CraftStatus.InFlight))
                {
                    manager.UndockAll();
                }
            }
        }


        //TODO!:: patch does auto docking on reaching target for stations; rebuild!
        //[HarmonyPatch(typeof(RocketClusterDestinationSelector), nameof(RocketClusterDestinationSelector.OnClusterLocationChanged))]
        //public static class AutoDockToSpaceStation
        //{
        //    public static bool Prefix(RocketClusterDestinationSelector __instance, object data)
        //    {
        //        if (__instance.TryGetComponent<DockingManager>(out var mng))
        //        {
        //            if (mng.clustercraft.status != Clustercraft.CraftStatus.InFlight)
        //            {
        //                mng.UndockAll(true);
        //                return true;
        //            }

        //            ClusterLocationChangedEvent locationChangedEvent = (ClusterLocationChangedEvent)data;
        //            if (locationChangedEvent.newLocation != __instance.m_destination)
        //                return true;

        //            var AllDockerObjects = ClusterGrid.Instance.GetVisibleEntitiesAtCell(mng.clustercraft.Location).FindAll(e => e.TryGetComponent(out DockingManager manager));
        //            var AllDockers = AllDockerObjects
        //                .Select(e => e.GetComponent<DockingManager>())
        //                .Where(t_mng => t_mng.HasDoors() && t_mng.GetCraftType == DockableType.SpaceStation || t_mng.GetCraftType == DockableType.Derelict)
        //                .ToList();
        //            //SgtLogger.l(AllDockers.Count +"", "dockers");
        //            if (AllDockers.Count() == 0)
        //                return true;

        //            //SgtLogger.l(AllDockers.First().GetWorldId() + "", "firstworld");
        //            //SgtLogger.l(mng.GetWorldId() + "", "ownWorld");
        //            int targetWorldID = AllDockers.First().WorldId;
        //            mng.AddPendingDock(targetWorldID);

        //            if (!__instance.m_repeat)
        //                return false;

        //            SetupAwaitRocketLoading(__instance, mng, targetWorldID);

        //            return false;
        //        }
        //        return true;
        //    }
        //    static void SetupAwaitRocketLoading(RocketClusterDestinationSelector selector, DockingSpacecraftHandler handler, int targetWorld)
        //    {
        //        handler.OnFinishedLoading += new System.Action(() =>
        //        {

        //            handler.UnDockFromTargetWorld(targetWorld, OnFinishedUndock: () => InitReturn(selector, manager));
        //        });
        //    }
        //    static void InitReturn(RocketClusterDestinationSelector selector, DockingManager manager)
        //    {
        //        selector.SetUpReturnTrip();
        //        manager.OnFinishedLoading = null;
        //    }
        //}
    }
}
