using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb.BuildingConfigurations;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.Buildings.ConfigInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	static class BuildingManager
	{
		public static Dictionary<string, BuildingInjectionEntry> BuildingInjections = new Dictionary<string, BuildingInjectionEntry>();
		public static HashSet<string> DisabledBuildingIDs = new HashSet<string>();

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

			var IdField = AccessTools.Field(buildingType, "ID");
			if(IdField == null)
			{
				throw new ArgumentException($"Building type {buildingType.Name} does not have a static ID field.");
			}
			var buildingId = (string)IdField.GetValue(null);

			BuildingConfigurationEntry entry = AddOrGetEntry(buildingId);
			if (typeof(IHasConfigurableWattage).IsAssignableFrom(buildingType))
			{
				var wattageConfigurator = (IHasConfigurableWattage)Activator.CreateInstance(buildingType);
				if(wattageConfigurator != default)
				{
					//set the default value in the config class
					entry.SetDefaultWattage(wattageConfigurator.GetWattage());
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
					entry.SetDefaultStorageCapacity(storageConfigurator.GetStorageCapacity());
					//apply the config wattage to the entry; if its user set, it will override the default
					storageConfigurator.SetStorageCapacity(entry.GetStorageCapacity());
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

			//#if DEBUG
			//			if (typeof(IBuildingConfig).IsAssignableFrom(buildingType))
			//			{
			//				var buildingConfig = (IBuildingConfig)Activator.CreateInstance(buildingType);
			//				if (buildingConfig != default)
			//				{
			//					var buildingDef = buildingConfig.CreateBuildingDef();
			//					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(BuildingConfigManager.Instance.baseTemplate);
			//					KPrefabID component = gameObject.GetComponent<KPrefabID>();
			//					component.PrefabTag = buildingDef.Tag;
			//					component.SetDlcRestrictions((IHasDlcRestrictions)buildingDef);
			//					gameObject.GetComponent<Building>().Def = buildingDef;
			//					gameObject.GetComponent<OccupyArea>().SetCellOffsets(buildingDef.PlacementOffsets);
			//					gameObject.AddTag(GameTags.RoomProberBuilding);

			//					buildingConfig.ConfigureBuildingTemplate(gameObject, buildingDef.Tag);

			//					List<ElementConverter.ConsumedElement> Consumes = [];
			//					List<ElementConverter.OutputElement> Creates = [];

			//					foreach (var converter in gameObject.GetComponents<ElementConverter>()) 
			//					{
			//						Consumes.AddRange(converter.consumedElements);
			//						Creates.AddRange(converter.outputElements);
			//					}
			//					UnityEngine.Object.Destroy(gameObject);
			//				}
			//			}
			//#endif
			BuildingInjectionEntry injection = BuildingInjectionEntry.Create(buildingId);
			entry.SetInjection(injection);

			if (entry.IsBuildingEnabled() && allowedByDlc)
				BuildingInjections.Add(buildingId, injection);
			return injection;
		}

		internal static void LoadConfigFile()
		{
			ConfigCollection = BuildingConfigurationCollection.LoadFromFile();
		}
		public static void AddBuildingsToPlanScreen()
		{
			foreach(var entry in BuildingInjections)
			{
				entry.Value.RegisterPlanscreen();
			}
		}
		public static void AddBuildingsToTechs()
		{
			foreach (var entry in BuildingInjections)
			{
				entry.Value.RegisterTech();
			}
		}
		internal static void ResetConfigChanges()
		{
			foreach(var config in ConfigCollection.BuildingConfigurations.Values)
			{
				config.ResetChanges();
			}
			ConfigCollection.WriteToFile();
		}
	}
}
