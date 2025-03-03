using Klei;
using KMod;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig._ModuleConfig
{
	internal class ModuleConfigManager
	{
		[JsonIgnore] static string ModuleConfigPath;
		[JsonIgnore] public const string ConfigFileNameModules = "RocketModuleConfiguration";
		[JsonIgnore] public static ModuleConfigManager Instance;

		[JsonIgnore] public Dictionary<string, ModuleSettingSerializable> OriginalModuleDefinitions;
		public Dictionary<string, ModuleSettingSerializable> CustomModuleDefinitions;

		public ModuleConfigManager() { }
		public static void Init()
		{
			if (Instance != null)
				return;

			InitializeFolderPath();
			var path = Path.Combine(ModuleConfigPath, ConfigFileNameModules + ".json");
			if (!IO_Utils.ReadFromFile<ModuleConfigManager>(path, out Instance))
			{
				Instance = new ModuleConfigManager();
				Instance.OriginalModuleDefinitions = new Dictionary<string, ModuleSettingSerializable>();
				Instance.CustomModuleDefinitions = new Dictionary<string, ModuleSettingSerializable>();
			}
		}

		public static void InitializeFolderPath()
		{

			SgtLogger.debuglog("Initializing file paths..");
			ModuleConfigPath = FileSystem.Normalize(Path.Combine(Path.Combine(Manager.GetDirectory(), "config"), "RocketryExpanded"));

			SgtLogger.debuglog("Initializing folders..");
			try
			{
				System.IO.Directory.CreateDirectory(ModuleConfigPath);
			}
			catch (Exception e)
			{
				SgtLogger.error("Could not create folder, Exception:\n" + e);
			}
			SgtLogger.log("Folders succesfully initialized");
		}
		ModuleSettingSerializable preparedItem;


		internal void FinalizeRegistration(BuildingDef def)
		{
			OriginalModuleDefinitions[def.PrefabID] = preparedItem;
		}
		internal void PrepareBuildingDefForRegistration(BuildingDef def)
		{
			if (def == null)
				return;
			preparedItem = new ModuleSettingSerializable(def.PrefabID);
			preparedItem.Width = def.WidthInCells;
			preparedItem.Height = def.HeightInCells;
		}
		public void RegisterGenerators(BuildingDef def)
		{
			if (OriginalModuleDefinitions.ContainsKey(def.PrefabID))
			{
				OriginalModuleDefinitions[def.PrefabID].ModuleGeneratorCapacity = (int)def.GeneratorBaseCapacity;
				OriginalModuleDefinitions[def.PrefabID].ModuleGeneratorWattage = (int)def.GeneratorWattageRating;
			}
		}


		internal void AddOriginalModuleDefinitions(GameObject template, int burden, float enginePower, float fuelCostPerDistance)
		{
			string Id = template.GetComponent<Building>().Def.PrefabID;
			if (OriginalModuleDefinitions.ContainsKey(Id))
			{
				var toModify = OriginalModuleDefinitions[Id];
				toModify.ModuleWeight = burden;
				toModify.ModuleEnginePower = (int)enginePower;
				toModify.FuelCostPerDistance = fuelCostPerDistance;
				if (template.TryGetComponent<RocketEngineCluster>(out var engineCluster))
				{

					toModify.MaximumRocketHeight = engineCluster.maxHeight;

					if (engineCluster.fuelTag == ModAssets.Tags.RocketFuelTag)
						toModify.AllowFuelTanks = true;
				}
			}

		}

		internal void LoadCustomValuesForModuleBuildingDef(BuildingDef def)
		{
			if (CustomModuleDefinitions.ContainsKey(def.PrefabID))
			{
				ModuleSettingSerializable ValuesToLoad = CustomModuleDefinitions[def.PrefabID];

				def.WidthInCells = ValuesToLoad.Width;
				def.HeightInCells = ValuesToLoad.Height;

				if (ValuesToLoad.ModuleEnginePower > 0)
				{
					def.GeneratorWattageRating = ValuesToLoad.ModuleEnginePower;
					def.GeneratorBaseCapacity = ValuesToLoad.ModuleGeneratorCapacity > 0 ? ValuesToLoad.ModuleGeneratorCapacity : ValuesToLoad.ModuleEnginePower * 30f;
				}

				def.GenerateOffsets();
			}
		}
		internal void LoadCustomValuesForRocketModuleDefinition(GameObject template, ref int burden, ref float enginePower, ref float fuelCostPerDistance)
		{
			string id = template.GetComponent<Building>().Def.PrefabID;

			if (CustomModuleDefinitions.ContainsKey(id))
			{
				var toReadValuesFrom = CustomModuleDefinitions[id];
				burden = toReadValuesFrom.ModuleWeight;
				enginePower = toReadValuesFrom.ModuleEnginePower;
				fuelCostPerDistance = toReadValuesFrom.FuelCostPerDistance;
			}
		}


		public class ModuleSettingSerializable
		{
			public ModuleSettingSerializable(string id)
			{
				BuildingID = id;
			}

			public bool ModuleIsEnabled = true;

			public string BuildingID;
			public int Width, Height, ModuleWeight, ModuleEnginePower, ModuleGeneratorWattage, ModuleGeneratorCapacity, MaximumRocketHeight;
			public float FuelCostPerDistance;

			public bool AllowFuelTanks = false;

			public ModuleSettingSerializable GetClone()
			{
				var newSetting = new ModuleSettingSerializable(this.BuildingID);
				newSetting.Width = this.Width;
				newSetting.Height = this.Height;
				newSetting.ModuleWeight = this.ModuleWeight;
				newSetting.ModuleEnginePower = this.ModuleEnginePower;
				newSetting.ModuleGeneratorWattage = this.ModuleGeneratorWattage;
				newSetting.ModuleGeneratorCapacity = this.ModuleGeneratorCapacity;
				newSetting.FuelCostPerDistance = this.FuelCostPerDistance;
				newSetting.MaximumRocketHeight = this.MaximumRocketHeight;
				newSetting.AllowFuelTanks = this.AllowFuelTanks;
				return newSetting;
			}

		}

	}
}
