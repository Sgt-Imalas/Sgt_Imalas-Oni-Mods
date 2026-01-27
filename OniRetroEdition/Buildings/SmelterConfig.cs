using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace OniRetroEdition.Buildings
{
	internal class SmelterConfig : IBuildingConfig
	{
		public static readonly string ID = "RetroOni_Smelter";

		public override BuildingDef CreateBuildingDef()
		{
			float[] tieR5 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
			string[] allMinerals = TUNING.MATERIALS.ALL_MINERALS;
			EffectorValues tieR6 = NOISE_POLLUTION.NOISY.TIER6;
			EffectorValues tieR2 = TUNING.BUILDINGS.DECOR.PENALTY.TIER2;
			EffectorValues noise = tieR6;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 2, "smelter_kanim", 30, 60f, tieR5, allMinerals, 2400f, BuildLocationRule.OnFloor, tieR2, noise);
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 1200f;
			buildingDef.SelfHeatKilowattsWhenActive = 16f;
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.UtilityInputOffset = new CellOffset(1, 0);
			buildingDef.OutputConduitType = ConduitType.Liquid;
			buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
			buildingDef.ViewMode = OverlayModes.Power.ID;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.AudioSize = "large";
			buildingDef.AddSearchTerms((string)SEARCH_TERMS.METAL);
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<DropAllWorkable>();
			go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
			LiquidCooledRefinery fabricator = go.AddOrGet<LiquidCooledRefinery>();
			fabricator.duplicantOperated = true;
			fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
			fabricator.keepExcessLiquids = true;
			
			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<CopyBuildingSettings>();
			ComplexFabricatorWorkable fabricatorWorkable = go.AddOrGet<ComplexFabricatorWorkable>();
			fabricatorWorkable.SetOffsets([new(-1, 0)]);
			BuildingTemplates.CreateComplexFabricatorStorage(go, (ComplexFabricator)fabricator);
			fabricator.coolantTag = MetalRefineryConfig.COOLANT_TAG;
			fabricator.minCoolantMass = 400f;
			fabricator.outStorage.capacityKg = 2000f;
			fabricator.thermalFudge = 0.8f;
			fabricator.inStorage.SetDefaultStoredItemModifiers(MetalRefineryConfig.RefineryStoredItemModifiers);
			fabricator.buildStorage.SetDefaultStoredItemModifiers(MetalRefineryConfig.RefineryStoredItemModifiers);
			fabricator.outStorage.SetDefaultStoredItemModifiers(MetalRefineryConfig.RefineryStoredItemModifiers);
			fabricator.outputOffset = new Vector3(1f, 0.5f);
			KAnimFile[] kanimFileArray = new KAnimFile[1]
			{
				Assets.GetAnim((HashedString) "anim_interacts_smelter2_kanim")
			};
			fabricatorWorkable.overrideAnims = kanimFileArray;
			go.AddOrGet<RequireOutputs>().ignoreFullPipe = true;
			ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
			conduitConsumer.capacityTag = GameTags.Liquid;
			conduitConsumer.capacityKG = 800f;
			conduitConsumer.storage = fabricator.inStorage;
			conduitConsumer.alwaysConsume = true;
			conduitConsumer.forceAlwaysSatisfied = true;
			ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
			conduitDispenser.storage = fabricator.outStorage;
			conduitDispenser.conduitType = ConduitType.Liquid;
			conduitDispenser.elementFilter = (SimHashes[])null;
			conduitDispenser.alwaysDispense = true;

			Prioritizable.AddRef(go);


			var metalRefineryRecipes = ComplexRecipeManager.Get().preProcessRecipes.Where(r => r.fabricators.Contains(MetalRefineryConfig.ID));
			foreach(var recipe in metalRefineryRecipes)
			{
				string id = ComplexRecipeManager.MakeRecipeID(ID, recipe.ingredients, recipe.results);
				new ComplexRecipe(id, recipe.ingredients, recipe.results)
				{
					time = recipe.time * 1.25f,
					description = recipe.description,
					customName = recipe.customName,
					nameDisplay = recipe.nameDisplay,
					fabricators = [TagManager.Create(ID)]
				};
			}
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			SymbolOverrideControllerUtil.AddToPrefab(go);
			go.AddOrGetDef<PoweredActiveStoppableController.Def>();
			go.GetComponent<KPrefabID>().prefabSpawnFn += (KPrefabID.PrefabFn)(game_object =>
			{
				ComplexFabricatorWorkable component = game_object.GetComponent<ComplexFabricatorWorkable>();
				component.WorkerStatusItem = Db.Get().DuplicantStatusItems.Processing;
				component.AttributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
				component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
				component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
				component.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
			});
		}
	}

}
