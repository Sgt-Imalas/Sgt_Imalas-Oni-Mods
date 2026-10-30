using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UtilLibs;
using static STRINGS.BUILDING.STATUSITEMS;

namespace Rockets_TinyYetBig.Content.ModDb.BuildingConfigurations
{
	public class BuildingConfigurationEntry
	{
		public string BuildingID = string.Empty;
		public bool BuildingEnabled = true;
		public float BuildingMassCapacity = -1;
		public float BuildingWattage = -1;
		public float BuildingTileRange = -1;
		[JsonIgnore]
		public List<int> BuildingCategories => IsInjected ? BuildingInjection.GetConfigCategories() : [];
		[JsonIgnore]
		public float BuildingMassCapacityDefault = -1;
		[JsonIgnore] public string StorageUnitLabel = null;
		[JsonIgnore] public string RangeUnitLabel = null;

		[JsonIgnore] public string StorageToolTip = null;
		[JsonIgnore] public string RangeToolTip = null;

		[JsonIgnore] public float BuildingWattageDefault = -1;
		[JsonIgnore] public float BuildingTileRangeDefault = -1; 

		[JsonIgnore] public BuildingInjectionEntry BuildingInjection = null;
		[JsonIgnore] public bool IsGenerator;
		[JsonIgnore] public bool IsOtherConfigurable;
		[JsonIgnore] public string OtherConfigurablesDescription = null;

		[JsonIgnore] public string RangeLabel = null;
		[JsonIgnore] public string StorageLabel = null;

		[JsonIgnore] public Tuple<int, int> RangeMinMax = null;
		[JsonIgnore] public Tuple<int, int> StorageMinMax = null;
		[JsonIgnore] public Tuple<int, int> WattageMinMax = null;

		[JsonIgnore]
		public bool IsInjected => BuildingInjection != null;


		public BuildingConfigurationEntry()
		{
			// Default constructor
		}
		public BuildingConfigurationEntry(string buildingID, bool buildingEnabled = true)
		{
			BuildingID = buildingID;
			BuildingEnabled = buildingEnabled;
		}
		public BuildingConfigurationEntry SetDefaultWattage(float wattage, Tuple<int, int> minmax)
		{
			if (minmax != null)
				WattageMinMax = minmax;
			BuildingWattageDefault = wattage;
			return this;
		}
		public BuildingConfigurationEntry SetDefaultStorageCapacity(float storage, Tuple<int, int> minmax)
		{
			if (minmax != null)
				StorageMinMax = minmax;
			BuildingMassCapacityDefault = storage;			
			return this;
		}
		public BuildingConfigurationEntry SetDefaultTileRange(int range, Tuple<int, int> minmax)
		{
			if (minmax != null)
				RangeMinMax = minmax;
			BuildingTileRangeDefault = range;
			return this;
		}
		public float GetWattage()
		{
			return BuildingWattage < 0 ? BuildingWattageDefault : BuildingWattage;
		}
		public float GetStorageCapacity()
		{
			return BuildingMassCapacity <= 0 ? BuildingMassCapacityDefault : BuildingMassCapacity;
		}
		public int GetTileRange()
		{
			return BuildingTileRange < 0 ? (int)BuildingTileRangeDefault : (int)BuildingTileRange;
		}

		public bool HasWattage(out float wattage)
		{
			wattage = GetWattage();
			return BuildingWattageDefault >= 0;
		}
		public bool HasStorageCapacity(out float capacity)
		{
			capacity = GetStorageCapacity();
			return BuildingMassCapacityDefault >= 0;
		}
		public bool HasTileRange(out int range)
		{
			range = GetTileRange();
			return BuildingTileRangeDefault >= 0;
		}
		public bool HasTileRangeDescriptor(out string descriptor)
		{
			descriptor = RangeLabel;
			return !string.IsNullOrEmpty(RangeLabel);
		}
		public bool HasStorageLabelDescriptor(out string descriptor)
		{
			descriptor = StorageLabel;
			return !string.IsNullOrEmpty(StorageLabel);
		}
		public bool IsBuildingEnabled() => BuildingEnabled && IsInjected && BuildingInjection.CanEnableBuildingFromConfig();
		public bool DisabledByCategory() => !IsInjected || BuildingInjection.CanEnableBuildingFromConfig() == false;
		public bool ShowBuildingEnabled(out string reason)
		{
			if (IsInjected && BuildingInjection.CanEnableBuildingFromConfig())
			{
				reason = BuildingEnabled ? STRINGS.UI.RE_BUILDINGEDITOR.BUILDINGENABLEDREASONS.IS_ENABLED : STRINGS.UI.RE_BUILDINGEDITOR.BUILDINGENABLEDREASONS.IS_DISABLED;
				return BuildingEnabled;
			}
			reason = STRINGS.UI.RE_BUILDINGEDITOR.BUILDINGENABLEDREASONS.GROUP_DISABLED;
			return false;
		}

		public void SetTileRange(float range)
		{
			if (RangeMinMax != null)
				range = Math.Clamp(range, RangeMinMax.first, RangeMinMax.second);
			if(range > 0)
				BuildingTileRange = range;
		}
		public void SetWattage(float wattage)
		{
			if (WattageMinMax != null)
				wattage = Math.Clamp(wattage, WattageMinMax.first, WattageMinMax.second);
			if (wattage > 0)
				BuildingWattage = wattage;
		}
		public void SetMassCapacity(float mass)
		{
			if (StorageMinMax != null)
				mass = Math.Clamp(mass, StorageMinMax.first, StorageMinMax.second);
			if (mass > 0)
				BuildingMassCapacity = mass;
		}
		internal void SetBuildingEnabled(bool on, bool propagate = true)
		{
			BuildingEnabled = on;
			if (propagate)
				BuildingInjection?.SyncGroup(on);
		}
		internal void SetInjection(BuildingInjectionEntry injection)
		{
			BuildingInjection = injection;
			//SgtLogger.l("adding injection source for " + BuildingID + " from " + injection.BuildingID);
			injection.Source = this;
		}

		internal void ResetChanges()
		{
			BuildingMassCapacity = -1;
			BuildingWattage = -1;
			BuildingTileRange = -1;
			BuildingEnabled = true;
		}

		internal string GetDisplayName()
		{
			return global::STRINGS.UI.StripLinkFormatting(Strings.Get($"STRINGS.BUILDINGS.PREFABS.{BuildingID.ToUpperInvariant()}.NAME"));
		}
		internal string GetDisplayDescription()
		{
			return Strings.Get($"STRINGS.BUILDINGS.PREFABS.{BuildingID.ToUpperInvariant()}.EFFECT");
		}

		internal string GetCategoryText()
		{
			var text = STRINGS.UI.RE_BUILDINGEDITOR.MOD_ORIGIN_TEXT.ToString();
			if (RTB_BuildingCategoryType.TryGetCategoryName(IsInjected ? BuildingInjection.PrimaryCategory : -1, out var primary))
				text += "\n\n" + UIUtils.EmboldenText(primary);

			//for (int i = 0; i < BuildingCategories.Count; i++)
			//{
			//	int mod = BuildingCategories[i];
			//	if (!RTB_BuildingCategoryType.TryGetCategoryName(mod, out var ModName))
			//		continue;
			//	text += "\n- " + ModName;
			//}
			return text;
		}

		internal bool HasEditorConfigurables()
		{
			return HasWattage(out _) || HasStorageCapacity(out _) || HasTileRange(out _);
		}
		internal bool GetOtherConfigStrings(out string descs)
		{
			descs = OtherConfigurablesDescription;
			return IsOtherConfigurable;
		}

		internal void SetIsGenerator(bool v)
		{
			IsGenerator = v;
		}
		internal void SetIsOtherConfigurable(bool v, List<string> descriptions = null)
		{
			IsOtherConfigurable = v;
			string tt = STRINGS.UI.RE_BUILDINGEDITOR.OTHERCONFIGLOCATION;
			if (descriptions != null && descriptions.Count > 0)
			{
				tt += ":";
				foreach (var desc in descriptions)
				{
					tt += "\n• " + desc;
				}
			}
			OtherConfigurablesDescription = tt;
		}
	}
}
