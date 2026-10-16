using MugConversionRecipes.Content.Scripts;
using TUNING;
using UnityEngine;

namespace MugConversionRecipes.Content.Defs
{
	internal class ArtifactRecombinatorConfig : IBuildingConfig
	{
		public static readonly string ID = "MCR_Recombinator";
		private const float INPUT_KG = 100f;
		private const float OUTPUT_KG = 100f;
		public static float OUTPUT_TEMPERATURE = 313.15f;
		private HashedString[] dupeInteractAnims;

		public override BuildingDef CreateBuildingDef()
		{
			float[] tieR5 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
			string[] allMetals = TUNING.MATERIALS.ALL_METALS;
			EffectorValues tieR6 = NOISE_POLLUTION.NOISY.TIER6;
			EffectorValues tieR2 = TUNING.BUILDINGS.DECOR.PENALTY.TIER2;
			EffectorValues noise = tieR6;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 3, "fabricator_generic_kanim", 100, 300f, tieR5, allMetals, 2400f, BuildLocationRule.OnFloor, tieR2, noise);
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 960f;
			buildingDef.SelfHeatKilowattsWhenActive = 32f;
			buildingDef.ViewMode = OverlayModes.Power.ID;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.AudioSize = "large";
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddOrGet<DropAllWorkable>();
			go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
			var fabricator = go.AddOrGet<UncharmingComplexFabricator>();
			fabricator.heatedTemperature = OUTPUT_TEMPERATURE;
			fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
			fabricator.duplicantOperated = true;
			fabricator.fetchChoreTypeIdHash = Db.Get().ChoreTypes.FabricateFetch.IdHash;
			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<CopyBuildingSettings>();
			go.AddOrGet<ComplexFabricatorWorkable>();
			BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);
			Prioritizable.AddRef(go);			
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			ComplexFabricatorWorkable component = go.GetComponent<ComplexFabricatorWorkable>();
			component.WorkerStatusItem = Db.Get().DuplicantStatusItems.Processing;
			component.AttributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
			component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
			component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
			component.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;

			component.overrideAnims = [Assets.GetAnim("anim_interacts_fabricator_generic_kanim")];	
			component.synchronizeAnims = true;
		}
	}
}
