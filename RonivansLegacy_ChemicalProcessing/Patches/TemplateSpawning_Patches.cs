using FMOD;
using HarmonyLib;
using ProcGen;
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static ProcGenGame.TemplateSpawning;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class TemplateSpawning_Patches
	{

		[HarmonyPatch(typeof(TemplateSpawning), nameof(TemplateSpawning.SpawnStoryTraitTemplates))]
		public class TemplateSpawning_SpawnStoryTraitTemplates_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled;

			///spawn extra ammonium geyser on starter world
			public static void Postfix(WorldGenSettings settings, List<TerrainCell> terrainCells, SeededRandom myRandom, ref List<TemplateSpawner> templateSpawnTargets, ref List<RectInt> placedPOIBounds, ref List<WorldTrait> placedStoryTraits, bool isRunningDebugGen, WorldGen.OfflineCallbackFunction successCallbackFn)
			{
				if (settings.worldType == WorldPlacement.LocationType.Startworld || DlcManager.IsPureVanilla())
				{
					ProcGen.World.TemplateSpawnRules rule = new ProcGen.World.TemplateSpawnRules();
					rule.listRule = ProcGen.World.TemplateSpawnRules.ListRule.TryOne;
					rule.ruleId = "Ronivan_AIO_AmmoniumGeyser";
					rule.useRelaxedFiltering = true;
					rule.allowExtremeTemperatureOverlap = true;
					rule.names = ["geysers/ronivan_aio_ammonium"];
					rule.priority = 10;
					rule.allowedCellsFilter = [new ProcGen.World.AllowedCellsFilter(){
							command = ProcGen.World.AllowedCellsFilter.Command.Replace,
							tagcommand = ProcGen.World.AllowedCellsFilter.TagCommand.NotAtTag,
							tag = "NoGlobalFeatureSpawning"}];


					List<TemplateSpawner> newTemplateSpawnTargets = [];
					HashSet<string> usedTemplates = [];
					bool ammoniaGeyserPlaced = TemplateSpawning.ApplyTemplateRule(settings, terrainCells, myRandom, ref templateSpawnTargets, ref placedPOIBounds, rule, ref usedTemplates, out string errorMessage, ref newTemplateSpawnTargets);

					if (ammoniaGeyserPlaced)
						SgtLogger.l("Successfully added an Ammonia Vent to " + Strings.Get(settings.world.name));
					else
						SgtLogger.l("Failed to add an Ammonia Vent to " + Strings.Get(settings.world.name) + ", reason: " + errorMessage);
				}
			}
		}
	}
}
