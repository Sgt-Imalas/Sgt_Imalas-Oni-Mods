using HarmonyLib;
using KSerialization;
using RonivansLegacy_ChemicalProcessing;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using UtilLibs.BuildingPortUtils;
using static RonivansLegacy_ChemicalProcessing.STRINGS.UI.CHEMICAL_COMPLEXFABRICATOR_STRINGS;

namespace Dupes_Industrial_Overhaul.Chemical_Processing.Buildings
{
	//===[ CHEMICAL: GLASS FOUNDRY CONFIG ]========================================================================
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Chemical_GlassFoundryConfig : IBuildingConfig
	{
		//--[ Base Information ]-----------------------------------------------------------------------------------
		public static string ID = "Chemical_GlassFoundry";

		//--[ Identification and DLC stuff ]------------------------------------------------------------------------
		private const float INPUT_KG = 100f;

		//--[ Special Settings ]------------------------------------------------------------------------------------
		static readonly CellOffset GlassOutputOffset = new CellOffset(0, -2);
		private static readonly PortDisplayOutput GlassOutputPort = new PortDisplayOutput(ConduitType.Liquid, GlassOutputOffset);


		//--[ Building Definitions ]---------------------------------------------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			float[] singleArray1 = [500f, 200f];
			string[] textArray1 = [SimHashes.Ceramic.ToString(), "RefinedMetal"];

			EffectorValues noise = TUNING.NOISE_POLLUTION.NOISY.TIER6;
			BuildingDef def = BuildingTemplates.CreateBuildingDef(ID, 3, 3, "glass_foundry_kanim", 60, 90f, singleArray1, textArray1, 2400f, BuildLocationRule.OnFloor, TUNING.BUILDINGS.DECOR.PENALTY.TIER2, noise, 0.2f);
			def.RequiresPowerInput = true;
			def.EnergyConsumptionWhenActive = 2400f;
			def.SelfHeatKilowattsWhenActive = 8f;
			def.ViewMode = OverlayModes.Power.ID;
			def.AudioCategory = "HollowMetal";
			def.AudioSize = "large";
			SoundUtils.CopySoundsToAnim("glass_foundry_kanim", "glassrefinery_kanim");
			return def;
		}

		//--[ Building Operation Definitions ]-------------------------------------------------------------------------
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddOrGet<DropAllWorkable>();
			go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<CopyBuildingSettings>();

			Chemical_GlassForge glassforge = go.AddOrGet<Chemical_GlassForge>();
			glassforge.MeltingTemperature = ElementLoader.FindElementByHash(SimHashes.MoltenGlass).lowTemp;
			glassforge.HeatedOutputOffset = GlassOutputOffset;

			ComplexFabricator fabricator = go.AddOrGet<ComplexFabricator>();
			fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;

			ComplexFabricatorWorkable workable = go.AddOrGet<ComplexFabricatorWorkable>();
			workable.overrideAnims = [Assets.GetAnim("anim_interacts_fabricator_generic_kanim")];

			go.AddOrGet<LoopingSounds>();

			fabricator.duplicantOperated = true;

			BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);
			fabricator.outStorage.capacityKg = 2000f;
			fabricator.storeProduced = true;
			fabricator.inStorage.SetDefaultStoredItemModifiers(ModAssets.AllStorageMods);
			fabricator.buildStorage.SetDefaultStoredItemModifiers(ModAssets.AllStorageMods);
			fabricator.outStorage.SetDefaultStoredItemModifiers(ModAssets.AllStorageMods);
			fabricator.outputOffset = new Vector3(1f, 0.5f);
			fabricator.heatedTemperature = 296.15f;

			PipedConduitDispenser dispenser = go.AddComponent<PipedConduitDispenser>();
			dispenser.storage = fabricator.outStorage;
			dispenser.elementFilter = null;
			dispenser.AssignPort(GlassOutputPort);
			dispenser.alwaysDispense = true;
			dispenser.SkipSetOperational = true;

			PipedOptionalExhaust exhaustGlass = go.AddComponent<PipedOptionalExhaust>();
			exhaustGlass.dispenser = dispenser;
			exhaustGlass.elementTag = SimHashes.MoltenGlass.CreateTag();
			exhaustGlass.capacity = 100f;

			PipedOptionalExhaust exhaustWater = go.AddComponent<PipedOptionalExhaust>();
			exhaustWater.dispenser = dispenser;
			exhaustWater.elementTag = GameTags.AnyWater;
			exhaustWater.capacity = 500f;

			this.AttachPort(go);
			Prioritizable.AddRef(go);
			this.ConfigureRecipes();
		}

		//===[ CHEMICAL: GLASS FOUNDRY RECIPES ]====================================================================================
		private void ConfigureRecipes()
		{
			//---- [ Glass Smelting ] ----------------------------------------------------------------------------------------------
			// Ingredient: Sand - 300kg        
			// Result: Molten Glass - 100kg
			//----------------------------------------------------------------------------------------------------------------------
			RecipeBuilder.Create(ID, 30f)
				.Input(SimHashes.Sand.CreateTag(), 300f)
				.Output(SimHashes.MoltenGlass.CreateTag(), 100f, ComplexRecipe.RecipeElement.TemperatureOperation.Melted)
				.Description1I1O(ARCFURNACE_SMELT)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
				.Build();

			//---- [ Ice Melting ] ---------------------------------------------------------------------------------------------------
			// Ingredient: Ice - 500kg        
			// Result: Water - 500kg
			//------------------------------------------------------------------------------------------------------------------------

			foreach(var element in ElementLoader.elements.FindAll(e => e.IsSolid && e.HasTag(GameTags.IceOre) && e.highTempTransition.HasTag(GameTags.AnyWater)))
			{
				var ownTag = element.id.CreateTag();
				var meltsTo = element.highTempTransitionTarget.CreateTag();

				RecipeBuilder.Create(ID, 10f)
					.Input(ownTag, 500f)
					.Output(meltsTo, 500f, ComplexRecipe.RecipeElement.TemperatureOperation.Melted)
					.Description1I1O(ARCFURNACE_MELT)
					.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
					.Build();
			}

		}

		private void AttachPort(GameObject go)
		{
			PortDisplayController controller = go.AddComponent<PortDisplayController>();
			controller.Init(go);

			controller.AssignPort(go, GlassOutputPort);
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			this.AttachPort(go);
		}

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			this.AttachPort(go);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			SymbolOverrideControllerUtil.AddToPrefab(go);
			go.AddOrGetDef<PoweredActiveStoppableController.Def>();
			go.GetComponent<KPrefabID>().prefabSpawnFn += delegate (GameObject game_object)
			{
				ComplexFabricatorWorkable component = game_object.GetComponent<ComplexFabricatorWorkable>();
				component.WorkerStatusItem = Db.Get().DuplicantStatusItems.Processing;
				component.AttributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
				component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
				component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
				component.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
			};
		}
	}
}

