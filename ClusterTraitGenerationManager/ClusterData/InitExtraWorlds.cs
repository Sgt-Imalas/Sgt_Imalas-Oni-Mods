using Newtonsoft.Json;
using ProcGen;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static ClusterTraitGenerationManager.ClusterData.CGSMClusterManager;
using static ProcGen.World;

namespace ClusterTraitGenerationManager.ClusterData
{

	public static class InitExtraWorlds
	{
		//ProcGenGame.WorldGen.LoadSettings_Internal()
		static bool initialized = false;

		static void CopyAllWorldTraits(ProcGen.World source, ProcGen.World target, StarmapItemCategory category)
		{
			target.worldTraitRules = new List<ProcGen.World.TraitRule>();

			Debug.Assert(source != target, "Source == Target");
			Debug.Assert(source.worldTraitRules != target.worldTraitRules, "Source.worldTraitRules == Target.worldTraitRules");
			List<ProcGen.World.TraitRule> newRules = new();

			if (source.worldTraitRules != null && source.worldTraitRules.Count > 0)
			{
				foreach (var rule in source.worldTraitRules)
				{
					var newRule = new ProcGen.World.TraitRule(rule.min, rule.max);
					newRule.requiredTags = rule.requiredTags != null ? new List<string>(rule.requiredTags) : new();
					newRule.specificTraits = rule.specificTraits != null ? new List<string>(rule.specificTraits) : new();
					newRule.forbiddenTags = rule.forbiddenTags != null ? new List<string>(rule.forbiddenTags) : new();
					newRule.forbiddenTraits = rule.forbiddenTraits != null ? new List<string>(rule.forbiddenTraits) : new();

					if (category != StarmapItemCategory.Starter)
					{
						if (newRule.forbiddenTags.Contains("StartWorldOnly"))
							newRule.forbiddenTags.Remove("StartWorldOnly");
						if (!newRule.forbiddenTags.Contains("StartChange")) //displaced pod trait
							newRule.forbiddenTags.Add("StartChange");
					}
					else
					{
						if (!newRule.forbiddenTags.Contains("StartWorldOnly"))
							newRule.forbiddenTags.Add("StartWorldOnly");
						if (newRule.forbiddenTags.Contains("StartChange"))
							newRule.forbiddenTags.Remove("StartChange");
					}
					target.worldTraitRules.Add(newRule);
				}
			}
		}

		/// <summary>
		/// Populate the dictionary with the other two variants for each asteroid
		/// </summary>
		public static void InitWorlds()
		{
			if (initialized)
				return;
			initialized = true;

			if (!DlcManager.IsExpansion1Active())
				return;

			ProcGen.Worlds __instance = SettingsCache.worlds;
			int ceresMoonletCounter = 0;

			SgtLogger.l("Initializing generation of additional planetoids, current count: " + __instance.worldCache.Count());
			Dictionary<string, ProcGen.World> toAdd = new();
			foreach (var sourceWorld in __instance.worldCache)
			{

				if ((int)sourceWorld.Value.skip >= 99 || sourceWorld.Value.moduleInterior)
					continue;


				if (CGSMClusterManager.SkipWorldForDlcReasons(sourceWorld.Key, sourceWorld.Value)
					|| CGSMClusterManager.IsWorldMixingAsteroid(sourceWorld.Key)
					|| ModAssets.Moonlets.Contains(sourceWorld.Key)) ///Moonlets already exist in all 3 configurations
				{
					continue;
				}

				string BaseName = sourceWorld.Key.Replace("Start", "").Replace("Outer", "").Replace("Warp", "");

				SgtLogger.l(sourceWorld.Key, "current planet");

				bool prohibitChallengeStarts = !Config.Instance.IncludeChallengeStarts;
				bool isChallengeWorld = sourceWorld.Key.Contains("NiobiumMoonlet") || sourceWorld.Key.Contains("RegolithMoonlet");

				if (isChallengeWorld && prohibitChallengeStarts
					//|| sourceWorld.Key.Contains("MooMoonlet")
					|| sourceWorld.Key.Contains("worlds/SandstoneDefault")
					|| PlanetByIdIsMiniBase(sourceWorld.Key.ToUpperInvariant())

					)
				{
					SgtLogger.l($"skipping {sourceWorld.Key} to avoid unlivable planets");
					continue;
				}

				//3 shattered ceres moonlets would overlap in name...
				if (BaseName.Contains("MiniShattered"))
				{
					BaseName += ceresMoonletCounter;
					ceresMoonletCounter++;

				}

				var OriginPlanetType = DeterminePlanetType(sourceWorld.Value, true);

				string starterName = BaseName + "Start";
				string warpName = BaseName + "Warp";
				string outerName = BaseName + "Outer";

				bool hasStarterAlready = __instance.worldCache.ContainsKey(starterName) || toAdd.ContainsKey(starterName);
				bool hasWarpAlready = __instance.worldCache.ContainsKey(warpName) || toAdd.ContainsKey(warpName);
				bool hasOuterAlready = __instance.worldCache.ContainsKey(outerName) || toAdd.ContainsKey(outerName);

				bool hasBaseAlready = __instance.worldCache.TryGetValue(BaseName, out var existingBase) || toAdd.TryGetValue(BaseName, out existingBase);

				if (hasBaseAlready)
				{
					var basePlanetType = DeterminePlanetType(existingBase);
					if (basePlanetType == StarmapItemCategory.Starter)
						hasStarterAlready = true;
					else if (basePlanetType == StarmapItemCategory.Outer)
						hasOuterAlready = true;
					else if (basePlanetType == StarmapItemCategory.Warp)
						hasWarpAlready = true;
				}

				List<string> additionalTemplates = new List<string>();

				if (sourceWorld.Value.startingBaseTemplate != null && sourceWorld.Value.startingBaseTemplate.Count() > 0 &&
					(sourceWorld.Value.startingBaseTemplate.Contains("sap_tree_room") || sourceWorld.Value.startingBaseTemplate.Contains("poi_satellite_3_a")))
				{
					additionalTemplates.Add(sourceWorld.Value.startingBaseTemplate);
				}

				///StartWorld
				if (OriginPlanetType != StarmapItemCategory.Starter && !hasStarterAlready)
				{
					string newWorldPath_Start = starterName;

					var StartWorld = DeepClone(sourceWorld.Value);

					if (StartWorld.worldsize.X < 100 || StartWorld.worldsize.Y < 100)
					{
						float planetSizeRatio = ((float)StartWorld.worldsize.Y) / ((float)StartWorld.worldsize.X);
						float newX, newY;
						if (planetSizeRatio > 1)
						{
							newX = 100f;
							newY = 100f * planetSizeRatio;
						}
						else
						{
							newX = 100f * (1f / planetSizeRatio);
							newY = 100f;
						}
						StartWorld.worldsize = new Vector2I(Mathf.RoundToInt(newX), Mathf.RoundToInt(newY));
					}

					if (StartWorld.subworldFiles != null && StartWorld.subworldFiles.Count > 0)
					{
						for (int i = StartWorld.subworldFiles.Count - 1; i >= 0; --i)
						{
							if (StartWorld.subworldFiles[i].name.Contains("Start"))
								StartWorld.subworldFiles.RemoveAt(i);
						}
					}
					if (StartWorld.unknownCellsAllowedSubworlds != null && StartWorld.unknownCellsAllowedSubworlds.Count > 0)
					{
						for (int i = StartWorld.unknownCellsAllowedSubworlds.Count - 1; i >= 0; --i)
						{
							if (StartWorld.unknownCellsAllowedSubworlds[i].subworldNames.Any(world => world.ToLowerInvariant().Contains("start")))
								StartWorld.unknownCellsAllowedSubworlds.RemoveAt(i);
						}
					}

					StartWorld.filePath = newWorldPath_Start;
					string oldStartSubworldName = StartWorld.startSubworldName;
					string newStartSubworldName = ModAPI.GetStartAreaSubworld(StartWorld, false);
					if (newStartSubworldName != oldStartSubworldName)
					{
						foreach (var template in StartWorld.worldTemplateRules)
						{
							if (template.allowedCellsFilter == null)
								continue;
							foreach (var filter in template.allowedCellsFilter)
							{
								if (filter.subworldNames.Contains(oldStartSubworldName))
								{
									filter.subworldNames.Remove(oldStartSubworldName);
									filter.subworldNames.Add(newStartSubworldName);
								}
							}
						}
						StartWorld.startSubworldName = newStartSubworldName;
					}
					StartWorld.startingBaseTemplate = ModAPI.GetStarterBaseTemplate(StartWorld, false);

					//Starter Biome subworld files
					var startBiome = new WeightedSubworldName(ModAPI.GetStartAreaSubworld(StartWorld, false), 1);
					startBiome.overridePower = 3;

					var startBiomeWater = new WeightedSubworldName(ModAPI.GetStartAreaWaterSubworld(StartWorld), 1);
					startBiomeWater.overridePower = 0.7f;
					startBiomeWater.minCount = 1;
					startBiomeWater.maxCount = 4;

					StartWorld.subworldFiles.Insert(0, startBiomeWater);
					StartWorld.subworldFiles.Insert(0, startBiome);

					StartWorld.seasons = new List<string>(sourceWorld.Value.seasons);

					//Starter biome placement rules

					ProcGen.World.AllowedCellsFilter CoreSandstone = new ProcGen.World.AllowedCellsFilter();
					CoreSandstone.tagcommand = ProcGen.World.AllowedCellsFilter.TagCommand.Default;
					CoreSandstone.command = ProcGen.World.AllowedCellsFilter.Command.Replace;
					CoreSandstone.subworldNames = new List<string>() { ModAPI.GetStartAreaSubworld(StartWorld, false) };

					ProcGen.World.AllowedCellsFilter MiniWater = new ProcGen.World.AllowedCellsFilter();
					MiniWater.tagcommand = ProcGen.World.AllowedCellsFilter.TagCommand.DistanceFromTag;
					MiniWater.tag = "AtStart";
					MiniWater.minDistance = 1;
					MiniWater.maxDistance = 1;
					MiniWater.command = ProcGen.World.AllowedCellsFilter.Command.Replace;
					MiniWater.subworldNames = new List<string>() { ModAPI.GetStartAreaWaterSubworld(StartWorld) };

					StartWorld.unknownCellsAllowedSubworlds.Insert(0, MiniWater);
					StartWorld.unknownCellsAllowedSubworlds.Insert(0, CoreSandstone);

					//Teleporter PlacementRules

					ProcGen.World.TemplateSpawnRules TeleporterSpawn = new ProcGen.World.TemplateSpawnRules();

					//Deleting any of the existing teleporter templates
					StartWorld.worldTemplateRules.RemoveAll((template) => ModAPI.IsATeleporterTemplate(StartWorld, template));

					TeleporterSpawn.names = new List<string>()
						{
							"expansion1::poi/warp/sender_mini",///MaterialTeleporter sender
                            "expansion1::poi/warp/receiver_mini",///MaterialTeleporter reciever
                            "expansion1::poi/warp/teleporter_mini" ///Big Dupe Teleporter Building
                        
                        };
					if (additionalTemplates.Count > 0)
						TeleporterSpawn.names.AddRange(additionalTemplates);

					TeleporterSpawn.listRule = ProcGen.World.TemplateSpawnRules.ListRule.GuaranteeAll;
					TeleporterSpawn.priority = 90;
					TeleporterSpawn.allowedCellsFilter = new List<ProcGen.World.AllowedCellsFilter>()
						{
							new ProcGen.World.AllowedCellsFilter()
							{
								command = ProcGen.World.AllowedCellsFilter.Command.Replace,
								tagcommand = ProcGen.World.AllowedCellsFilter.TagCommand.DistanceFromTag,
								tag = "AtStart",
								minDistance = 1,
								maxDistance = 2,
							},
							new ProcGen.World.AllowedCellsFilter()
							{
								command = ProcGen.World.AllowedCellsFilter.Command.ExceptWith,
								tagcommand = ProcGen.World.AllowedCellsFilter.TagCommand.DistanceFromTag,
								tag = "AtDepths",
								minDistance = 0,
								maxDistance = 0,
							},new ProcGen.World.AllowedCellsFilter()
							{
								command = ProcGen.World.AllowedCellsFilter.Command.ExceptWith,
								zoneTypes = isChallengeWorld ? [SubWorld.ZoneType.Space] :  [SubWorld.ZoneType.Space, SubWorld.ZoneType.MagmaCore]

							},
							new ProcGen.World.AllowedCellsFilter()
							{
								command = ProcGen.World.AllowedCellsFilter.Command.ExceptWith,
								tagcommand = ProcGen.World.AllowedCellsFilter.TagCommand.AtTag,
								tag = "NoGravitasFeatures"
							}
						};

					StartWorld.worldTemplateRules.Insert(0, TeleporterSpawn);

					toAdd.Add(newWorldPath_Start, StartWorld);
					ModAssets.AddModPlanetOrigin(newWorldPath_Start, sourceWorld.Key);

					SgtLogger.l(newWorldPath_Start, "Created Starter Planet Variant");

				}
				else SgtLogger.l("Skipping Starter variant for " + sourceWorld.Key + " because it already exists");
				///Warp planet variant
				if (OriginPlanetType != StarmapItemCategory.Warp && !hasWarpAlready)
				{
					string newWorldPath_Warp = warpName;
					SgtLogger.l("making warp world of " + warpName);
					var WarpWorld = DeepClone(sourceWorld.Value);
					//CopyValues(WarpWorld, sourceWorld.Value);
					if (WarpWorld.worldsize.X < 100 || WarpWorld.worldsize.Y < 100)
					{
						float planetSizeRatio = ((float)WarpWorld.worldsize.Y) / ((float)WarpWorld.worldsize.X);
						float newX, newY;
						if (planetSizeRatio > 1)
						{
							newX = 100f;
							newY = 100f * planetSizeRatio;
						}
						else
						{
							newX = 100f * (1f / planetSizeRatio);
							newY = 100f;
						}
						WarpWorld.worldsize = new Vector2I(Mathf.RoundToInt(newX), Mathf.RoundToInt(newY));
					}

					if (WarpWorld.subworldFiles != null && WarpWorld.subworldFiles.Count > 0)
					{
						for (int i = WarpWorld.subworldFiles.Count - 1; i >= 0; --i)
						{
							if (WarpWorld.subworldFiles[i].name.Contains("Start"))
							{
								WarpWorld.subworldFiles.RemoveAt(i);
							}
						}
					}
					if (WarpWorld.unknownCellsAllowedSubworlds != null && WarpWorld.unknownCellsAllowedSubworlds.Count > 0)
					{
						for (int i = WarpWorld.unknownCellsAllowedSubworlds.Count - 1; i >= 0; --i)
						{
							if (WarpWorld.unknownCellsAllowedSubworlds[i].subworldNames.Any(world => world.ToLowerInvariant().Contains("start")))
								WarpWorld.unknownCellsAllowedSubworlds.RemoveAt(i);
						}
					}

					WarpWorld.filePath = newWorldPath_Warp;
					WarpWorld.startingBaseTemplate = ModAPI.GetStarterBaseTemplate(WarpWorld, true);

					string originalStartSubWorld = WarpWorld.startSubworldName;
					WarpWorld.filePath = newWorldPath_Warp;
					WarpWorld.startingBaseTemplate = ModAPI.GetStarterBaseTemplate(WarpWorld, true);
					string oldStartSubworldName = WarpWorld.startSubworldName;
					string newStartSubworldName = ModAPI.GetStartAreaSubworld(WarpWorld, true);
					if (newStartSubworldName != oldStartSubworldName)
					{
						foreach (var template in WarpWorld.worldTemplateRules)
						{
							if (template.allowedCellsFilter == null)
								continue;
							foreach (var filter in template.allowedCellsFilter)
							{
								if (filter.subworldNames == null)
									continue;

								if (filter.subworldNames.Contains(oldStartSubworldName))
								{
									filter.subworldNames.Remove(oldStartSubworldName);
									filter.subworldNames.Add(newStartSubworldName);
								}
							}
						}
						WarpWorld.startSubworldName = newStartSubworldName;
					}

					//Starter Biome subworld files
					var startBiome = new WeightedSubworldName(ModAPI.GetStartAreaSubworld(WarpWorld, true), 1);
					startBiome.overridePower = 3;

					WarpWorld.subworldFiles.Insert(0, startBiome);

					//Starter biome placement rules
					ProcGen.World.AllowedCellsFilter CoreBiome = new ProcGen.World.AllowedCellsFilter();
					CoreBiome.tagcommand = ProcGen.World.AllowedCellsFilter.TagCommand.Default;
					CoreBiome.command = ProcGen.World.AllowedCellsFilter.Command.Replace;
					CoreBiome.subworldNames = new List<string>() { ModAPI.GetStartAreaSubworld(WarpWorld, true) };

					WarpWorld.unknownCellsAllowedSubworlds.Insert(0, CoreBiome);


					//Teleporter PlacementRules

					//Deleting any of the existing teleporter templates

					WarpWorld.worldTemplateRules.RemoveAll((template) => ModAPI.IsATeleporterTemplate(WarpWorld, template));

					//Deleting researchPortals 
					WarpWorld.worldTemplateRules.RemoveAll(ModAPI.IsResearchPortalTemplate);

					ProcGen.World.TemplateSpawnRules TeleporterSpawn = new ProcGen.World.TemplateSpawnRules();
					TeleporterSpawn.names = new List<string>()
						{
							"expansion1::poi/warp/sender_mini", ///MaterialTeleporter sender
                            "expansion1::poi/warp/receiver_mini" ///MaterialTeleporter reciever 
                        };

					if (additionalTemplates.Count > 0)
						TeleporterSpawn.names.AddRange(additionalTemplates);

					TeleporterSpawn.listRule = ProcGen.World.TemplateSpawnRules.ListRule.GuaranteeAll;
					TeleporterSpawn.priority = 90;
					TeleporterSpawn.allowedCellsFilter = new List<ProcGen.World.AllowedCellsFilter>()
						{
							new ProcGen.World.AllowedCellsFilter()
							{
								command = ProcGen.World.AllowedCellsFilter.Command.Replace,
								tagcommand = ProcGen.World.AllowedCellsFilter.TagCommand.DistanceFromTag,
								tag = "AtStart",
								minDistance = 1,
								maxDistance = 2,
							},
							new ProcGen.World.AllowedCellsFilter()
							{
								command = ProcGen.World.AllowedCellsFilter.Command.ExceptWith,
								tagcommand = ProcGen.World.AllowedCellsFilter.TagCommand.DistanceFromTag,
								tag = "AtDepths",
								minDistance = 0,
								maxDistance = 0,
							},new ProcGen.World.AllowedCellsFilter()
							{
								command = ProcGen.World.AllowedCellsFilter.Command.ExceptWith,
								zoneTypes = isChallengeWorld ? [SubWorld.ZoneType.Space] :  [SubWorld.ZoneType.Space, SubWorld.ZoneType.MagmaCore]

							},
							new ProcGen.World.AllowedCellsFilter()
							{
								command = ProcGen.World.AllowedCellsFilter.Command.ExceptWith,
								tagcommand = ProcGen.World.AllowedCellsFilter.TagCommand.AtTag,
								tag = "NoGravitasFeatures"
							}
						};

					WarpWorld.worldTemplateRules.Insert(0, TeleporterSpawn);

					PostProcessDemolior(WarpWorld);

					toAdd.Add(newWorldPath_Warp, WarpWorld);
					ModAssets.AddModPlanetOrigin(newWorldPath_Warp, sourceWorld.Key);

					SgtLogger.l(newWorldPath_Warp, "Created Warp Planet Variant");

				}
				else SgtLogger.l("Skipping Warp variant for " + sourceWorld.Key + " because it already exists");

				if (OriginPlanetType != StarmapItemCategory.Outer && !hasOuterAlready)
				{
					string newWorldPath_Outer = outerName;

					var OuterWorld = DeepClone(sourceWorld.Value);

					//CopyValues(OuterWorld, sourceWorld.Value);	
					OuterWorld.filePath = newWorldPath_Outer;
					OuterWorld.startingBaseTemplate = null;
					//StartWorld.startSubworldName = string.Empty;
					

					//Deleting researchPortals 
					OuterWorld.worldTemplateRules.RemoveAll(ModAPI.IsResearchPortalTemplate);
					//StartWorld.unknownCellsAllowedSubworlds.RemoveAll(cellsfilter => cellsfilter.tag == "AtStart");
					//StartWorld.subworldFiles.RemoveAll(cellsfilter => cellsfilter.name.Contains("Start"));
					OuterWorld.worldTemplateRules.RemoveAll((template) => ModAPI.IsATeleporterTemplate(OuterWorld, template));

					//StartWorld.worldTemplateRules.ForEach(TemplateRule =>
					//{
					//    if (TemplateRule.listRule == ProcGen.World.TemplateSpawnRules.ListRule.GuaranteeAll)
					//        TemplateRule.listRule = ProcGen.World.TemplateSpawnRules.ListRule.GuaranteeSomeTryMore;
					//}
					//);

					//StartWorld.worldTemplateRules.RemoveAll(cellsfilter => cellsfilter.allowedCellsFilter.Any(item=> item.tag=="AtStart"));

					PostProcessDemolior(OuterWorld);
					toAdd.Add(newWorldPath_Outer, OuterWorld);
					ModAssets.AddModPlanetOrigin(newWorldPath_Outer, sourceWorld.Key);

					SgtLogger.l(newWorldPath_Outer, "Created Outer Planet Variant");
				}
				else SgtLogger.l("Skipping Outer variant for " + sourceWorld.Key + " because it already exists");
			}
			foreach (var worldMixing in SettingsCache.worldMixingSettings.Values)
			{
				var asteroidId = worldMixing.world;
				if (!__instance.worldCache.TryGetValue(asteroidId, out var mixingWorld))
				{
					SgtLogger.warning("could not find mixing asteroid " + asteroidId + " for dynamic outer version generation");
					continue;
				}
				string newWorldPath_Outer = $"CGM_Dynamic_{asteroidId}_Outer";
				ModAssets.OldStandaloneFragmentRedirects[asteroidId] = newWorldPath_Outer;

				var OuterMixingWorld = new ProcGen.World();

				OuterMixingWorld = DeepClone(mixingWorld);
				//CopyValues(OuterMixingWorld, mixingWorld);
				OuterMixingWorld.filePath = newWorldPath_Outer;
				OuterMixingWorld.startingBaseTemplate = null;
				//StartWorld.startSubworldName = string.Empty;

				CopyAllWorldTraits(mixingWorld, OuterMixingWorld, StarmapItemCategory.Outer);
				OuterMixingWorld.worldTemplateRules.RemoveAll((template) => ModAPI.IsATeleporterTemplate(OuterMixingWorld, template));
				PostProcessDemolior(OuterMixingWorld);
				toAdd.Add(newWorldPath_Outer, OuterMixingWorld);
				ModAssets.AddModPlanetOrigin(newWorldPath_Outer, asteroidId);
			}

			foreach (var item in toAdd)
			{
				if (!__instance.worldCache.ContainsKey(item.Key))
				{
					SgtLogger.l("Adding " + item.Key + " to world cache");

					item.Value.isModded = true;
					__instance.worldCache.Add(item.Key, item.Value);
				}
				else
				{
					SgtLogger.warning("" + item.Key + " already existed in world cache");
				}
			}
		}

		private static void PostProcessDemolior(ProcGen.World world)
		{
			if (world.seasons.Contains("LargeImpactor"))
			{
				//Remove the Demolior from the world, it breaks on non-start asteroids
				world.seasons.Remove("LargeImpactor");
				SgtLogger.l("PostProcessing " + world.filePath + " to remove Demolior, since it does not work on non-starters");
			}
		}
		public static T DeepClone<T>(this T obj)
		{
			using (var ms = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(ms, obj);
				ms.Position = 0;

				return (T)formatter.Deserialize(ms);
			}
		}
	}
}
