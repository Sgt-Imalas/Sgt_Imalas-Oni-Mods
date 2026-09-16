using BionicBoostersPlus.Content.Scripts;
using TUNING;
using UnityEngine;

namespace BionicBoostersPlus.Content.Defs.Buildings
{
	internal class BoosterRecyclerConfig : IBuildingConfig
	{
		public static readonly string ID = "BBP_BoosterRecycler";
		public override BuildingDef CreateBuildingDef()
		{
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
				ID, 
				3, 4,
				"bbp_booster_recycler_kanim", 
				30, 60f,
				TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, 				
				TUNING.MATERIALS.ALL_METALS, 
				2400f, 
				BuildLocationRule.OnFloor,
				TUNING.BUILDINGS.DECOR.PENALTY.TIER2,
				NOISE_POLLUTION.NOISY.TIER6);

			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 360f;
			buildingDef.SelfHeatKilowattsWhenActive = 8f;
			buildingDef.ViewMode = OverlayModes.Power.ID;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.AudioSize = "large";
			return buildingDef;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			var prefab = go.GetComponent<KPrefabID>();
			prefab.AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);

			go.AddOrGet<DropAllWorkable>();
			var fabricator = go.AddOrGet<ComplexFabricatorRandomOutput>();
			fabricator.duplicantOperated = false;

			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<CopyBuildingSettings>();
			go.AddOrGet<Prioritizable>();
			BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);
		}
	}
}
