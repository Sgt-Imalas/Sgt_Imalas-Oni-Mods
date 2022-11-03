using Database;
using HarmonyLib;
using Rockets_TinyYetBig.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        class ExtendSolarNotification
        {
            [HarmonyPatch(typeof(BuildingStatusItems), "CreateStatusItems")]
            public static class IncreaseCapacityto1350
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
