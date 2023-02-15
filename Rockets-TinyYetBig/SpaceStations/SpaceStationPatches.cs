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

namespace Rockets_TinyYetBig.SpaceStations
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
        /// TODO!!!
        /// </summary>
        [HarmonyPatch(typeof(MissionControlCluster.Instance))]
        [HarmonyPatch(nameof(MissionControlCluster.Instance.UpdateWorkableRocketsInRange))]
        public static class DisableRocketBoost
        {
            public static void Postfix(MissionControlCluster.Instance __instance, List<Clustercraft> ___boostableClustercraft)
            {
                for(int i = ___boostableClustercraft.Count-1; i>0; i--)
                {
                    if (___boostableClustercraft[i] is SpaceStation)
                    {
                        ___boostableClustercraft.RemoveAt(i);
                    }
                }
                if (___boostableClustercraft.Count == 0)
                {
                    __instance.sm.WorkableRocketsAreInRange.Set(false, __instance);
                }

            }
        }



        /// <summary>
        /// NameableStations
        /// </summary>
        [HarmonyPatch(typeof(UserNameable), "SetName")]
        public static class NameableStationsPatch
        {
            public static void Postfix(string name, UserNameable __instance)
            {
                if (__instance.GetComponent<SpaceStation>() != null) { 
                    ClusterNameDisplayScreen.Instance.UpdateName((ClusterGridEntity)__instance.GetComponent<SpaceStation>());
                    __instance.Trigger(1102426921, (object)name);
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
                UIUtils.AddClonedSideScreen<SpaceStationBuilderModuleSideScreen>("SpaceStationBuilderModuleSideScreen", "ArtableSelectionSideScreen", typeof(ArtableSelectionSideScreen));
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
                if (world.IsModuleInterior && world.TryGetComponent<SpaceStation>(out var station))
                    return false;
                return true;
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


        [HarmonyPatch(typeof(RailGunConfig))]
        [HarmonyPatch(nameof(RailGunConfig.ConfigureBuildingTemplate))]
        public static class RemoveFromSpaceStations
        {
            public static void Postfix(GameObject go)
            {
                go.GetComponent<KPrefabID>().AddTag(GameTags.NotRocketInteriorBuilding);
            }
        }


        ///From Here on: Railgun Methods that are way too crashy to be implemented to fire at space stations

        //[HarmonyPatch(typeof(ClusterDestinationSelector))]
        //[HarmonyPatch(nameof(ClusterDestinationSelector.GetDestinationWorld))]
        //public static class FixDescrCrash
        //{
        //    public static void Postfix(AxialI ___m_destination, ref int __result)
        //    {
        //        if (__result == -1)
        //        {
        //            __result = SpaceStationManager.GetSpaceStationWorldIdAtLocation(___m_destination);
        //        }
        //    }
        //}

        //        [HarmonyPatch(typeof(ClusterGrid))]
        //        [HarmonyPatch("GetPath")]
        //        [HarmonyPatch(new Type[] { typeof(AxialI), typeof(AxialI), typeof(ClusterDestinationSelector), typeof(string) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out })]
        //        public static class tarnspilerforPathSpaceStation
        //        {
        //            static ClusterGridEntity AllowSpaceStation(ClusterGridEntity original, ClusterDestinationSelector selector, AxialI target)
        //            {
        //                //SgtLogger.debuglog("All params: " + original + ", " + selector + ", " + target);
        //                if (original == null && selector.requireAsteroidDestination)
        //                {
        //                    var station = ClusterGrid.Instance.GetEntitiesOnCell(target).OfType<SpaceStation>();
        //                    if (station != null && station.Count() > 0)
        //                    {
        //                        return station.First();
        //                    }

        //                }

        //                return original;
        //            }

        //            private static readonly MethodInfo AllowSpaceStationMethod = AccessTools.Method(
        //               typeof(tarnspilerforPathSpaceStation),
        //               nameof(tarnspilerforPathSpaceStation.AllowSpaceStation)
        //            );


        //            private static readonly MethodInfo MethodToFind = AccessTools.Method(
        //                typeof(ClusterGrid),
        //               nameof(ClusterGrid.GetVisibleEntityOfLayerAtCell)
        //            );

        //            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        //            {
        //                var code = instructions.ToList();
        //                var insertionIndex = code.FindIndex(ci => ci.opcode == OpCodes.Stloc_1);

        //                if (insertionIndex != -1)
        //                {
        //#if DEBUG
        //                    SgtLogger.debuglog("GetPathMethod found");
        //#endif
        //                    code.Insert(insertionIndex, new CodeInstruction(OpCodes.Ldarg_3));
        //                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Ldarg_2));
        //                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, AllowSpaceStationMethod));

        //                    //foreach (var v in code)
        //                    //Console.WriteLine("OPcode: " + v.opcode + ", operand: " + v.operand);
        //                }

        //                return code;
        //            }
        //        }




        //[HarmonyPatch(typeof(ClusterDestinationSelector))]
        //[HarmonyPatch(nameof(ClusterDestinationSelector.SetDestination))]
        //public static class removeAssertInSetDestination
        //{
        //    public static bool Prefix(AxialI location, ClusterDestinationSelector __instance, ref AxialI ___m_destination)
        //    {
        //        if (__instance.requireAsteroidDestination)
        //            Debug.Assert(ClusterUtil.GetAsteroidWorldIdAtLocation(location) != -1 || SpaceStationManager.GetSpaceStationWorldIdAtLocation(location) != -1, (object)string.Format("Cannot SetDestination to {0} as there is no world there", (object)location));
        //        ___m_destination = location;
        //        __instance.Trigger(543433792, (object)location);
        //        return false;
        //    }
        //}

        //[HarmonyPatch(typeof(RailGunPayload.StatesInstance))]
        //[HarmonyPatch(nameof(RailGunPayload.StatesInstance.Launch))]
        //public static class PatchRailgunPayloadLaunch
        //{
        //    public static bool Prefix(AxialI source, AxialI destination, RailGunPayload.StatesInstance __instance)
        //    {
        //        __instance.GetComponent<BallisticClusterGridEntity>().Configure(source, destination);
        //        if (ClusterUtil.GetAsteroidWorldIdAtLocation(destination) != -1)
        //            __instance.sm.destinationWorld.Set(ClusterUtil.GetAsteroidWorldIdAtLocation(destination), __instance);
        //        else
        //            __instance.sm.destinationWorld.Set(SpaceStationManager.GetSpaceStationWorldIdAtLocation(destination), __instance);
        //        __instance.GoTo((StateMachine.BaseState)__instance.sm.takeoff);
        //        return false;
        //    }
        //}

        //[HarmonyPatch(typeof(RailGunPayload.StatesInstance))]
        //[HarmonyPatch(nameof(RailGunPayload.StatesInstance.Travel))]
        //public static class PatchRailgunPayloadTravel
        //{
        //    public static bool Prefix(AxialI source, AxialI destination, RailGunPayload.StatesInstance __instance)
        //    {
        //        __instance.GetComponent<BallisticClusterGridEntity>().Configure(source, destination);
        //        if (ClusterUtil.GetAsteroidWorldIdAtLocation(destination) != -1)
        //            __instance.sm.destinationWorld.Set(ClusterUtil.GetAsteroidWorldIdAtLocation(destination), __instance);
        //        else
        //            __instance.sm.destinationWorld.Set(SpaceStationManager.GetSpaceStationWorldIdAtLocation(destination), __instance);
        //        __instance.GoTo((StateMachine.BaseState)__instance.sm.travel);
        //        return false;
        //    }
        //}



        //[HarmonyPatch(typeof(TerrainBG), "LateUpdate")]
        //public static class BackgroundEvalRemoveClustercraft
        //{
        //    public static bool Prefix(TerrainBG __instance, Texture3D ___noiseVolume, Mesh ___starsPlane, int ___layer, MaterialPropertyBlock[] ___propertyBlocks, Mesh ___worldPlane, Mesh ___gasPlane)
        //    {

        //        WorldContainer world = ClusterManager.Instance.activeWorld;
        //        if (world.IsModuleInterior && world.TryGetComponent<SpaceStation>(out var station))
        //        {
        //            Material material = __instance.starsMaterial_space;
        //            material.renderQueue = RenderQueues.Stars;
        //            material.SetTexture("_NoiseVolume", (Texture)___noiseVolume);
        //            Graphics.DrawMesh(___starsPlane, new Vector3(0.0f, 0.0f, Grid.GetLayerZ(Grid.SceneLayer.Background) + 1f), Quaternion.identity, material, ___layer);
        //            __instance.backgroundMaterial.renderQueue = RenderQueues.Backwall;
        //            for (int index = 0; index < Lighting.Instance.Settings.BackgroundLayers; ++index)
        //            {
        //                if (index >= Lighting.Instance.Settings.BackgroundLayers - 1)
        //                {
        //                    float t = (float)index / (float)(Lighting.Instance.Settings.BackgroundLayers - 1);
        //                    float x = Mathf.Lerp(1f, Lighting.Instance.Settings.BackgroundDarkening, t);
        //                    float z = Mathf.Lerp(1f, Lighting.Instance.Settings.BackgroundUVScale, t);
        //                    float w = 1f;
        //                    if (index == Lighting.Instance.Settings.BackgroundLayers - 1)
        //                        w = 0.0f;
        //                    MaterialPropertyBlock propertyBlock = ___propertyBlocks[index];
        //                    propertyBlock.SetVector("_BackWallParameters", new Vector4(x, Lighting.Instance.Settings.BackgroundClip, z, w));
        //                    Graphics.DrawMesh(___worldPlane, new Vector3(0.0f, 0.0f, Grid.GetLayerZ(Grid.SceneLayer.Background)), Quaternion.identity, __instance.backgroundMaterial, ___layer, (Camera)null, 0, propertyBlock);
        //                }
        //            }
        //            __instance.gasMaterial.renderQueue = RenderQueues.Gas;
        //            Graphics.DrawMesh(___gasPlane, new Vector3(0.0f, 0.0f, Grid.GetLayerZ(Grid.SceneLayer.Gas)), Quaternion.identity, __instance.gasMaterial, ___layer);
        //            Graphics.DrawMesh(___gasPlane, new Vector3(0.0f, 0.0f, Grid.GetLayerZ(Grid.SceneLayer.GasFront)), Quaternion.identity, __instance.gasMaterial, ___layer);
        //            return false;
        //        }
        //        return true;
        //    }

        //}
    }
}
