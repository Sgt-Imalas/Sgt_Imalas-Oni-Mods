using Rockets_TinyYetBig.Content.Defs.Entities;
using Rockets_TinyYetBig.Content.Scripts.Buildings.Research;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using static STRINGS.DUPLICANTS.CHORES;

namespace Rockets_TinyYetBig.Content.Defs.Buildings.Research
{
    class DeepSpaceResearchTelescopeConfig : IBuildingConfig
	{
		public const string ID = "RTB_DeepSpaceResearchTelescope";
		public const int SCAN_RADIUS = 4;
		public const int VERTICAL_SCAN_OFFSET = 1;
		public static readonly SkyVisibilityInfo SKY_VISIBILITY_INFO = new SkyVisibilityInfo(new CellOffset(0, 1), 3, new CellOffset(0, 1), 3, 0);

		public override string[] GetRequiredDlcIds() => DlcManager.EXPANSION1;

		public override BuildingDef CreateBuildingDef()
		{
			float[] tieR4 = [300,50];
			string[] material = [MATERIALS.REFINED_METAL,ModAssets.Tags.NeutroniumAlloy.ToString()];
			EffectorValues tieR1 = NOISE_POLLUTION.NOISY.TIER1;
			EffectorValues none = BUILDINGS.DECOR.NONE;
			EffectorValues noise = tieR1;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 4, "temporal_tear_analyzer_kanim", 30, 30f, tieR4, material, 1600f, BuildLocationRule.OnFloor, none, noise);
			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 960f;
			buildingDef.ExhaustKilowattsWhenActive = 1.25f;
			buildingDef.SelfHeatKilowattsWhenActive = 0.0f;
			buildingDef.ViewMode = OverlayModes.Power.ID;
			buildingDef.AudioCategory = "Metal";
			buildingDef.AudioSize = "large";
			buildingDef.RequiredSkillPerkID = Db.Get().SkillPerks.CanUseClusterTelescope.Id;
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
			
			///go.AddOrGet<InOrbitRequired>();

			var fabricator = go.AddOrGet<DeepSpaceAnalyzer>();
			fabricator.duplicantOperated = true;
			BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);

			var fabricatorWorkable = go.AddOrGet<DeepSpaceAnalyzerWorkable>();
			fabricatorWorkable.overrideAnims = [Assets.GetAnim("anim_interacts_temporal_tear_analyzer_mod_kanim")];


			go.AddOrGetDef<SkyVisibilityMonitor.Def>().skyVisibilityInfo = SKY_VISIBILITY_INFO;
			go.AddOrGet<LoopingSounds>();
			ConfigureRecipes();
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			AddVisualizer(go);
			RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
			roomTracker.requiredRoomType = Db.Get().RoomTypes.Laboratory.Id;
			roomTracker.requirement = RoomTracker.Requirement.Required;
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go) => AddVisualizer(go);

		public override void DoPostConfigureUnderConstruction(GameObject go) => AddVisualizer(go);

		private static void AddVisualizer(GameObject prefab)
		{
			SkyVisibilityVisualizer visibilityVisualizer = prefab.AddOrGet<SkyVisibilityVisualizer>();
			visibilityVisualizer.OriginOffset.y = 1;
			visibilityVisualizer.RangeMin = -3;
			visibilityVisualizer.RangeMax = 3;
			visibilityVisualizer.SkipOnModuleInteriors = false;
		}
		private void ConfigureRecipes()
		{
			ComplexRecipe.RecipeElement[] inputs =[new (EmptyDataCardConfig.TAG, 1f)];
			ComplexRecipe.RecipeElement[] outputs =[new (DeepSpaceInsightConfig.TAG, 1f)];
			ComplexRecipe complexRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(ID, inputs, outputs), inputs, outputs)
			{
				time = 600f,
				description = (string)STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.RTB_DEEPSPACEINSIGHT.RECIPE_DESC,
				nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
				fabricators = new List<Tag>() { (Tag)ID},
				sortOrder = 21
			};
		}
	}

}
