using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RonivansLegacy_ChemicalProcessing.STRINGS.UI;
using UtilLibs;
using TUNING;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesEngineering
{
	public class CementMixerConfig : IBuildingConfig
	{
		public static string ID = "CementMixer";
		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER5;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 2, "cement_mixer_kanim", 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, tier, 0.2f);
			buildingDef.Overheatable = false;
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 120f;
			buildingDef.ExhaustKilowattsWhenActive = 16f;
			buildingDef.SelfHeatKilowattsWhenActive = 4f;
			buildingDef.AudioCategory = "HollowMetal";
			return buildingDef;
		}
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
			go.AddOrGet<DropAllWorkable>();
			go.AddOrGet<BuildingComplete>().isManuallyOperated = true;

			ComplexFabricatorWorkable workable = go.AddOrGet<ComplexFabricatorWorkable>();
			workable.overrideAnims = [Assets.GetAnim("anim_interacts_metalrefinery_kanim")];

			go.AddOrGet<LoopingSounds>();

			ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
			complexFabricator.heatedTemperature = 298.15f;
			complexFabricator.duplicantOperated = true;
			complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<CopyBuildingSettings>();
			BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
			this.ConfigureRecipes();
			Prioritizable.AddRef(go);


		}
		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGetDef<PoweredActiveController.Def>().showWorkingStatus = true;
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			base.DoPostConfigurePreview(def, go);
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			base.DoPostConfigureUnderConstruction(go);
		}

		private void ConfigureRecipes()
		{
			//---- [ Cement from Crushables ] ----------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 40)
				.Input(RefinementRecipeHelper.GetCrushables().Select(e => e.id.CreateTag()), 25f)
				.Input(SimHashes.Sand,60)
				.Input(SimHashes.Lime,5)
				.Output(SimHashes.Cement,100)
				.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.CEMENT_MIXER_CEMENT_3, 3,1)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Custom)
				.NameOverride(CHEMICAL_COMPLEXFABRICATOR_STRINGS.CRUSHEDROCK_CEMENT)
				.Build();

			//---- [ Cement from Crushed Rock ] ----------------------------------------------------------------------------------------------------------
			///Cement from "Limestone"
			RecipeBuilder.Create(ID, 40)
				.Input(SimHashes.CrushedRock, 25f)
				.Input(SimHashes.Sand, 60)
				.Input(SimHashes.Lime, 5)
				.Output(SimHashes.Cement, 100)
				.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.CEMENT_MIXER_CEMENT_3, 3, 1)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Custom)
				.NameOverride(CHEMICAL_COMPLEXFABRICATOR_STRINGS.CRUSHEDROCK_CEMENT)
				.Build();

			//new recipes:

			RecipeBuilder.Create(ID, 30)
				.Input(SimHashes.Cement, 100)
				.Input(SimHashes.Sand, 200)
				.Input(SimHashes.CrushedRock, 300)
				.Input(SimHashes.Water, 25)
				.Output(ModElements.ConcreteBlock_Solid, 600, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.Description(CHEMICAL_COMPLEXFABRICATOR_STRINGS.ARCFURNACE_STEEL_2, 4, 1)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
				.Build();
		}
	}
}
