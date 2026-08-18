using BionicBoostersPlus.Content.Defs.Buildings;
using BionicBoostersPlus.Content.Scripts;
using Database;
using Klei.AI;
using rail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;
using UtilLibs;
using static BionicBoostersPlus.STRINGS.ITEMS.BIONIC_BOOSTERS;
using static BionicBoostersPlus.Content.ModDb.BB_TUNING;

namespace BionicBoostersPlus.Content.ModDb
{
	internal class BB_Boosters
	{
		const string DreamBoosterID = "BB_Booster_Dream";
		const string BatteryBoosterID = "BB_Booster_Batteryslot";
		const string WaterproofBoosterID = "BB_Booster_Waterproofed";
		const string MedikitBoosterID = "BB_Booster_Medikit";

		public static Dictionary<string, string> OC_Boosters_with_original = [];


		static BionicUpgradeComponentConfig ConfigInstance = null;
		public static void RegisterBoosters(BionicUpgradeComponentConfig instance, List<GameObject> boosterList)
		{
			ConfigInstance = instance;
			MakeBoosters_Overclocked(instance, boosterList);

			MakeBooster_Dreamer(boosterList);
			MakeBooster_BatterySlot(boosterList);
			MakeBooster_Waterproofing(boosterList);
			MakeBooster_Medikit(boosterList);

			//release reference.
			ConfigInstance = null;
		}
		#region OC boosters
		public static void MakeBoosters_Overclocked(BionicUpgradeComponentConfig instance, List<GameObject> boosterList)
		{
			var db = Db.Get();
			var perks = db.SkillPerks;
			var attributes = db.Attributes;

			//Construction
			boosterList.Add(MakeOCBooster(BionicUpgradeComponentConfig.Booster_Construct1,
				"basic_construction_0",
				attributes.Construction.Id,
				[perks.CanDemolish],
				["hat_role_building1", "hat_role_building2", "hat_role_building3"]
				));

			//Digging
			boosterList.Add(MakeOCBooster(BionicUpgradeComponentConfig.Booster_Dig2,
				"excavation_1",
				attributes.Digging.Id,
				[perks.CanDigVeryFirm, perks.CanDigNearlyImpenetrable, perks.CanDigSuperDuperHard],
				["hat_role_mining1", "hat_role_mining2", "hat_role_mining3"],
				[perks.CanDigRadioactiveMaterials],
				["hat_role_mining4"]
				));

			//Farming
			List<SkillPerk> farmingSkills = [perks.CanFarmTinker, perks.CanFarmStation, perks.CanSalvagePlantFiber];
			if (DlcManager.IsContentSubscribed(DlcManager.DLC5_ID))
				farmingSkills.Add(perks.CanFarmClams);
			if (DlcManager.IsExpansion1Active())
				farmingSkills.Add(perks.CanIdentifyMutantSeeds);
			boosterList.Add(MakeOCBooster(BionicUpgradeComponentConfig.Booster_Farm1,
			"agriculture_0",
			attributes.Botanist.Id,
			farmingSkills,
			["hat_role_farming1", "hat_role_farming2", "hat_role_farming3"]
			));

			//Ranching
			boosterList.Add(MakeOCBooster(BionicUpgradeComponentConfig.Booster_Ranch1,
				"ranching_0",
				attributes.Ranching.Id,
				[perks.CanWrangleCreatures, perks.CanUseRanchStation, perks.CanUseMilkingStation],
				["hat_role_rancher1", "hat_role_rancher2"]
			));

			//Cooking
			boosterList.Add(MakeOCBooster(BionicUpgradeComponentConfig.Booster_Cook1,
				"cooking_0",
				attributes.Cooking.Id,
				[perks.CanElectricGrill, perks.CanDeepFry, perks.CanGasRange, perks.CanSushiBar, perks.CanSpiceGrinder],
				["hat_role_cooking1", "hat_role_cooking2"]
			));

			//Artist
			boosterList.Add(MakeOCBooster(BionicUpgradeComponentConfig.Booster_Art1,
				"creativity_0",
				attributes.Art.Id,
				[perks.CanArt, perks.CanClothingAlteration, perks.CanArtGreat],
				["hat_role_art1", "hat_role_art2", "hat_role_art3"],
				[perks.CanStudyArtifact]
			));

			//Researching

			boosterList.Add(MakeOCBooster(BionicUpgradeComponentConfig.Booster_Research1,
				"science_4",
				attributes.Learning.Id,
				[perks.AllowAdvancedResearch, perks.CanStudyWorldObjects, perks.AllowGeyserTuning, perks.AllowChemistry],
				["hat_role_research1", "hat_role_research2"]
			));
			if (DlcManager.IsPureVanilla())
			{
				boosterList.Add(MakeOCBooster(BionicUpgradeComponentConfig.Booster_Research2,
					"science_2",
					attributes.Learning.Id,
					[perks.CanMissionControl, perks.AllowInterstellarResearch],
					["hat_role_research3"]
				));
			}
			else
			{
				boosterList.Add(MakeOCBooster(BionicUpgradeComponentConfig.Booster_Research2,
					"science_2",
					attributes.Learning.Id,
					[perks.CanMissionControl, perks.CanUseClusterTelescope, perks.AllowOrbitalResearch],
					["hat_role_research3", "hat_role_research4"]
				));
				boosterList.Add(MakeOCBooster(BionicUpgradeComponentConfig.Booster_Research3,
					"science_3",
					attributes.Learning.Id,
					[perks.AllowNuclearResearch],
					["hat_role_research5"]
				));
			}
			//Piloting
			if (DlcManager.IsPureVanilla())
			{
				boosterList.Add(MakeOCBooster(BionicUpgradeComponentConfig.Booster_PilotVanilla1,
					"piloting_vanilla_0",
					attributes.Athletics.Id,
					[perks.CanUseRockets],
					["hat_role_astronaut1", "hat_role_astronaut2"],
					athleticsAmount: 5
				));
			}
			else
			{
				boosterList.Add(MakeOCBooster(BionicUpgradeComponentConfig.Booster_Pilot1,
					"piloting_0",
					attributes.SpaceNavigation.Id,
					[perks.CanUseRocketControlStation],
					["hat_role_astronaut1", "hat_role_astronaut2"]
				));
			}
			//suit wearing
			boosterList.Add(MakeOCBooster(BionicUpgradeComponentConfig.Booster_Suits1,
					"suits_0",
					attributes.Athletics.Id,
					[perks.CanUseRocketControlStation],
					["hat_role_suits1", "hat_role_suits2"]
				));

			//carrying
			boosterList.Add(MakeOCBooster(BionicUpgradeComponentConfig.Booster_Carry1,
					"basic_strength_0",
					attributes.Strength.Id,
					[BB_SkillPerks.BB_BionicCarryCapacityOC],
					["hat_role_hauling1", "hat_role_hauling2"]
				));

			//electrical engineering
			boosterList.Add(MakeOCBooster(BionicUpgradeComponentConfig.Booster_Op1,
					"machinery_0",
					attributes.Machinery.Id,
					[perks.CanPowerTinker, perks.CanCraftElectronics],
					["hat_role_technicals1", "hat_role_technicals2"]
				));

			//mechatronics engineering
			boosterList.Add(MakeOCBooster(BionicUpgradeComponentConfig.Booster_Op2,
					"machinery_1",
					attributes.Machinery.Id,
					[perks.ConveyorBuild],
					["hat_role_engineering1"]
				));

			//doctoring
			boosterList.Add(MakeOCBooster(BionicUpgradeComponentConfig.Booster_Medicine1,
					"medicine_0",
					attributes.Caring.Id,
					[perks.CanCompound, perks.CanDoctor, perks.CanAdvancedMedicine],
					["hat_role_medicalaid1", "hat_role_medicalaid2", "hat_role_medicalaid3"]
				));

			//Tidying
			boosterList.Add(MakeOCBooster(BionicUpgradeComponentConfig.Booster_Tidy1,
					"tidy_0",
					attributes.Strength.Id,
					[perks.CanDoPlumbing, perks.CanMakeMissiles],
					["hat_role_basekeeping1", "hat_role_basekeeping2", "hat_role_pyrotechnics"]
				));

		}

		public static AttributeModifier[] GetOCModifiersForPrimaryAttribute(string boosterId, string primaryAttributeId, float primaryIncrease = 8, float athleticsIncrease = 4, float stressDeltaPerFycle = OC_Stressdelta)
		{
			Dictionary<string, float> attributes = new Dictionary<string, float>();

			var db = Db.Get();
			string atletics = db.Attributes.Athletics.Id;

			attributes[primaryAttributeId] = primaryIncrease;

			if (primaryAttributeId != atletics)
				attributes[atletics] = athleticsIncrease;

			attributes[db.Amounts.Stress.deltaAttribute.Id] = stressDeltaPerFycle / 600f;

			return ConfigInstance.CreateBoosterModifiers(boosterId, attributes);

		}

		public static string GetOCId(string originalVariant) => "bbp_overclocked_" + originalVariant;
		internal static string GetOCName(string originalVariant)
		{
			string originalName = global::STRINGS.UI.StripLinkFormatting(Strings.Get("STRINGS.ITEMS.BIONIC_BOOSTERS." + originalVariant.ToUpper() + ".NAME"));
			string name = global::STRINGS.UI.StripLinkFormatting(originalName);
			string id = GetOCId(originalVariant);

			name = global::STRINGS.UI.FormatAsLink(string.Format(STRINGS.ITEMS.BIONIC_BOOSTERS.BB_OVERCLOCKED.OC_PREFIX, name), id.ToUpperInvariant());
			return name;
		}
		public static GameObject MakeOCBooster(string originalVariant, string animStateName, string primaryAttributeId, List<SkillPerk> skillPerks, List<string> hats, List<SkillPerk> skillPerksSO = null, List<string> hatsSO = null, float athleticsAmount = 4)
		{
			string id = GetOCId(originalVariant);

			string name = GetOCName(originalVariant);
			string desc = Strings.Get("STRINGS.ITEMS.BIONIC_BOOSTERS." + originalVariant.ToUpper() + ".DESC");
			desc += "\n\n" + BB_OVERCLOCKED.OC_SUFFIX;

			if(OC_Wattage > 0)
			{
				desc+= "\n\n" + string.Format(global::STRINGS.DUPLICANTS.MODIFIERS.BIONIC_WATTS.TOOLTIP.STANDARD_ACTIVE_TEMPLATE, global::STRINGS.DUPLICANTS.MODIFIERS.BIONIC_WATTS.NAME, OC_Wattage);
			}


			Strings.Add("STRINGS.ITEMS.BIONIC_BOOSTERS." + id.ToUpper() + ".NAME", name);
			Strings.Add("STRINGS.ITEMS.BIONIC_BOOSTERS." + id.ToUpper() + ".DESC", desc);

			AttributeModifier[] attributeModifiers = GetOCModifiersForPrimaryAttribute(id, primaryAttributeId, athleticsIncrease: athleticsAmount);

			if (skillPerksSO != null)
				skillPerks.AddRange(skillPerksSO);
			if (hatsSO != null)
				hats.AddRange(hatsSO);

			var boosterDef = new BionicUpgrade_SkilledWorker.Def(id, primaryAttributeId, attributeModifiers, skillPerks.ToArray(), hats.ToArray());
			OC_Boosters_with_original[id] = originalVariant;
			ComplexRecipe.RecipeElement[] ingredients = [new ComplexRecipe.RecipeElement(originalVariant, 2f), new ComplexRecipe.RecipeElement(PowerStationToolsConfig.tag, 3f)];
			var boosterGO = BionicUpgradeComponentConfig.CreateNewUpgradeComponent(
				id,
				name,
				desc,
				OC_Wattage,
				(smi => new BionicUpgrade_SkilledWorkerOverclocked.OC_Instance(smi.GetMaster(), boosterDef)),
				 $"{boosterDef.GetDescription()}\n\n{string.Format(global::STRINGS.ITEMS.BIONIC_BOOSTERS.FABRICATION_SOURCE, (object)STRINGS.BUILDINGS.PREFABS.BBP_MK3BOOSTERMAKER.NAME)}",
				DlcManager.DLC3,
				"bbp_overclocked_upgrade_disk_kanim",
				animStateName,
				booster: BionicUpgradeComponentConfig.BoosterType.Overclocked,
				skillPerks: skillPerks.ToArray(),
				recipeInputOverride: ingredients,
				addTechItem: false);

			//float amount = 8;
			//if (UpgradesData.TryGetValue(originalVariant, out var data))
			//	amount = data.Booster switch { BoosterType.Basic => 2, BoosterType.Intermediate => 4 , _ => 8 };

			//the game automatically makes a soldering station recipe, gotta replace that for the mk3boostermaker.
			string vanillaGennedRecipe = ComplexRecipeManager.MakeRecipeID(id, ingredients, [new ComplexRecipe.RecipeElement(id, 1f)]);
			var vanillaRecipe = ComplexRecipeManager.Get().preProcessRecipes.FirstOrDefault(recipe => recipe.id == vanillaGennedRecipe);
			if (vanillaGennedRecipe != null && vanillaGennedRecipe != default)
			{
				var builder = RecipeBuilder.Create(Mk3BoosterMakerConfig.ID, vanillaRecipe.time);
				foreach (var ingredient in vanillaRecipe.ingredients)
					builder.Input(ingredient);
				foreach (var product in vanillaRecipe.results)
					builder.Output(product);
				builder.Description(vanillaRecipe.description);
				builder.NameDisplay(vanillaRecipe.nameDisplay);
				builder.InputHEP(10);
				builder.RuntimeDescription(vanillaRecipe.runTimeDescription);

				ComplexRecipeManager.Get().preProcessRecipes.Remove(vanillaRecipe);

				builder.Build();
			}

			return boosterGO;
		}
		#endregion
		public static void MakeBooster_Dreamer(List<GameObject> boosterList)
		{

			var db = Db.Get();

			string id = DreamBoosterID;
			string targetAttributeId = db.Attributes.Athletics.Id;

			SkillPerk[] skillPerks = [BB_SkillPerks.BB_BionicDream];

			AttributeModifier[] modifiers = ConfigInstance.CreateBoosterModifiers(id, new Dictionary<string, float>()
			{
				{
					targetAttributeId,
					-8
				}
			});

			//sb.Clear();
			//sb.AppendLine(global::STRINGS.UI.UISIDESCREENS.BIONIC_SIDE_SCREEN.BOOSTER_ASSIGNMENT.HEADER_PERKS);
			//sb.AppendLine(STRINGS.ITEMS.BIONIC_BOOSTERS.BB_BOOSTER_DREAM.EFFECT);
			//sb.AppendLine();
			//sb.AppendLine(string.Format(global::STRINGS.DUPLICANTS.MODIFIERS.BIONIC_WATTS.TOOLTIP.STANDARD_ACTIVE_TEMPLATE, global::STRINGS.DUPLICANTS.MODIFIERS.BIONIC_WATTS.NAME, DreamBooster_Wattage));
			//sb.AppendLine();
			//sb.AppendLine(string.Format(global::STRINGS.ITEMS.BIONIC_BOOSTERS.FABRICATION_SOURCE, global::STRINGS.BUILDINGS.PREFABS.ADVANCEDCRAFTINGTABLE.NAME));


			var boosterDef = new BionicUpgrade_DreamerBoosterMonitor.Def(id, modifiers);
			var boosterGO = BionicUpgradeComponentConfig.CreateNewUpgradeComponent(
				id,
				null,
				null,
				DreamBooster_Wattage,
				(smi => new BionicUpgrade_DreamerBoosterMonitor.Instance(smi.GetMaster(), boosterDef)),
				$"{boosterDef.GetDescription()}\n\n{string.Format(global::STRINGS.ITEMS.BIONIC_BOOSTERS.FABRICATION_SOURCE, global::STRINGS.BUILDINGS.PREFABS.ADVANCEDCRAFTINGTABLE.NAME)}",
				DlcManager.DLC3,
				"bbp_dreambooster_kanim",
				booster: BionicUpgradeComponentConfig.BoosterType.Sleep,
				skillPerks: skillPerks,
				recipeInputOverride: [new ComplexRecipe.RecipeElement(PowerStationToolsConfig.tag, 6f), new ComplexRecipe.RecipeElement((Tag)SleepClinicPajamas.ID, 1f)],
				addTechItem: false);

			boosterGO.AddOrGetDef<BionicUpgrade_DreamerBooster.Def>();

			boosterList.Add(boosterGO);

		}
		public static void MakeBooster_BatterySlot(List<GameObject> boosterList)
		{

			var db = Db.Get();

			string id = BatteryBoosterID;

			AttributeModifier[] modifiers = ConfigInstance.CreateBoosterModifiers(id, new Dictionary<string, float>()
			{
				{
					db.Attributes.BionicBatteryCountCapacity.Id,
					1
				},
				{
					db.Attributes.Athletics.Id,
					2
				},
				{
					db.Amounts.Stress.deltaAttribute.Id,
					Batteryslot_Stressdelta / CONSTS.CYCLE_LENGTH
				},
			});

			var boosterDef = new BionicUpgrade_BatterySlot.Def(id, modifiers);
			var boosterGO = BionicUpgradeComponentConfig.CreateNewUpgradeComponent(
				id,
				null,
				null,
				0,
				(smi => new BionicUpgrade_BatterySlot.Instance(smi.GetMaster(), boosterDef)),
				$"{boosterDef.GetDescription()}\n\n{string.Format(global::STRINGS.ITEMS.BIONIC_BOOSTERS.FABRICATION_SOURCE, global::STRINGS.BUILDINGS.PREFABS.ADVANCEDCRAFTINGTABLE.NAME)}",
				DlcManager.DLC3,
				"bbp_batterybooster_kanim",
				booster: BionicUpgradeComponentConfig.BoosterType.Overclocked,
				skillPerks: [],
				recipeInputOverride: [new ComplexRecipe.RecipeElement(PowerStationToolsConfig.tag, 2f), new ComplexRecipe.RecipeElement(RefinementRecipeHelper.GetAllMetals().ToArray(), 5f)],
				addTechItem: false);


			boosterList.Add(boosterGO);

		}		
		public static void MakeBooster_Waterproofing(List<GameObject> boosterList)
		{

			var db = Db.Get();

			string id = WaterproofBoosterID;

			AttributeModifier[] modifiers = ConfigInstance.CreateBoosterModifiers(id, new Dictionary<string, float>()
			{
				{
					db.Attributes.Athletics.Id,
					4
				},
				{
					db.Amounts.BionicOil.deltaAttribute.Id, Waterproofed_ExtraOilConsumption / CONSTS.CYCLE_LENGTH
				}
			});
			SkillPerk[] skillPerks = [BB_SkillPerks.BB_ReducedWaterStress];

			var boosterDef = new BionicUpgrade_SkilledWorker.Def(id, db.Attributes.Athletics.Id, modifiers,skillPerks);
			var boosterGO = BionicUpgradeComponentConfig.CreateNewUpgradeComponent(
				id,
				null,
				null,
				0,
				(smi => new BionicUpgrade_SkilledWorker.Instance(smi.GetMaster(), boosterDef)),
				$"{boosterDef.GetDescription()}\n\n{string.Format(global::STRINGS.ITEMS.BIONIC_BOOSTERS.FABRICATION_SOURCE, global::STRINGS.BUILDINGS.PREFABS.ADVANCEDCRAFTINGTABLE.NAME)}",
				DlcManager.DLC3,
				"bbp_waterproofbooster_kanim",
				booster: BionicUpgradeComponentConfig.BoosterType.Intermediate,
				skillPerks: skillPerks,
				recipeInputOverride: [new ComplexRecipe.RecipeElement(PowerStationToolsConfig.tag, 6f), new ComplexRecipe.RecipeElement(RefinementRecipeHelper.GetPlastics().ToArray(), 5f)],
				addTechItem: false);


			boosterList.Add(boosterGO);

		}
		public static void MakeBooster_Medikit(List<GameObject> boosterList)
		{
			var db = Db.Get();
			string id = MedikitBoosterID;

			AttributeModifier[] modifiers = ConfigInstance.CreateBoosterModifiers(id, new Dictionary<string, float>()
			{
				{
					db.Attributes.Athletics.Id,
					4
				}
			});

			var boosterDef = new BionicUpgrade_Medikit.Def(id, modifiers);
			var boosterGO = BionicUpgradeComponentConfig.CreateNewUpgradeComponent(
				id,
				null,
				null,
				Medkit_Wattage,
				(smi => new BionicUpgrade_Medikit.Instance(smi.GetMaster(), boosterDef)),
				$"{boosterDef.GetDescription()}\n\n{string.Format(global::STRINGS.ITEMS.BIONIC_BOOSTERS.FABRICATION_SOURCE, global::STRINGS.BUILDINGS.PREFABS.ADVANCEDCRAFTINGTABLE.NAME)}",
				DlcManager.DLC3,
				"bbp_medbooster_kanim",
				booster: BionicUpgradeComponentConfig.BoosterType.Intermediate,
				skillPerks: [],
				recipeInputOverride: [new ComplexRecipe.RecipeElement(PowerStationToolsConfig.tag, 6f), new ComplexRecipe.RecipeElement(RefinementRecipeHelper.GetPlastics().ToArray(), 5f)],
				addTechItem: false);


			boosterList.Add(boosterGO);

		}

	}
}
