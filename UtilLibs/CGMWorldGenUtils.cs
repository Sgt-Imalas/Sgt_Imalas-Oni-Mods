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
		public static readonly string CGM_Heatpump_StoryTrait = "CGM_GeothermalHeatPump";
		static Dictionary<string, bool> CachedPumpInfo = new();
		public static bool ShouldStoryBeInteractable(string storyId, bool hasHeatpump)
		{
			if (storyId == CGM_Heatpump_StoryTrait)
			{
				return !hasHeatpump;
			}
			return true;
		}

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
				if(HasGeothermalPump(worldData))
				{
					return true;
				}

			}
			return false;
		}
		public static bool HasGeothermalPump(ProcGen.World world)
		{
			
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
	}
}
