using Database;
using HarmonyLib;
using Rockets_TinyYetBig.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            public static void Postfix(Clustercraft __instance)
            {
                Tuple<float, float> data = new Tuple<float, float>(0, 0);
                KSelectable selectable = __instance.GetComponent<KSelectable>();
                foreach (var module in __instance.ModuleInterface.ClusterModules)
                {
                    if(module.Get().gameObject.TryGetComponent<RTB_ModuleGenerator>(out var generator))
                    {
                        var genStats = generator.GetConsumptionStatusStats();
                        data.first += genStats.first;
                        data.second += genStats.second;
                        //generator.FuelStatusHandle =
                    }
                }
                selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, ModAssets.StatusItems.RTB_ModuleGeneratorFuelStatus, (object)data);
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
                        if(data is ModuleSolarPanel) 
                        { 
                            ModuleSolarPanel moduleSolarPanel = (ModuleSolarPanel)data;
                            str = str.Replace("{Wattage}", GameUtil.GetFormattedWattage(moduleSolarPanel.CurrentWattage));
                        }
                        else if(data is ModuleSolarPanelAdjustable)
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
