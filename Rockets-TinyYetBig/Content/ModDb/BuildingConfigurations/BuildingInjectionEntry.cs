using System;
using System.Collections.Generic;
using System.Linq;
using UtilLibs;
using static UtilLibs.RocketryUtils;

namespace Rockets_TinyYetBig.Content.ModDb.BuildingConfigurations
{
	public class BuildingInjectionEntry
	{
		bool MoveExisting = false;
		string _buildingID;
		string _techID;
		string _planScreenCategory;
		string PlanScreenRelativeBuildingID;
		string _subcategoryID = "uncategorized";
		RocketCategory[] rocketModuleCategories = [];
		public string BuildingID => _buildingID;
		public string TechID => _techID;
		public string PlanScreenCategory => _planScreenCategory;
		internal BuildingConfigurationEntry Source;
		bool isRocketModule => rocketModuleCategories.Length > 0;

		ModUtil.BuildingOrdering BuildingOrdering = ModUtil.BuildingOrdering.After;
		List<int> configCategories = new();
		public List<int> ConfigCategories => configCategories;
		int _primaryCategory;
		public int PrimaryCategory => _primaryCategory;
		string _linkedCategory = null;

		static Dictionary<string, List<BuildingInjectionEntry>> _groupLinks = [];

		//public HashSet<string> MigrateOldIdsFrom = [];
		public Func<bool> CanEnableBuildingFromConfig = () => true;


		public static BuildingInjectionEntry Create(string buildingID)
		{
			var entry = new BuildingInjectionEntry();
			entry._buildingID = buildingID;
			return entry;
		}
		///link the enabled state of all buildings of the same category
		public BuildingInjectionEntry CategoryLink(string id)
		{
			_linkedCategory = id;
			if (!_groupLinks.ContainsKey(id))
				_groupLinks[id] = [this];
			else
				_groupLinks[id].Add(this);
			return this;
		}

		public BuildingInjectionEntry AddToTech(string techID)
		{
			_techID = techID;
			return this;
		}
		public BuildingInjectionEntry ForceCategory(bool move = true)
		{
			MoveExisting = move;
			return this;
		}
		internal BuildingInjectionEntry ConfigEnabled(Func<bool> configEnabled)
		{
			CanEnableBuildingFromConfig = configEnabled;
			return this;
		}
		public BuildingInjectionEntry AddToCategory(string category, string relativeBuildingID, ModUtil.BuildingOrdering ordering = ModUtil.BuildingOrdering.After)
		{
			_planScreenCategory = category;
			PlanScreenRelativeBuildingID = relativeBuildingID;
			BuildingOrdering = ordering;
			return this;
		}
		public BuildingInjectionEntry SubCategory(string subcat)
		{
			PlanScreenRelativeBuildingID = string.Empty;
			_subcategoryID = subcat;
			return this;
		}
		public BuildingInjectionEntry AddToCategory(RocketCategory category, string relativeBuildingID, ModUtil.BuildingOrdering ordering = ModUtil.BuildingOrdering.After) => AddToCategory([category], relativeBuildingID, ordering);
		public BuildingInjectionEntry AddToCategory(RocketCategory[] categories, string relativeBuildingID, ModUtil.BuildingOrdering ordering = ModUtil.BuildingOrdering.After)
		{
			rocketModuleCategories = categories;
			foreach (var cat in categories)
				if (!configCategories.Contains((int)cat))
					configCategories.Add((int)cat);
			PlanScreenRelativeBuildingID = relativeBuildingID;
			BuildingOrdering = ordering;
			return this;
		}
		public BuildingInjectionEntry PrimaryConfigCategory(int mod) => AddCategory(mod, true);
		public BuildingInjectionEntry AddCategory(int mod, bool primary = false, string migrateOldPrefabId = null)
		{
			//if(!string.IsNullOrEmpty(migrateOldPrefabId))
			//{
			//	MigrateOldIdsFrom.Add(migrateOldPrefabId);
			//}
			if (primary)
				_primaryCategory = mod;
			if (!configCategories.Contains(mod))
			{
				configCategories.Add(mod);
			}
			return this;
		}


		public List<int> GetConfigCategories()
		{
			return configCategories;
		}
		internal void RegisterTech()
		{
			if (string.IsNullOrEmpty(_techID))
			{
				return;
			}
			InjectionMethods.AddBuildingToTechnology(_techID, _buildingID);
		}
		internal void RegisterPlanscreen()
		{
			if (isRocketModule)
			{
				RegisterRocketryScreen();
				return;
			}
			if (string.IsNullOrEmpty(_planScreenCategory) || (string.IsNullOrEmpty(PlanScreenRelativeBuildingID) && string.IsNullOrEmpty(_subcategoryID)))
			{
				return;
			}
			if (MoveExisting)
				InjectionMethods.MoveExistingBuildingToNewCategory(_planScreenCategory, _buildingID, PlanScreenRelativeBuildingID, _subcategoryID, ordering: BuildingOrdering);
			else
				InjectionMethods.AddBuildingToPlanScreenBehindNext(_planScreenCategory, _buildingID, PlanScreenRelativeBuildingID, _subcategoryID, ordering: BuildingOrdering);
		}
		void RegisterRocketryScreen()
		{
			RocketryUtils.AddRocketModuleToBuildList(_buildingID, rocketModuleCategories, PlanScreenRelativeBuildingID, BuildingOrdering == ModUtil.BuildingOrdering.Before);
		}
		public bool TryGetSyncedEntries(out List<BuildingInjectionEntry> entries)
		{
			entries = null;
			if (_linkedCategory.IsNullOrWhiteSpace())
				return false;
			return _groupLinks.TryGetValue(_linkedCategory, out entries);
		}

		internal void SyncGroup(bool on)
		{
			if (_linkedCategory.IsNullOrWhiteSpace())
				return;
			if (!_groupLinks.TryGetValue(_linkedCategory, out var toSync))
			{
				SgtLogger.error("Could not find sync group for " + _linkedCategory);
				return;
			}
			foreach (var entry in toSync)
			{
				if (entry == this)
					continue;
				entry.SyncOnReceive(on);
			}
		}
		void SyncOnReceive(bool on)
		{
			Source.SetBuildingEnabled(on, false);
		}

		//internal void RegisterLegacyMigrations()
		//{
		//	if (!MigrateOldIdsFrom.Any())
		//		return;

		//	var prefab = Assets.TryGetPrefab(_buildingID);
		//	var savemng = SaveLoader.Instance.saveManager;

		//	foreach (var oldPrefabTag in MigrateOldIdsFrom)
		//	{
		//		if (!savemng.prefabMap.ContainsKey(oldPrefabTag))
		//		{
		//			savemng.prefabMap.Add(oldPrefabTag, prefab);
		//		}
		//		else
		//			SgtLogger.warning("Skipping migration registration of " + oldPrefabTag + " to " + _buildingID + " as it is already registered." );
		//	}
		//}
	}
}
