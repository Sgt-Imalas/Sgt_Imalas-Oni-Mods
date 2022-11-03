using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
                    //Debug.Log(def.PrefabID + " - Is SpaceStationBuilding; state: " + __result);
                    
                    if (
                        def.BuildingComplete.HasTag(GameTags.NotRocketInteriorBuilding) && def.BuildingComplete.HasTag(ModAssets.Tags.SpaceStationOnlyInteriorBuilding))
                    {
                        if (!DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive && !ProductInfoScreen.MaterialsMet(def.CraftRecipe))
                            __result = PlanScreen.RequirementsState.Materials;
                        else
                            __result = PlanScreen.RequirementsState.Complete;
                    }
                }
                if (def.BuildingComplete.HasTag(ModAssets.Tags.SpaceStationOnlyInteriorBuilding) && SpaceStationManager.ActiveWorldIsRocketInterior())
                {
                    __result = PlanScreen.RequirementsState.RocketInteriorForbidden;
                }
            }
        }


        [HarmonyPatch(typeof(BaseModularLaunchpadPortConfig), "ConfigureBuildingTemplate")]
        public static class AllowPortLoadersInSpaceStation
        {
            public static void Postfix(GameObject go)
            {
                KPrefabID component = go.GetComponent<KPrefabID>();
                component.AddTag(ModAssets.Tags.SpaceStationOnlyInteriorBuilding);
            }
        }






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
                if(__instance is SpaceStation)
                    return false;
                return true;
            }

        }
        [HarmonyPatch(typeof(Clustercraft))]
        [HarmonyPatch(nameof(Clustercraft.TotalBurden))]
        [HarmonyPatch(MethodType.Getter)]
        public static class NoBurdenForSpaceStation
        {
            public static bool Prefix(Clustercraft __instance, float __result)
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
            public static bool Prefix(Clustercraft __instance, bool __result)
            {
                if (__instance is SpaceStation)
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }

        //[HarmonyPatch(typeof(FloatingRocketDiagnostic), "Evaluate")]
        //public static class DisableRocketEvaluationOnSpaceStation
        //{
        //    public static bool Prefix(FloatingRocketDiagnostic __instance)
        //    {

        //        WorldContainer world = ClusterManager.Instance.GetWorld(__instance.worldID);
        //        if (world.IsModuleInterior && world.TryGetComponent<SpaceStation>(out var station))
        //            return false;
        //        return true;
        //    }

        //}

        //[HarmonyPatch(typeof(RocketFuelDiagnostic), "Evaluate")]
        //public static class DisableRocketFuelEvaluationOnSpaceStation
        //{
        //    public static bool Prefix(FloatingRocketDiagnostic __instance)
        //    {
        //        WorldContainer world = ClusterManager.Instance.GetWorld(__instance.worldID);
        //        if (world.IsModuleInterior && world.TryGetComponent<SpaceStation>(out var station))
        //            return false;
        //        return true;
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
