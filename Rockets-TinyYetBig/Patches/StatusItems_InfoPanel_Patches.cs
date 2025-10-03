using Database;
using HarmonyLib;
using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.Docking;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Rockets_TinyYetBig.STRINGS;
using static Rockets_TinyYetBig.STRINGS.UI_MOD.CLUSTERMAPROCKETSIDESCREEN;

namespace Rockets_TinyYetBig.Patches
{
	class StatusItems_InfoPanel_Patches
	{
		class StatusItemsRegisterPatch
		{
			[HarmonyPatch(typeof(Database.BuildingStatusItems), "CreateStatusItems")]
			public static class Database_BuildingStatusItems_CreateStatusItems_Patch
			{
				public static void Postfix()
				{
					ModAssets.StatusItems.Register();
				}
			}
		}



		[HarmonyPatch(typeof(Clustercraft))]
		[HarmonyPatch(nameof(Clustercraft.UpdateStatusItem))]
		public static class GeneratorModuleStatusItems
		{
			//static Dictionary<KSelectable, Guid> batteryStatusItemGuids = new Dictionary<KSelectable, Guid>();
			//static Dictionary<KSelectable, Guid> generatorStatusItemGuids = new Dictionary<KSelectable, Guid>();
			public static void Postfix(Clustercraft __instance)
			{
				KSelectable selectable = __instance.GetComponent<KSelectable>();

				//selectable.RemoveStatusItem(ModAssets.StatusItems.RTB_RocketBatteryStatus);
				//selectable.RemoveStatusItem(ModAssets.StatusItems.RTB_ModuleGeneratorFuelStatus);
				//selectable.RemoveStatusItem(ModAssets.StatusItems.RTB_SpaceStationConstruction_Status);

				//Tuple<float, float> data = new Tuple<float, float>(0, 0);
				//Tuple<float, float> dataBattery = new Tuple<float, float>(0, 0);
				SpaceStationBuilder constructionModule = null;

				foreach (var module in __instance.ModuleInterface.ClusterModules)
				{
					//if (moduleGet.gameObject.TryGetComponent<RTB_ModuleGenerator>(out var generator))
					//{
					//    var genStats = generator.GetConsumptionStatusStats();
					//    data.first += genStats.first;
					//    data.second += genStats.second;
					//    //generator.FuelStatusHandle =
					//}
					//if (moduleGet.gameObject.TryGetComponent<ModuleBattery>(out var battery))
					//{
					//    dataBattery.first += battery.JoulesAvailable;
					//    dataBattery.second += battery.capacity;
					//    //generator.FuelStatusHandle =
					//}
					if (module.Get().gameObject.TryGetComponent<SpaceStationBuilder>(out var builder))
					{
						constructionModule = builder;
					}
				}

				//if (data.first > 0 || data.second > 0)
				//    selectable.SetStatusItem(Db.Get().StatusItemCategories.Power, ModAssets.StatusItems.RTB_ModuleGeneratorFuelStatus, (object)data);
				//else
				//    selectable.RemoveStatusItem(ModAssets.StatusItems.RTB_ModuleGeneratorFuelStatus);


				//if (dataBattery.first > 0 || dataBattery.second > 0)
				//    selectable.SetStatusItem(Db.Get().StatusItemCategories.OperatingEnergy, ModAssets.StatusItems.RTB_RocketBatteryStatus, (object)dataBattery);
				//else
				//    selectable.RemoveStatusItem(ModAssets.StatusItems.RTB_RocketBatteryStatus);

				if (__instance.TryGetComponent<DockingSpacecraftHandler>(out var manager) && manager.GetConnectedWorlds().Count > 0)
				{
					selectable.SetStatusItem(Db.Get().StatusItemCategories.WoundEffects, ModAssets.StatusItems.RTB_DockingActive, (object)manager.GetConnectedWorlds());
				}
				else
					selectable.RemoveStatusItem(ModAssets.StatusItems.RTB_DockingActive);

				if (constructionModule != null)
				{
					selectable.SetStatusItem(Db.Get().StatusItemCategories.AccessControl, ModAssets.StatusItems.RTB_SpaceStationConstruction_Status, (object)constructionModule);
				}
				else
					selectable.RemoveStatusItem(ModAssets.StatusItems.RTB_SpaceStationConstruction_Status);

			}

		}

		class ExtendSolarNotification
		{
			[HarmonyPatch(typeof(BuildingStatusItems), "CreateStatusItems")]
			public static class SolarNoseconeStatusItems
			{
				public static void Postfix(BuildingStatusItems __instance)
				{
					__instance.ModuleSolarPanelWattage.resolveStringCallback = (Func<string, object, string>)((str, data) =>
					{
						if (data is ModuleSolarPanel)
						{
							ModuleSolarPanel moduleSolarPanel = (ModuleSolarPanel)data;
							str = str.Replace("{Wattage}", GameUtil.GetFormattedWattage(moduleSolarPanel.CurrentWattage));
						}
						else if (data is ModuleSolarPanelAdjustable)
						{
							ModuleSolarPanelAdjustable moduleSolarPanel = (ModuleSolarPanelAdjustable)data;
							str = str.Replace("{Wattage}", GameUtil.GetFormattedWattage(moduleSolarPanel.CurrentWattage));
						}

						return str;
					});
				}

			}

		}

	}
}
