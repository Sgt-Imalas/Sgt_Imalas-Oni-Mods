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
		/// Caches whether a cluster has a geothermal pump or not to avoid repeatedly checking the same cluster.
		/// </summary>
		static Dictionary<string, bool> CachedPumpInfo = new();
		/// <summary>
		/// caches whether a cluster has an impactor shower or not to avoid repeatedly checking the same cluster.
		/// </summary>
		static Dictionary<string, bool> CachedImpactorInfo = new();
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
			return true;
		}

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
