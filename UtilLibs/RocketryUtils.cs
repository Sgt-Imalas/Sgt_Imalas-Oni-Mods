using PeterHan.PLib.Core;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TUNING;

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
		public static void AddPowerPlugToModule(BuildingDef def, CellOffset offset)
		{

			def.RequiresPowerOutput = true;
			def.PowerInputOffset = offset;
			def.PowerOutputOffset = offset;
			def.UseWhitePowerOutputConnectorColour = true;
		}

		public static bool IsRocketTraveling(Clustercraft craft)
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

		public static void CategorizeRocketModule(string moduleId, Dictionary<int, List<string>> sortedModules)
		{
			foreach (var moduleList in sortedModules.Values)
			{
				if (moduleList.Contains(moduleId))
				{
#if DEBUG
                    Debug.Log(module + " already in category");

#endif
					return;
				}
			}


			bool categoryFound = false;
			if (moduleId.Contains("Engine"))
			{
				AddIfNotExists(sortedModules[(int)RocketCategory.engines], moduleId);
				categoryFound = true;
#if DEBUG
                Debug.Log("Added " + module + " to category engines");
#endif
			}
			if (moduleId.Contains("HabitatModule")||moduleId.Contains("RoboPilotModule"))
			{
				AddIfNotExists(sortedModules[(int)RocketCategory.habitats], moduleId);
				categoryFound = true;
#if DEBUG
                Debug.Log("Added " + module + " to category habitats");
#endif
			}
			if (moduleId.Contains("Nosecone") || moduleId == HabitatModuleSmallConfig.ID)
			{
				AddIfNotExists(sortedModules[(int)RocketCategory.nosecones], moduleId);
				categoryFound = true;
#if DEBUG
                Debug.Log("Added " + module + " to category nosecones");
#endif
			}
			if (moduleId == "OrbitalCargoModule" || moduleId == "ScoutModule" || moduleId == "PioneerModule")
			{
				AddIfNotExists(sortedModules[(int)RocketCategory.deployables], moduleId);
				categoryFound = true;
#if DEBUG
                Debug.Log("Added " + module + " to category deployables");
#endif
			}
			if (moduleId.Contains("Tank"))
			{
				AddIfNotExists(sortedModules[(int)RocketCategory.fuel], moduleId);
				categoryFound = true;
#if DEBUG
                Debug.Log("Added " + module + " to category fuel");
#endif
			}
			if (moduleId.Contains("CargoBay")||moduleId == "ResearchClusterModule")
			{
				AddIfNotExists(sortedModules[(int)RocketCategory.cargo], moduleId);
				categoryFound = true;
#if DEBUG
                Debug.Log("Added " + module + " to category cargo");
#endif
			}
			if (moduleId.Contains("Battery") || moduleId.Contains("SolarPanel"))
			{
				AddIfNotExists(sortedModules[(int)RocketCategory.power], moduleId);
				categoryFound = true;
#if DEBUG
                Debug.Log("Added " + module + " to category power");
#endif
			}
			if (moduleId == "ScannerModule" || moduleId.Contains("Research"))
			{
				AddIfNotExists(sortedModules[(int)RocketCategory.utility], moduleId);
				categoryFound = true;
#if DEBUG
                Debug.Log("Added " + module + " to category util");
#endif
			}
			if (!categoryFound)
			{
				SgtLogger.logwarning("No Category found for " + moduleId);
				AddIfNotExists(sortedModules[(int)RocketCategory.uncategorized], moduleId);
			}
		}


		public static bool AddIfNotExists<T>(List<T> list, T value, int index = -1)
		{

			if (!list.Contains(value))
			{
				if (index == -1)
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
					categories[ToSortcategory.Key].Insert(GetInsertionIndex(categories[ToSortcategory.Key]), itemToShuffle.first);
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


			Debug.Log("Rocketry Expanded: Addon mod reordering done, putting data to PRegistry");
			RocketModuleList.SetRocketModuleList(categories);
		}



		public static void AddRocketModuleToBuildList(
			string moduleId,
			RocketCategory category = RocketCategory.uncategorized,
			string placebehind = "", bool placebefore = false)
		{
			InsertRocketModuleToCategory(moduleId, category, placebehind, placebefore);
		}

		public static void AddRocketModuleToBuildList(
			string moduleId,
			RocketCategory[] categories,
			string placebehind = "", bool placebefore = false)
		{
			InsertRocketModuleToCategory(moduleId, categories, placebehind, placebefore);
		}

		public static void PutModuleToReshuffleData(Dictionary<int, List<Tuple<string, string>>> list)
		{
			PRegistry.PutData(AddonModsDataKey, list);
		}
		public static Dictionary<int, List<Tuple<string, string>>> GetModuleToReshuffleData()
		{
			var ReturnValue = PRegistry.GetData<Dictionary<int, List<Tuple<string, string>>>>(AddonModsDataKey);
			if (ReturnValue == null)
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

		public static void InsertRocketModuleToCategory(string moduleId, RocketCategory category = RocketCategory.uncategorized, string placeBehindId = "", bool placebefore = false)
		{
			InsertRocketModuleToCategory(moduleId, new RocketCategory[] { category }, placeBehindId, placebefore);
		}
		public static void InsertRocketModuleToCategory(string moduleId, RocketCategory[] categories, string placeBehindId = "", bool placebefore = false)
		{
			var ModuleLists = RocketModuleList.GetRocketModuleList();


			foreach (var category in categories)
			{
				if (placeBehindId != "")
				{
					var indexOfPlaceBehind = ModuleLists[(int)category].IndexOf(placeBehindId);
					if (indexOfPlaceBehind == -1)
					{
						AddIfNotExists(ModuleLists[(int)category], (moduleId));
						AddModuleToReshuffleData(category, moduleId, placeBehindId);///Add To ReshuffleList
					}
					else
					{
						AddIfNotExists(ModuleLists[(int)category], (moduleId), placebefore ? indexOfPlaceBehind : ++indexOfPlaceBehind);
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
				var index = GetInsertionIndex(SelectModuleSideScreen.moduleButtonSortOrder, placeBehindId, placebefore);
				SelectModuleSideScreen.moduleButtonSortOrder.Insert(index, moduleId);
			}
		}

		public static int GetInsertionIndex(List<string> list, string indexID = "", bool placebefore = false)
		{
			int startIndex = indexID != "" ? list.IndexOf(indexID) : -1;
			int insertionIndex = (startIndex == -1) ? list.Count : placebefore ? startIndex : ++startIndex;
			return insertionIndex;
		}


		public static Vector2I GetCustomInteriorSize(string templateString)
		{
			Regex getSize = new Regex(@"\(([0-9]*?)[,]([0-9]*?)\)");
			MatchCollection matches = getSize.Matches(templateString);
			if (matches.Count == 1)
			{
				Debug.Log(matches[0] + " " + matches[0].Groups.Count.ToString() + " " + matches[0].Groups[0].Value + " " + matches[0].Groups[1].Value);
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
