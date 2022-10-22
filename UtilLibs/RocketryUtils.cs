using Epic.OnlineServices.Platform;
using HarmonyLib;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace UtilLibs
{
    public static class RocketryUtils
    {
        public const string CategoryDataKey = "Sgt_Imalas_RocketModuleSortOrder";
        public const string CategoryInitKey = "Sgt_Imalas_VanillaRocketModulesCategorized";


        public static bool IsRocketInFlight(Clustercraft craft)
        {
            var LocationCurrent = craft.Location;
            var LocationTarget = craft.ModuleInterface.GetClusterDestinationSelector().GetDestination();
            return LocationCurrent != LocationTarget;
        }
        public enum RocketCategory
        {
            engines = 0,
            habitats = 1,
            nosecones = 2,
            deployables = 3,
            fuel = 4,
            cargo = 5,
            power = 6,
            production = 7,
            utility = 8,
            uncategorized = -1
        }
        public class RocketModuleList
        {
            public static Dictionary<int, List<string>> GetRocketModuleList()

            {
                bool init = PRegistry.GetData<bool>(CategoryInitKey);
                if (init == false)
                {
                    Debug.Log("Rocketry Utils: Initializing global keys");
                    PRegistry.PutData(CategoryInitKey, true);

                    var moduleList = (Dictionary<int, List<string>>)new RocketModuleList().CategorizedButtonSortOrder;
                    //CategorizeVanillaModules(moduleList);
                    SetRocketModuleList(moduleList);
                    return moduleList;
                }
                else
                {
                    var ReturnValue = PRegistry.GetData<Dictionary<int, List<string>>>(CategoryDataKey);
                    //Debug.Log("Rocketry Utils. Existing global categories found: " + ReturnValue);
                    return ReturnValue;
                }
            }
            public static void SetRocketModuleList(Dictionary<int, List<string>> list)
            {
                PRegistry.PutData(CategoryDataKey, list);
            }


            public bool VanillaModulesCategorized;
            public Dictionary<int, List<string>> CategorizedButtonSortOrder;
            public RocketModuleList()
            {
                CategorizedButtonSortOrder = new Dictionary<int, List<string>>
                {
                { (int)RocketCategory.engines,new List<string>()},
                { (int)RocketCategory.habitats,new List<string>()},
                { (int)RocketCategory.nosecones,new List<string>()},
                { (int)RocketCategory.deployables,new List<string>()},
                { (int)RocketCategory.fuel,new List<string>()},
                { (int)RocketCategory.cargo,new List<string>()},
                { (int)RocketCategory.power,new List<string>()},
                { (int)RocketCategory.production,new List<string>()},
                { (int)RocketCategory.utility,new List<string>()},
                { (int)RocketCategory.uncategorized,new List<string>()},
                };
            }
        }

        public static void CategorizeRocketModule(string module, Dictionary<int, List<string>> sortedModules)
        {
            foreach (var moduleList in sortedModules.Values)
            {
                if (moduleList.Contains(module))
                {
#if DEBUG
                    Debug.Log(module + " already in category");

#endif
                    return;
                }
            }


            bool categoryFound = false;
            if (module.Contains("Engine"))
            {
                AddIfNotExists(sortedModules[(int)RocketCategory.engines], module);
                categoryFound = true;
#if DEBUG
                Debug.Log("Added " + module + " to category engines");
#endif
            }
            if (module.Contains("HabitatModule"))
            {
                AddIfNotExists(sortedModules[(int)RocketCategory.habitats], module);
                categoryFound = true;
#if DEBUG
                Debug.Log("Added " + module + " to category habitats");
#endif
            }
            if (module.Contains("Nosecone") || module == HabitatModuleSmallConfig.ID)
            {
                AddIfNotExists(sortedModules[(int)RocketCategory.nosecones], module);
                categoryFound = true;
#if DEBUG
                Debug.Log("Added " + module + " to category nosecones");
#endif
            }
            if (module == "OrbitalCargoModule" || module == "ScoutModule" || module == "PioneerModule")
            {
                AddIfNotExists(sortedModules[(int)RocketCategory.deployables], module);
                categoryFound = true;
#if DEBUG
                Debug.Log("Added " + module + " to category deployables");
#endif
            }
            if (module.Contains("Tank"))
            {
                AddIfNotExists(sortedModules[(int)RocketCategory.fuel], module);
                categoryFound = true;
#if DEBUG
                Debug.Log("Added " + module + " to category fuel");
#endif
            }
            if (module.Contains("CargoBay"))
            {
                AddIfNotExists(sortedModules[(int)RocketCategory.cargo], module);
                categoryFound = true;
#if DEBUG
                Debug.Log("Added " + module + " to category cargo");
#endif
            }
            if (module.Contains("Battery") || module.Contains("SolarPanel"))
            {
                AddIfNotExists(sortedModules[(int)RocketCategory.power], module);
                categoryFound = true;
#if DEBUG
                Debug.Log("Added " + module + " to category power");
#endif
            }
            if (module == "ScannerModule")
            {
                AddIfNotExists(sortedModules[(int)RocketCategory.utility], module);
                categoryFound = true;
#if DEBUG
                Debug.Log("Added " + module + " to category util");
#endif
            }
            if (!categoryFound)
            {
                Debug.LogWarning("No Category found for " + module);
                AddIfNotExists(sortedModules[(int)RocketCategory.uncategorized], module);
            }
        }


        public static bool AddIfNotExists<T>(List<T> list, T value)
        {
            if (!list.Contains(value))
            {
                list.Add(value);
                return true;
            }
            return false;
        }

        public static void CategorizeVanillaModules()
        {
            var allModules = SelectModuleSideScreen.moduleButtonSortOrder;
            var categories = RocketModuleList.GetRocketModuleList();

            foreach (var module in allModules)
            {
                CategorizeRocketModule(module, categories);
            }
            Debug.Log("Vanilla rocket parts categorized");
        }

        public static void AddRocketModuleToBuildList(
            string moduleId,
            RocketCategory category = RocketCategory.uncategorized,
            string placebehind = "")
        {
            var sorted = RocketModuleList.GetRocketModuleList();



            if (!sorted[(int)category].Contains(moduleId))
            {
                sorted[(int)category].Insert(
                GetInsertionIndex(sorted[(int)category], placebehind)
                , moduleId);
                RocketModuleList.SetRocketModuleList(sorted);
            }
            if (!SelectModuleSideScreen.moduleButtonSortOrder.Contains(moduleId))
            {
                SelectModuleSideScreen.moduleButtonSortOrder.Insert(
                    GetInsertionIndex(SelectModuleSideScreen.moduleButtonSortOrder, placebehind),
                    moduleId);
            }
#if DEBUG
            Debug.Log("Added " + moduleId + " to category " + category.ToString());
#endif
        }

        public static void AddRocketModuleToBuildList(
            string moduleId,
            RocketCategory[] categories,
            string placebehind = "")
        {
            var sorted = RocketModuleList.GetRocketModuleList();


            foreach(var category in categories)
            {
                if (!sorted[(int)category].Contains(moduleId))
                {
                    sorted[(int)category].Insert(
                        GetInsertionIndex(sorted[(int)category], placebehind)
                        , moduleId);
                    RocketModuleList.SetRocketModuleList(sorted);
#if DEBUG
                    Debug.Log("Added " + moduleId + " to category " + category.ToString());
#endif
                }
            }
            if (!SelectModuleSideScreen.moduleButtonSortOrder.Contains(moduleId))
            {
                SelectModuleSideScreen.moduleButtonSortOrder.Insert(
                    GetInsertionIndex(SelectModuleSideScreen.moduleButtonSortOrder, placebehind),
                    moduleId);
            }

        }

        public static int GetInsertionIndex(List<string> list, string indexID = "")
        {
            int startIndex = indexID != "" ? list.IndexOf(indexID) : -1;
            int insertionIndex = (startIndex == -1) ? list.Count : ++startIndex;
            return insertionIndex;
        }


        public static Vector2I GetCustomInteriorSize(string templateString)
        {
            Regex getSize = new Regex(@"\(([0-9]*?)[,]([0-9]*?)\)");
            MatchCollection matches = getSize.Matches(templateString);
            if (matches.Count == 1)
            {
                Debug.Log(matches[0] +" "+ matches[0].Groups.Count.ToString() + " " + matches[0].Groups[0].Value + " " + matches[0].Groups[1].Value);
                if (matches[0].Groups.Count == 3)
                {
                    Debug.Log("reachedGroups");
                    var x = int.Parse(matches[0].Groups[1].Value);
                    var y = int.Parse(matches[0].Groups[2].Value); 
                    return new Vector2I(x, y);
                }
            }
            return ROCKETRY.ROCKET_INTERIOR_SIZE;
        }
    }
}
