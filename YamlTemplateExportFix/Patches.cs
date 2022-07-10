using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateClasses;

namespace YamlTemplateExportFix
{
    class Patches
    {
        [HarmonyPatch(typeof(DebugBaseTemplateButton))]
        [HarmonyPatch("GetSelectionAsAsset")]
        public static class FixPositionInYaml_patch
        {
            public static void Postfix(ref TemplateContainer __result)
            {
                Debug.Log("PATCHING YAML EXPORT");

                if (__result.cells != null && __result.cells.Count > 0)
                {
                    foreach (var cell in __result.cells)
                    {
                        cell.location_x--;
                        cell.location_y--;
                    }
                    Debug.Log("Cells Done");
                }
                if (__result.buildings != null && __result.buildings.Count > 0)
                {
                    foreach (Prefab building in __result.buildings)
                    {
                        building.location_x--;
                        building.location_y--;
                    }
                    Debug.Log("Buildings Done");
                }
                if (__result.pickupables != null && __result.pickupables.Count > 0)
                {
                    foreach (Prefab pickupable in __result.pickupables)
                    {
                        pickupable.location_x--;
                        pickupable.location_y--;
                    }
                    Debug.Log("Items Done");
                }
                if (__result.elementalOres != null && __result.elementalOres.Count > 0)
                {
                    foreach (Prefab elementalOre in __result.elementalOres)
                    {
                    elementalOre.location_x--;
                    elementalOre.location_y--;
                    }
                }
                if (__result.otherEntities != null && __result.otherEntities.Count > 0)
                {
                    foreach (Prefab entity in __result.otherEntities)
                    {
                        entity.location_x--;
                        entity.location_y--;
                    }
                    Debug.Log("Entities Done");
                }

            }
        }

    }
}
