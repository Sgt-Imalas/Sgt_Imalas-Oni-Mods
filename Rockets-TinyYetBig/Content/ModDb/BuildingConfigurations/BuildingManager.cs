using HarmonyLib;
using Rockets_TinyYetBig.Content.ModDb.BuildingConfigurations.BuildingConfigInterfaces;
using System;
using System.Collections.Generic;
using UtilLibs;
using static GameUtil;

namespace Rockets_TinyYetBig.Content.ModDb.BuildingConfigurations
{
	static class BuildingManager
	{
		public static Dictionary<string, BuildingInjectionEntry> BuildingInjections = new Dictionary<string, BuildingInjectionEntry>();
		public static HashSet<string> DisabledBuildingIDs = [];

		public static BuildingConfigurationCollection ConfigCollection = new BuildingConfigurationCollection();
		public static BuildingConfigurationEntry AddOrGetEntry(string buildingID)
		{


			if (ConfigCollection.BuildingConfigurations.TryGetValue(buildingID, out var entry))
			{
				return entry;
			}
			else
			{
				var newEntry = new BuildingConfigurationEntry { BuildingID = buildingID };
				ConfigCollection.BuildingConfigurations[buildingID] = newEntry;
				return newEntry;
			}
		}
		public static BuildingInjectionEntry CreateEntry<T>()
		{
			var buildingType = typeof(T);
			string buildingId = null;
			if (typeof(IRocketModuleUpgradeBuilding).IsAssignableFrom(buildingType))
			{
				var moduleInfo = (IRocketModuleUpgradeBuilding)Activator.CreateInstance(buildingType);
				if (moduleInfo != default)
				{
					buildingId = moduleInfo.GetID();
				}
			}
			else
			{
				var IdField = AccessTools.Field(buildingType, "ID") ?? throw new ArgumentException($"Building type {buildingType.Name} does not have a static ID field.");

				if (!IdField.IsStatic)
					throw new ArgumentException($"Field ID on {buildingType.Name} is not static.");

				if (IdField.FieldType != typeof(string))
					throw new ArgumentException($"Field ID on {buildingType.Name} is not a string.");

				buildingId = (string)IdField.GetValue(null);
			}

			if (buildingId.IsNullOrWhiteSpace())
			{
				SgtLogger.error("Could not create building entry from type " + buildingType.Name);
				return null;
			}

			BuildingConfigurationEntry entry = AddOrGetEntry(buildingId);
			if (typeof(IHasConfigurableWattage).IsAssignableFrom(buildingType))
			{
				var wattageConfigurator = (IHasConfigurableWattage)Activator.CreateInstance(buildingType);
				if (wattageConfigurator != default)
				{
					//set the default value in the config class
					entry.SetDefaultWattage(wattageConfigurator.GetWattage(), wattageConfigurator.GetLimitsWattage());
					//apply the config wattage to the entry; if its user set, it will override the default
					wattageConfigurator.SetWattage(entry.GetWattage());
				}
			}
			if (typeof(IHasConfigurableStorageCapacity).IsAssignableFrom(buildingType))
			{
				var storageConfigurator = (IHasConfigurableStorageCapacity)Activator.CreateInstance(buildingType);
				if (storageConfigurator != default)
				{
					//set the default value in the config class

					entry.StorageUnitLabel = storageConfigurator.StorageUnitLabel();
					entry.StorageToolTip = storageConfigurator.GetStorageTooltip();
					entry.StorageLabel = storageConfigurator.GetStorageLabel();

					entry.SetDefaultStorageCapacity(storageConfigurator.GetStorageCapacity(), storageConfigurator.GetLimitsStorage());
					//apply the config wattage to the entry; if its user set, it will override the default
					storageConfigurator.SetStorageCapacity(entry.GetStorageCapacity());
				}
			}
			//if (typeof(IHasConfigurableRateMultiplier).IsAssignableFrom(buildingType))
			//{
			//	var generatorMultiplier = (IHasConfigurableRateMultiplier)Activator.CreateInstance(buildingType);
			//	if (generatorMultiplier != default)
			//	{
			//		//set the default value in the config class
			//		entry.SetDefaultMultiplier(generatorMultiplier.GetDefaultMultiplier());
			//		entry.SetRateStrings(generatorMultiplier.GetRateLabel(), generatorMultiplier.GetCurrentRateDescription());
			//		generatorMultiplier.SetMultiplier(entry.GetRateMultiplier());
			//	}
			//}
			if (typeof(IGeneratorBuilding).IsAssignableFrom(buildingType))
			{
				entry.SetIsGenerator(true);
			}
			if (typeof(IHasConfigurableRange).IsAssignableFrom(buildingType))
			{
				var rangeConfigurator = (IHasConfigurableRange)Activator.CreateInstance(buildingType);
				if (rangeConfigurator != default)
				{
					entry.RangeLabel = rangeConfigurator.GetDescriptorTextRange();
					entry.RangeUnitLabel = rangeConfigurator.RangeUnitLabel();
					entry.RangeToolTip = rangeConfigurator.RangeTooltip();
					entry.SetDefaultTileRange(rangeConfigurator.GetTileRange(), rangeConfigurator.GetLimitsRange());

					rangeConfigurator.SetTileRange(entry.GetTileRange());
				}
			}
			bool allowedByDlc = true;
			if (typeof(IHasDlcRestrictions).IsAssignableFrom(buildingType))
			{
				var dlcRestrictions = (IHasDlcRestrictions)Activator.CreateInstance(buildingType);
				if (dlcRestrictions != default)
				{
					allowedByDlc = DlcManager.IsCorrectDlcSubscribed(dlcRestrictions);
				}
			}
			BuildingInjectionEntry injection = BuildingInjectionEntry.Create(buildingId);
			entry.SetInjection(injection);
			if (allowedByDlc)
				BuildingInjections.Add(buildingId, injection);
			return injection;
		}

		internal static void LoadConfigFile()
		{
			SgtLogger.l("Loading Building Config file");
			ConfigCollection = BuildingConfigurationCollection.LoadFromFile();
		}

		static bool buildingsRegistered = false;
		static void InitRegistration()
		{
			if (buildingsRegistered)
				return;
			SgtLogger.l("REGISTER BUILDINGS - INIT");
			BuildingDatabase.RegisterBuildings();
			buildingsRegistered = true;
		}

		public static bool TechExists(string techID)
		{
			if (techID.IsNullOrWhiteSpace())
				return false;

			return Db.Get().Techs.Exists(techID);
		}

		public static bool BuildingEnabled(string buildingId) => BuildingInjections != null && BuildingInjections.TryGetValue(buildingId, out var entry) && entry.Source.IsBuildingEnabled() && TechExists(entry.TechID);

		public static void AddBuildingsToPlanScreen()
		{
			SgtLogger.l("AddingBuildingsToPlanScreen Register");
			InitRegistration();
			foreach (var entry in BuildingInjections)
			{
				if (entry.Value.Source.IsBuildingEnabled()
					//	&& TechExists(entry.Value.TechID)
					)
					entry.Value.RegisterPlanscreen();
				else
				{
					string id = entry.Value.Source.BuildingID;
					DisabledBuildingIDs.Add(id);
					var def = Assets.GetBuildingDef(id);
					def.DebugOnly = true;
					CodexUtils.HideFromCodex(def.BuildingComplete);
				}
			}
		}
		public static void AddBuildingsToTechs()
		{
			SgtLogger.l("AddingBuildingsToTech Register");
			InitRegistration();
			foreach (var entry in BuildingInjections)
			{
				if (entry.Value.Source.IsBuildingEnabled() && TechExists(entry.Value.TechID))
					entry.Value.RegisterTech();
			}
		}
		internal static void ResetConfigChanges()
		{
			foreach (var config in ConfigCollection.BuildingConfigurations.Values)
			{
				config.ResetChanges();
			}
			ConfigCollection.WriteToFile();
		}
		internal static void ResetSingleConfig(BuildingConfigurationEntry target)
		{
			if (ConfigCollection.BuildingConfigurations.TryGetValue(target.BuildingID, out var cfg))
			{
				cfg.ResetChanges();
				cfg.SetBuildingEnabled(true);
				ConfigCollection.WriteToFile();
			}
		}
	}
}
