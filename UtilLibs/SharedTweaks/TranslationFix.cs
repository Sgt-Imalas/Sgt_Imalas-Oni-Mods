using HarmonyLib;
using Klei.CustomSettings;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static global::STRINGS.ROOMS;

namespace UtilLibs.SharedTweaks
{
	public class TranslationFix : PForwardedComponent
	{
		public static void Register()
		{
			new TranslationFix().RegisterForForwarding();
		}
		public override Version Version => new Version(1, 0, 0, 0);

		public override void Initialize(Harmony plibInstance)
		{
			try
			{
				var targetMethod = AccessTools.Method(typeof(Db), nameof(Db.Initialize));
				var postfix = AccessTools.Method(typeof(TranslationFix), nameof(TranslationFix.Postfix));
				plibInstance.Patch(targetMethod, postfix: new(postfix));
				Debug.Log(Assembly.GetExecutingAssembly().GetName().Name + ": "+this.GetType().ToString() + " successfully patched");
			}
			catch (Exception e)
			{
				Debug.LogWarning(this.GetType().ToString() + " patch failed!");
				Debug.LogWarning(e.Message);
			}
		}
		/// <summary>
		/// fixes translations of things falling back to english due to loading too early from mods
		/// </summary>
		public static void Postfix()
		{
			if (Localization.GetSelectedLanguageType() == Localization.SelectedLanguageType.None)
				return;

			FixSettingsTranslations();
			FixTraitTranslations();
			FixRoomConstrains();
		}


		static void FixSettingsTranslations()
		{
			ReapplyTranslatedSettingStrings(CustomGameSettingConfigs.WorldgenSeed, "WORLDGEN_SEED");
			ReapplyTranslatedSettingStrings(CustomGameSettingConfigs.ClusterLayout, "CLUSTER_CHOICE");
			ReapplyTranslatedSettingStrings(CustomGameSettingConfigs.SandboxMode);
			ReapplyTranslatedSettingStrings(CustomGameSettingConfigs.FastWorkersMode);
			ReapplyTranslatedSettingStrings(CustomGameSettingConfigs.SaveToCloud);
			ReapplyTranslatedSettingStrings(CustomGameSettingConfigs.CalorieBurn, "CALORIE_BURN");
			ReapplyTranslatedSettingStrings(CustomGameSettingConfigs.BionicWattage, "BIONICPOWERUSE");
			ReapplyTranslatedSettingStrings(CustomGameSettingConfigs.ImmuneSystem);
			ReapplyTranslatedSettingStrings(CustomGameSettingConfigs.Morale);
			ReapplyTranslatedSettingStrings(CustomGameSettingConfigs.Durability);
			ReapplyTranslatedSettingStrings(CustomGameSettingConfigs.Radiation);
			ReapplyTranslatedSettingStrings(CustomGameSettingConfigs.Stress);
			ReapplyTranslatedSettingStrings(CustomGameSettingConfigs.StressBreaks, "STRESS_BREAKS");
			ReapplyTranslatedSettingStrings(CustomGameSettingConfigs.CarePackages);
			ReapplyTranslatedSettingStrings(CustomGameSettingConfigs.Teleporters);
			ReapplyTranslatedSettingStrings(CustomGameSettingConfigs.MeteorShowers, null, new() { { "ClearSkies", "CLEAR_SKIES" } });
			ReapplyTranslatedSettingStrings(CustomGameSettingConfigs.DemoliorDifficulty);
		}

		const string settingLevelsKey = "LEVELS.";
		const string generalSettingsRoot = "STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.";
		static void ReapplyTranslatedSettingStrings(SettingConfig config, string settingsStringId = null, Dictionary<string, string> LevelIdOverrides = null)
		{
			if (settingsStringId == null)
				settingsStringId = config.id;

			settingsStringId = settingsStringId.ToUpperInvariant() + ".";

			if (TryGetTranslatedString(generalSettingsRoot + settingsStringId + "NAME", out var configLabel))
				config.label = configLabel;
			if (TryGetTranslatedString(generalSettingsRoot + settingsStringId + "TOOLTIP", out var configTooltip))
				config.tooltip = configTooltip;
			if (config is ToggleSettingConfig toggleSetting)
			{
				List<SettingLevel> levels = [toggleSetting.off_level, toggleSetting.on_level];

				foreach (var level in levels)
				{
					string labelId = level.id.ToUpperInvariant() + ".";
					if (LevelIdOverrides != null && LevelIdOverrides.TryGetValue(level.id, out var idOverride))
					{
						labelId = idOverride.ToUpperInvariant() + ".";
					}
					if (TryGetTranslatedString(generalSettingsRoot + settingsStringId + settingLevelsKey + labelId + "NAME", out var levelLabel))
						level.label = levelLabel;
					if (TryGetTranslatedString(generalSettingsRoot + settingsStringId + settingLevelsKey + labelId + "TOOLTIP", out var levelTooltip))
						level.tooltip = levelTooltip;
				}
			}

			else if (config is ListSettingConfig listSettingConfig && listSettingConfig.levels != null && listSettingConfig.levels.Any())
			{
				foreach (var level in listSettingConfig.levels)
				{
					string labelId = level.id.ToUpperInvariant() + ".";
					if (LevelIdOverrides != null && LevelIdOverrides.TryGetValue(level.id, out var idOverride))
					{
						labelId = idOverride.ToUpperInvariant() + ".";
					}
					if (TryGetTranslatedString(generalSettingsRoot + settingsStringId + settingLevelsKey + labelId + "NAME", out var levelLabel))
						level.label = levelLabel;
					if (TryGetTranslatedString(generalSettingsRoot + settingsStringId + settingLevelsKey + labelId + "TOOLTIP", out var levelTooltip))
						level.tooltip = levelTooltip;
				}
			}
		}


		/// <summary>
		/// fixes translations of traits
		/// </summary>
		static void FixTraitTranslations()
		{
			foreach (var trait in Db.Get().traits.resources)
			{
				string traitId = trait.Id.ToUpperInvariant();
				//skip base traits of critters and dupes
				if (traitId.Contains("BASE"))
					continue;

				///usually traits are under this structure
				string nameStringKey = $"STRINGS.DUPLICANTS.TRAITS.{traitId}.NAME";
				string descStringKey = $"STRINGS.DUPLICANTS.TRAITS.{traitId}.DESC";
				if (!Strings.TryGet(nameStringKey, out _))
				{
					//need traits are under here
					nameStringKey = $"STRINGS.DUPLICANTS.TRAITS.NEEDS.{traitId}.NAME";
					descStringKey = $"STRINGS.DUPLICANTS.TRAITS.NEEDS.{traitId}.DESC";
					if (!Strings.TryGet(nameStringKey, out _))
					{
						//usually unusued, but old congenitals are here
						nameStringKey = $"STRINGS.DUPLICANTS.TRAITS.CONGENITALTRAITS.{traitId}.NAME";
						//skip the trait if we dont find traits at any of these locations
						if (!Strings.TryGet(nameStringKey, out _))
							continue;
						descStringKey = $"STRINGS.DUPLICANTS.TRAITS.CONGENITALTRAITS.{traitId}.DESC";
					}
				}
				if (TryGetTranslatedString(nameStringKey, out var translatedName))
					trait.Name = translatedName;
				if (TryGetTranslatedString(descStringKey, out var translatedDesc))
					trait.description = translatedDesc;
			}
		}

		static bool TryGetTranslatedString(string key, out StringEntry translated) => Strings.TryGet(key, out translated);

		/// <summary>
		/// Rebuild those strings bc they didn't translate from loading the class to early..
		/// </summary>
		public static void FixRoomConstrains()
		{
			try
			{
				//SgtLogger.l("fixing room constraint strings");
				RoomConstraints.CEILING_HEIGHT_4.name = string.Format(CRITERIA.CEILING_HEIGHT.NAME, "4");
				RoomConstraints.CEILING_HEIGHT_4.description = string.Format(CRITERIA.CEILING_HEIGHT.DESCRIPTION, "4");
				RoomConstraints.CEILING_HEIGHT_6.name = string.Format(CRITERIA.CEILING_HEIGHT.NAME, "6");
				RoomConstraints.CEILING_HEIGHT_6.description = string.Format(CRITERIA.CEILING_HEIGHT.DESCRIPTION, "6");
				RoomConstraints.MINIMUM_SIZE_12.name = string.Format(CRITERIA.MINIMUM_SIZE.NAME, "12");
				RoomConstraints.MINIMUM_SIZE_12.description = string.Format(CRITERIA.MINIMUM_SIZE.DESCRIPTION, "12");
				RoomConstraints.MINIMUM_SIZE_24.name = string.Format(CRITERIA.MINIMUM_SIZE.NAME, "24");
				RoomConstraints.MINIMUM_SIZE_24.description = string.Format(CRITERIA.MINIMUM_SIZE.DESCRIPTION, "24");
				RoomConstraints.MINIMUM_SIZE_32.name = string.Format(CRITERIA.MINIMUM_SIZE.NAME, "32");
				RoomConstraints.MINIMUM_SIZE_32.description = string.Format(CRITERIA.MINIMUM_SIZE.DESCRIPTION, "32");
				RoomConstraints.MAXIMUM_SIZE_64.name = string.Format(CRITERIA.MAXIMUM_SIZE.NAME, "64");
				RoomConstraints.MAXIMUM_SIZE_64.description = string.Format(CRITERIA.MAXIMUM_SIZE.DESCRIPTION, "64");
				RoomConstraints.MAXIMUM_SIZE_96.name = string.Format(CRITERIA.MAXIMUM_SIZE.NAME, "96");
				RoomConstraints.MAXIMUM_SIZE_96.description = string.Format(CRITERIA.MAXIMUM_SIZE.DESCRIPTION, "96");
				RoomConstraints.MAXIMUM_SIZE_120.name = string.Format(CRITERIA.MAXIMUM_SIZE.NAME, "120");
				RoomConstraints.MAXIMUM_SIZE_120.description = string.Format(CRITERIA.MAXIMUM_SIZE.DESCRIPTION, "120");
				RoomConstraints.NO_INDUSTRIAL_MACHINERY.name = CRITERIA.NO_INDUSTRIAL_MACHINERY.NAME;
				RoomConstraints.NO_INDUSTRIAL_MACHINERY.description = CRITERIA.NO_INDUSTRIAL_MACHINERY.DESCRIPTION;
				RoomConstraints.NO_COTS.name = CRITERIA.NO_COTS.NAME;
				RoomConstraints.NO_COTS.description = CRITERIA.NO_COTS.DESCRIPTION;
				RoomConstraints.NO_LUXURY_BEDS.name = CRITERIA.NO_COTS.NAME; RoomConstraints.NO_LUXURY_BEDS.description = CRITERIA.NO_COTS.DESCRIPTION;
				RoomConstraints.NO_OUTHOUSES.name = CRITERIA.NO_OUTHOUSES.NAME; RoomConstraints.NO_OUTHOUSES.description = CRITERIA.NO_OUTHOUSES.DESCRIPTION;
				RoomConstraints.NO_MESS_STATION.name = CRITERIA.NO_MESS_STATION.NAME; RoomConstraints.NO_MESS_STATION.description = CRITERIA.NO_MESS_STATION.DESCRIPTION;
				RoomConstraints.HAS_LUXURY_BED.name = CRITERIA.HAS_LUXURY_BED.NAME; RoomConstraints.HAS_LUXURY_BED.description = CRITERIA.HAS_LUXURY_BED.DESCRIPTION;
				RoomConstraints.HAS_BED.name = CRITERIA.HAS_BED.NAME; RoomConstraints.HAS_BED.description = CRITERIA.HAS_BED.DESCRIPTION;
				RoomConstraints.SCIENCE_BUILDINGS.name = CRITERIA.SCIENCE_BUILDINGS.NAME; RoomConstraints.SCIENCE_BUILDINGS.description = CRITERIA.SCIENCE_BUILDINGS.DESCRIPTION;
				RoomConstraints.BED_SINGLE.name = CRITERIA.BED_SINGLE.NAME; RoomConstraints.BED_SINGLE.description = CRITERIA.BED_SINGLE.DESCRIPTION;
				RoomConstraints.LUXURY_BED_SINGLE.name = CRITERIA.LUXURYBEDTYPE.NAME; RoomConstraints.LUXURY_BED_SINGLE.description = CRITERIA.LUXURYBEDTYPE.DESCRIPTION;
				RoomConstraints.BUILDING_DECOR_POSITIVE.name = CRITERIA.BUILDING_DECOR_POSITIVE.NAME; RoomConstraints.BUILDING_DECOR_POSITIVE.description = CRITERIA.BUILDING_DECOR_POSITIVE.DESCRIPTION;
				RoomConstraints.DECORATIVE_ITEM.name = string.Format(CRITERIA.DECORATIVE_ITEM.NAME, 1); RoomConstraints.DECORATIVE_ITEM.description = string.Format(CRITERIA.DECORATIVE_ITEM.DESCRIPTION, 1);
				RoomConstraints.DECORATIVE_ITEM_2.name = string.Format(CRITERIA.DECORATIVE_ITEM.NAME, 2); RoomConstraints.DECORATIVE_ITEM_2.description = string.Format(CRITERIA.DECORATIVE_ITEM.DESCRIPTION, 2);
				RoomConstraints.POWER_STATION.name = CRITERIA.POWERPLANT.NAME; RoomConstraints.POWER_STATION.description = CRITERIA.POWERPLANT.DESCRIPTION;
				RoomConstraints.FARM_STATION.name = CRITERIA.FARMSTATIONTYPE.NAME; RoomConstraints.FARM_STATION.description = CRITERIA.FARMSTATIONTYPE.DESCRIPTION;
				RoomConstraints.RANCH_STATION.name = CRITERIA.RANCHSTATIONTYPE.NAME; RoomConstraints.RANCH_STATION.description = CRITERIA.RANCHSTATIONTYPE.DESCRIPTION;
				RoomConstraints.SPICE_STATION.name = CRITERIA.SPICESTATION.NAME; RoomConstraints.SPICE_STATION.description = CRITERIA.SPICESTATION.DESCRIPTION;
				RoomConstraints.COOK_TOP.name = CRITERIA.COOKTOP.NAME; RoomConstraints.COOK_TOP.description = CRITERIA.COOKTOP.DESCRIPTION;
				RoomConstraints.KITCHENREFRIGERATOR.name = CRITERIA.KITCHENREFRIGERATOR.NAME; RoomConstraints.KITCHENREFRIGERATOR.description = CRITERIA.KITCHENREFRIGERATOR.DESCRIPTION;
				RoomConstraints.REC_BUILDING.name = CRITERIA.RECBUILDING.NAME; RoomConstraints.REC_BUILDING.description = CRITERIA.RECBUILDING.DESCRIPTION;
				RoomConstraints.MACHINE_SHOP.name = CRITERIA.MACHINESHOPTYPE.NAME; RoomConstraints.MACHINE_SHOP.description = CRITERIA.MACHINESHOPTYPE.DESCRIPTION;
				RoomConstraints.DESTRESSING_BUILDING.name = CRITERIA.DESTRESSINGBUILDING.NAME; RoomConstraints.DESTRESSING_BUILDING.description = CRITERIA.DESTRESSINGBUILDING.DESCRIPTION;
				RoomConstraints.MASSAGE_TABLE.name = CRITERIA.MASSAGE_TABLE.NAME; RoomConstraints.MASSAGE_TABLE.description = CRITERIA.MASSAGE_TABLE.DESCRIPTION;
				RoomConstraints.DINING_TABLE.name = CRITERIA.DININGTABLETYPE.NAME; RoomConstraints.DINING_TABLE.description = CRITERIA.DININGTABLETYPE.DESCRIPTION;
				RoomConstraints.MESS_STATION_SINGLE.name = CRITERIA.NO_BASIC_MESS_STATIONS.NAME; RoomConstraints.MESS_STATION_SINGLE.description = CRITERIA.DININGTABLETYPE.DESCRIPTION;
				RoomConstraints.MULTI_MINION_DINING_TABLE.name = CRITERIA.MULTI_MINION_DINING_TABLE.NAME; RoomConstraints.MULTI_MINION_DINING_TABLE.description = CRITERIA.MULTI_MINION_DINING_TABLE.DESCRIPTION;
				RoomConstraints.TOILET.name = CRITERIA.TOILETTYPE.NAME; RoomConstraints.TOILET.description = CRITERIA.TOILETTYPE.DESCRIPTION;
				RoomConstraints.FLUSH_TOILET.name = CRITERIA.FLUSHTOILETTYPE.NAME; RoomConstraints.FLUSH_TOILET.description = CRITERIA.FLUSHTOILETTYPE.DESCRIPTION;
				RoomConstraints.WASH_STATION.name = CRITERIA.WASHSTATION.NAME; RoomConstraints.WASH_STATION.description = CRITERIA.WASHSTATION.DESCRIPTION;
				RoomConstraints.ADVANCEDWASHSTATION.name = CRITERIA.ADVANCEDWASHSTATION.NAME; RoomConstraints.ADVANCEDWASHSTATION.description = CRITERIA.ADVANCEDWASHSTATION.DESCRIPTION;
				RoomConstraints.CLINIC.name = CRITERIA.CLINIC.NAME; RoomConstraints.CLINIC.description = CRITERIA.CLINIC.DESCRIPTION;
				RoomConstraints.PARK_BUILDING.name = CRITERIA.PARK.NAME; RoomConstraints.PARK_BUILDING.description = CRITERIA.PARK.DESCRIPTION;
				RoomConstraints.IS_BACKWALLED.name = CRITERIA.IS_BACKWALLED.NAME; RoomConstraints.IS_BACKWALLED.description = CRITERIA.IS_BACKWALLED.DESCRIPTION;
				RoomConstraints.WILDANIMAL.name = CRITERIA.WILDANIMAL.NAME; RoomConstraints.WILDANIMAL.description = CRITERIA.WILDANIMAL.DESCRIPTION;
				RoomConstraints.WILDANIMALS.name = CRITERIA.WILDANIMALS.NAME; RoomConstraints.WILDANIMALS.description = CRITERIA.WILDANIMALS.DESCRIPTION;
				RoomConstraints.WILDPLANT.name = CRITERIA.WILDPLANT.NAME; RoomConstraints.WILDPLANT.description = CRITERIA.WILDPLANT.DESCRIPTION;
				RoomConstraints.WILDPLANTS.name = CRITERIA.WILDPLANTS.NAME; RoomConstraints.WILDPLANTS.description = CRITERIA.WILDPLANTS.DESCRIPTION;
				RoomConstraints.ORNAMENTDISPLAYED.name = CRITERIA.ORNAMENT.NAME; RoomConstraints.ORNAMENTDISPLAYED.description = CRITERIA.ORNAMENT.DESCRIPTION;
			}
			catch (Exception e)
			{
				Debug.LogWarning(e);
			}
		}
	}
}
