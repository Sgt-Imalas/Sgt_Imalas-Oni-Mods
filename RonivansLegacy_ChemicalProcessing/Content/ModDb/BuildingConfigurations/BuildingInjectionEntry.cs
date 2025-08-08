using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb.BuildingConfigurations
{
    public class BuildingInjectionEntry
	{
		bool MoveExisting = false;
		string _buildingID;
        string _techID;
        string _planScreenCategory;
        string PlanScreenRelativeBuildingID;
		public string BuildingID => _buildingID;
		public string TechID => _techID;
		public string PlanScreenCategory => _planScreenCategory;



		ModUtil.BuildingOrdering BuildingOrdering = ModUtil.BuildingOrdering.After;
		List<SourceModInfo> modsFrom = new();
		public List<SourceModInfo> SourceMods => modsFrom;

		public static BuildingInjectionEntry Create(string buildingID)
		{
			var entry = new BuildingInjectionEntry();
			entry._buildingID = buildingID;
			return entry;
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
		public BuildingInjectionEntry AddToCategory(string category, string relativeBuildingID, ModUtil.BuildingOrdering ordering = ModUtil.BuildingOrdering.After)
        {
			_planScreenCategory = category;
			PlanScreenRelativeBuildingID = relativeBuildingID;
			BuildingOrdering = ordering;
			return this;			
        }
		public BuildingInjectionEntry AddModFrom(SourceModInfo mod)
		{
			if(!modsFrom.Contains(mod))
			{
				modsFrom.Add(mod);
			}
			return this;
		}
		public List<SourceModInfo> GetModsFrom()
		{
			return modsFrom;
		}

		internal void RegisterTech()
		{
			if(string.IsNullOrEmpty(_techID))
			{
				return;
			}
			InjectionMethods.AddBuildingToTechnology(_techID, _buildingID);
		}
		internal void RegisterPlanscreen()
		{
			if (string.IsNullOrEmpty(_planScreenCategory) || string.IsNullOrEmpty(PlanScreenRelativeBuildingID))
			{
				return;
			}
			if(MoveExisting)
				InjectionMethods.MoveExistingBuildingToNewCategory(_planScreenCategory,_buildingID,PlanScreenRelativeBuildingID,ordering: BuildingOrdering);
			else
				InjectionMethods.AddBuildingToPlanScreenBehindNext(_planScreenCategory, _buildingID, PlanScreenRelativeBuildingID, ordering: BuildingOrdering);
		}
	}
}
