using OniRetroEdition.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace OniRetroEdition.Buildings
{
	internal class MetalReclaimerConfig : IBuildingConfig
	{
		public const string ID = "RetroOni_MetalReclaimer";

		public override BuildingDef CreateBuildingDef()
		{
			float[] tieR2 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
			string[] farmable = MATERIALS.ALL_METALS;
			EffectorValues none1 = NOISE_POLLUTION.NONE;
			EffectorValues none2 = BUILDINGS.DECOR.BONUS.TIER1;
			EffectorValues noise = none1;
			var anim = Assets.GetAnim("metalreclaimer_kanim") != null ? "metalreclaimer_kanim" : "rockrefinery_kanim";
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 4, anim, 100, 30f, tieR2, farmable, 1600f, BuildLocationRule.OnFloor, none2, noise);
			SoundUtils.CopySoundsToAnim("metalreclaimer_kanim", "rockrefinery_kanim");
			buildingDef.Overheatable = false;
			buildingDef.AudioCategory = "Metal";
			buildingDef.AudioSize = "small";
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 240f;
			buildingDef.SelfHeatKilowattsWhenActive = 16f;
			buildingDef.ViewMode = OverlayModes.Power.ID;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.AudioSize = "large";
			buildingDef.AddSearchTerms((string)global::STRINGS.SEARCH_TERMS.METAL);
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<DropAllWorkable>();
			go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
			ComplexFabricator fabricator = go.AddOrGet<ComplexFabricator>();
			fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
			fabricator.duplicantOperated = false;
			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<CopyBuildingSettings>();
			BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);
			Prioritizable.AddRef(go);

			var rockCrusherRecipe = ComplexRecipeManager.Get().preProcessRecipes.Where(r => r.fabricators.Contains(RockCrusherConfig.ID)).ToList();
			foreach (var recipe in rockCrusherRecipe)
			{
				string id = ComplexRecipeManager.MakeRecipeID(ID, recipe.ingredients, recipe.results);
				new ComplexRecipe(id, recipe.ingredients, recipe.results)
				{
					time = recipe.time * 1.2f,
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
		}
	}
}
