using HarmonyLib;
using Rockets_TinyYetBig.Docking;
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
                if (id == "RussellsTeapot")
                    return;

                string templatename = Path.Combine(DerelictSubPath, id + DerelictTemplateName);
                if (!TemplateCache.TemplateExists(templatename))
                {
                    SgtLogger.l(templatename, "template missing");
                    return;
                }
                //if (__result.TryGetComponent<ArtifactPOIClusterGridEntity>(out var old))
                //    UnityEngine.Object.Destroy(old);


                RocketClusterDestinationSelector destinationSelector = __result.AddOrGet<RocketClusterDestinationSelector>();
                destinationSelector.assignable = false;
                destinationSelector.shouldPointTowardsPath = false;
                destinationSelector.requireAsteroidDestination = false;

                __result.AddOrGet<CraftModuleInterface>();
                var traveler = __result.AddOrGet<ClusterTraveler>();
                traveler.stopAndNotifyWhenPathChanges = false;

                __result.AddOrGetDef<AlertStateManager.Def>();

                var newEntity = __result.AddOrGet<DerelictStation>();
                newEntity.m_name = name;
                newEntity.m_Anim = anim;
                newEntity.InteriorTemplate = templatename;
                newEntity.InteriorSize = new Vector2I(19, 18);
                newEntity.poiID = name;

            }
        }
        [HarmonyPatch(typeof(LoreBearer), nameof(LoreBearer.OnClickRead))]
        public static class RevealDerelictOnLoreRead
        {
            public static void Postfix()
            {
                ClusterManager.Instance.Trigger(1943181844, (object)"lorebearer revealed");

            }
        }
    }
}
