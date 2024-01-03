using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TemplateClasses;
using UnityEngine;
using UtilLibs;
using Rockets_TinyYetBig.Science;
using Klei.AI;
using static Operational;
using Rockets_TinyYetBig.Elements;
using static ResearchTypes;

namespace Rockets_TinyYetBig.SpaceStations.Patches
{

    class SpaceStationPatches
    {
        /// <summary>
        /// Allows for "Space Station Only Buildings"
        /// </summary>
        [HarmonyPatch(typeof(PlanScreen), "GetBuildableStateForDef")]
        public static class AllowCertainBuildingsInSpaceStations
        {
            public static void Postfix(BuildingDef def, ref PlanScreen.RequirementsState __result)
            {
                if (def.BuildingComplete.HasTag(ModAssets.Tags.SpaceStationOnlyInteriorBuilding) && SpaceStationManager.ActiveWorldIsSpaceStationInterior())
                {
                    //SgtLogger.debuglog(def.PrefabID + " - Is SpaceStationBuilding; state: " + __result);

                    if (
                        def.BuildingComplete.HasTag(GameTags.NotRocketInteriorBuilding) && def.BuildingComplete.HasTag(ModAssets.Tags.SpaceStationOnlyInteriorBuilding))
                    {
                        if (!DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive && !ProductInfoScreen.MaterialsMet(def.CraftRecipe))
                            __result = PlanScreen.RequirementsState.Materials;
                        else
                            __result = PlanScreen.RequirementsState.Complete;
                    }
                }
                if (def.BuildingComplete.HasTag(ModAssets.Tags.SpaceStationOnlyInteriorBuilding) && SpaceStationManager.ActiveWorldIsRocketInterior() || def.BuildingComplete.HasTag(ModAssets.Tags.RocketInteriorOnlyBuilding) && SpaceStationManager.ActiveWorldIsSpaceStationInterior())
                {
                    __result = PlanScreen.RequirementsState.RocketInteriorForbidden;
                }
            }
        }

        /// <summary>
        /// No Boosting
        /// </summary>
        [HarmonyPatch(typeof(MissionControlCluster.Instance))]
        [HarmonyPatch(nameof(MissionControlCluster.Instance.CanBeBoosted))]
        public static class DisableRocketBoost
        {
            public static bool Prefix(Clustercraft clustercraft, ref bool __result)
            {
                if (clustercraft is SpaceStation)
                {
                    __result = false;
                    return false;
                }
                return true;

            }
        }
        /// <summary>
        /// Removes Station from launchpad
        /// </summary>
        [HarmonyPatch(typeof(ClusterGrid))]
        [HarmonyPatch(nameof(ClusterGrid.GetEntitiesInRange))]
        public static class HideStationsFromLandingPads
        {
            public static void Postfix(ref List<ClusterGridEntity> __result)
            {
                List<ClusterGridEntity> AdjustedList = new List<ClusterGridEntity>();
                foreach (var entity in __result)
                {
                    if (!(entity is SpaceStation))
                    {
                        AdjustedList.Add(entity);
                    }
                }
                __result = AdjustedList;

            }
        }


        [HarmonyPatch(typeof(CameraController))]
        [HarmonyPatch(nameof(CameraController.ConstrainToWorld))]
        public static class ConstrainToSmallerWorld_FixForModules
        {
            public static bool Prefix(CameraController __instance)
            {
                if (Game.Instance == null || Game.Instance.IsLoading() || __instance.FreeCameraEnabled)
                    return false;

                WorldContainer activeWorld = ClusterManager.Instance.activeWorld;



                var bottomLeftBoundryBox = activeWorld.WorldOffset; // + (Vector2)activeWorld.WorldSize * 0.05f
                var topRightBoundryBox = activeWorld.WorldOffset + (Vector2)activeWorld.WorldSize;

                if (activeWorld.TryGetComponent<SpaceStation>(out var spaceStation))
                {
                    bottomLeftBoundryBox = activeWorld.WorldOffset + spaceStation.bottomLeftCorner;
                    topRightBoundryBox = activeWorld.WorldOffset + spaceStation.topRightCorner;
                }


                var Pos = __instance.baseCamera.transform.GetPosition();

                bool modify = false;
                if (Pos.x < bottomLeftBoundryBox.x)
                {
                    modify = true;
                    Pos.x = bottomLeftBoundryBox.x;
                }
                else if (Pos.x > topRightBoundryBox.x)
                {
                    modify = true;
                    Pos.x = topRightBoundryBox.x;
                }
                if (Pos.y < bottomLeftBoundryBox.y)
                {
                    modify = true;
                    Pos.y = bottomLeftBoundryBox.y;
                }
                else if (Pos.y > topRightBoundryBox.y)
                {
                    modify = true;
                    Pos.y = topRightBoundryBox.y;
                }

                if (modify)
                {
                    Pos.z = -100;
                    __instance.transform.SetPosition(Pos);
                }
                return false;
            }
        }


        /// <summary>
        /// Nameable Stations
        /// </summary>
        [HarmonyPatch(typeof(UserNameable), "SetName")]
        public static class NameableStationsPatch
        {
            public static void Postfix(string name, UserNameable __instance)
            {
                if (__instance.TryGetComponent<SpaceStation>(out var station))
                {
                    ClusterNameDisplayScreen.Instance.UpdateName(station);
                    __instance.Trigger(1102426921, name);
                }
            }
        }

        /// <summary>
        /// Adding custom sidescreen
        /// </summary>
        [HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
        public static class CustomSideScreenPatch_SatelliteCarrier
        {
            public static void Postfix(List<DetailsScreen.SideScreenRef> ___sideScreens)
            {
               // UIUtils.AddClonedSideScreen<SpaceStationBuilderModuleSideScreen>("SpaceStationBuilderModuleSideScreen", "ArtableSelectionSideScreen", typeof(ArtableSelectionSideScreen));
                UIUtils.AddClonedSideScreen<SpaceStationSideScreen>("SpaceStationSideScreen", "ClusterGridWorldSideScreen", typeof(ClusterGridWorldSideScreen));
            }
        }


        /// <summary>
        /// Removing that method from space station interiors (to prevent crash)
        /// </summary>
        [HarmonyPatch(typeof(ClusterManager), "UpdateWorldReverbSnapshot")]
        public static class DisableAudioReverbGetter
        {
            public static bool Prefix(int worldId, ClusterManager __instance)
            {
                //AudioMixer.instance.Stop(AudioMixerSnapshots.Get().SmallRocketInteriorReverbSnapshot);
                //AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MediumRocketInteriorReverbSnapshot);

                WorldContainer world = __instance.GetWorld(worldId);
                if (world != null && world.IsModuleInterior && world.TryGetComponent<SpaceStation>(out _))
                    return false;
                return true;
            }
        }
        [HarmonyPatch(typeof(CraftModuleInterface), nameof(CraftModuleInterface.GetInteriorWorld))]
        public static class GetInteriorWorld_SpaceStation
        {
            public static bool Prefix(CraftModuleInterface __instance, ref WorldContainer __result)
            {
                if (__instance.TryGetComponent(out SpaceStation station))
                {
                    __result = null;
                    if (station.SpaceStationInteriorId != -1)
                        __result = ClusterManager.Instance.GetWorld(station.SpaceStationInteriorId);
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(RadiationBalanceDisplayer))]
        [HarmonyPatch(nameof(RadiationBalanceDisplayer.GetTooltip))]
        public static class Fixes_Crash_for_out_of_world_dupes
        {
            public static bool Prefix(Amount master, AmountInstance instance)
            {
                int targetCell = Grid.PosToCell(instance.gameObject);
                return Grid.IsValidCell(targetCell);
            }
        }


        [HarmonyPatch(typeof(LandingBeacon))]
        [HarmonyPatch(nameof(LandingBeacon.UpdateLineOfSight))]
        public static class LandingBeacon_SpaceStationFix
        {
            public static bool Prefix(LandingBeacon.Instance smi)
            {
                var worldId = smi.GetMyWorldId();
                if (SpaceStationManager.WorldIsSpaceStationInterior(worldId))
                {
                    bool flag = true;
                    var myWorld = ClusterManager.Instance.GetWorld(worldId);
                    int num = Grid.PosToCell(smi);
                    for (int y = (int)myWorld.maximumBounds.y; Grid.CellRow(num) <= y; num = Grid.CellAbove(num))
                    {
                        if (!Grid.IsValidCell(num) || Grid.Solid[num] && Grid.Element[num].id != ModElements.SpaceStationForceField.SimHash)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (smi.skyLastVisible == flag)
                        return false;
                    smi.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NoSurfaceSight, !flag);
                    smi.operational.SetFlag(LandingBeacon.noSurfaceSight, flag);
                    smi.skyLastVisible = flag;

                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(RailGun))]
        [HarmonyPatch(nameof(RailGun.Sim200ms))]
        public static class Railgun_SpaceStationFix
        {
            public static bool Prefix(RailGun __instance)
            {
                var worldId = __instance.GetMyWorldId();
                if (SpaceStationManager.WorldIsSpaceStationInterior(worldId))
                {
                    var myWorld = ClusterManager.Instance.GetWorld(worldId);
                    Extents extents = __instance.GetComponent<Building>().GetExtents();

                    int x1 = extents.x;
                    int x2 = extents.x + extents.width - 2;
                    int y1 = extents.y + extents.height;
                    int cell1 = Grid.XYToCell(x1, y1);
                    int y2 = y1;
                    int cell2 = Grid.XYToCell(x2, y2);
                    bool flag = true;
                    int y3 = (int)myWorld.maximumBounds.y;
                    for (int index1 = cell1; index1 <= cell2; ++index1)
                    {
                        for (int index2 = index1; Grid.CellRow(index2) <= y3; index2 = Grid.CellAbove(index2))
                        {
                            if (!Grid.IsValidCell(index2) || Grid.Solid[index2] && Grid.Element[index2].id != ModElements.SpaceStationForceField.SimHash)
                            {
                                flag = false;
                                break;
                            }
                        }
                    }
                    __instance.operational.SetFlag(RailGun.noSurfaceSight, flag);
                    __instance.operational.SetFlag(RailGun.noDestination, __instance.destinationSelector.GetDestinationWorld() >= 0);
                    if (__instance.TryGetComponent<KSelectable>(out var component))
                    {
                        component.ToggleStatusItem(RailGun.noSurfaceSightStatusItem, !flag);
                        component.ToggleStatusItem(RailGun.noDestinationStatusItem, __instance.destinationSelector.GetDestinationWorld() < 0);
                    }
                    __instance.UpdateMeters();
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(WorldContainer))]
        [HarmonyPatch(nameof(WorldContainer.maximumBounds))]
        [HarmonyPatch(MethodType.Getter)]
        public static class DimensionLimiter_SpaceStation_max
        {
            public static void Postfix(WorldContainer __instance, ref Vector2 __result)
            {
                if (!__instance.IsNullOrDestroyed() && __instance.TryGetComponent<SpaceStation>(out var spaceStation))
                {
                    __result = Vector2.Min(__result, __instance.WorldOffset + spaceStation.topRightCorner);
                }
            }
        }
        [HarmonyPatch(typeof(WorldContainer))]
        [HarmonyPatch(nameof(WorldContainer.minimumBounds))]
        [HarmonyPatch(MethodType.Getter)]
        public static class DimensionLimiter_SpaceStation_min
        {
            public static void Postfix(WorldContainer __instance, ref Vector2 __result)
            {
                if (!__instance.IsNullOrDestroyed() && __instance.TryGetComponent<SpaceStation>(out var spaceStation))
                {
                    __result = Vector2.Max(__result, __instance.WorldOffset + spaceStation.bottomLeftCorner);
                }
            }
        }



        [HarmonyPatch(typeof(Clustercraft))]
        [HarmonyPatch(nameof(Clustercraft.Destination))]
        [HarmonyPatch(MethodType.Getter)]
        public static class NoDestinationSelection
        {
            public static bool Prefix(Clustercraft __instance)
            {
                if (__instance is SpaceStation)
                    return false;
                return true;
            }

        }
        [HarmonyPatch(typeof(ClusterMapVisualizer))]
        [HarmonyPatch("OnSpawn")]
        public static class SwitchOutAnimator
        {
            public static bool Prefix(ClusterMapVisualizer __instance, ClusterGridEntity ___entity)
            {
                if (___entity is SpaceStation)
                {
                    //new ClusterMapFXAnimator.StatesInstance(__instance, ___entity).StartSM();
                    return false;

                }
                return true;
            }

        }

        [HarmonyPatch(typeof(Clustercraft))]
        [HarmonyPatch(nameof(Clustercraft.TotalBurden))]
        [HarmonyPatch(MethodType.Getter)]
        public static class NoBurdenForSpaceStation
        {
            public static bool Prefix(Clustercraft __instance, ref float __result)
            {
                if (__instance is SpaceStation)
                {
                    __result = 0f;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Clustercraft))]
        [HarmonyPatch(nameof(Clustercraft.CanLandAtAsteroid))]
        public static class NoLandingForSpaceStation
        {
            public static bool Prefix(Clustercraft __instance, ref bool __result)
            {
                if (__instance is SpaceStation)
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(RocketLaunchConditionVisualizerEffect))]
        [HarmonyPatch(nameof(RocketLaunchConditionVisualizerEffect.HasClearPathToSpace))]
        public static class LaunchVisualizerWorldBounds
        {
            public static void Postfix(int cell, Vector2I worldMax, ref bool __result)
            {
                if (!__result && ClusterManager.Instance != null && SpaceStationManager.ActiveWorldIsSpaceStationInterior())
                {
                    if (!Grid.IsValidCell(cell))
                    {
                        return;
                    }

                    int cell2 = cell;
                    while ((!Grid.IsSolidCell(cell2) || Grid.Element[cell2].id == ModElements.SpaceStationForceField.SimHash || Grid.Element[cell2].id == SimHashes.Unobtanium)  && Grid.CellToXY(cell2).y < worldMax.y)
                    {
                        cell2 = Grid.CellAbove(cell2);
                    }
                    if ((!Grid.IsSolidCell(cell2) || Grid.Element[cell2].id == ModElements.SpaceStationForceField.SimHash || Grid.Element[cell2].id == SimHashes.Unobtanium) && Grid.CellToXY(cell2).y == worldMax.y)
                    {
                        __result = true;
                    }

                }
            }
        }




        /// <summary>
        /// No status items
        /// </summary>
        [HarmonyPatch(typeof(Clustercraft))]
        [HarmonyPatch(nameof(Clustercraft.UpdateStatusItem))]
        public static class NoStatusItemsForSpaceStation
        {
            public static bool Prefix(Clustercraft __instance)
            {
                if (__instance is SpaceStation)
                {
                    return false;
                }
                return true;
            }
        }
        //[HarmonyPatch(typeof(RocketSimpleInfoPanel))]
        //[HarmonyPatch(nameof(RocketSimpleInfoPanel.Refresh))]
        //public static class NoStatusItemsForSpaceStation2
        //{
        //    public static void Prefix(ref CollapsibleDetailContentPanel rocketStatusContainer,ref GameObject selectedTarget)
        //    {
        //        if (selectedTarget.TryGetComponent<SpaceStation>(out var Station))
        //        {
        //            selectedTarget = null;
        //            rocketStatusContainer.gameObject.SetActive(false);
        //            rocketStatusContainer.Commit();
        //        }
        //    }
        //}

        /// <summary>
        /// prob. not needed anymore. 
        /// removes ability to land for stations
        /// </summary>
        [HarmonyPatch(typeof(Clustercraft))]
        [HarmonyPatch(nameof(Clustercraft.CanLandAtPad))]
        public static class NoLandingForSpaceStation2
        {
            public static bool Prefix(Clustercraft __instance, ref Clustercraft.PadLandingStatus __result)
            {
                if (__instance is SpaceStation)
                {
                    __result = Clustercraft.PadLandingStatus.CanNeverLand;
                    return false;
                }
                return true;
            }
        }
        /// <summary>
        /// No Self Destruct button.. for now
        /// </summary>
        [HarmonyPatch(typeof(SelfDestructButtonSideScreen))]
        [HarmonyPatch(nameof(SelfDestructButtonSideScreen.IsValidForTarget))]
        public static class NoSpaceStationSelfDestruct
        {
            public static void Postfix(GameObject target, ref bool __result)
            {
                if (target.TryGetComponent<SpaceStation>(out var station))
                {
                    __result = false;
                }
            }
        }

        /// <summary>
        /// Bouncy Stations
        /// </summary>
        [HarmonyPatch(typeof(ClusterMapScreen))]
        [HarmonyPatch(nameof(ClusterMapScreen.FloatyAsteroidAnimation))]
        public static class Bouncing_Space_Stations
        {
            public static void Postfix(ClusterMapScreen __instance)
            {
                float num = 0.0f;
                foreach (var worldContainer in ClusterManager.Instance.WorldContainers)
                {
                    if (worldContainer.TryGetComponent<SpaceStation>(out var component))
                    {
                        if (__instance.m_gridEntityVis.ContainsKey(component))
                            __instance.m_gridEntityVis[component].GetFirstAnimController().Offset = new Vector2(0.0f, __instance.floatCycleOffset + __instance.floatCycleScale * Mathf.Sin(__instance.floatCycleSpeed * (num + GameClock.Instance.GetTime())));
                        ++num;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(RocketConduitReceiver))]
        [HarmonyPatch(nameof(RocketConduitReceiver.FindPartner))]
        public static class FixReceiverPortsInsideStationOnLoad
        {
            public static bool IsTrueRocketInterior(WorldContainer target)
            {
                return SpaceStationManager.WorldIsRocketInterior(target.id);
            }


            public static readonly MethodInfo IsTrueRocket = AccessTools.Method(
               typeof(FixReceiverPortsInsideStationOnLoad),
               ("IsTrueRocketInterior"));

            public static readonly MethodInfo IsRocketInteriorGetter = AccessTools.Method(
               typeof(WorldContainer),
               ("get_IsModuleInterior"));
            public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
            {
                var codes = orig.ToList();

                // find injection point
                var isModuleInteriorIndex = codes.FindIndex(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo f && f == IsRocketInteriorGetter);

                if (isModuleInteriorIndex == -1)
                {
                    SgtLogger.warning("IsModuleInteriorCall not found");
                    return codes;
                }
                codes[isModuleInteriorIndex] = new CodeInstruction(OpCodes.Callvirt, IsTrueRocket);

                return codes;
            }
        }

        [HarmonyPatch(typeof(RocketConduitSender))]
        [HarmonyPatch(nameof(RocketConduitSender.FindPartner))]
        public static class FixSenderPortsInsideStationOnLoad
        {
            public static bool IsTrueRocketInterior(WorldContainer target)
            {
                return SpaceStationManager.WorldIsRocketInterior(target.id);
            }


            public static readonly MethodInfo IsTrueRocket= AccessTools.Method(
               typeof(FixSenderPortsInsideStationOnLoad),
               ("IsTrueRocketInterior"));

            public static readonly MethodInfo IsRocketInteriorGetter = AccessTools.Method(
               typeof(WorldContainer),
               ("get_IsModuleInterior"));
            public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
            {
                var codes = orig.ToList();

                // find injection point
                var isModuleInteriorIndex = codes.FindIndex(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo f && f == IsRocketInteriorGetter);

                if (isModuleInteriorIndex == -1)
                {
                    SgtLogger.warning("IsModuleInteriorCall not found");
                    return codes;
                }
                codes[isModuleInteriorIndex] = new CodeInstruction(OpCodes.Callvirt, IsTrueRocket);

                return codes;
            }
        }



        [HarmonyPatch(typeof(ClusterUtil))]
        [HarmonyPatch(nameof(ClusterUtil.GetAsteroidWorldIdAtLocation))]
        public static class WorldTargetForSpaceStation
        {
            public static void Postfix(AxialI location, ref int __result)
            {
                if (__result == -1)
                {
                    foreach (ClusterGridEntity clusterGridEntity in ClusterGrid.Instance.cellContents[location])
                    {
                        if (clusterGridEntity is SpaceStation)
                        {
                            __result = (clusterGridEntity as SpaceStation).SpaceStationInteriorId;
                        }
                    }
                }
            }
        }


        [HarmonyPatch(typeof(ClusterMapScreen))]
        [HarmonyPatch(nameof(ClusterMapScreen.GetSelectorGridEntity))]
        public static class FixInteriorRailgunsAndSensor
        {
            public static bool Prefix(ClusterDestinationSelector selector, ref ClusterGridEntity __result)
            {
                if (selector.TryGetComponent<LogicClusterLocationSensor>(out var logicSensor))
                {
                    var world = logicSensor.GetMyWorld();
                    if (world != null && world.TryGetComponent<ClusterGridEntity>(out var clusterGridEntity))
                    {
                        __result = clusterGridEntity;
                        return false;
                    }
                }

                ClusterGridEntity component = selector.GetComponent<ClusterGridEntity>();
                if (component != null && ClusterGrid.Instance.IsVisible(component))
                    return component;
                ClusterGridEntity entityOfLayerAtCell = ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(selector.GetMyWorldLocation(), EntityLayer.Asteroid);
                if (entityOfLayerAtCell == null && SpaceStationManager.GetSpaceStationAtLocation(selector.GetMyWorldLocation(), out var station))
                {
                    entityOfLayerAtCell = station;
                }

                Debug.Assert(component != null || entityOfLayerAtCell != null, string.Format("{0} has no grid entity and isn't located at a visible asteroid at {1}", selector, selector.GetMyWorldLocation()));
                __result = (bool)entityOfLayerAtCell ? entityOfLayerAtCell : component;
                return false;
            }
        }

        /// <summary>
        /// Patch for Railguns inside spacestation
        /// </summary>
        [HarmonyPatch(typeof(ClusterGrid))]
        [HarmonyPatch(nameof(ClusterGrid.GetPath))]
        [HarmonyPatch(new Type[] { typeof(AxialI), typeof(AxialI), typeof(ClusterDestinationSelector), typeof(string), typeof(bool) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal })]
        public static class tarnspilerforPathSpaceStation
        {
            static ClusterGridEntity AllowSpaceStation(ClusterGridEntity original, ClusterDestinationSelector selector, AxialI target)
            {
                //SgtLogger.debuglog("All params: " + original + ", " + selector + ", " + target);
                if (original == null && selector.requireAsteroidDestination)
                {
                    var station = ClusterGrid.Instance.GetEntitiesOnCell(target).OfType<SpaceStation>();
                    if (station != null && station.Count() > 0)
                    {
                        return station.First();
                    }

                }

                return original;
            }

            private static readonly MethodInfo AllowSpaceStationMethod = AccessTools.Method(
               typeof(tarnspilerforPathSpaceStation),
               nameof(tarnspilerforPathSpaceStation.AllowSpaceStation)
            );


            private static readonly MethodInfo MethodToFind = AccessTools.Method(
                typeof(ClusterGrid),
               nameof(ClusterGrid.GetVisibleEntityOfLayerAtCell)
            );

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();
                var insertionIndex = code.FindIndex(ci => ci.opcode == OpCodes.Stloc_1);

                if (insertionIndex != -1)
                {
#if DEBUG
                            SgtLogger.debuglog("GetPathMethod found");
#endif
                    code.Insert(insertionIndex, new CodeInstruction(OpCodes.Ldarg_3));
                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Ldarg_2));
                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, AllowSpaceStationMethod));

                    //foreach (var v in code)
                    //Console.WriteLine("OPcode: " + v.opcode + ", operand: " + v.operand);
                }

                return code;
            }
        }


        [HarmonyPatch]
        public static class PatchRailgunPayloadTravel
        {
            [HarmonyPostfix]
            public static void Postfix(RailGunPayload.StatesInstance __instance, AxialI source, AxialI destination)
            {
                if (__instance.master.gameObject.TryGetComponent(out ClusterTraveler clusterTraveler) && SpaceStationManager.IsSpaceStationAt(destination) && clusterTraveler.quickTravelToAsteroidIfInOrbit == true)
                {
                    clusterTraveler.quickTravelToAsteroidIfInOrbit = false;
                    SgtLogger.l("Railgun projectile set for space station, deactivating orbit fast travel");

                }
            }
            [HarmonyTargetMethods]
            internal static IEnumerable<MethodBase> TargetMethods()
            {
                yield return typeof(RailGunPayload.StatesInstance).GetMethod("Travel");
                yield return typeof(RailGunPayload.StatesInstance).GetMethod("Launch");
            }
        }
        //[HarmonyPatch(typeof(GridSettings))]
        //[HarmonyPatch(nameof(GridSettings.Reset))]
        //public static class Size_Experiment
        //{
        //    public static void Prefix(ref int width,ref int height)
        //    {
        //        width += 500;
        //        height += 500;
        //    }
        //}
    }
}
