using ClusterTraitGenerationManager.ClusterData;
using HarmonyLib;
using Klei.CustomSettings;
using ProcGen;
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static ClusterTraitGenerationManager.ClusterData.CGSMClusterManager;
using static ClusterTraitGenerationManager.STRINGS.UI;

namespace ClusterTraitGenerationManager
{
	internal class Patches
	{
		/// <summary>
		/// These patches have to run manually or they break translations on certain screens
		/// </summary>
		[HarmonyPatch(typeof(Assets), nameof(Assets.OnPrefabInit))]
		public static class OnASsetPrefabPatch
		{
			public static void Postfix()
			{
				LayoutNameFix.AssetOnPrefabInitPostfix(Mod.harmonyInstance);
				ColonyDestinationSelectScreen_ShuffleClicked_Patch.AssetOnPrefabInitPostfix(Mod.harmonyInstance);
				ColonyDestinationSelectScreen_CoordinateChanged_Patch.AssetOnPrefabInitPostfix(Mod.harmonyInstance);
			}
		}
		//[HarmonyPatch(typeof(SaveGame), "GetColonyToolTip")]
		public static class LayoutNameFix
		{
			public static void AssetOnPrefabInitPostfix(Harmony harmony)
			{
				var m_TargetMethod = AccessTools.Method("SaveGame, Assembly-CSharp:GetColonyToolTip");
				var m_Postfix = AccessTools.Method(typeof(LayoutNameFix), "Postfix");

				harmony.Patch(m_TargetMethod, null, new HarmonyMethod(m_Postfix), null);
			}

			public static void Postfix(ref List<Tuple<string, TextStyleSetting>> __result, SaveGame __instance)
			{
				if (Game.clusterId == CustomClusterID || SaveGameData.IsCustomCluster())
				{
					var array = __result.ToArray();
					array[1] = new Tuple<string, TextStyleSetting>((string)STRINGS.CLUSTER_NAMES.CGM.NAME, ToolTipScreen.Instance.defaultTooltipBodyStyle);
					__result = array.ToList();
				}
			}
		}
		/// <summary>
		/// Custom cluster in load menu
		/// </summary>
		[HarmonyPatch(typeof(LoadScreen), nameof(LoadScreen.ShowColonySave))]
		public static class LoadScreen_NameFix
		{
			public static void Prefix() => RemoveFromCache();

			public static void Postfix(LoadScreen.SaveGameFileDetails save, LoadScreen __instance)
			{
				if (save.FileInfo.clusterId == CustomClusterID)
				{
					HierarchyReferences component1 = __instance.colonyViewRoot.GetComponent<HierarchyReferences>();
					component1.GetReference<LocText>("InfoWorld").text = string.Format((string)global::STRINGS.UI.FRONTEND.LOADSCREEN.COLONY_INFO_FMT, (object)global::STRINGS.UI.FRONTEND.LOADSCREEN.WORLD_NAME, STRINGS.CLUSTER_NAMES.CGM.NAME);
				}
			}
		}



		/// <summary>
		/// Init. auto translation
		/// </summary>
		[HarmonyPatch(typeof(Localization), "Initialize")]
		public static class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				LocalisationUtil.Translate(typeof(STRINGS), true);
			}
		}

		/// <summary>
		/// CGM in the copied seed in the pause screen for custom clusters
		/// </summary>
		[HarmonyPatch(typeof(PauseScreen), "OnSpawn")]
		public static class PauseScreen_OnSpawn_Patch
		{
			public static void Postfix(PauseScreen __instance)
			{
				if (Game.clusterId == CustomClusterID || SaveGameData.IsCustomCluster())
				{
					var inst = CustomGameSettings.Instance;
					SettingLevel currentQualitySetting2 = inst.GetCurrentQualitySetting(CustomGameSettingConfigs.WorldgenSeed);
					string otherSettingsCode = inst.GetOtherSettingsCode();
					string storyTraitSettingsCode = inst.GetStoryTraitSettingsCode();
					string mixingSettingsCode = inst.GetMixingSettingsCode();
					string settingsCoordinate = $"{CustomClusterIDCoordinate}-{currentQualitySetting2.id}-{otherSettingsCode}-{storyTraitSettingsCode}-{mixingSettingsCode}";
					string[] settingCoordinate = CustomGameSettings.ParseSettingCoordinate(settingsCoordinate);

					__instance.worldSeed.SetText(string.Format((string)global::STRINGS.UI.FRONTEND.PAUSE_SCREEN.WORLD_SEED, (object)settingsCoordinate));
					__instance.worldSeed.GetComponent<ToolTip>().toolTip = string.Format((string)global::STRINGS.UI.FRONTEND.PAUSE_SCREEN.WORLD_SEED_TOOLTIP, CustomClusterIDCoordinate, settingCoordinate[2], (object)settingCoordinate[3], (object)settingCoordinate[4], settingCoordinate[5]);

				}
			}
		}
		public class SaveGamePatch
		{
			[HarmonyPatch(typeof(SaveGame), "OnPrefabInit")]
			public class SaveGame_OnPrefabInit_Patch
			{
				public static void Postfix(SaveGame __instance)
				{
					SgtLogger.l("Savegame_OnPrefabInit");
					__instance.gameObject.AddOrGet<SaveGameData>();
				}
			}
		}


		/// <summary>
		/// adds gear button to cluster view
		/// </summary>
		[HarmonyPatch(typeof(ColonyDestinationSelectScreen))]
		[HarmonyPatch(nameof(ColonyDestinationSelectScreen.OnSpawn))]
		public static class InsertCustomClusterOption
		{
			public static void Prefix(ColonyDestinationSelectScreen __instance)
			{
				InitExtraWorlds.InitWorlds();
				OverrideWorldSizeOnDataGetting.ResetCustomSizes();

				CGSMClusterManager.selectScreen = __instance;
			}
			public static void Postfix(ColonyDestinationSelectScreen __instance)
			{
				var InsertLocation = __instance.shuffleButton.transform.parent; //__instance.transform.Find("Layout/DestinationInfo/Content/InfoColumn/Horiz/Section - Destination/DestinationDetailsHeader/");
				var copyButton = Util.KInstantiateUI(__instance.shuffleButton.gameObject, InsertLocation.gameObject, true); //UIUtils.GetShellWithoutFunction(InsertLocation, "CoordinateContainer", "cgsm");


				UIUtils.TryFindComponent<Image>(copyButton.transform, "FG").sprite = Assets.GetSprite("icon_gear");
				UIUtils.TryFindComponent<ToolTip>(copyButton.transform, "").toolTip = STRINGS.UI.CGMBUTTON.DESC;
				UIUtils.TryFindComponent<KButton>(copyButton.transform, "").onClick += () => CGSMClusterManager.InstantiateClusterSelectionView(__instance);

				LoadCustomCluster = false;

				if (CGSMClusterManager.LastGenFailed)
				{
					CGSMClusterManager.InstantiateClusterSelectionView(__instance);
					LastWorldGenDidFail(false);
				}
			}
		}
		[HarmonyPatch(typeof(CustomGameSettings))]
		[HarmonyPatch(nameof(CustomGameSettings.SetMixingSetting))]
		[HarmonyPatch(new Type[] { typeof(SettingConfig), typeof(string) })]
		public static class RegenerateOnMixingSettingsChanged
		{
			public static void Postfix(CustomGameSettings __instance, SettingConfig config, string value)
			{
				if (__instance == null || LoadCustomCluster)
					return;
				RegenerateCGM(__instance, "Mixing Setting " + config.id);
			}
		}
		/// <summary>
		/// Regenerates Custom cluster with newly created traits on seed shuffle
		/// </summary>
		[HarmonyPatch(typeof(CustomGameSettings))]
		[HarmonyPatch(nameof(CustomGameSettings.SetQualitySetting))]
		[HarmonyPatch(new Type[] { typeof(SettingConfig), typeof(string) })]
		public static class RegenerateOnSeedOrClusterChanged
		{
			public static void Postfix(CustomGameSettings __instance, SettingConfig config, string value)
			{
				if (__instance == null || LoadCustomCluster)
					return;

				if (config.id != "WorldgenSeed" && config.id != "ClusterLayout")
					return;
				SgtLogger.l(config.id, "cgm setting changed");
				RegenerateCGM(__instance, config.id);
			}
		}
		public static void RegenerateCGM(CustomGameSettings __instance, string changedConfigID)
		{
			if (StillLoading || ApplyCustomGen.IsGenerating)
				return;

			if (CGSMClusterManager.LastGenFailed)
			{
				SgtLogger.l("Skipping regenerating due to failed previous worldgen.");

				return;
			}
			string clusterPath = __instance.GetCurrentQualitySetting(CustomGameSettingConfigs.ClusterLayout).id;
			if (clusterPath == null || clusterPath.Length == 0)
			{
				///default is no path selected, this picks either classic Terra on "classic" selection/base game or Terrania on "spaced out" selection
				if (DlcManager.IsExpansion1Active())
					clusterPath = DestinationSelectPanel.ChosenClusterCategorySetting == 1 ? "expansion1::clusters/VanillaSandstoneCluster" : "expansion1::clusters/SandstoneStartCluster";
				else
					clusterPath = "SandstoneDefault";
			}

			if (CGM_Screen == null || !CGM_Screen.isActiveAndEnabled)
			{
				//CGM_MainScreen_UnityScreen.Instance.PresetApplied = false;
				CGSMClusterManager.LoadCustomCluster = false;
				SgtLogger.l("Regenerating Cluster from " + clusterPath + ". Reason: " + changedConfigID + " changed.");
				CGSMClusterManager.CreateCustomClusterFrom(clusterPath, ForceRegen: true);
			}
			else
			{
				SgtLogger.l("Regenerating Traits for " + clusterPath + ". Reason: " + changedConfigID + " changed.");
				CGSMClusterManager.RerollTraits();
				SgtLogger.l("Regenerating Mixings for " + clusterPath + ". Reason: " + changedConfigID + " changed.");
				CGSMClusterManager.RerollMixings();
			}
		}

		//[HarmonyPatch(typeof(ColonyDestinationSelectScreen))]
		//[HarmonyPatch(nameof(ColonyDestinationSelectScreen.ShuffleClicked))]
		public static class ColonyDestinationSelectScreen_ShuffleClicked_Patch
		{
			public static void AssetOnPrefabInitPostfix(Harmony harmony)
			{
				var m_TargetMethod = AccessTools.Method("ColonyDestinationSelectScreen, Assembly-CSharp:ShuffleClicked");
				var m_Postfix = AccessTools.Method(typeof(ColonyDestinationSelectScreen_ShuffleClicked_Patch), "Postfix");

				harmony.Patch(m_TargetMethod, null, new HarmonyMethod(m_Postfix), null);
			}

			public static void Postfix(ColonyDestinationSelectScreen __instance)
			{
				CGSMClusterManager.selectScreen = __instance;
				if (__instance.newGameSettingsPanel == null)
					return;

				RegenerateCGM(__instance.newGameSettingsPanel.settings, "Coordinate");
			}
		}

		[HarmonyPatch(typeof(SpacecraftManager))]
		[HarmonyPatch(nameof(SpacecraftManager.RestoreDestinations))]
		public static class VanillaStarmap_InsertModified
		{
			public static bool SkipPatch = false;
			public static bool Prefix(SpacecraftManager __instance)
			{
				if (SkipPatch)
					return true;

				SgtLogger.l("SpacecraftManager.RestoreDestinations");
				if (CGSMClusterManager.LoadCustomCluster && CustomCluster != null)
				{
					SgtLogger.l("Overriding Vanilla Starmap gen");

					if (!__instance.destinationsGenerated)
					{
						__instance.destinations = new List<SpaceDestination>();

						foreach (var band in CustomCluster.VanillaStarmapItems)
						{
							SgtLogger.l(band.Key.ToString(), "Band");
							foreach (var destinationType in band.Value)
							{

								SgtLogger.l(destinationType.ToString(), "POI here");
								__instance.destinations.Add(new SpaceDestination(__instance.destinations.Count(), destinationType, band.Key));
							}
						}
						SgtLogger.l("all POIs added");
						__instance.destinations.Sort(((a, b) => a.distance.CompareTo(b.distance)));


						//shenanigans in the vanilla code:
						List<float> list = new List<float>();
						for (int index = 0; index < 10; ++index)
							list.Add((float)index / 10f);
						for (int index1 = 0; index1 < 20; ++index1)
						{
							list.Shuffle<float>();
							int index2 = 0;
							foreach (SpaceDestination destination in __instance.destinations)
							{
								if (destination.distance == index1)
								{
									++index2;
									destination.startingOrbitPercentage = list[index2];
								}
							}
						}
						__instance.destinationsGenerated = true;
						return false;
					}
				}
				return true;
			}
		}

		//[HarmonyPatch(typeof(ColonyDestinationSelectScreen))]
		//[HarmonyPatch(nameof(ColonyDestinationSelectScreen.CoordinateChanged))]
		public static class ColonyDestinationSelectScreen_CoordinateChanged_Patch
		{
			public static void AssetOnPrefabInitPostfix(Harmony harmony)
			{
				var m_TargetMethod = AccessTools.Method("ColonyDestinationSelectScreen, Assembly-CSharp:CoordinateChanged");
				var m_Postfix = AccessTools.Method(typeof(ColonyDestinationSelectScreen_CoordinateChanged_Patch), "Postfix");

				harmony.Patch(m_TargetMethod, null, new HarmonyMethod(m_Postfix), null);
			}


			public static void Postfix(ColonyDestinationSelectScreen __instance)
			{
				CGSMClusterManager.selectScreen = __instance;
				if (__instance.newGameSettingsPanel == null)
					return;
				RegenerateCGM(__instance.newGameSettingsPanel.settings, "Coordinate");
			}
		}


		[HarmonyPatch(typeof(NewGameSettingsPanel))]
		[HarmonyPatch(nameof(NewGameSettingsPanel.SetSetting))]
		public static class ReplaceDefaultName
		{
			public static bool Prefix(SettingConfig setting, string level)
			{
				if (LoadCustomCluster) return false;
				return true;
			}
		}


		/// <summary>
		/// </summary>
		[HarmonyPatch(typeof(MinionSelectScreen))]
		[HarmonyPatch(nameof(MinionSelectScreen.OnProceed))]
		public static class Generate_Preset_On_NewGame
		{
			public static void Postfix()
			{
				if (!Config.Instance.AutomatedClusterPresets)
					return;

				if (CGSMClusterManager.LoadCustomCluster && CGSMClusterManager.CustomCluster != null)
				{
					string name = SaveGame.Instance.BaseName;
					CustomClusterSettingsPreset tempStats = CustomClusterSettingsPreset.CreateFromCluster(CGSMClusterManager.CustomCluster, name);
					tempStats.WriteToFile();
				}
			}
		}



		///// <summary>
		///// make WorldMixing (not subworld mixing!) disable with cgm cluster
		///// </summary>
		//[HarmonyPatch(typeof(SettingsCache))]
		//[HarmonyPatch(nameof(SettingsCache.LoadWorldMixingSettings))]
		//public static class LoadWorldMixingSettings_Postfix_Exclusion
		//{
		//    public static void Postfix()
		//    {
		//        foreach(var worldMixingSetting in SettingsCache.worldMixingSettings.Values)
		//        {
		//            if (worldMixingSetting != null && worldMixingSetting.forbiddenClusterTags != null && !worldMixingSetting.forbiddenClusterTags.Contains(CustomClusterClusterTag))
		//                worldMixingSetting.forbiddenClusterTags.Add(CustomClusterClusterTag);
		//        }
		//    }
		//}

		/// <summary>
		/// Prevents the normal cluster menu from closing when the custom cluster menu is open
		/// </summary>
		[HarmonyPatch(typeof(NewGameFlowScreen))]
		[HarmonyPatch(nameof(NewGameFlowScreen.OnKeyDown))]
		public static class CatchGoingBack
		{
			public static bool Prefix(KButtonEvent e)
			{
				if (CGSMClusterManager.Screen != null && CGSMClusterManager.Screen.activeSelf)
					return false;
				return true;
			}
		}

		/// <summary>
		/// Load missing moonlet type
		/// </summary>
		[HarmonyPatch(typeof(Worlds))]
		[HarmonyPatch(nameof(Worlds.LoadReferencedWorlds))]
		public static class LoadAdditionalWorlds
		{
			public static void Prefix(ISet<string> referencedWorlds)
			{
				if (!DlcManager.IsExpansion1Active())
					return;

				SgtLogger.l("checking if any moonlets should be added");
				List<string> additionalWorlds = new List<string>();
				foreach (var world in referencedWorlds)
				{
					if (ModAssets.IsModdedAsteroid(world, out _))
						continue;

					if (ModAssets.Moonlets.Any(world.Contains))
					{
						string outerWorld = world.Replace("Start", string.Empty).Replace("Warp", string.Empty);
						string startWorld = outerWorld + "Start", warpWorld = outerWorld + "Warp";
						if (!referencedWorlds.Contains(outerWorld) && !additionalWorlds.Contains(outerWorld))
							additionalWorlds.Add(outerWorld);
						if (!referencedWorlds.Contains(warpWorld) && !additionalWorlds.Contains(warpWorld))
							additionalWorlds.Add(warpWorld);
						if (!referencedWorlds.Contains(startWorld) && !additionalWorlds.Contains(startWorld))
							additionalWorlds.Add(startWorld);
					}
				}
				foreach (var item in additionalWorlds)
				{
					SgtLogger.l("adding additional world: " + item);
					referencedWorlds.Add(item);
				}
			}
		}

		////[HarmonyPatch(typeof(Db),(nameof(Db.Initialize)))]
		///Planet generation in spaced out
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
				List<KeyValuePair<string, ProcGen.World>> toAdd = new List<KeyValuePair<string, ProcGen.World>>();
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


					if (
						sourceWorld.Key.Contains("NiobiumMoonlet")
						|| sourceWorld.Key.Contains("RegolithMoonlet")
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



					var TypeToIgnore = DeterminePlanetType(sourceWorld.Value);
					if (TypeToIgnore == StarmapItemCategory.Starter)
					{
						if (
						__instance.worldCache.ContainsKey(sourceWorld.Key.Replace("Start", "")) && sourceWorld.Key.Contains("Start")
						|| __instance.worldCache.ContainsKey(sourceWorld.Key.Replace("Start", "").Replace("Outer", "") + "Warp")
						)
						{
							SgtLogger.l($"skipping {sourceWorld.Key} bc there is already a warp and normal asteroid");
							continue;
						}
					}
					else if (TypeToIgnore == StarmapItemCategory.Warp)
					{
						if (
						__instance.worldCache.ContainsKey(sourceWorld.Key.Replace("Warp", "")) && sourceWorld.Key.Contains("Warp")
						|| __instance.worldCache.ContainsKey(sourceWorld.Key.Replace("Warp", "").Replace("Outer", "") + "Start")
						)
						{
							SgtLogger.l($"skipping {sourceWorld.Key} bc there is already a start and outer asteroid");
							continue;
						}
					}
					else if (TypeToIgnore == StarmapItemCategory.Outer)
					{
						if (
						   __instance.worldCache.ContainsKey(sourceWorld.Key + "Warp")
						|| __instance.worldCache.ContainsKey(sourceWorld.Key + "Start")
						)
						{
							SgtLogger.l($"skipping {sourceWorld.Key} there is already a warp and Start asteroid");
							continue;
						}
					}

					List<string> additionalTemplates = new List<string>();

					if (sourceWorld.Value.startingBaseTemplate != null && sourceWorld.Value.startingBaseTemplate.Count() > 0 &&
						(sourceWorld.Value.startingBaseTemplate.Contains("sap_tree_room") || sourceWorld.Value.startingBaseTemplate.Contains("poi_satellite_3_a")))
					{
						additionalTemplates.Add(sourceWorld.Value.startingBaseTemplate);
					}

					//StartWorld

					if (TypeToIgnore != StarmapItemCategory.Starter)
					{
						string newWorldPath_Start = BaseName + "Start";

						var StartWorld = new ProcGen.World();

						CopyValues(StartWorld, sourceWorld.Value);


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

						StartWorld.unknownCellsAllowedSubworlds = new List<ProcGen.World.AllowedCellsFilter>(sourceWorld.Value.unknownCellsAllowedSubworlds);
						StartWorld.subworldFiles = new List<WeightedSubworldName>(sourceWorld.Value.subworldFiles);

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



						StartWorld.worldTemplateRules = new List<ProcGen.World.TemplateSpawnRules>();

						if (sourceWorld.Value.worldTemplateRules != null && sourceWorld.Value.worldTemplateRules.Count > 0)
						{
							foreach (var rule in sourceWorld.Value.worldTemplateRules)
							{
								var ruleNew = new ProcGen.World.TemplateSpawnRules();
								CopyValues(ruleNew, rule);

								//if (ruleNew.listRule == ProcGen.World.TemplateSpawnRules.ListRule.GuaranteeAll)
								//    ruleNew.listRule = ProcGen.World.TemplateSpawnRules.ListRule.GuaranteeSomeTryMore;
								StartWorld.worldTemplateRules.Add(ruleNew);
							}

						}
						CopyAllWorldTraits(sourceWorld.Value, StartWorld, StarmapItemCategory.Starter);

						StartWorld.filePath = newWorldPath_Start;
						StartWorld.startSubworldName = ModAPI.GetStartAreaSubworld(StartWorld, false);
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
							},
							new ProcGen.World.AllowedCellsFilter()
							{
								command = ProcGen.World.AllowedCellsFilter.Command.ExceptWith,
								zoneTypes  = new List<SubWorld.ZoneType>() { SubWorld.ZoneType.Space
								, SubWorld.ZoneType.MagmaCore
								}
							},
							new ProcGen.World.AllowedCellsFilter()
							{
								command = ProcGen.World.AllowedCellsFilter.Command.ExceptWith,
								tagcommand = ProcGen.World.AllowedCellsFilter.TagCommand.AtTag,
								tag = "NoGravitasFeatures"
							}
						};
						StartWorld.worldTemplateRules.Insert(0, TeleporterSpawn);

						toAdd.Add(new(newWorldPath_Start, StartWorld));
						ModAssets.ModPlanetOriginPaths.Add(newWorldPath_Start, sourceWorld.Key);

						SgtLogger.l(newWorldPath_Start, "Created Starter Planet Variant");

					}
					///Warp planet variant
					if (TypeToIgnore != StarmapItemCategory.Warp)
					{
						string newWorldPath_Warp = BaseName + "Warp";

						var WarpWorld = new ProcGen.World();

						CopyValues(WarpWorld, sourceWorld.Value);

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


						WarpWorld.unknownCellsAllowedSubworlds = new List<ProcGen.World.AllowedCellsFilter>(sourceWorld.Value.unknownCellsAllowedSubworlds);
						WarpWorld.subworldFiles = new List<WeightedSubworldName>(sourceWorld.Value.subworldFiles);
						WarpWorld.worldTemplateRules = new List<ProcGen.World.TemplateSpawnRules>();

						if (sourceWorld.Value.worldTemplateRules != null && sourceWorld.Value.worldTemplateRules.Count > 0)
						{
							foreach (var rule in sourceWorld.Value.worldTemplateRules)
							{
								var ruleNew = new ProcGen.World.TemplateSpawnRules();
								CopyValues(ruleNew, rule);

								WarpWorld.worldTemplateRules.Add(ruleNew);
							}
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


						CopyAllWorldTraits(sourceWorld.Value, WarpWorld, StarmapItemCategory.Warp);

						WarpWorld.filePath = newWorldPath_Warp;
						WarpWorld.startingBaseTemplate = ModAPI.GetStarterBaseTemplate(WarpWorld, true);
						WarpWorld.startSubworldName = ModAPI.GetStartAreaSubworld(WarpWorld, true);

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
							},
							new ProcGen.World.AllowedCellsFilter()
							{
								command = ProcGen.World.AllowedCellsFilter.Command.ExceptWith,
								zoneTypes  = new List<SubWorld.ZoneType>() { SubWorld.ZoneType.Space
								, SubWorld.ZoneType.MagmaCore
								}
							},
							new ProcGen.World.AllowedCellsFilter()
							{
								command = ProcGen.World.AllowedCellsFilter.Command.ExceptWith,
								tagcommand = ProcGen.World.AllowedCellsFilter.TagCommand.AtTag,
								tag = "NoGravitasFeatures"
							}
						};
						WarpWorld.worldTemplateRules.Insert(0, TeleporterSpawn);

						toAdd.Add(new(newWorldPath_Warp, WarpWorld));
						ModAssets.ModPlanetOriginPaths.Add(newWorldPath_Warp, sourceWorld.Key);

						SgtLogger.l(newWorldPath_Warp, "Created Warp Planet Variant");

					}

					if (TypeToIgnore != StarmapItemCategory.Outer)
					{
						string newWorldPath_Outer = BaseName + "Outer";

						var OuterWorld = new ProcGen.World();

						CopyValues(OuterWorld, sourceWorld.Value);
						OuterWorld.filePath = newWorldPath_Outer;
						OuterWorld.startingBaseTemplate = null;
						//StartWorld.startSubworldName = string.Empty;

						OuterWorld.unknownCellsAllowedSubworlds = new List<ProcGen.World.AllowedCellsFilter>(sourceWorld.Value.unknownCellsAllowedSubworlds);
						OuterWorld.subworldFiles = new List<WeightedSubworldName>(sourceWorld.Value.subworldFiles);
						OuterWorld.worldTemplateRules = new List<ProcGen.World.TemplateSpawnRules>();


						if (sourceWorld.Value.worldTemplateRules != null && sourceWorld.Value.worldTemplateRules.Count > 0)
						{
							foreach (var rule in sourceWorld.Value.worldTemplateRules)
							{
								var ruleNew = new ProcGen.World.TemplateSpawnRules();
								CopyValues(ruleNew, rule);

								//if (ruleNew.listRule == ProcGen.World.TemplateSpawnRules.ListRule.GuaranteeAll)
								//    ruleNew.listRule = ProcGen.World.TemplateSpawnRules.ListRule.GuaranteeSomeTryMore;
								OuterWorld.worldTemplateRules.Add(ruleNew);
							}
						}

						CopyAllWorldTraits(sourceWorld.Value, OuterWorld, StarmapItemCategory.Outer);

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

						toAdd.Add(new(newWorldPath_Outer, OuterWorld));
						ModAssets.ModPlanetOriginPaths.Add(newWorldPath_Outer, sourceWorld.Key);

						SgtLogger.l(newWorldPath_Outer, "Created Outer Planet Variant");
					}


				}
				foreach (var item in toAdd)
				{
					item.Value.isModded = true;
					__instance.worldCache[item.Key] = item.Value;
				}
			}
			static void CopyValues<T>(T targetObject, T sourceObject)
			{
				foreach (PropertyInfo property in typeof(T).GetProperties().Where(p => p.CanWrite))
				{
					property.SetValue(targetObject, property.GetValue(sourceObject, null), null);
				}
			}
		}


		/// <summary>
		/// Makes error msg display the actual error instead of "couldn't germinate"
		/// </summary>
		[HarmonyPatch(typeof(WorldGen))]
		[HarmonyPatch(nameof(WorldGen.ReportWorldGenError))]
		public static class betterError
		{
			public static void Prefix(Exception e, ref string errorMessage)
			{
				if (CGSMClusterManager.LoadCustomCluster)
					CGSMClusterManager.LastWorldGenDidFail();

				if (e.Message.Contains("Could not find a spot in the cluster for"))
				{
					string planetName = e.Message.Replace("Could not find a spot in the cluster for ", string.Empty).Split()[0];
					SgtLogger.l(planetName);
					var planetData = SettingsCache.worlds.GetWorldData(planetName);
					OverrideWorldSizeOnDataGetting.ResetCustomSizes();
					if (planetData != null)
					{
						if (Strings.TryGet(planetData.name, out var name))
						{
							SgtLogger.error(name + " could not be placed.");
							errorMessage = string.Format(STRINGS.ERRORMESSAGES.PLANETPLACEMENTERROR, name);
							return;
						}

					}
				}
				if (e is WorldgenException)
				{
					errorMessage = e.Message;
				}
				OverrideWorldSizeOnDataGetting.ResetCustomSizes();
			}
		}

		/// <summary>
		/// During Cluster generation, load traits from custom cluster instead of randomized
		/// </summary>
		[HarmonyPatch(typeof(SettingsCache))]
		[HarmonyPatch(nameof(SettingsCache.GetRandomTraits))]
		public static class OverrideWorldTraits
		{
			/// <summary>
			/// Inserting Custom Traits
			/// </summary>
			public static bool Prefix(int seed, ProcGen.World world, ref List<string> __result)
			{
				if (CGSMClusterManager.LoadCustomCluster && CGSMClusterManager.CustomCluster != null && ApplyCustomGen.IsGenerating)
				{
					__result = CGSMClusterManager.CustomCluster.GiveWorldTraitsForWorldGen(world, seed);
					return false;
				}
				return true;
			}
		}

		[HarmonyPatch(typeof(FileNameDialog))]
		[HarmonyPatch(nameof(FileNameDialog.OnActivate))]
		public static class FixCrashOnActivate
		{
			private static bool Prefix(FileNameDialog __instance)
			{
				if (CameraController.Instance == null)
				{
					__instance.OnShow(show: true);
					__instance.inputField.Select();
					__instance.inputField.ActivateInputField();
					return false;
				}
				return true;
			}
		}
		[HarmonyPatch(typeof(FileNameDialog))]
		[HarmonyPatch(nameof(FileNameDialog.OnDeactivate))]
		public static class FixCrashOnDeactivate
		{
			private static bool Prefix(FileNameDialog __instance)
			{
				if (CameraController.Instance == null)
				{
					__instance.OnShow(show: false);
					return false;
				}
				return true;
			}
		}


		[HarmonyPatch(typeof(WorldGenSettings))]
		[HarmonyPatch(nameof(WorldGenSettings.GetFloatSetting))]
		public static class WorldGenSettings_GetFloatSetting_Patch
		{
			private static void Postfix(WorldGenSettings __instance, string target, ref float __result)
			{
				if (!CGSMClusterManager.LoadCustomCluster)
					return;
				if ((target != "OverworldDensityMin") && (target != "OverworldDensityMax") && (target != "OverworldAvoidRadius") && (target != "OverworldMinNodes") && (target != "OverworldMaxNodes"))
					return;

				__result = GetMultipliedSizeFloat(__result, __instance);
			}
		}
		[HarmonyPatch(typeof(WorldGenSettings))]
		[HarmonyPatch(nameof(WorldGenSettings.GetIntSetting))]
		public static class WorldGenSettings_GetIntSetting_Patch
		{
			private static void Postfix(WorldGenSettings __instance, string target, ref int __result)
			{
				if (!CGSMClusterManager.LoadCustomCluster)
					return;
				if ((target != "OverworldDensityMin") && (target != "OverworldDensityMax") && (target != "OverworldAvoidRadius") && (target != "OverworldMinNodes") && (target != "OverworldMaxNodes"))
					return;

				__result = GetMultipliedSizeInt(__result, __instance);
			}
		}


		[HarmonyPatch(typeof(Border))]
		[HarmonyPatch(nameof(Border.ConvertToMap))]
		public static class Border_ConvertToMap_Patch
		{
			private static void Prefix(Border __instance)
			{
				if (CGSMClusterManager.LoadCustomCluster && !Mathf.Approximately(1, borderSizeMultiplier))
					__instance.width = Mathf.Max(0.25f, __instance.width * borderSizeMultiplier);
			}
		}
		// static float OriginalBorder = -1f;

		[HarmonyPatch(typeof(WorldGenSettings))]
		[HarmonyPatch(nameof(WorldGenSettings.GetSubworldsForWorld))]
		public static class Getw
		{
			private static void Postfix(WorldGenSettings __instance, ref List<WeightedSubWorld> __result)
			{
				if (!CGSMClusterManager.LoadCustomCluster)
					return;

				foreach (var weightedSubworld in __result)
				{
					if (weightedSubworld.minCount != 0)
						weightedSubworld.minCount = GetMultipliedSizeInt(weightedSubworld.minCount, __instance);
				}
			}
		}
		[HarmonyPatch(typeof(WorldLayout))]
		[HarmonyPatch(nameof(WorldLayout.ConvertUnknownCells))]
		public static class ApplyMultipliersToWeights
		{
			private static void Prefix(ref bool isRunningDebugGen)
			{
				if (CGSMClusterManager.LoadCustomCluster)
					isRunningDebugGen = true;
			}
		}


		[HarmonyPatch(typeof(WorldGen))]
		[HarmonyPatch(nameof(WorldGen.GenerateOffline))]
		public static class GrabPlanetGenerating
		{
			private static void Prefix(WorldGen __instance)
			{
				if (__instance != null && __instance.Settings != null)
				{
					borderSizeMultiplier = Mathf.Min(1, GetMultipliedSizeFloat(1f, __instance.Settings));
					WorldSizeMultiplier = GetMultipliedSizeFloat(1f, __instance.Settings);
					SgtLogger.l(borderSizeMultiplier.ToString(), "BorderSizeMultiplier");
				}

			}
		}
		static float borderSizeMultiplier = 1f;
		static float WorldSizeMultiplier = 1f;

		public static float GetMultipliedSizeFloat(float inputNumber, WorldGenSettings worldgen)
		{
			if (!CGSMClusterManager.LoadCustomCluster)
				return inputNumber;


			if (worldgen != null && worldgen.world != null && worldgen.world.filePath != null &&
				CGSMClusterManager.CustomCluster.HasStarmapItem(worldgen.world.filePath, out var item)
				&& (item.CurrentSizeMultiplier < 1 && !item.DefaultDimensions)
				)
			{
				if (Mathf.Approximately(item.CurrentSizeMultiplier, 1))
					return inputNumber;

				float newValue = Mathf.RoundToInt(item.ApplySizeMultiplierToValue((float)inputNumber));

				SgtLogger.l($"changed input float: {inputNumber}, multiplied: {newValue}", "CGM WorldgenModifier");

				return newValue;
			}
			return inputNumber;
		}
		public static int GetMultipliedSizeInt(int inputNumber, WorldGenSettings worldgen)
		{

			if (!CGSMClusterManager.LoadCustomCluster)
				return inputNumber;

			if (worldgen != null && worldgen.world != null && worldgen.world.filePath != null &&
				CGSMClusterManager.CustomCluster.HasStarmapItem(worldgen.world.filePath, out var item)
				&& (item.CurrentSizeMultiplier < 1 && !item.DefaultDimensions)
				)
			{

				if (Mathf.Approximately(item.CurrentSizeMultiplier, 1))
					return inputNumber;

				SgtLogger.l($"GetMultipliedSizeInt: {inputNumber}, multiplied: {item.ApplySizeMultiplierToValue((float)inputNumber)}", "CGM WorldgenModifier");


				return Mathf.RoundToInt(item.ApplySizeMultiplierToValue((float)inputNumber));
			}
			return inputNumber;
		}




		[HarmonyPatch(typeof(Worlds), nameof(Worlds.GetWorldData), new Type[] { typeof(string) })]
		public static class OverrideWorldSizeOnDataGetting
		{
			public static Dictionary<string, Vector2I> OriginalPlanetSizes = new Dictionary<string, Vector2I>();
			static Dictionary<string, float> OriginalWorldTraitScales = new();


			public static void ResetCustomSizes()
			{
				foreach (var world in SettingsCache.worlds.worldCache)
				{
					if (OriginalPlanetSizes.TryGetValue(world.Key, out var originalSize))
					{
						SgtLogger.l("Resetting custom planet size to " + world.Key + ", new size: " + originalSize.X + "x" + originalSize.Y, "CGM WorldgenModifier");
						world.Value.worldsize = originalSize;
					}
					if (OriginalWorldTraitScales.TryGetValue(world.Key, out var originalScale))
					{
						world.Value.worldTraitScale = originalScale;
					}
				}
				//OriginalPlanetSizes.Clear();
			}
			public static void Postfix(string name, ref ProcGen.World __result)
			{
				if (__result != null && name != null)
				{
					if (!OriginalPlanetSizes.ContainsKey(name))
						OriginalPlanetSizes.Add(name, __result.worldsize);
					if (!OriginalWorldTraitScales.ContainsKey(name))
						OriginalWorldTraitScales.Add(name, __result.worldTraitScale);

					if (CGSMClusterManager.LoadCustomCluster && CGSMClusterManager.CustomCluster != null && CGSMClusterManager.CustomCluster.HasStarmapItem(name, out var item) && !item.DefaultDimensions)
					{
						if (__result.worldsize != item.CustomPlanetDimensions)
						{
							__result.worldsize = item.CustomPlanetDimensions;
							SgtLogger.l("CGM generating, applied custom planet size to " + item.DisplayName + ", new size: " + __result.worldsize.ToString(), "CGM WorldgenModifier");
						}
						if (OriginalWorldTraitScales.TryGetValue(name, out var originalTraitScale))
						{
							__result.worldTraitScale = item.ApplySizeMultiplierToValue(originalTraitScale);
						}
					}
					else
					{
						if (OriginalPlanetSizes.ContainsKey(name))
						{
							if (__result.worldsize != OriginalPlanetSizes[name])
							{
								__result.worldsize = OriginalPlanetSizes[name];
								SgtLogger.l("CGM not generating, worlgen size for " + name + " set to default: " + __result.worldsize.ToString(), "CGM WorldgenModifier");
							}
						}
						if (OriginalWorldTraitScales.TryGetValue(name, out var originalTraitScale))
						{
							__result.worldTraitScale = originalTraitScale;
						}
					}
				}
			}

		}

		[HarmonyPatch(typeof(TemplateSpawning))]
		[HarmonyPatch(nameof(TemplateSpawning.SpawnTemplatesFromTemplateRules))]
		public static class AddSomeGeysers
		{
			static Dictionary<string, Dictionary<List<string>, int>> OriginalGeyserAmounts = new Dictionary<string, Dictionary<List<string>, int>>();
			static Dictionary<ProcGen.World.TemplateSpawnRules, Vector2I> placementOverridesAdjustments = new Dictionary<ProcGen.World.TemplateSpawnRules, Vector2I>();
			/// <summary>
			/// Inserting Custom Traits
			/// </summary>
			public static void Prefix(WorldGenSettings settings, SeededRandom myRandom)
			{
				const string geyserKey = "GEYSER";
				if (CGSMClusterManager.LoadCustomCluster && CGSMClusterManager.CustomCluster != null)
				{
					if (!OriginalGeyserAmounts.ContainsKey(settings.world.filePath))
						OriginalGeyserAmounts[settings.world.filePath] = new Dictionary<List<string>, int>();

					if (CGSMClusterManager.CustomCluster.HasStarmapItem(settings.world.filePath, out var item) && !item.DefaultDimensions && !Mathf.Approximately(item.CurrentSizeMultiplier, 1))
					{
						int seed = myRandom.seed;
						SgtLogger.l(seed.ToString(), "geyserSeed");

						float SizeModifier = item.CurrentSizeMultiplier;

						if (Mathf.Approximately(SizeModifier, 1))
							return;
						// if (SizeModifier < 1)
						//   SizeModifier = (1 + SizeModifier) / 2;
						///Geyser Penalty needs a better implementation...

						foreach (var WorldTemplateRule in settings.world.worldTemplateRules)
						{
							if (WorldTemplateRule.names.Any(name => name.ToUpperInvariant().Contains(geyserKey)))
							{
								if (!OriginalGeyserAmounts[settings.world.filePath].ContainsKey(WorldTemplateRule.names))
								{
									OriginalGeyserAmounts[settings.world.filePath][WorldTemplateRule.names] = WorldTemplateRule.times;
								}

								float newGeyserAmount = (((float)OriginalGeyserAmounts[settings.world.filePath][WorldTemplateRule.names]) * SizeModifier);
								SgtLogger.l(string.Format("Adjusting geyser roll amount to worldsize for {0}; {1} -> {2}", WorldTemplateRule.names.FirstOrDefault(), OriginalGeyserAmounts[settings.world.filePath][WorldTemplateRule.names], newGeyserAmount), item.id);

								float chance = ((float)new KRandom(seed + WorldTemplateRule.names.First().GetHashCode()).Next(100)) / 100f;

								if (newGeyserAmount > 1)
								{
									WorldTemplateRule.times = Mathf.FloorToInt(newGeyserAmount);
									SgtLogger.l("new Geyser amount has a chance of " + newGeyserAmount % 1f + " for an additional spawn, rolling...", "CGM WorldgenModifier");

									SgtLogger.l("rolled: " + chance);
									//chance = 0;///always atleast 1
									if (chance <= (newGeyserAmount % 1f))
									{
										SgtLogger.l("roll for additional spawn succeeded: " + chance * 100f, "POI Chance: " + newGeyserAmount.ToString("P"));
										WorldTemplateRule.times += 1;
									}
									else
									{
										SgtLogger.l("roll for additional spawn failed: " + chance * 100f, "POI Chance: " + newGeyserAmount.ToString("P"));
									}
									SgtLogger.l("final geyser spawn count: " + WorldTemplateRule.times);

								}
								else
								{
									SgtLogger.l("new Geyser amount below 1, rolling for the geyser to appear at all...");
									SgtLogger.l("rolled: " + chance);
									//chance = 0;///always atleast 1
									if (chance <= newGeyserAmount)
									{
										SgtLogger.l("roll succeeded: " + chance * 100f, "POI Chance: " + newGeyserAmount.ToString("P"));
										WorldTemplateRule.times = 1;
									}
									else
									{
										SgtLogger.l("roll failed: " + chance * 100f, "POI Chance: " + newGeyserAmount.ToString("P"));
										WorldTemplateRule.times = 0;
									}
								}

								//WorldTemplateRule.times = Math.Max(1, Mathf.RoundToInt(((float)OriginalGeyserAmounts[settings.world.filePath][WorldTemplateRule.names]) * (float)item.CurrentSizePreset / 100f));
								//SgtLogger.l(string.Format("Adjusting geyser roll amount to worldsize for {0}; {1} -> {2}", WorldTemplateRule.names.FirstOrDefault(), OriginalGeyserAmounts[settings.world.filePath][WorldTemplateRule.names], WorldTemplateRule.times), item.id);
							}

							if (WorldTemplateRule.overridePlacement != Vector2I.minusone)
							{
								if (!placementOverridesAdjustments.ContainsKey(WorldTemplateRule))
								{
									SgtLogger.l(WorldTemplateRule.overridePlacement.ToString(), "vanilla override placement");
									placementOverridesAdjustments[WorldTemplateRule] = (WorldTemplateRule.overridePlacement);
								}
								WorldTemplateRule.overridePlacement = new Vector2I(Mathf.RoundToInt(((float)WorldTemplateRule.overridePlacement.X) * (float)item.SizeMultiplierX()), Mathf.RoundToInt(((float)WorldTemplateRule.overridePlacement.Y) * (float)item.SizeMultiplierY()));
								SgtLogger.l(WorldTemplateRule.overridePlacement.ToString(), "adjusted override placement, modifier: " + item.SizeMultiplierX());

							}

							///Fixed Templates on KleiFest2023 asteroid can only spawn once
							///if it has a fixed position                                           this part here
							if (WorldTemplateRule.times > 1 && WorldTemplateRule.overridePlacement != Vector2I.minusone)
							{
								WorldTemplateRule.times = 1;
							}
						}
					}

				}
				else
				{
					if (OriginalGeyserAmounts.ContainsKey(settings.world.filePath))
					{
						foreach (var WorldTemplateRule in settings.world.worldTemplateRules)
						{
							if (OriginalGeyserAmounts[settings.world.filePath].ContainsKey(WorldTemplateRule.names))
							{
								SgtLogger.l(string.Format("Resetting Geyser rules back for {0}; {1} -> {2}", WorldTemplateRule.names.FirstOrDefault(), WorldTemplateRule.times, OriginalGeyserAmounts[settings.world.filePath][WorldTemplateRule.names]), WorldTemplateRule.ruleId);
								WorldTemplateRule.times = OriginalGeyserAmounts[settings.world.filePath][WorldTemplateRule.names];
								OriginalGeyserAmounts.Remove(settings.world.filePath);
							}
							if (placementOverridesAdjustments.ContainsKey(WorldTemplateRule))
							{
								WorldTemplateRule.overridePlacement = placementOverridesAdjustments[WorldTemplateRule];
								placementOverridesAdjustments.Remove(WorldTemplateRule);
							}
						}
						OriginalGeyserAmounts.Remove(settings.world.filePath);
					}
				}
			}
		}
		[HarmonyPatch(typeof(MobSettings), "GetMob")]
		public static class MobSettings_GetMob_Patch
		{
			static Dictionary<string,MinMax> OriginalMobModifiers = new Dictionary<string,MinMax>();

			public static void Postfix(string id, ref Mob __result)
			{
				if (__result == null || !CGSMClusterManager.LoadCustomCluster)
					return;

				string prefabID = __result.prefabName ?? __result.name;

				if (prefabID == null)
					return;

				
				if (Mathf.Approximately(WorldSizeMultiplier, 1))
				{
					if (OriginalMobModifiers.TryGetValue(prefabID, out var value))
					{
						__result.density = value;
						//OriginalMobModifiers.Remove(prefabID);
					}
					return;
				}
				if (!OriginalMobModifiers.ContainsKey(prefabID))
					OriginalMobModifiers.Add(prefabID, __result.density);

				var original = OriginalMobModifiers[prefabID];

				__result.density = new(original.min * WorldSizeMultiplier, original.max * WorldSizeMultiplier);
			}
		}

		[HarmonyPatch(typeof(Cluster), nameof(Cluster.AssignClusterLocations))]
		public static class Cluster_StarmapInit_Patch
		{

			public static void Postfix(bool __result, Cluster __instance)
			{

				//SgtLogger.l($"{!CGSMClusterManager.LoadCustomCluster}, {CGSMClusterManager.CustomCluster == null}, {!DlcManager.IsExpansion1Active()}, {CustomCluster.SO_Starmap == null},{CustomCluster.SO_Starmap.UsingCustomLayout == false} ");

				//SgtLogger.l("AssignClusterLocationsPostfix");
				if (!CGSMClusterManager.LoadCustomCluster
					|| CGSMClusterManager.CustomCluster == null
					|| !DlcManager.IsExpansion1Active()
					|| CustomCluster.SO_Starmap == null
					//|| CustomCluster.SO_Starmap.UsingCustomLayout == false
					)
					return;

				SgtLogger.l("Applying CGM custom starmap");

				HashSet<AxialI> UsedLocations = new();
				HashSet<string> Worlds = new HashSet<string>();


				__instance.poiPlacements.Clear();
				int seed = __instance.seed;
				var worldPlacements = GeneratedLayout.worldPlacements;

				foreach (var placementData in CustomCluster.SO_Starmap.OverridePlacements)
				{
					SgtLogger.l(placementData.Value);

					int pos = worldPlacements.FindIndex(placement => placement.world == placementData.Value);
					if (pos != -1)
					{
						__instance.worlds[pos].SetClusterLocation(placementData.Key);
						UsedLocations.Add(placementData.Key);
					}
					else if (ModAssets.IsSOPOI(placementData.Value))
					{
						if (placementData.Value == ModAssets.RandomPOIId)
						{
							string selectedRandomPoiId = CGSMClusterManager.GetRandomPOI(seed);
							seed++;
							__instance.poiPlacements.Add(placementData.Key, selectedRandomPoiId);
						}
						else
							__instance.poiPlacements.Add(placementData.Key, placementData.Value);
						UsedLocations.Add(placementData.Key);
					}

				}
			}
		}
		[HarmonyPatch(typeof(MinionSelectScreen))]
		[HarmonyPatch(nameof(MinionSelectScreen.OnSpawn))]
		public class Add_NewClusterPresetButton
		{
			public static void Postfix(MinionSelectScreen __instance)
			{
				if (CGSMClusterManager.LoadCustomCluster && CGSMClusterManager.CustomCluster != null)
				{
					if (SaveGameData.Instance != null)
					{
						SgtLogger.l("writing custom cluster tags");
						SaveGameData.SetCustomCluster();
						SaveGameData.WriteCustomClusterTags(CGSMClusterManager.GeneratedLayout.clusterTags);
					}
				}

				if (Config.Instance.AutomatedClusterPresets)
					return;

				if (CGSMClusterManager.LoadCustomCluster && CGSMClusterManager.CustomCluster != null)
				{
					var makeNewClusterPresetButton = Util.KInstantiateUI<KButton>(__instance.backButton.gameObject, __instance.proceedButton.transform.parent.gameObject, true);
					UIUtils.AddActionToButton(makeNewClusterPresetButton.transform, "", () =>
					{
						CustomClusterSettingsPreset tempStats = CustomClusterSettingsPreset.CreateFromCluster(CGSMClusterManager.CustomCluster);
						tempStats.OpenPopUpToChangeName(() => makeNewClusterPresetButton.interactable = false, __instance.gameObject);
					});
					makeNewClusterPresetButton.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
					UIUtils.AddSimpleTooltipToObject(makeNewClusterPresetButton.transform, CGMEXPORT_SIDEMENUS.PRESETWINDOWCGM.HORIZONTALLAYOUT.ITEMINFO.BUTTONS.GENERATEFROMCURRENT.TOOLTIP);
					UIUtils.TryChangeText(makeNewClusterPresetButton.transform, "Text", CGMEXPORT_SIDEMENUS.PRESETWINDOWCGM.HORIZONTALLAYOUT.ITEMINFO.BUTTONS.GENERATEFROMCURRENT.TEXT_STARTSCREEN.ToString().ToUpperInvariant());
					makeNewClusterPresetButton.transform.Find("FG").GetComponent<Image>().sprite = Assets.GetSprite("icon_positive");

					var index = __instance.proceedButton.transform.GetSiblingIndex();
					makeNewClusterPresetButton.transform.SetSiblingIndex(index - 1);
				}
			}
		}


		[HarmonyPatch(typeof(Cluster), nameof(Cluster.Save))]
		public static class SaveAsOriginalCluster
		{
			public static void Prefix(Cluster __instance)
			{
				//CustomLayout
				if (CGSMClusterManager.LoadCustomCluster && __instance.Id == CGSMClusterManager.CustomClusterID)
				{
					var originalClusterID = CustomCluster.GetOriginalClusterId();
					SgtLogger.l("changing cluster id to original: " + originalClusterID);
					__instance.Id = originalClusterID;
					CustomGameSettings.Instance.SetQualitySetting(CustomGameSettingConfigs.ClusterLayout, originalClusterID);
				}
			}
		}
		//[HarmonyPatch(typeof(SettingsCache), nameof(SettingsCache.GetRandomTraits))]
		//public static class GetRandomTraits_test
		//{
		//    public static void Postfix(List<string> __result, int seed, ProcGen.World world)
		//    {
		//        SgtLogger.l(world.filePath.ToString() + ", " + seed);
		//        foreach (var item in __result)
		//        {
		//            SgtLogger.l(item.ToString());
		//        }

		//    }
		//}


		[HarmonyPatch(typeof(Cluster), MethodType.Constructor)]
		[HarmonyPatch(new Type[] { typeof(string), typeof(int), typeof(List<string>), typeof(bool), typeof(bool), typeof(bool) })]
		public static class ApplyCustomGen
		{
			public static bool IsGenerating = false;
			/// <summary>
			/// Setting ClusterID to custom cluster if it should load
			/// 
			/// </summary>
			public static void Prefix(ref string clusterName)
			{
				SgtLogger.l(clusterName, "CLUSTERID");
				//CustomLayout
				if (CGSMClusterManager.LoadCustomCluster)
				{
					SgtLogger.l("Custom ClusterConstructor has started");

					IsGenerating = true;

					//doesnt work, gotta do it manually
					//CustomGameSettings.Instance.RemoveInvalidMixingSettings();

					//foreach (var worldMixingSetting in SettingsCache.worldMixingSettings.Values)
					//{
					//    if (worldMixingSetting != null && worldMixingSetting.forbiddenClusterTags != null && !worldMixingSetting.forbiddenClusterTags.Contains(CustomClusterClusterTag))
					//        worldMixingSetting.forbiddenClusterTags.Add(CustomClusterClusterTag);
					//}
					//List<string> ToDisableMixings = new();
					//foreach(var mix in CustomGameSettings.Instance.CurrentMixingLevelsBySetting)
					//{
					//    var mixingSetting = SettingsCache.TryGetCachedWorldMixingSetting(mix.Key);
					//    if (mixingSetting != null)
					//    {
					//        SgtLogger.l("disabling " + mix.Key);
					//        ToDisableMixings.Add(mix.Key);
					//    }

					//    SgtLogger.l(mix.Key+": " + mix.Value, "current mixing setting");
					//}
					//foreach(var todisable in ToDisableMixings)
					//{
					//}

					//dirty manual exclusion to fix crash
					CustomGameSettings.Instance.CurrentMixingLevelsBySetting["CeresAsteroidMixing"] = WorldMixingSettingConfig.DisabledLevelId;
					if (DlcManager.IsContentOwned(DlcManager.DLC2_ID) && DlcManager.IsContentSubscribed(DlcManager.DLC2_ID) && CustomCluster.HasCeresAsteroid)
					{
						SgtLogger.l("Enabling Frosty Planet for Custom Cluster");
						CustomGameSettings.Instance.CurrentMixingLevelsBySetting["DLC2_ID"] = DlcMixingSettingConfig.EnabledLevelId;
					}
					//if (CGSMClusterManager.CustomCluster == null)
					//{
					//    ///Generating custom cluster if null
					//    CGSMClusterManager.AddCustomClusterAndInitializeClusterGen();
					//}
					clusterName = CGSMClusterManager.CustomClusterID;
					IsGenerating = true;
				}
			}


		}
		//[HarmonyPatch(typeof(WorldgenMixing), nameof(WorldgenMixing.DoWorldMixingInternal))]
		//public static class Worldmixing_Patch
		//{
		//    public static void Prefix(MutatedClusterLayout mutatedClusterLayout, int seed)
		//    {
		//        var layout = mutatedClusterLayout.layout;
		//        SgtLogger.l(layout.name, "DoMixingInternal");
		//        SgtLogger.l(layout.clusterTags.Count()+"", "TagCount");
		//        foreach(var tag in layout.clusterTags)
		//        {
		//            SgtLogger.l(tag, "clusterTag");
		//        }
		//    }
		//}
		//[HarmonyPatch(typeof(WorldgenMixing), nameof(WorldgenMixing.FindWorldMixingOption))]
		//public static class WorldPlacement_IsMixing
		//{
		//    public static bool Prefix(WorldPlacement worldPlacement, List<WorldgenMixing.WorldMixingOption> options, ref WorldgenMixing.WorldMixingOption __result)
		//    {
		//        options = options.StableSort<WorldgenMixing.WorldMixingOption>().ToList<WorldgenMixing.WorldMixingOption>();
		//        foreach (WorldgenMixing.WorldMixingOption option in options)
		//        {
		//            if (!option.IsExhausted)
		//            {
		//                SgtLogger.l("Testing: "+ worldPlacement.world + " for "+ Strings.Get(option.mixingSettings.name));
		//                bool allowedByTags = true;
		//                foreach (string requiredTag in worldPlacement.worldMixing.requiredTags)
		//                {
		//                    if (!option.cachedWorld.worldTags.Contains(requiredTag))
		//                    {
		//                        SgtLogger.l(worldPlacement.world + " is missing required tag: " + requiredTag);
		//                        allowedByTags = false;
		//                        break;
		//                    }
		//                }
		//                foreach (string forbiddenTag in worldPlacement.worldMixing.forbiddenTags)
		//                {
		//                    if (option.cachedWorld.worldTags.Contains(forbiddenTag))
		//                    {
		//                        SgtLogger.l(worldPlacement.world + " has forbidden tag: " + forbiddenTag);
		//                        allowedByTags = false;
		//                        break;
		//                    }
		//                }
		//                if (allowedByTags)
		//                {
		//                    SgtLogger.l(worldPlacement.world +" fulfilled all purposes");
		//                    __result = option;
		//                    return false;
		//                }
		//            }
		//        }
		//        __result = null;
		//        return false;
		//    }
		//}

		[HarmonyPatch(typeof(MainMenu), nameof(MainMenu.OnSpawn))]
		public static class MainMenu_Initialize_Patch
		{
			public static void Postfix()
			{
				ApplyCustomGen.IsGenerating = false;
				borderSizeMultiplier = 1f;
				WorldSizeMultiplier = 1f;
				LoadCustomCluster = false;
				StillLoading = false;
				CGSMClusterManager.RemoveFromCache();
			}
		}
		public static bool StillLoading = true;
	}
}
