using HarmonyLib;
using Rockets_TinyYetBig.Docking;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Derelicts
{
    internal class DerelictPatches
    {
        public static readonly string DerelictSubPath  = "derelictInteriors";
        public static readonly string DerelictTemplateName = "_RTB_DerelictInterior";

        /// <summary>
        /// Fixes description not existing on artifact POIs, also removes the incorrect "requires drillcone" part from the description
        /// </summary>
        [HarmonyPatch(typeof(ArtifactPOIConfig), nameof(ArtifactPOIConfig.CreateArtifactPOI))]
        public static class AddDerelictInteriorToArtifactPOIs
        {
            public static void Postfix(string id,
                string anim,
                string name,
                string desc,
                HashedString poiType,
                ref GameObject __result)
            {

                var firstLineBreak = desc.IndexOf("\n");
                if (firstLineBreak != -1)
                {
                    desc = desc.Substring(0, firstLineBreak);
                }

                __result.AddOrGet<InfoDescription>().description = desc;// Strings.Get("STRINGS.UI.SPACEDESTINATIONS.ARTIFACT_POI." + spst.poiID.ToUpperInvariant() + ".DESC");

            }
        }
        [HarmonyPatch(typeof(LoreBearer), nameof(LoreBearer.OnClickRead))]
        public static class RevealDerelictOnLoreRead
        {
            public static void Postfix(LoreBearer __instance)
            {
                ClusterManager.Instance.Trigger(1943181844, (object)"lorebearer revealed");
                if(__instance.TryGetComponent<ArtifactPOIClusterGridEntity>(out var artifact))
                {
                    DerelictStation.SpawnNewDerelictStation(artifact);
                }

            }
        }
        [HarmonyPatch(typeof(ArtifactPOIClusterGridEntity), nameof(ArtifactPOIClusterGridEntity.IsVisible), MethodType.Getter)]
        public static class ArtifactPOIClusterGridEntity_ReplaceOnReveal
        {
            public static void Postfix(ArtifactPOIClusterGridEntity __instance,ref bool __result)
            {
                if(__instance.TryGetComponent<LoreBearer>(out var loreBearer))
                {
                    if(SpaceStationManager.IsSpaceStationAt(__instance.Location))
                        __result = !loreBearer.BeenClicked;
                }

            }
        }
        [HarmonyPatch(typeof(ArtifactHarvestModule.StatesInstance), nameof(ArtifactHarvestModule.StatesInstance.CheckIfCanHarvest))]
        public static class ArtifactHarvestModule_AllowHarvestInteriorPOI
        {
            public static void Postfix(ArtifactHarvestModule.StatesInstance __instance, ref bool __result)
            {
                if (__result)
                    return;

                var LocationToCheck = __instance.GetComponent<RocketModuleCluster>().CraftInterface.m_clustercraft.Location;
                
                if(SpaceStationManager.GetSpaceStationAtLocation(LocationToCheck, out var station))
                {
                    var artifact = station.GetSMI<ArtifactPOIStates.Instance>();
                    if(artifact != null && artifact.CanHarvestArtifact () && __instance.receptacle.Occupant == null)
                    {

                        __instance.sm.canHarvest.Set(true, __instance);
                        __result = true;    
                    }
                }

            }
        }
        [HarmonyPatch(typeof(ArtifactHarvestModule.StatesInstance), nameof(ArtifactHarvestModule.StatesInstance.HarvestFromPOI))]
        public static class ArtifactHarvestModule_HarvestPOIInterior
        {
            public static void Postfix(ArtifactHarvestModule.StatesInstance __instance)
            {
                if (__instance.receptacle.Occupant != null)
                    return;

                var LocationToCheck = __instance.GetComponent<RocketModuleCluster>().CraftInterface.m_clustercraft.Location;

                if (SpaceStationManager.GetSpaceStationAtLocation(LocationToCheck, out var station))
                {
                    var artifact = station.GetSMI<ArtifactPOIStates.Instance>();
                    if (artifact != null && artifact.CanHarvestArtifact() && __instance.receptacle.Occupant == null)
                    {
                        string artifactToHarvest = artifact.GetArtifactToHarvest();
                        if (artifactToHarvest == null)
                            return;
                        GameObject gameObject = Util.KInstantiate(Assets.GetPrefab((Tag)artifactToHarvest), __instance.transform.position);
                        gameObject.SetActive(true);
                        __instance.receptacle.ForceDeposit(gameObject);
                        __instance.storage.Store(gameObject);
                        artifact.HarvestArtifact();
                    }
                }

            }
        }
    }
}
