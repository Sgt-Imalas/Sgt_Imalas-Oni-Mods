using Database;
using FMOD.Studio;
using HarmonyLib;
using ProcGen;
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using static STRINGS.UI.SPACEDESTINATIONS.CLUSTERMAPMETEORS;

namespace DemoliorStoryTrait.Patches
{
	class MutatedWorldData_Patches
	{
		static bool WorldTypeIsStartWorld(WorldPlacement.LocationType type) => DlcManager.IsPureVanilla() || type == WorldPlacement.LocationType.Startworld;

		/// <summary>
		/// add the extra impactor data to the target asteroid of the story trait
		/// </summary>
		[HarmonyPatch(typeof(WorldGenSettings), nameof(WorldGenSettings.ApplyStoryTrait))]
		public class WorldGenSettings_ApplyStoryTrait_Patch
		{
			public static void Postfix(WorldGenSettings __instance, WorldTrait storyTrait)
			{
				SgtLogger.l("applying story trait: " + storyTrait.filePath);
				if (storyTrait.filePath == Stories_Patches.CGM_Impactor_Path)
				{
					if (WorldTypeIsStartWorld(__instance.worldType))
					{
						__instance.world.AddSeasons(["LargeImpactor"]);
						SgtLogger.l("Adding extra season for impactor story trait to " + __instance.mutatedWorldData.world.filePath);
					}
					else
					{
						SgtLogger.warning("cannot place large impactor story trait on " + __instance.world.filePath + ", it is not a startworld!");
					}
				}
			}

		}
		/// <summary>
		/// Prevent the story trait from landing on other asteroids that arent the startworld
		/// </summary>
		[HarmonyPatch(typeof(TemplateSpawning), nameof(TemplateSpawning.ApplyTemplateRule))]
		public class TemplateSpawning_ApplyTemplateRule_Patch
		{
			public static bool Prefix(WorldGenSettings settings,
				ProcGen.World.TemplateSpawnRules rule,
				ref HashSet<string> usedTemplates,
				ref bool __result)
			{
				bool isStartWorld = WorldTypeIsStartWorld(settings.worldType);

				if (rule.names.Any(rule => rule.Contains("cgm_impactor_story_trait")) && !isStartWorld)
				{
					SgtLogger.warning(settings.world.filePath + " was not a startworld!");
					__result = false;
					return false;
				}

				return true;
			}
		}
	}
}
