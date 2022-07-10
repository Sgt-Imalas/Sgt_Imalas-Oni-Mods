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
            public static void Postfix(ref TemplateContainer __result, DebugBaseTemplateButton __instance)
            {
                bool doY, doX;
                int lXmin = 0, lXmax = 0, lYmin=0, lYmax=0;

                foreach (var cell in __result.cells)
                {
                    lXmin = cell.location_x < lXmin ? cell.location_x : lXmin;
                    lYmin = cell.location_y < lYmin ? cell.location_y : lYmin;
                    lXmax = cell.location_x > lXmax ? cell.location_x : lXmax;
                    lYmax = cell.location_y > lYmax ? cell.location_y : lYmax;
                }
                doY = Math.Abs(lYmax - lYmin) % 2 != 0;
                doX = Math.Abs(lXmax - lXmin) % 2 != 0;
                Debug.Log(string.Format("Patching X coordinates: {0}, Patching Y coordinates: {1}", doX, doY));

                if (__result.cells != null && __result.cells.Count > 0)
                {


                    foreach (var cell in __result.cells)
                    {
                        if(doX)
                        cell.location_x--;
                        if(doY)
                        cell.location_y--;
                    }
                    Debug.Log("Cells Done");
                }
                if (__result.buildings != null && __result.buildings.Count > 0)
                {
                    foreach (Prefab building in __result.buildings)
                    {
                        if (doX)
                            building.location_x--;
                        if (doY)
                            building.location_y--;
                    }
                    Debug.Log("Buildings Done");
                }
                if (__result.pickupables != null && __result.pickupables.Count > 0)
                {
                    foreach (Prefab pickupable in __result.pickupables)
                    {
                        if (doX)
                            pickupable.location_x--;
                        if (doY)
                            pickupable.location_y--;
                    }
                    Debug.Log("Items Done");
                }
                if (__result.elementalOres != null && __result.elementalOres.Count > 0)
                {
                    foreach (Prefab elementalOre in __result.elementalOres)
                    {
                        if (doX)
                            elementalOre.location_x--;
                        if (doY)
                            elementalOre.location_y--;
                    }
                }
                if (__result.otherEntities != null && __result.otherEntities.Count > 0)
                {
                    foreach (Prefab entity in __result.otherEntities)
                    {
                        if (doX)
                            entity.location_x--;                       
                        if (doY) 
                            entity.location_y--;
                    }
                    Debug.Log("Entities Done");
                }

            }
        }

    }
}
