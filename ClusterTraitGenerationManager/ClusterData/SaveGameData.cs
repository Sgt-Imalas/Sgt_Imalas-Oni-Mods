using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace ClusterTraitGenerationManager.ClusterData
{
    internal class SaveGameData : KMonoBehaviour
    {
        [SerializeField][Serialize]public List<string> CGM_ClusterTags = new();

        public static SaveGameData Instance = null;

        //internal bool TryGetGeyserOverride(GameObject placer, out string overrideID)
        //{
        //    overrideID = null;
        //    return false;
        //    var world = placer.GetMyWorld();
        //}
        public static void WriteCustomClusterTags(List<string> tags)
        {
            SgtLogger.l("writing tags, hasInstance:" + (Instance != null));
            if (Instance == null)
                return;
            Instance.CGM_ClusterTags = new(tags);
            foreach(var tag in Instance.CGM_ClusterTags)
            {
                SgtLogger.l(tag, "added tag");
            }
        }
        public override void OnSpawn()
        {
            base.OnSpawn();
            SgtLogger.l("SaveGameData.OnSpawn, " + CGM_ClusterTags.Count);
        }

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Instance = this;
            SgtLogger.l("SaveGameData.OnPrefabInit, " + CGM_ClusterTags.Count);
        }
        public override void OnCleanUp()
        {
            base.OnCleanUp();
            Instance = null;
        }

        internal bool IsCeresAsteroidInCluster(string clusterTag)
        {
            if (CGM_ClusterTags.Contains(clusterTag))
            {
                return true;
            }
            if (clusterTag == "CeresCluster" || clusterTag == "GeothermalImperative")
            {
                foreach (WorldContainer planet in ClusterManager.Instance.WorldContainers)
                {
                    if (planet.worldTags != null && planet.worldTags.Contains("Ceres"))
                    {
                        //Retroactively adding those to cgm clusters
                        SgtLogger.l("ceres asteroid found");
                        CGM_ClusterTags.Add("CeresCluster");
                        CGM_ClusterTags.Add("GeothermalImperative");
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
