using Database;
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
		/// <summary>
		/// apply the fixed trait to the target asteroid of the story trait
		/// </summary>
		[HarmonyPatch(typeof(WorldContainer), nameof(WorldContainer.SetWorldDetails))]
		public class WorldContainer_SetWorldDetails_Patch
		{
			public static void Postfix(WorldContainer __instance, WorldGen world)
			{
				if (__instance.WorldTraitIds.Contains(Stories_Patches.CGM_Impactor_Path))
					__instance.largeImpactorFragmentsFixedTrait = FIXEDTRAITS.LARGEIMPACTORFRAGMENTS.NAME.ALLOWED;

			}
		}

		/// <summary>
		/// add the extra season to the target asteroid of the story trait
		/// </summary>
		[HarmonyPatch(typeof(WorldGenSettings), nameof(WorldGenSettings.ApplyStoryTrait))]
		public class WorldGenSettings_ApplyStoryTrait_Patch
		{
			public static void Postfix(WorldGenSettings __instance, WorldTrait storyTrait)
			{
				if (storyTrait.filePath == Stories_Patches.CGM_Impactor_Path)
				{
					if (__instance.worldType != WorldPlacement.LocationType.Startworld)
					{
						SgtLogger.warning("cannot place large impactor story trait on "+__instance.world.filePath + ", it is not a startworld!");
					}
					else
					{
						__instance.mutatedWorldData.world.AddSeasons(["LargeImpactor"]);
						SgtLogger.l("Adding extra season for impactor story trait to " +__instance.mutatedWorldData.world.filePath);
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
				if (rule.names.Any(rule => rule.Contains("cgm_impactor_story_trait")) && settings.worldType != WorldPlacement.LocationType.Startworld)
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
