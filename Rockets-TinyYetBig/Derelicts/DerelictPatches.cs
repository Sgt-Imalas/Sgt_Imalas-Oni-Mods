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
        public static readonly string DerelictSubPath = "derelictInteriors";
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
                if (__instance.TryGetComponent<ArtifactPOIClusterGridEntity>(out var artifact))
                {
                    DerelictStation.SpawnNewDerelictStation(artifact);
                }

            }
        }
        [HarmonyPatch(typeof(Clustercraft), nameof(Clustercraft.GetPOIAtCurrentLocation))]
        public static class Clustercraft_GetPOIAtCurrentLocation_Redirect_DerelictStation
        {
            public static void Postfix(ref ClusterGridEntity __result)
            {
                if (__result == null) return;

                if (__result.TryGetComponent<DerelictStation>(out var derelictStation))
                {
                    foreach (var poi in ClusterGrid.Instance.GetEntitiesOfLayerAtCell(derelictStation.Location, EntityLayer.POI))
                    {
                        if (poi.TryGetComponent<ArtifactPOIConfigurator>(out _))
                        {
                            __result = poi;
                            return;
                        }
                    }
                }
            }
        }
        [HarmonyPatch(typeof(SpacePOISimpleInfoPanel), nameof(SpacePOISimpleInfoPanel.Refresh))]
        public static class SpacePOISimpleInfoPanel_Redirect_DerelictStation
        {
            public static void Prefix(ref GameObject selectedTarget)
            {
                if(selectedTarget == null) return;

                if (selectedTarget.TryGetComponent<DerelictStation>(out var derelictStation))
                {
                    foreach (var poi in ClusterGrid.Instance.GetEntitiesOfLayerAtCell(derelictStation.Location, EntityLayer.POI))
                    {
                        if(poi.TryGetComponent<ArtifactPOIConfigurator>(out _))
                        {
                            selectedTarget = poi.gameObject;
                            return;
                        }
                    }
                }
            }
        }
        [HarmonyPatch(typeof(ArtifactPOIClusterGridEntity), nameof(ArtifactPOIClusterGridEntity.IsVisible), MethodType.Getter)]
        public static class ArtifactPOIClusterGridEntity_ReplaceOnReveal
        {
            public static void Postfix(ArtifactPOIClusterGridEntity __instance, ref bool __result)
            {
                if (__instance.TryGetComponent<LoreBearer>(out var loreBearer))
                {
                    if (SpaceStationManager.IsSpaceStationAt(__instance.Location))
                        __result = !loreBearer.BeenClicked;
                }

            }
        }
    }
}
