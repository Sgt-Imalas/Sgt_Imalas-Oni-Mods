using Database;
using HarmonyLib;
using KSerialization;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilLibs;

namespace ClusterTraitGenerationManager.ClusterData
{
	internal class SaveGameData : KMonoBehaviour
	{
		[SerializeField][Serialize] public List<string> CGM_ClusterTags = new();

		public static SaveGameData Instance = null;

		[SerializeField][Serialize] private bool _isCustomCluster = false;

		//internal bool TryGetGeyserOverride(GameObject placer, out string overrideID)
		//{
		//    overrideID = null;
		//    return false;
		//    var world = placer.GetMyWorld();
		//}
		public static void SetCustomCluster(bool isCustomCluster = true)
		{
			if (Instance != null)
			{
				Instance._isCustomCluster = isCustomCluster;
			}
		}
		public static bool IsCustomCluster()
		{
			if (Instance != null)
				return Instance._isCustomCluster;
			return Game.clusterId == CGSMClusterManager.CustomClusterID;
		}





		public static void WriteCustomClusterTags(List<string> tags)
		{
			SgtLogger.l("writing tags, hasInstance:" + (Instance != null));
			if (Instance == null)
				return;
			Instance.CGM_ClusterTags = new(tags);
			foreach (var tag in Instance.CGM_ClusterTags)
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

		internal bool IsClusterTagAsteroidInCluster(string clusterTag)
		{
			if (CGM_ClusterTags.Contains(clusterTag))
			{
				return true;
			}
			string worldTag = clusterTag.Replace("Cluster", "");
			if (ClusterManager.Instance.WorldContainers.Any(world => world.worldTags != null && world.worldTags.Contains(worldTag)))
			{
				SgtLogger.l("Assigning missing " + clusterTag + " to CGM custom cluster");
				CGM_ClusterTags.Add(clusterTag);
				return true;
			}

			if (clusterTag == "DemoliorImperative" && CGM_ClusterTags.Contains("DemoliorImminentImpact")
				|| clusterTag == "DemoliorImminentImpact" && CGM_ClusterTags.Contains("DemoliorImperative"))
				return false;

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
			if (clusterTag == "PrehistoricCluster" || clusterTag == "DemoliorImperative")
			{
				foreach (WorldContainer planet in ClusterManager.Instance.WorldContainers)
				{
					if (CGMWorldGenUtils.HasImpactorShower(planet.GetSeasonIds()))
					{
						//Retroactively adding those to cgm clusters
						SgtLogger.l("prehistoric asteroid found");
						CGM_ClusterTags.Add("PrehistoricCluster");
						CGM_ClusterTags.Add("DemoliorImperative");
						return true;
					}
				}
			}
			return false;
		}
	}
}
