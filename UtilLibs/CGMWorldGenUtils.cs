using Database;
using ProcGen;
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilLibs
{
	public class CGMWorldGenUtils
	{
		/// <summary>
		/// Prehistoric Planet giant asteroid as a story trait
		/// </summary>
		public static readonly string CGM_Impactor_StoryTrait = "CGM_ImpactorStoryTrait";
		/// <summary>
		/// Frosty Planet Heatpump Story Trait
		/// </summary>
		public static readonly string CGM_Heatpump_StoryTrait = "CGM_GeothermalHeatPump";
		/// <summary>
		/// Aquatic Planet Minnow Story Trait
		/// </summary>
		public static readonly string CGM_Minnow_StoryTrait = "CGM_Minnow";

		/// <summary>
		/// Caches whether a cluster has a geothermal pump or not to avoid repeatedly checking the same cluster.
		/// </summary>
		static Dictionary<string, bool> CachedPumpInfo = new();
		/// <summary>
		/// caches whether a cluster has an impactor shower or not to avoid repeatedly checking the same cluster.
		/// </summary>
		static Dictionary<string, bool> CachedImpactorInfo = new();
		/// <summary>
		/// caches whether a cluster has minnow pois or not to avoid repeatedly checking the same cluster.
		/// </summary>
		static Dictionary<string, bool> CachedMinnowInfo = new();
		public static bool ShouldStoryBeInteractable(string storyId, List<WorldPlacement> worlds)
		{
			if (storyId == CGM_Heatpump_StoryTrait)
			{
				return !HasGeothermalPumpInCluster(worlds);
			}
			else if (storyId == CGM_Impactor_StoryTrait)
			{
				return !HasImpactorShowerInCluster(worlds);
			}
			else if (storyId == CGM_Minnow_StoryTrait)
			{
				return !HasMinnowInCluster(worlds);
			}
			return true;
		}

		public static bool IsImpactorTrait(string storyId) => storyId == CGM_Impactor_StoryTrait;

		#region DLC5
		public static bool HasMinnowOnWorld(ProcGen.World world)
		{
			if (world == null)
				return false;
			foreach (var rule in world.worldTemplateRules)
			{
				if (rule.names == null || !rule.names.Any())
					continue;

				if (rule.names.Contains("dlc5::poi/imperative/minnowPOI_A")
				|| rule.names.Contains("dlc5::poi/imperative/minnowPOI_B")
				|| rule.names.Contains("dlc5::poi/imperative/minnowPOI_C")
				|| rule.names.Contains("dlc5::poi/imperative/minnowPOI_C_small")
					)

				{
					//SgtLogger.l("world " + world.name + " has geothermal pump");
					return true;
				}
			}
			//SgtLogger.l("world " + world.name + " has no geothermal pump!");
			return false;
		}
		public static bool HasMinnowOnWorld(List<string> worldTags) => worldTags != null && worldTags.Contains("Aquatic");
		public static bool HasMinnowOnWorld(Tag[] worldTags) => worldTags != null && worldTags.Contains("Aquatic");

		public static bool HasMinnowInCluster(List<WorldPlacement> worldPlacements)
		{
			foreach (WorldPlacement placement in worldPlacements)
			{
				var world = placement.world;
				var worldData = SettingsCache.worlds.GetWorldData(world);
				if (worldData == null)
				{
					SgtLogger.warning("world " + world + " not found in world layouts");
					continue;
				}
				if (HasMinnowOnWorld(worldData))
				{
					return true;
				}
			}
			return false;
		}
		public static bool HasMinnowInCluster(string clusterID)
		{
			if(CachedMinnowInfo.TryGetValue(clusterID, out bool hasMinnow))
			{
				return hasMinnow;
			}

			var cluster = SettingsCache.clusterLayouts.GetClusterData(clusterID);
			if (cluster == null)
			{
				SgtLogger.warning("cluster " + clusterID + " not found in cluster layouts");
				return false;
			}
			hasMinnow = HasMinnowInCluster(cluster.worldPlacements);
			SgtLogger.l("cluster " + clusterID + " has minnow: " + hasMinnow);
			CachedMinnowInfo[clusterID] = hasMinnow;
			return hasMinnow;
		}

		#endregion

		#region DLC4
		public static bool HasImpactorShower(ProcGen.World world) => world != null && world.seasons != null && HasImpactorShower(world.seasons);
		public static bool HasImpactorShower(List<string> seasons) =>  seasons.Contains("LargeImpactor");

		public static bool HasImpactorShowerInCluster(List<WorldPlacement> worldPlacements)
		{
			foreach (WorldPlacement placement in worldPlacements)
			{
				var world = placement.world;
				var worldData = SettingsCache.worlds.GetWorldData(world);
				if (worldData == null)
				{
					SgtLogger.warning("world " + world + " not found in world layouts");
					continue;
				}
				if (HasImpactorShower(worldData))
				{
					return true;
				}
			}
			return false;
		}

		public static bool HasImpactorShowerInCluster(string clusterID)
		{
			if (CachedImpactorInfo.TryGetValue(clusterID, out bool hasImpactor))
			{
				return hasImpactor;
			}

			var cluster = SettingsCache.clusterLayouts.GetClusterData(clusterID);
			if (cluster == null)
			{
				SgtLogger.warning("cluster " + clusterID + " not found in cluster layouts");
				return false;
			}
			hasImpactor = HasImpactorShowerInCluster(cluster.worldPlacements);
			SgtLogger.l("cluster " + clusterID + " has largeimpactor shower: " + hasImpactor);
			CachedImpactorInfo[clusterID] = hasImpactor;
			return hasImpactor;
		}
		#endregion
		#region DLC2
		public static bool HasGeothermalPumpInCluster(List<WorldPlacement> worldPlacements)
		{
			foreach (WorldPlacement placement in worldPlacements)
			{
				var world = placement.world;
				var worldData = SettingsCache.worlds.GetWorldData(world);
				if (worldData == null)
				{
					SgtLogger.warning("world " + world + " not found in world layouts");
					continue;
				}
				if (HasGeothermalPump(worldData))
				{
					return true;
				}

			}
			return false;
		}
		public static bool HasGeothermalPump(ProcGen.World world)
		{
			if (world == null)
				return false; 
			foreach (var rule in world.worldTemplateRules)
			{
				if (rule.names == null || !rule.names.Any())
					continue;

				if (rule.names.Contains("dlc2::poi/geothermal/geothermal_controller")
					|| rule.names.Contains("dlc2::poi/geothermal/shattered_geothermal_controller")
					)

				{
					//SgtLogger.l("world " + world.name + " has geothermal pump");
					return true;
				}
			}
			//SgtLogger.l("world " + world.name + " has no geothermal pump!");
			return false;
		}

		public static bool HasGeothermalPumpInCluster(string clusterID)
		{
			if (CachedPumpInfo.TryGetValue(clusterID, out bool hasPump))
			{
				return hasPump;
			}

			var cluster = SettingsCache.clusterLayouts.GetClusterData(clusterID);
			if (cluster == null)
			{
				SgtLogger.warning("cluster " + clusterID + " not found in cluster layouts");
				return false;
			}
			bool hasGeothermalPump = HasGeothermalPumpInCluster(cluster.worldPlacements);
			SgtLogger.l("cluster " + clusterID + " has geothermal pump: " + hasGeothermalPump);
			CachedPumpInfo[clusterID] = hasGeothermalPump;
			return hasGeothermalPump;
		}
		#endregion
	}
}
