using Database;
using FMOD.Studio;
using HarmonyLib;
using ProcGen;
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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
		/// add the extra impactor data to the target asteroid of the story trait
		/// </summary>
		[HarmonyPatch(typeof(TemplateSpawning), nameof(TemplateSpawning.SpawnStoryTraitTemplates))]
		public class TemplateSpawning_SpawnStoryTraitTemplates_Patch
		{
			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{
				var codes = orig.ToList();

				MethodInfo toInject = AccessTools.Method(typeof(TemplateSpawning_SpawnStoryTraitTemplates_Patch), nameof(InjectImpactorSeason));

				string logString = "Applied story trait '";
				var fieldIndex = codes.FindIndex(ci => ci.opcode == OpCodes.Ldstr && ci.operand?.ToString() == logString);
				if(fieldIndex <0)
				{
					SgtLogger.transpilerfail("TemplateSpawning.SpawnStoryTraitTemplates fieldIndex");
				}

				var localTraitIndex = TranspilerHelper.FindIndexOfNextLocalIndex(codes, fieldIndex, false);
				if (localTraitIndex < 0)
				{
					SgtLogger.transpilerfail("TemplateSpawning.SpawnStoryTraitTemplates localTraitIndex");
				}

				codes.InsertRange(fieldIndex,
					[
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Ldloc_S,localTraitIndex),
					new CodeInstruction(OpCodes.Call,toInject)

					]);

				//TranspilerHelper.PrintInstructions(codes);
				return codes;
			}

			public static void InjectImpactorSeason(WorldGenSettings settings, WorldTrait storyTrait)
			{
				//SgtLogger.l("applying story trait: " + storyTrait.filePath);
				if (storyTrait.filePath == Stories_Patches.CGM_Impactor_Path)
				{
					if (WorldTypeIsStartWorld(settings.worldType))
					{
						settings.mutatedWorldData.world.AddSeasons(["LargeImpactor"]);
						SgtLogger.l("Adding extra season for impactor story trait to " + settings.mutatedWorldData.world.filePath);
					}
					else
					{
						SgtLogger.warning("cannot place large impactor story trait on " + settings.world.filePath + ", it is not a startworld!");
					}
				}
			}
		}
		static bool WorldTypeIsStartWorld(WorldPlacement.LocationType type) => DlcManager.IsPureVanilla() || type == WorldPlacement.LocationType.Startworld;

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
				if (rule.names == null)
					return true;

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
