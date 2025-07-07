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
		string BuildingID;
        string TechID;
        string PlanScreenCategory;
        string PlanScreenRelativeBuildingID;
		ModUtil.BuildingOrdering BuildingOrdering = ModUtil.BuildingOrdering.After;
		List<SourceMod> ModsFrom = new();

        public static BuildingInjectionEntry Create(string buildingID)
		{
			var entry = new BuildingInjectionEntry();
			entry.BuildingID = buildingID;
			return entry;
		}
        public BuildingInjectionEntry AddToTech(string techID)
		{
			TechID = techID;
			return this;
		}
		public BuildingInjectionEntry ForceCategory(bool move = true)
		{
			MoveExisting = move;
			return this;
		}
		public BuildingInjectionEntry AddToCategory(string category, string relativeBuildingID, ModUtil.BuildingOrdering ordering = ModUtil.BuildingOrdering.After)
        {
			PlanScreenCategory = category;
			PlanScreenRelativeBuildingID = relativeBuildingID;
			BuildingOrdering = ordering;
			return this;			
        }
		public BuildingInjectionEntry AddModFrom(SourceMod mod)
		{
			if(!ModsFrom.Contains(mod))
			{
				ModsFrom.Add(mod);
			}
			return this;
		}
		public List<SourceMod> GetModsFrom()
		{
			return ModsFrom;
		}

		internal void RegisterTech()
		{
			if(string.IsNullOrEmpty(TechID))
			{
				return;
			}
			InjectionMethods.AddBuildingToTechnology(TechID, BuildingID);
		}
		internal void RegisterPlanscreen()
		{
			if (string.IsNullOrEmpty(PlanScreenCategory) || string.IsNullOrEmpty(PlanScreenRelativeBuildingID))
			{
				return;
			}
			if(MoveExisting)
				InjectionMethods.MoveExistingBuildingToNewCategory(PlanScreenCategory,BuildingID,PlanScreenRelativeBuildingID,ordering: BuildingOrdering);
			else
				InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanScreenCategory, BuildingID, PlanScreenRelativeBuildingID, ordering: BuildingOrdering);
		}
	}
}
