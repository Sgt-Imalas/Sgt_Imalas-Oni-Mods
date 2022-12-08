using Database;
using HarmonyLib;
using Rockets_TinyYetBig.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static STRINGS.BUILDINGS.PREFABS;

namespace Rockets_TinyYetBig.Patches
{
    class StatusItemsPatches
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
            static Dictionary<KSelectable, Guid> batteryStatusItemGuids = new Dictionary<KSelectable, Guid>();
            static Dictionary<KSelectable, Guid> generatorStatusItemGuids = new Dictionary<KSelectable, Guid>();
            public static void Postfix(Clustercraft __instance)
            {
                KSelectable selectable = __instance.GetComponent<KSelectable>();

                selectable.RemoveStatusItem(ModAssets.StatusItems.RTB_RocketBatteryStatus);
                selectable.RemoveStatusItem(ModAssets.StatusItems.RTB_ModuleGeneratorFuelStatus);

                Tuple<float, float> data = new Tuple<float, float>(0, 0);
                foreach (var module in __instance.ModuleInterface.ClusterModules)
                {
                    if (module.Get().gameObject.TryGetComponent<RTB_ModuleGenerator>(out var generator))
                    {
                        var genStats = generator.GetConsumptionStatusStats();
                        data.first += genStats.first;
                        data.second += genStats.second;
                        //generator.FuelStatusHandle =
                    }
                }

                if (data.first > 0 || data.second > 0)
                    selectable.SetStatusItem(Db.Get().StatusItemCategories.Power, ModAssets.StatusItems.RTB_ModuleGeneratorFuelStatus, (object)data);
                else
                    selectable.RemoveStatusItem(ModAssets.StatusItems.RTB_ModuleGeneratorFuelStatus);

                //if (data.first > 0 || data.second > 0)
                //{
                //    if (!generatorStatusItemGuids.ContainsKey(selectable))
                //    {
                //        generatorStatusItemGuids.Add(selectable, selectable.AddStatusItem(ModAssets.StatusItems.RTB_ModuleGeneratorFuelStatus, (object)data));
                //    }
                //    else
                //    {
                //        generatorStatusItemGuids[selectable] = selectable.ReplaceStatusItem(generatorStatusItemGuids[selectable], ModAssets.StatusItems.RTB_ModuleGeneratorFuelStatus, (object)data);
                //    }
                //}
                //else
                //{
                //    selectable.RemoveStatusItem(ModAssets.StatusItems.RTB_ModuleGeneratorFuelStatus);
                //    if (generatorStatusItemGuids.ContainsKey(selectable))
                //    {
                //        generatorStatusItemGuids.Remove(selectable);
                //    }
                //}


                Tuple<float, float> dataBattery = new Tuple<float, float>(0, 0);
                foreach (var module in __instance.ModuleInterface.ClusterModules)
                {
                    if (module.Get().gameObject.TryGetComponent<ModuleBattery>(out var battery))
                    {
                        dataBattery.first += battery.JoulesAvailable;
                        dataBattery.second += battery.capacity;
                        //generator.FuelStatusHandle =
                    }
                }
                if (dataBattery.first > 0 || dataBattery.second > 0)
                    selectable.SetStatusItem(Db.Get().StatusItemCategories.OperatingEnergy, ModAssets.StatusItems.RTB_RocketBatteryStatus, (object)dataBattery);
                else
                    selectable.RemoveStatusItem(ModAssets.StatusItems.RTB_RocketBatteryStatus);

                //if (dataBattery.first > 0 || dataBattery.second > 0)
                //{
                //    if (!batteryStatusItemGuids.ContainsKey(selectable))
                //    {
                //        batteryStatusItemGuids.Add(selectable, selectable.AddStatusItem(ModAssets.StatusItems.RTB_RocketBatteryStatus, (object)dataBattery));
                //    }
                //    else
                //    {
                //        batteryStatusItemGuids[selectable] = selectable.ReplaceStatusItem(batteryStatusItemGuids[selectable], ModAssets.StatusItems.RTB_RocketBatteryStatus, (object)dataBattery);
                //    }
                //}
                //else
                //{
                //    selectable.RemoveStatusItem(ModAssets.StatusItems.RTB_ModuleGeneratorFuelStatus);
                //    if (batteryStatusItemGuids.ContainsKey(selectable))
                //    {
                //        batteryStatusItemGuids.Remove(selectable);
                //    }
                // }


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
