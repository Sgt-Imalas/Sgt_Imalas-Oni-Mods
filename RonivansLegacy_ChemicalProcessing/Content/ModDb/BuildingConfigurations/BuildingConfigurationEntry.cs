using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb.BuildingConfigurations
{
	class BuildingConfigurationEntry
	{
		public string BuildingID;
		public List<SourceMod> ModsFrom = new();
		public bool BuildingEnabled = true;
		public float BuildingMassCapacity = -1;
		public float BuildingWattage = -1;
		[JsonIgnore]
		public float BuildingMassCapacityDefault = -1;
		[JsonIgnore]
		public float BuildingWattageDefault = -1;
		[JsonIgnore]
		public BuildingInjectionEntry BuildingInjection = null;
		
		
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
		public BuildingConfigurationEntry SetDefaultWattage(float wattage)
		{
			BuildingWattageDefault = wattage;
			return this;
		}
		public BuildingConfigurationEntry SetDefaultStorageCapacity(float wattage)
		{
			BuildingMassCapacityDefault = wattage;
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

		public bool IsBuildingEnabled()
		{
			return BuildingEnabled;
		}

		public void SetWattage(float wattage)
		{
			if (wattage >= 0)
				BuildingWattage = wattage;
		}
		public void SetMassCapacity(float mass)
		{
			if (mass > 0)
				BuildingMassCapacity = mass;
		}
		internal void SetBuildingEnabled(bool on)
		{
			BuildingEnabled = on;
		}
		internal void SetInjection(BuildingInjectionEntry injection)
		{
			BuildingInjection = injection;
			ModsFrom = injection.GetModsFrom();
		}

		internal void ResetChanges()
		{
			BuildingMassCapacity = -1;
			BuildingWattage = -1;
			BuildingEnabled = true;
		}

		internal string GetDisplayName()
		{
			return Strings.Get($"STRINGS.BUILDINGS.PREFABS.{BuildingID.ToUpperInvariant()}.NAME");
		}
		internal string GetDisplayDescription()
		{
			return Strings.Get($"STRINGS.BUILDINGS.PREFABS.{BuildingID.ToUpperInvariant()}.EFFECT");
		}

		internal string GetModOriginText()
		{
			var text = STRINGS.UI.BUILDINGEDITOR.MOD_ORIGIN_TEXT.ToString();
			for(int i = 0; i < ModsFrom.Count; i++)
			{
				var mod = ModsFrom[i];
				var ModName = Strings.Get($"STRINGS.AIO_MODSOURCE.{mod.ToString().ToUpperInvariant()}").ToString();
				if(ModName.Contains("MISSING"))
					ModName = mod.ToString(); // Fallback to mod name if translation is missing

				string entry = "\n- "+ ModName;
				if(i== 0)
					entry = UIUtils.EmboldenText(entry);
				text += entry;
			}
			return text;
		}

		internal bool HasConfigurables()
		{
			return HasWattage(out _) || HasStorageCapacity(out _);
		}
	}
}
