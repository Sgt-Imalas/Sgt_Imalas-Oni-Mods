using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboRockets.Patches
{
    internal class StatusItemsAdder
    {/// <summary>
     /// register custom status items
     /// </summary>
        [HarmonyPatch(typeof(Database.BuildingStatusItems), "CreateStatusItems")]
        public static class Database_BuildingStatusItems_CreateStatusItems_Patch
        {
            public static void Postfix()
            {
                ModAssets.RegisterStatusItems();
            }
        }
    }
}
