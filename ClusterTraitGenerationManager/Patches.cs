﻿using ClusterTraitGenerationManager.ClusterData;
using HarmonyLib;
using Klei.CustomSettings;
using ProcGen;
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using UtilLibs;
using static ClusterTraitGenerationManager.ClusterData.CGSMClusterManager;
using static ClusterTraitGenerationManager.STRINGS.UI;
using static STRINGS.UI;

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
				if (SaveGameData.IsCustomCluster())
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
				LocalisationUtil.FixTranslationStrings();
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
				if (SaveGameData.IsCustomCluster())
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
				else
				{
					CGSMClusterManager.GenerateDefaultCluster();
				}
			}
		}
		[HarmonyPatch(typeof(CustomGameSettings), nameof(CustomGameSettings.SetMixingSetting), [typeof(SettingConfig), typeof(string), typeof(bool)])]
		public static class RegenerateOnMixingSettingsChanged
		{
			static string PreviousValue = string.Empty;
			public static void Prefix(CustomGameSettings __instance, SettingConfig config, string value)
			{
				if (__instance == null || LoadCustomCluster)
					return;
				PreviousValue = __instance.GetCurrentMixingSettingLevel(config).id;
			}

			public static void Postfix(CustomGameSettings __instance, SettingConfig config, string value)
			{
				if (__instance == null || LoadCustomCluster || PreviousValue == value)
					return;
				RegenerateCGM(__instance, "Mixing Setting" + config.id, rerollTraits: false);
			}
		}
		/// <summary>
		/// Regenerates Custom cluster with newly created traits on seed shuffle
		/// </summary>
		[HarmonyPatch(typeof(CustomGameSettings), nameof(CustomGameSettings.SetQualitySetting), [typeof(SettingConfig), typeof(string)])]
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
		public static void RegenerateCGM(CustomGameSettings __instance, string changedConfigID, bool rerollTraits = true)
		{
			if (StillLoading || ApplyCustomGen.IsGenerating)
				return;

			if (CGSMClusterManager.LastGenFailed)
			{
				SgtLogger.l("Skipping regenerating due to failed previous worldgen.");

				return;
			}
			string clusterPath = __instance.GetCurrentQualitySetting(CustomGameSettingConfigs.ClusterLayout).id;
			//SgtLogger.l("clusterPath: " + clusterPath);

			if (clusterPath == null || clusterPath.Length == 0)
			{
				//SgtLogger.l("DestinationSelectPanel.ChosenClusterCategorySetting: " + DestinationSelectPanel.ChosenClusterCategorySetting);
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
				if (rerollTraits)
				{
					SgtLogger.l("Regenerating Traits for " + clusterPath + ". Reason: " + changedConfigID + " changed.");
					CGSMClusterManager.RerollTraits();
				}
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


						//shenanigans in the vanilla code that determine the horizontal starting position of a space poi, adjusted for when there are a lot of pois in a band added by cgm

						int numberPercentageSteps = 10;
						List<float> startingPercentageRandoms = new List<float>();
						for (int index = 0; index < numberPercentageSteps; ++index)
							startingPercentageRandoms.Add(index / (float)numberPercentageSteps);
						for (int index1 = 0; index1 < __instance.destinations.Count; ++index1)
						{
							startingPercentageRandoms.Shuffle<float>();
							int percentageIndex = 0;
							foreach (SpaceDestination destination in __instance.destinations)
							{
								if (destination.distance == index1)
								{
									++percentageIndex;
									destination.startingOrbitPercentage = startingPercentageRandoms[percentageIndex % numberPercentageSteps];
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



		/// <summary>
		/// make WorldMixing (not subworld mixing!) disable with cgm cluster
		/// </summary>
		[HarmonyPatch(typeof(SettingsCache))]
		[HarmonyPatch(nameof(SettingsCache.LoadWorldMixingSettings))]
		public static class LoadWorldMixingSettings_Postfix_Exclusion
		{
			public static void Postfix()
			{
				foreach (var worldMixingSetting in SettingsCache.worldMixingSettings.Values)
				{
					if (worldMixingSetting != null)
					{
						if (worldMixingSetting.forbiddenClusterTags == null)
							worldMixingSetting.forbiddenClusterTags = new List<string>();

						if (!worldMixingSetting.forbiddenClusterTags.Contains(CustomClusterClusterTag))
						{
							worldMixingSetting.forbiddenClusterTags.Add(CustomClusterClusterTag);
						}
					}
				}
			}
		}

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

		[HarmonyPatch(typeof(FileNameDialog), nameof(FileNameDialog.OnActivate))]
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
		[HarmonyPatch(typeof(FileNameDialog), nameof(FileNameDialog.OnDeactivate))]
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
					__instance.width = Mathf.Max(0.33f, __instance.width * borderSizeMultiplier);
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

		/// <summary>
		/// runs right before band masses are converted
		/// </summary>
		[HarmonyPatch(typeof(MutatedWorldData), nameof(MutatedWorldData.ApplyWorldTraits))]
		public static class IncreaseTileMassForTinyWorlds
		{
			private static void Postfix(ProcGen.MutatedWorldData __instance)
			{
				if (!CGSMClusterManager.LoadCustomCluster)
					return;


				if (__instance.world != null && __instance.world.filePath != null &&
				CGSMClusterManager.CustomCluster.HasStarmapItem(__instance.world.filePath, out var item)
				&& (item.CurrentSizeMultiplier < 1 && !item.DefaultDimensions)
				)
				{
					float densityMultiplier = 1f / item.CurrentSizeMultiplier;
					SgtLogger.l("element mass multiplier: " + densityMultiplier);

					foreach (KeyValuePair<string, ElementBandConfiguration> bandConfiguration in __instance.biomes.BiomeBackgroundElementBandConfigurations)
					{
						foreach (ElementGradient elementGradient in bandConfiguration.Value)
						{
							ProcGen.WorldTrait.ElementBandModifier modifier = new WorldTrait.ElementBandModifier();
							modifier.element = elementGradient.content;
							var element = ElementLoader.FindElementByName(elementGradient.content);


							if (element != null && element.id == SimHashes.Magma)
							{
								// no modifier for magma. breaks the bottom of the world
								modifier.massMultiplier = 1f;
							}
							else if (element != null && element.IsLiquid)
							{
								// give the player at most 2x liquids otherwise they really break out of the pockets they spawn in
								modifier.massMultiplier = (float)Math.Min(2f, densityMultiplier);
							}
							else
							{
								// everything else - gasses and solid tiles should just use whatever density modifier the game has
								modifier.massMultiplier = densityMultiplier;

							}
							elementGradient.Mod(modifier);
						}
					}
				}
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

				float newValue = item.ApplySizeMultiplierToValue((float)inputNumber);

				newValue = (float)Math.Round(newValue, 4, MidpointRounding.ToEven);

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

		[HarmonyPatch(typeof(TemplateSpawning), nameof(TemplateSpawning.SpawnTemplatesFromTemplateRules))]
		public static class AddSomeGeysers
		{
			//static Dictionary<string, Dictionary<List<string>, int>> OriginalGeyserAmounts = new Dictionary<string, Dictionary<List<string>, int>>();
			static Dictionary<ProcGen.World.TemplateSpawnRules, int> OriginalTemplateAmounts = new();
			static Dictionary<ProcGen.World.TemplateSpawnRules, Vector2I> placementOverridesAdjustments = new Dictionary<ProcGen.World.TemplateSpawnRules, Vector2I>();
			/// <summary>
			/// Inserting Custom Traits
			/// </summary>
			public static void Prefix(WorldGenSettings settings, SeededRandom myRandom)
			{
				const string geyserKey = "GEYSER";
				if (CGSMClusterManager.LoadCustomCluster && CGSMClusterManager.CustomCluster != null)
				{
					//if (!OriginalGeyserAmounts.ContainsKey(settings.world.filePath))
					//	OriginalGeyserAmounts[settings.world.filePath] = new Dictionary<List<string>, int>();

					if (CGSMClusterManager.CustomCluster.HasStarmapItem(settings.world.filePath, out var item)
						//	&& !item.DefaultDimensions && !Mathf.Approximately(item.CurrentSizeMultiplier, 1)
						)
					{
						int seed = myRandom.seed;
						SgtLogger.l(seed.ToString(), "geyserSeed");

						bool customDimensions = (!item.DefaultDimensions && !Mathf.Approximately(item.CurrentSizeMultiplier, 1));
						float SizeModifier = item.CurrentSizeMultiplier;

						if (customDimensions)
						{
							SgtLogger.l("Asteroid has custom size, adjusting geyser spawning rules", item.id);
						}

						foreach (var WorldTemplateRule in settings.world.worldTemplateRules)
						{
							bool isWorldTraitRule = false;
							float baseNumber = 0;
							//These are shared between asteroids, so we need to reset them back to default
							if (ModAssets.TraitTemplateRules.TryGetValue(WorldTemplateRule, out int targetTimesTrait) && WorldTemplateRule.times != targetTimesTrait)
							{
								SgtLogger.l("Resetting trait template rule back to default value for " + WorldTemplateRule.ruleId + "; " + WorldTemplateRule.times + " -> " + targetTimesTrait, item.id);
								WorldTemplateRule.times = targetTimesTrait;
								isWorldTraitRule = true;
								baseNumber = targetTimesTrait;
							}
							else if (OriginalTemplateAmounts.TryGetValue(WorldTemplateRule, out var originalAmount) && WorldTemplateRule.times != originalAmount)
							{
								SgtLogger.l("Resetting world template rule back for " + WorldTemplateRule.names.FirstOrDefault() + "; " + WorldTemplateRule.times + " -> " + originalAmount, item.id);
								WorldTemplateRule.times = originalAmount;
								baseNumber = originalAmount;
							}


							if (customDimensions)
							{
								//if the template places a geyser
								if (WorldTemplateRule.names.Any(name => name.ToUpperInvariant().Contains(geyserKey)))
								{
									//create backup for non-trait rules
									if (!isWorldTraitRule)
									{
										if (!OriginalTemplateAmounts.ContainsKey(WorldTemplateRule))
											OriginalTemplateAmounts[WorldTemplateRule] = WorldTemplateRule.times;
										baseNumber = OriginalTemplateAmounts[WorldTemplateRule];
									}

									if (!Mathf.Approximately(SizeModifier, 1))
									{
										float newGeyserAmount = baseNumber * SizeModifier;
										SgtLogger.l(string.Format("Adjusting geyser roll amount to worldsize for {0}; {1} -> {2}", WorldTemplateRule.names.FirstOrDefault(), baseNumber, newGeyserAmount), item.id);

										float chance = ((float)new KRandom(seed + WorldTemplateRule.names.First().GetHashCode()).Next(100)) / 100f;

										if (newGeyserAmount > 1)
										{
											WorldTemplateRule.times = Mathf.FloorToInt(newGeyserAmount);
											bool hasChanceToRoll = !Mathf.Approximately(newGeyserAmount % 1f, 0f);
											if (hasChanceToRoll)
											{
												SgtLogger.l("new Geyser amount has a chance of " + newGeyserAmount % 1f + " for an additional spawn, rolling...", "CGM WorldgenModifier");
												SgtLogger.l("rolled: " + chance);
											}
											//chance = 0;///always atleast 1
											if (chance <= (newGeyserAmount % 1f))
											{
												SgtLogger.l("roll for additional spawn succeeded: " + chance * 100f, "POI Chance: " + (newGeyserAmount % 1f).ToString("P"));
												WorldTemplateRule.times += 1;
											}
											else
											{
												if (hasChanceToRoll)
													SgtLogger.l("roll for additional spawn failed: " + chance * 100f, "POI Chance: " + (newGeyserAmount % 1f).ToString("P"));
											}

										}
										else
										{
											SgtLogger.l("new Geyser amount below 1, rolling for the geyser to appear at all...");
											SgtLogger.l("rolled: " + chance);
											//chance = 0;///always atleast 1
											if (chance <= newGeyserAmount)
											{
												SgtLogger.l("roll succeeded: " + chance * 100f, "POI Chance: " + (newGeyserAmount % 1f).ToString("P"));
												WorldTemplateRule.times = 1;
											}
											else
											{
												SgtLogger.l("roll failed: " + chance * 100f, "POI Chance: " + (newGeyserAmount % 1f).ToString("P"));
												WorldTemplateRule.times = 0;
											}
										}
										SgtLogger.l("final geyser spawn count: " + WorldTemplateRule.times);
									}
								}
							}

							///Fixed Templates on KleiFest2023 asteroid can only spawn once
							///if it has a fixed position              this part here
							if (WorldTemplateRule.overridePlacement != Vector2I.minusone)
							{
								if (!placementOverridesAdjustments.ContainsKey(WorldTemplateRule))
								{
									SgtLogger.l(WorldTemplateRule.overridePlacement.ToString(), "vanilla override placement");
									placementOverridesAdjustments[WorldTemplateRule] = (WorldTemplateRule.overridePlacement);
								}
								var original = placementOverridesAdjustments[WorldTemplateRule];
								WorldTemplateRule.overridePlacement = new Vector2I(Mathf.RoundToInt(((float)original.X) * (float)item.SizeMultiplierX()), Mathf.RoundToInt(((float)original.Y) * (float)item.SizeMultiplierY()));
								SgtLogger.l(WorldTemplateRule.overridePlacement.ToString(), "adjusted override placement, modifier: " + item.SizeMultiplierX());
								if (WorldTemplateRule.times > 1)
								{
									WorldTemplateRule.times = 1;
								}

								float chance = ((float)new KRandom(seed + WorldTemplateRule.names.First().GetHashCode()).Next(100)) / 100f;
							}
						}
					}
				}
				else
				{
					//apparently not necessary for the OriginalTemplateAmounts, because those get reloaded from the files between worldgens,
					//very necessary for the trait template rules tho.
					SgtLogger.l("resetting any custom values for vanilla generation on " + settings.world.filePath);
					foreach (var WorldTemplateRule in settings.world.worldTemplateRules)
					{
						if (ModAssets.TraitTemplateRules.TryGetValue(WorldTemplateRule, out int targetTimesTrait)
							&& WorldTemplateRule.times != targetTimesTrait
							)
						{
							SgtLogger.l("Resetting world template rule back for traitRule" + WorldTemplateRule.ruleId + "; " + WorldTemplateRule.times + " -> " + targetTimesTrait);
							WorldTemplateRule.times = targetTimesTrait;
						}
						else if (OriginalTemplateAmounts.TryGetValue(WorldTemplateRule, out int targetTimesGeyser)
							&& WorldTemplateRule.times != targetTimesGeyser
							)
						{
							SgtLogger.l("Resetting world template rule back for " + WorldTemplateRule.ruleId + " on " + settings.world.filePath + "; " + WorldTemplateRule.times + " -> " + targetTimesTrait);
							WorldTemplateRule.times = targetTimesTrait;
							OriginalTemplateAmounts.Remove(WorldTemplateRule);
						}
					}
				}
			}
		}
		[HarmonyPatch(typeof(MobSettings), "GetMob")]
		public static class MobSettings_GetMob_Patch
		{
			static Dictionary<string, MinMax> OriginalMobModifiers = new Dictionary<string, MinMax>();


			/// <summary>
			/// increase mob density for smaller worlds
			/// </summary>
			/// <param name="id"></param>
			/// <param name="__result"></param>
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


				float DensityMultiplier = 1f / WorldSizeMultiplier;


				var original = OriginalMobModifiers[prefabID];

				//cap at double the plants & buried objects, dont decrease below original value
				var modifiedMin = Mathf.Max(original.min, Mathf.Min(2, original.min * DensityMultiplier));
				var modifiedMax = Mathf.Max(original.max, Mathf.Min(2, original.max * DensityMultiplier));

				__result.density = new(modifiedMin, modifiedMax);
				//SgtLogger.l("density multiplier for " + prefabID + ": multiplied with " + DensityMultiplier);
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
				SgtLogger.l(clusterName, "VANILLA CLUSTERID");
				//CustomLayout
				if (CGSMClusterManager.LoadCustomCluster)
				{
					SgtLogger.l("Custom ClusterConstructor has started");

					IsGenerating = true;


					///old and not very functional solution to preventing the game from overriding the mixing settings of the custom cluster with the vanilla ruling (which could lead to mixings overriding each other). 
					///mixing is now handled by cgm and prohibited by adding the cgm cluster tag to the prohibited mixing tags of all world mixings

					//world mixing worlds are added to the cluster as regulars, so we disable the mixing setting level to prevent them overriding eachother
					//foreach (var worldMixingSetting in SettingsCache.worldMixingSettings.Values)
					//{
					//	if (worldMixingSetting != null && worldMixingSetting.forbiddenClusterTags != null && !worldMixingSetting.forbiddenClusterTags.Contains(CustomClusterClusterTag))
					//		worldMixingSetting.forbiddenClusterTags.Add(CustomClusterClusterTag);
					//}
					//List<string> ToDisableMixings = new();
					//foreach (var mix in SettingsCache.worldMixingSettings)
					//{
					//	var mixingSetting = SettingsCache.TryGetCachedWorldMixingSetting(mix.Key);
					//	if (mixingSetting != null)
					//	{
					//		ToDisableMixings.Add(mix.Key);
					//		mixingSetting.forbiddenClusterTags.add
					//	}

					//	SgtLogger.l(mix.Key + ": " + mix.Value, "current mixing setting");
					//}
					//foreach (var todisable in ToDisableMixings)
					//{
					//	if(CustomGameSettings.Instance.CurrentMixingLevelsBySetting.TryGetValue(todisable, out var current) && current != WorldMixingSettingConfig.DisabledLevelId)
					//	{
					//		SgtLogger.l("disabling world mixing to prevent double replacement: " + todisable);
					//		CustomGameSettings.Instance.CurrentMixingLevelsBySetting[todisable] = WorldMixingSettingConfig.DisabledLevelId;
					//	}
					//}


					//CustomGameSettings.Instance.CurrentMixingLevelsBySetting["CeresAsteroidMixing"] = WorldMixingSettingConfig.DisabledLevelId;
					//if (DlcManager.IsContentOwned(DlcManager.DLC2_ID) && DlcManager.IsContentSubscribed(DlcManager.DLC2_ID) && CustomCluster.HasCeresAsteroid)
					//{
					//	SgtLogger.l("Enabling Frosty Planet for Custom Cluster");
					//	CustomGameSettings.Instance.CurrentMixingLevelsBySetting["DLC2_ID"] = DlcMixingSettingConfig.EnabledLevelId;
					//}
					//if (CGSMClusterManager.CustomCluster == null)
					//{
					//    ///Generating custom cluster if null
					//    CGSMClusterManager.AddCustomClusterAndInitializeClusterGen();
					//}
					clusterName = CGSMClusterManager.CustomClusterID;
					SgtLogger.l(clusterName, "CGM CLUSTERID");
					IsGenerating = true;
				}
				else
				{
					SgtLogger.l("CGM inactive, vanilla generation will be active");
				}
			}
		}
		//[HarmonyPatch(typeof(WorldgenMixing), nameof(WorldgenMixing.DoWorldMixingInternal))]
		//public static class Worldmixing_Patch
		//{
		//	public static bool Prefix(MutatedClusterLayout mutatedClusterLayout, ref MutatedClusterLayout __result)
		//	{
		//		if (CGSMClusterManager.LoadCustomCluster && ApplyCustomGen.IsGenerating)
		//		{
		//			__result = mutatedClusterLayout;
		//			return false;
		//		}

		//		return true;

		//	}
		//}

		[HarmonyPatch(typeof(Cluster), nameof(Cluster.InitializeWorlds))]
		public class Cluster_InitializeWorlds_Patch
		{
			public static void Prefix()
			{
				if (!CGSMClusterManager.LoadCustomCluster)
					return;
				SgtLogger.l("Preconsuming CGM Mixings");

			}
			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{
				var codes = orig.ToList();

				var wasMixedField = AccessTools.Field(typeof(WorldPlacement.WorldMixing), "mixingWasApplied");
				var worldMixingGetter = AccessTools.PropertyGetter(typeof(WorldPlacement), "worldMixing");


				// find injection point
				var index = codes.FindIndex(ci => ci.Calls(worldMixingGetter));

				if (index == -1)
				{
					SgtLogger.error("INITIALIZEWORLDS TRANSPILER FAILED!");
					return codes;
				}

				var m_InjectedMethod = AccessTools.DeclaredMethod(typeof(Cluster_InitializeWorlds_Patch), "InjectedMethod");

				// inject right at the found index
				codes.InsertRange(index,
				[
							new CodeInstruction(OpCodes.Call, m_InjectedMethod)
						]);

				return codes;
			}

			private static WorldPlacement InjectedMethod(WorldPlacement placement)
			{
				if (!CGSMClusterManager.LoadCustomCluster && ApplyCustomGen.IsGenerating)
					return placement;


				SgtLogger.l("checking if mixing world: " + placement.world);
				if (!placement.worldMixing.mixingWasApplied && CGSMClusterManager.IsWorldMixingAsteroid(placement.world))
				{
					SgtLogger.l("mixing asteroid found!: " + placement.world);
					placement.worldMixing.mixingWasApplied = true;
				}

				return placement;
			}
		}


		//[HarmonyPatch(typeof(WorldgenMixing), nameof(WorldgenMixing.DoWorldMixingInternal))]
		//public static class Worldmixing_Patch
		//{
		//	public static bool Prefix(MutatedClusterLayout mutatedClusterLayout, int seed)
		//	{
		//		return !CGSMClusterManager.LoadCustomCluster;

		//          }
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
