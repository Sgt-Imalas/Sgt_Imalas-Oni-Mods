using Rockets_TinyYetBig.Content.Defs.Entities;
using Rockets_TinyYetBig.Content.Scripts.Buildings.Research;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Content.Defs.Buildings.Research
{
	class DeepSpaceResearchCenterConfig : IBuildingConfig
	{
		public const string ID = "RTB_DeepSpaceResearchCenter";
		public const float BASE_SECONDS_PER_POINT = 500f;
		public const float MASS_PER_POINT = 1f;
		public const float BASE_MASS_PER_SECOND = MASS_PER_POINT / BASE_SECONDS_PER_POINT;
		public const float CAPACITY = 300f;
		public static Tag INPUT_MATERIAL => DeepSpaceInsightConfig.TAG;

		public override BuildingDef CreateBuildingDef()
		{
			float[] tieR4 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
			string[] allMetals = MATERIALS.ALL_METALS;
			EffectorValues tieR1 = NOISE_POLLUTION.NOISY.TIER1;
			EffectorValues none = TUNING.BUILDINGS.DECOR.NONE;
			EffectorValues noise = tieR1;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 4, 4, "supermaterial_refinery_kanim", 30, 30f, tieR4, allMetals, 1600f, BuildLocationRule.OnFloor, none, noise);
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 2000f;
			buildingDef.ExhaustKilowattsWhenActive = 0.5f;
			buildingDef.SelfHeatKilowattsWhenActive = 4f;
			buildingDef.ViewMode = OverlayModes.Power.ID;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.AudioSize = "large";
			buildingDef.RequiredSkillPerkID = Db.Get().SkillPerks.AllowOrbitalResearch.Id;
			buildingDef.AddSearchTerms((string)SEARCH_TERMS.RESEARCH);
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			KPrefabID component = go.GetComponent<KPrefabID>();
			component.AddTag(GameTags.RocketInteriorBuilding);
			component.AddTag(ModAssets.Tags.SpaceStationOnlyInteriorBuilding);
			component.AddTag(RoomConstraints.ConstraintTags.ScienceBuilding);

			go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
			Prioritizable.AddRef(go);
			Storage storage = go.AddOrGet<Storage>();
			storage.capacityKg = 1000f;
			storage.showInUI = true;

			//go.AddOrGet<InOrbitRequired>();

			ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
			manualDeliveryKg.SetStorage(storage);
			manualDeliveryKg.RequestedItemTag = INPUT_MATERIAL;
			manualDeliveryKg.refillMass = 3f;
			manualDeliveryKg.capacity = 300f;
			manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.ResearchFetch.IdHash;

			var researchCenter = go.AddOrGet<DeepSpaceResearchCenter>();
			researchCenter.overrideAnims =
			[
				Assets.GetAnim((HashedString) "anim_interacts_research_space_kanim")
			];
			researchCenter.research_point_type_id = ModAssets.DeepSpaceScienceID;
			researchCenter.inputMaterial = INPUT_MATERIAL;
			researchCenter.mass_per_point = 1f; 
			researchCenter.requiredSkillPerk = Db.Get().SkillPerks.AllowInterstellarResearch.Id;
			researchCenter.workLayer = Grid.SceneLayer.BuildingFront;
			ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
			elementConverter.consumedElements =
			[new ElementConverter.ConsumedElement(INPUT_MATERIAL, BASE_MASS_PER_SECOND)];
			elementConverter.showDescriptors = false;
			go.AddOrGetDef<PoweredController.Def>();
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
		}
	}
}
