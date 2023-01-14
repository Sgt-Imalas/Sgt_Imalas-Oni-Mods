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
        public const string AddonModsDataKey = "Sgt_Imalas_ModulesToRearrange";



        public static void AddPowerPlugToModule(BuildingDef def)
        {
            AddPowerPlugToModule(def, CellOffset.none);
        }
        public static void AddPowerPlugToModule(BuildingDef def,CellOffset offset)
        {

            def.RequiresPowerOutput = true;
            def.PowerInputOffset = offset;
            def.PowerOutputOffset = offset;
            def.UseWhitePowerOutputConnectorColour = true;
        }

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


        public static bool AddIfNotExists<T>(List<T> list, T value, int index = -1)
        {
            

            if (!list.Contains(value))
            {
                if(index == -1)
                {
                    list.Add(value);
                }
                else
                {
                    list.Insert(index, value);
                }
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
            var data = GetModuleToReshuffleData();
            foreach (var ToSortcategory in data)
            {
                foreach (var itemToShuffle in ToSortcategory.Value)
                {
#if DEBUG
                    Debug.Log("Removing {0} from List {1}".F(itemToShuffle.first, (RocketCategory)ToSortcategory.Key));
#endif
                    categories[ToSortcategory.Key].Remove(itemToShuffle.first);
                    //int index = categories[ToSortcategory.Key].IndexOf(itemToShuffle.second);
                    //categories[ToSortcategory.Key].Insert(++index, itemToShuffle.first);
                }
                foreach (var itemToShuffle in ToSortcategory.Value)
                {
#if DEBUG
                    Debug.Log("Readding {0} to List {1} behind {2}".F(itemToShuffle.first, (RocketCategory)ToSortcategory.Key, itemToShuffle.second));
#endif
                    categories[ToSortcategory.Key].Insert(GetInsertionIndex(categories[ToSortcategory.Key]),itemToShuffle.first);
                    //int index = categories[ToSortcategory.Key].IndexOf(itemToShuffle.second);
                    //categories[ToSortcategory.Key].Insert(++index, itemToShuffle.first);
                }
            }
            //foreach(var category in categories)
            //{
            //    Debug.Log("{" + (RocketCategory)category.Key + "}");
                
            //    foreach (var module in category.Value)
            //    {
            //    Debug.Log("Module In List: " + module);
            //    }
            //}


            Debug.Log("Addon mod reordering done");
            RocketModuleList.SetRocketModuleList(categories);
        }



        public static void AddRocketModuleToBuildList(
            string moduleId,
            RocketCategory category = RocketCategory.uncategorized,
            string placebehind = "",bool placebefore = false)
        {
            InsertRocketModuleToCategory(moduleId, category, placebehind,placebefore);
        }

        public static void AddRocketModuleToBuildList(
            string moduleId,
            RocketCategory[] categories,
            string placebehind = "",bool placebefore = false)
        {
            InsertRocketModuleToCategory(moduleId, categories, placebehind, placebefore);
        }

        public static void PutModuleToReshuffleData(Dictionary<int, List<Tuple<string,string>>> list)
        {
            PRegistry.PutData(AddonModsDataKey, list);
        }
        public static Dictionary<int, List<Tuple<string, string>>> GetModuleToReshuffleData()
        {
            var ReturnValue = PRegistry.GetData< Dictionary<int, List<Tuple<string, string>>>>(AddonModsDataKey);
            if(ReturnValue == null)
            {
                ReturnValue = new Dictionary<int, List<Tuple<string, string>>>()
                {
                { (int)RocketCategory.engines,new List<Tuple<string, string>>()},
                { (int)RocketCategory.habitats,new List<Tuple<string, string>>()},
                { (int)RocketCategory.nosecones,new List<Tuple<string, string>>()},
                { (int)RocketCategory.deployables,new List<Tuple<string, string>>()},
                { (int)RocketCategory.fuel,new List<Tuple<string, string>>()},
                { (int)RocketCategory.cargo,new List<Tuple<string, string>>()},
                { (int)RocketCategory.power,new List<Tuple<string, string>>()},
                { (int)RocketCategory.production,new List<Tuple<string, string>>()},
                { (int)RocketCategory.utility,new List<Tuple<string, string>>()},
                { (int)RocketCategory.uncategorized,new List<Tuple<string, string>>()},
                };
                PutModuleToReshuffleData(ReturnValue);
            }

            return ReturnValue;
        }

        public static void AddModuleToReshuffleData(RocketCategory category, string moduleId, string placeBehind)
        {
#if DEBUG
            Debug.Log(moduleId + " scheduled for relocation");
#endif

            var data = GetModuleToReshuffleData();
            data[(int)category].Add(new Tuple<string, string>(moduleId, placeBehind));
            PutModuleToReshuffleData(data);
        }

        public static void InsertRocketModuleToCategory(string moduleId, RocketCategory category = RocketCategory.uncategorized, string placeBehindId = "",bool placebefore = false)
        {
            InsertRocketModuleToCategory(moduleId, new RocketCategory[] { category }, placeBehindId, placebefore);
        }
        public static void InsertRocketModuleToCategory(string moduleId, RocketCategory[] categories, string placeBehindId = "", bool placebefore = false)
        {
            var ModuleLists = RocketModuleList.GetRocketModuleList();
            foreach( var category in categories)
            {
                if(placeBehindId != "")
                {
                    var indexOfPlaceBehind = ModuleLists[(int)category].IndexOf(placeBehindId);
                    if(indexOfPlaceBehind == -1)
                    {
                        AddIfNotExists(ModuleLists[(int)category],(moduleId));
                        AddModuleToReshuffleData(category, moduleId, placeBehindId);///Add To ReshuffleList
                    }
                    else
                    {
                        AddIfNotExists(ModuleLists[(int)category], (moduleId), placebefore? indexOfPlaceBehind : ++indexOfPlaceBehind);
                        //ModuleLists[(int)category].Insert(++indexOfPlaceBehind, moduleId);
                    }
                }
                else
                {
                    AddIfNotExists(ModuleLists[(int)category], (moduleId));
                    //ModuleLists[(int)category].Insert(ModuleLists[(int)category].Count, moduleId);
                }
            }
            if (!SelectModuleSideScreen.moduleButtonSortOrder.Contains(moduleId))
            {
                var index = GetInsertionIndex(SelectModuleSideScreen.moduleButtonSortOrder, placeBehindId);
                SelectModuleSideScreen.moduleButtonSortOrder.Insert(placebefore? index : ++index,
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
