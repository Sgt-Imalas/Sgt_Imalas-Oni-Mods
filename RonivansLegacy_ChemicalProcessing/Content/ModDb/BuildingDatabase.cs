using Biochemistry.Buildings;
using Dupes_Industrial_Overhaul.Chemical_Processing.Buildings;
using Dupes_Machinery.Biological_Vats;
using Dupes_Machinery.Ethanol_Still;
using HarmonyLib;
using High_Pressure_Applications.BuildingConfigs;
using Metallurgy.Buildings;
using Mineral_Processing;
using Mineral_Processing_Mining.Buildings;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.ChemicalProcessing_BioChem;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.ChemicalProcessing_IO;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.CustomGenerators;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.CustomReservoirs;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesEngineering;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesEngineering.Tiles;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesEngineering.Walls;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesLogistics;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesRefrigeration;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.HighPressureApplications;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.HighPressureApplications.HighCapacityLogisticRails;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.HighPressureApplications.HPA_Gas;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.HighPressureApplications.HPA_liquid;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.HighPressureApplications.HPA_Solid;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.MineralProcessing_Metallurgy;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.NuclearProcessing;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using UtilLibs;
using static ResearchTypes;
using static RonivansLegacy_ChemicalProcessing.STRINGS.BUILDINGS.PREFABS;
using static RonivansLegacy_ChemicalProcessing.STRINGS.BUILDINGS.PREFABS.SPACERWALL.FACADES;
using static UtilLibs.GameStrings;
namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	class BuildingDatabase
	{
		/// <summary>
		/// Registers these as building tags, otherwise the name strings of those elements break
		/// </summary>
		internal static void RegisterAdditionalBuildingElements()
		{
			GameTags.MaterialBuildingElements.Add(SimHashes.Ceramic.CreateTag());
			GameTags.MaterialBuildingElements.Add(SimHashes.Brick.CreateTag());
			GameTags.MaterialBuildingElements.Add(SimHashes.Tungsten.CreateTag());
			GameTags.MaterialBuildingElements.Add(ModElements.Chromium_Solid.Tag);
			GameTags.MaterialBuildingElements.Add(ModElements.StainlessSteel_Solid.Tag);
			GameTags.MaterialBuildingElements.Add(ModAssets.Tags.AIO_SulphuricAcidBuildable);
		}
		internal static void RegisterExtraStrings()
		{
			//registering element names as tags for building material elements
			Strings.Add("STRINGS.MISC.TAGS.TUNGSTEN", global::STRINGS.ELEMENTS.TUNGSTEN.NAME);
			Strings.Add("STRINGS.MISC.TAGS.SANDSTONE", global::STRINGS.ELEMENTS.SANDSTONE.NAME);
			Strings.Add("STRINGS.MISC.TAGS.GRANITE", global::STRINGS.ELEMENTS.GRANITE.NAME);
			Strings.Add("STRINGS.MISC.TAGS.CEMENT", global::STRINGS.ELEMENTS.CEMENT.NAME);
			Strings.Add("STRINGS.MISC.TAGS.IGNEOUSROCK", global::STRINGS.ELEMENTS.IGNEOUSROCK.NAME);
			Strings.Add("STRINGS.MISC.TAGS.CONCRETEBLOCK", STRINGS.ELEMENTS.CONCRETEBLOCK.NAME);
			Strings.Add("STRINGS.MISC.TAGS.BRICK", global::STRINGS.ELEMENTS.BRICK.NAME);
			Strings.Add("STRINGS.MISC.TAGS.BITUMEN", global::STRINGS.ELEMENTS.BITUMEN.NAME);
			Strings.Add("STRINGS.MISC.TAGS.REFINEDCARBON", global::STRINGS.ELEMENTS.REFINEDCARBON.NAME);
			Strings.Add("STRINGS.MISC.TAGS.AIO_SULPHURICACIDBUILDABLE", STRINGS.ELEMENTS.LIQUIDSULFURIC.NAME);
			Strings.Add("STRINGS.MISC.TAGS.AIO_CHROMIUM_SOLID", STRINGS.ELEMENTS.AIO_CHROMIUM_SOLID.NAME);
			Strings.Add("STRINGS.MISC.TAGS.AIO_STAINLESSSTEEL_SOLID", STRINGS.ELEMENTS.AIO_STAINLESSSTEEL_SOLID.NAME);

			//renaming the vanilla moulding tile to marble tile
			global::STRINGS.BUILDINGS.PREFABS.MOULDINGTILE.NAME = STRINGS.BUILDINGS.PREFABS.MARBLETILESTRINGS.NAME;
			global::STRINGS.BUILDINGS.PREFABS.MOULDINGTILE.DESC = STRINGS.BUILDINGS.PREFABS.MARBLETILESTRINGS.DESC;
			global::STRINGS.BUILDINGS.PREFABS.MOULDINGTILE.EFFECT = STRINGS.BUILDINGS.PREFABS.MARBLETILESTRINGS.EFFECT;

			var gasPipeString = Strings.Get("STRINGS.BUILDINGS.PREFABS.HIGHPRESSUREGASCONDUIT.EFFECT");
			gasPipeString.String = gasPipeString.String.Replace("{CAPACITY}", GameUtil.GetFormattedMass(HighPressureConduitRegistration.GasCap_HP));
			var liquidPipeString = Strings.Get("STRINGS.BUILDINGS.PREFABS.HIGHPRESSURELIQUIDCONDUIT.EFFECT");
			liquidPipeString.String = liquidPipeString.String.Replace("{CAPACITY}", GameUtil.GetFormattedMass(HighPressureConduitRegistration.LiquidCap_HP));

			global::STRINGS.BUILDINGS.PREFABS.BIODIESELENGINE.EFFECT = global::STRINGS.BUILDINGS.PREFABS.BIODIESELENGINE.EFFECT.Replace(global::STRINGS.ELEMENTS.REFINEDLIPID.NAME, STRINGS.MISC.TAGS.AIO_BIOFUEL);
			global::STRINGS.BUILDINGS.PREFABS.BIODIESELENGINECLUSTER.EFFECT = global::STRINGS.BUILDINGS.PREFABS.BIODIESELENGINECLUSTER.EFFECT.Replace(global::STRINGS.ELEMENTS.REFINEDLIPID.NAME, STRINGS.MISC.TAGS.AIO_BIOFUEL);

			global::STRINGS.BUILDINGS.PREFABS.SODAFOUNTAIN.EFFECT = global::STRINGS.BUILDINGS.PREFABS.SODAFOUNTAIN.EFFECT.Replace(global::STRINGS.UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE"), STRINGS.MISC.TAGS.SODAFOUNTAINGAS);
		}

		public static void RegisterOilWellCapCustomPiping()
		{
			if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
			{
				var sodaFountain = Assets.GetBuildingDef(SodaFountainConfig.ID);
				CustomSodaFountain.AttachPorts(sodaFountain.BuildingPreview);
				CustomSodaFountain.AttachPorts(sodaFountain.BuildingUnderConstruction);
				CustomSodaFountain.AttachPorts(sodaFountain.BuildingComplete);


				var oilWell = Assets.GetBuildingDef(OilWellCapConfig.ID);
				Custom_OilWellCapConfig.AttachPorts(oilWell.BuildingPreview);
				Custom_OilWellCapConfig.AttachPorts(oilWell.BuildingUnderConstruction);
				Custom_OilWellCapConfig.AttachPorts(oilWell.BuildingComplete);
			}
		}

		public static void RegisterBuildings()
		{
			///disabled state is now handled by any enabled mod id in the registration method

			BuildingManager.LoadConfigFile();
			//if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
			RegisterBuildings_ChemicalProcessingIndustrialOverhaul();
			//if (Config.Instance.MineralProcessing_Metallurgy_Enabled)
			RegisterBuildings_MineralProcessingMetallurgy();
			//if (Config.Instance.ChemicalProcessing_BioChemistry_Enabled)
			RegisterBuildings_ChemicalProcessingBioChemistry();
			//if (Config.Instance.MineralProcessing_Mining_Enabled)
			RegisterBuildings_MineralProcessingMining();
			//if (Config.Instance.DupesMachinery_Enabled)
			RegisterBuildings_DupesMachinery();
			//if (Config.Instance.NuclearProcessing_Enabled)
			RegisterBuildings_NuclearProcessing();
			//if (Config.Instance.DupesEngineering_Enabled)
			RegisterBuildings_DupesEngineering();
			//if (Config.Instance.CustomReservoirs_Enabled)
			RegisterBuildings_CustomReservoirs();
			//if (Config.Instance.HighPressureApplications_Enabled)
			RegisterBuildings_HighPressureApplications();
			//if (Config.Instance.DupesLogistics_Enabled)
			RegisterBuildings_DupesLogistics();
			//if (Config.Instance.DupesRefrigeration_Enabled)
			RegisterBuildings_DupesRefrigeration();
			//if (Config.Instance.CustomGenerators_Enabled)
			RegisterBuildings_CustomGenerators();
		}
		private static void RegisterBuildings_ChemicalProcessingIndustrialOverhaul()
		{
			BuildingManager.CreateEntry<Chemical_AdvancedKilnConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, KilnConfig.ID)
			.AddToTech(Technology.SolidMaterial.Smelting)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
			.AddModFrom(SourceModInfo.MineralProcessing_Metallurgy, "Metallurgy_AdvancedKiln");

			BuildingManager.CreateEntry<Chemical_AdvancedMetalRefineryConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, MetalRefineryConfig.ID)
			.AddToTech(Technology.SolidMaterial.SuperheatedForging)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
			.AddModFrom(SourceModInfo.MineralProcessing_Metallurgy, "Metallurgy_AdvMetalRefinery");

			BuildingManager.CreateEntry<Chemical_AmmoniaBreakerConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, WaterPurifierConfig.ID)
			.AddToTech(Technology.Liquids.LiquidBasedRefinementProcess)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
			.AddModFrom(SourceModInfo.AddedBySgt_Imalas);

			BuildingManager.CreateEntry<Chemical_AmmoniaCompressorConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, WaterPurifierConfig.ID)
			.AddToTech(Technology.Gases.TemperatureModulation)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO);

			BuildingManager.CreateEntry<Chemical_BallCrusherMillConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, RockCrusherConfig.ID)
			.AddToTech(Technology.SolidMaterial.SuperheatedForging)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO);

			BuildingManager.CreateEntry<Chemical_CarbonDioxideCompressorConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, WaterPurifierConfig.ID)
			.AddToTech(Technology.Gases.TemperatureModulation)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
			.AddModFrom(SourceModInfo.DupesRefrigeration); //was already removed in dupes refrigeration, no migration of existing buildings needed

			BuildingManager.CreateEntry<Chemical_ChlorinePumpConfig>()
			.AddToCategory(PlanMenuCategory.Ventilation, GasMiniPumpConfig.ID)
			.AddToTech(Technology.Gases.Decontamination)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
			.AddModFrom(SourceModInfo.AddedBySgt_Imalas);

			BuildingManager.CreateEntry<Chemical_Co2PumpConfig>()
			.AddToCategory(PlanMenuCategory.Oxygen, CO2ScrubberConfig.ID)
			.AddToTech(Technology.Food.Agriculture)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
			.AddModFrom(SourceModInfo.DupesMachinery, "Co2Pump");

			BuildingManager.CreateEntry<Chemical_Co2RecyclerConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, OxyliteRefineryConfig.ID)
			.AddToTech(Technology.Gases.Catalytics)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
			.MigrateFrom("Chemical_Co2RecyclerDLC1");

			BuildingManager.CreateEntry<Chemical_CrudeOilRefineryStagedConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, OilRefineryConfig.ID)
			.AddToTech(Technology.Power.ValveMiniaturization)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
			.AddModFrom(SourceModInfo.AddedBySgt_Imalas);

			BuildingManager.CreateEntry<Chemical_CrudeOilRefineryConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, OilRefineryConfig.ID)
			.AddToTech(Technology.Power.FossilFuels)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO);

			BuildingManager.CreateEntry<Chemical_EndothermicUnitConfig>()
			.AddToCategory(PlanMenuCategory.Utilities, LiquidConditionerConfig.ID)
			.AddToTech(Technology.Liquids.LiquidTuning)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO);

			BuildingManager.CreateEntry<Chemical_GlassFoundryConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, GlassForgeConfig.ID)
			.AddToTech(Technology.SolidMaterial.SuperheatedForging)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
			.AddModFrom(SourceModInfo.MineralProcessing_Metallurgy, "Metallurgy_GlassFoundry");

			BuildingManager.CreateEntry<Chemical_SmallCrusherMillConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, RockCrusherConfig.ID)
			.AddToTech(Technology.SolidMaterial.BruteForceRefinement)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
			.AddModFrom(SourceModInfo.MineralProcessing_Metallurgy, "Metallurgy_JawCrusherMill");

			BuildingManager.CreateEntry<Chemical_MixingUnitConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, ChemicalRefineryConfig.ID)
			.AddToTech(Technology.Liquids.Emulsification)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
			.AddModFrom(SourceModInfo.AddedBySgt_Imalas);

			BuildingManager.CreateEntry<Chemical_NaphthaReformerConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, OilRefineryConfig.ID)
			.AddToTech(Technology.Power.FossilFuels)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO);

			BuildingManager.CreateEntry<Chemical_PropaneReformerConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, OilRefineryConfig.ID)
			.AddToTech(Technology.Power.FossilFuels)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO);

			BuildingManager.CreateEntry<Chemical_RawGasRefineryStagedConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, OilRefineryConfig.ID)
			.AddToTech(Technology.Power.ValveMiniaturization)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
			.AddModFrom(SourceModInfo.AddedBySgt_Imalas);

			BuildingManager.CreateEntry<Chemical_RawGasRefineryConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, OilRefineryConfig.ID)
			.AddToTech(Technology.Power.FossilFuels)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO);

			BuildingManager.CreateEntry<Chemical_RayonLoomConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, EthanolDistilleryConfig.ID)
			.AddToTech(Technology.Decor.TextileProduction)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO);

			BuildingManager.CreateEntry<Chemical_SelectiveArcFurnaceConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, SupermaterialRefineryConfig.ID)
			.AddToTech(Technology.SolidMaterial.Smelting)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO);

			BuildingManager.CreateEntry<Chemical_SoilMixerConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, CompostConfig.ID)
			.AddToTech(Technology.Food.Agriculture)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
			.AddModFrom(SourceModInfo.MineralProcessing_Metallurgy, "Metallurgy_SoilMixer");

			BuildingManager.CreateEntry<Chemical_SourWaterStripperConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, WaterPurifierConfig.ID)
			.AddToTech(Technology.Liquids.LiquidBasedRefinementProcess)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO);

			BuildingManager.CreateEntry<Chemical_SyngasRefineryConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, OilRefineryConfig.ID)
			.AddToTech(Technology.Liquids.Distillation)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO);

			BuildingManager.CreateEntry<Chemical_SynthesizerNitricConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, ChemicalRefineryConfig.ID)
			.AddToTech(Technology.Liquids.Emulsification)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO);

			BuildingManager.CreateEntry<Chemical_SynthesizerSaltWaterConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, DesalinatorConfig.ID)
			.AddToTech(Technology.Liquids.Distillation)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
			.AddModFrom(SourceModInfo.DupesMachinery, "SynthesizerSaltWater");

			BuildingManager.CreateEntry<Chemical_SynthesizerSulfuricConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, ChemicalRefineryConfig.ID)
			.AddToTech(Technology.Liquids.Emulsification)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO);

			BuildingManager.CreateEntry<Chemical_SourGasSweetenerConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, ChemicalRefineryConfig.ID)
			.AddToTech(Technology.Liquids.Emulsification)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
			.AddModFrom(SourceModInfo.AddedBySgt_Imalas);

			BuildingManager.CreateEntry<Chemical_ThermalDesalinatorConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, DesalinatorConfig.ID)
			.AddToTech(Technology.Liquids.LiquidBasedRefinementProcess)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO);

			BuildingManager.CreateEntry<Custom_PolymerizerConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, PolymerizerConfig.ID)
			.AddToTech(Technology.Power.PlasticManufacturing)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
			.AddModFrom(SourceModInfo.DupesMachinery, "EthanolPolymerizer");

			BuildingManager.CreateEntry<Chemical_Wooden_BoilerConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, OilRefineryConfig.ID)
			.AddToTech(Technology.Power.FossilFuels)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
			.AddModFrom(SourceModInfo.DupesMachinery, "Wooden_Boiler");

			BuildingManager.CreateEntry<Chemical_Coal_BoilerConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, OilRefineryConfig.ID)
			.AddToTech(Technology.Power.FossilFuels)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
			.AddModFrom(SourceModInfo.DupesMachinery, "Coal_Boiler");

			BuildingManager.CreateEntry<Chemical_Liquid_BoilerConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, OilRefineryConfig.ID)
			.AddToTech(Technology.Power.FossilFuels)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
			.AddModFrom(SourceModInfo.DupesMachinery)
			.AddModFrom(SourceModInfo.AddedBySgt_Imalas);

			BuildingManager.CreateEntry<Chemical_Gas_BoilerConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, OilRefineryConfig.ID)
			.AddToTech(Technology.Power.FossilFuels)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
			.AddModFrom(SourceModInfo.DupesMachinery, "Gas_Boiler");

			BuildingManager.CreateEntry<Chemical_ElectricBoilerConfig>()
			.AddToCategory(PlanMenuCategory.Refinement, OilRefineryConfig.ID)
			.AddToTech(Technology.Power.FossilFuels)
			.AddModFrom(SourceModInfo.ChemicalProcessing_IO);
		}
		private static void RegisterBuildings_ChemicalProcessingBioChemistry()
		{
			BuildingManager.CreateEntry<Biochemistry_AlgaeGrowingBasinConfig>()
				.AddToCategory(PlanMenuCategory.Oxygen, AlgaeHabitatConfig.ID)
				.AddToTech(Technology.Food.Agriculture)
				.AddModFrom(SourceModInfo.ChemicalProcessing_BioChemistry);

			BuildingManager.CreateEntry<Biochemistry_AnaerobicDigesterConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, FertilizerMakerConfig.ID)
				.AddToTech(Technology.Food.FoodRepurposing)
				.AddModFrom(SourceModInfo.ChemicalProcessing_BioChemistry);

			BuildingManager.CreateEntry<Biochemistry_BiodieselGeneratorConfig>()
				.AddToCategory(PlanMenuCategory.Power, PetroleumGeneratorConfig.ID)
				.AddToTech(ModTechs.Biochemistry_RenewableFuel_ID)
				.AddModFrom(SourceModInfo.ChemicalProcessing_BioChemistry);


			BuildingManager.CreateEntry<Biochemistry_SynthesizerPhytoOilConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, OilRefineryConfig.ID)
				.AddToTech(ModTechs.Biochemistry_RenewableFuel_ID)
				.AddModFrom(SourceModInfo.ChemicalProcessing_BioChemistry)
				.AddModFrom(SourceModInfo.AddedBySgt_Imalas);

			BuildingManager.CreateEntry<Biochemistry_BiodieselRefineryConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, OilRefineryConfig.ID)
				.AddToTech(ModTechs.Biochemistry_RenewableFuel_ID)
				.AddModFrom(SourceModInfo.ChemicalProcessing_BioChemistry);

			BuildingManager.CreateEntry<Biochemistry_BioplasticPrinterConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, PolymerizerConfig.ID)
				.AddToTech(Technology.Power.PlasticManufacturing)
				.AddModFrom(SourceModInfo.ChemicalProcessing_BioChemistry);


			BuildingManager.CreateEntry<Biochemistry_ExpellerPressConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, FertilizerMakerConfig.ID)
				.AddToTech(Technology.SolidMaterial.BruteForceRefinement)
				.AddModFrom(SourceModInfo.ChemicalProcessing_BioChemistry);
		}
		private static void RegisterBuildings_MineralProcessingMining()
		{
			BuildingManager.CreateEntry<Mining_CNCMachineConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, SupermaterialRefineryConfig.ID)
				.AddToTech(ModTechs.Mining_Mk2DrillTech_ID)
				.AddModFrom(SourceModInfo.MineralProcessing_Mining);

			BuildingManager.CreateEntry<Mining_AugerDrillConfig>()
				.AddToCategory(PlanMenuCategory.Utilities, OilWellCapConfig.ID)
				.AddToTech(ModTechs.Mining_Mk2DrillTech_ID)
				.AddModFrom(SourceModInfo.MineralProcessing_Mining);

			BuildingManager.CreateEntry<Mining_MineralDrillConfig>()
				.AddToCategory(PlanMenuCategory.Utilities, OilWellCapConfig.ID)
				.AddToTech(Technology.SolidMaterial.SolidManagement)
				.AddModFrom(SourceModInfo.MineralProcessing_Mining)
				.AddModFrom(SourceModInfo.AddedBySgt_Imalas);
		}
		private static void RegisterBuildings_MineralProcessingMetallurgy()
		{
			BuildingManager.CreateEntry<Metallurgy_PlasmaFurnaceConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, GlassForgeConfig.ID)
				.AddToTech(ModTechs.Metallurgy_PlasmaBasedRefinementTech_ID)
				.AddModFrom(SourceModInfo.MineralProcessing_Metallurgy)
				.AddModFrom(SourceModInfo.ChemicalProcessing_IO);

			BuildingManager.CreateEntry<Metallurgy_PyrolysisKilnConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, KilnConfig.ID, ModUtil.BuildingOrdering.Before)
				.AddToTech(Technology.SolidMaterial.BruteForceRefinement)
				.AddModFrom(SourceModInfo.MineralProcessing_Metallurgy)
				.AddModFrom(SourceModInfo.ChemicalProcessing_IO, "Chemical_PlasmaFurnace");

			BuildingManager.CreateEntry<Metallurgy_BasicOilRefineryConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, OilRefineryConfig.ID, ModUtil.BuildingOrdering.Before)
				.AddToTech(Technology.Power.FossilFuels)
				.AddModFrom(SourceModInfo.MineralProcessing_Metallurgy);

			BuildingManager.CreateEntry<Metallurgy_BallCrusherMillConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, RockCrusherConfig.ID)
				.AddToTech(Technology.SolidMaterial.SuperheatedForging)
				.AddModFrom(SourceModInfo.MineralProcessing_Metallurgy);
		}
		private static void RegisterBuildings_DupesMachinery()
		{
			BuildingManager.CreateEntry<Machinery_FlocculationSieveConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, WaterPurifierConfig.ID)
				.AddToTech(Technology.Liquids.LiquidBasedRefinementProcess)
				.AddModFrom(SourceModInfo.DupesMachinery)
				.AddModFrom(SourceModInfo.ChemicalProcessing_IO, "Chemical_FlocculationSieve");

			BuildingManager.CreateEntry<Machinery_AlgaeVatConfig>()
				.AddToCategory(PlanMenuCategory.Oxygen, AlgaeHabitatConfig.ID)
				.AddToTech(Technology.Liquids.AirSystems)
				.AddModFrom(SourceModInfo.DupesMachinery);

			BuildingManager.CreateEntry<Machinery_CoralVatConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, ChlorinatorConfig.ID)
				.AddToTech(Technology.Liquids.LiquidBasedRefinementProcess)
				.AddModFrom(SourceModInfo.DupesMachinery);

			BuildingManager.CreateEntry<Machinery_SlimeVatConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, AlgaeDistilleryConfig.ID)
				.AddToTech(Technology.Liquids.LiquidBasedRefinementProcess)
				.AddModFrom(SourceModInfo.DupesMachinery);

			BuildingManager.CreateEntry<Machinery_EthanolStillConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, EthanolDistilleryConfig.ID)
				.AddToTech(Technology.Liquids.Distillation)
				.AddModFrom(SourceModInfo.DupesMachinery);
		}
		private static void RegisterBuildings_NuclearProcessing()
		{
			if (DlcManager.IsPureVanilla())
				return;

			BuildingManager.CreateEntry<HepCalcinatorConfig>()
				.AddToCategory(PlanMenuCategory.Radiation, UraniumCentrifugeConfig.ID)
				.AddToTech(Technology.ColonyDevelopment.RadiationRefinement)
				.AddModFrom(SourceModInfo.NuclearProcessing);

			BuildingManager.CreateEntry<HepCentrifugeConfig>()
				.AddToCategory(PlanMenuCategory.Radiation, UraniumCentrifugeConfig.ID)
				.AddToTech(Technology.ColonyDevelopment.RadiationRefinement)
				.AddModFrom(SourceModInfo.NuclearProcessing);

			BuildingManager.CreateEntry<HepProjectorConfig>()
				.AddToCategory(PlanMenuCategory.Radiation, RadiationLightConfig.ID)
				.AddToTech(Technology.ColonyDevelopment.RadiationRefinement)
				.AddModFrom(SourceModInfo.NuclearProcessing);

			BuildingManager.CreateEntry<LightReactorConfig>()
				.AddToCategory(PlanMenuCategory.Radiation, NuclearReactorConfig.ID, ModUtil.BuildingOrdering.Before)
				.AddToTech(Technology.ColonyDevelopment.RadiationRefinement)
				.AddModFrom(SourceModInfo.NuclearProcessing);

			//BuildingManager.CreateEntry<BigReactorConfig>()
			//	.AddToCategory(PlanMenuCategory.Radiation, NuclearReactorConfig.ID)
			//	.AddToTech(Technology.ColonyDevelopment.RadiationRefinement)
			//	.AddModFrom(SourceModInfo.NuclearProcessing)
			//	.AddModFrom(SourceModInfo.AddedBySgt_Imalas);

		}
		private static void RegisterBuildings_DupesEngineering()
		{
			///Doors

			BuildingManager.CreateEntry<GlassDoorComplexConfig>()
				.AddToCategory(PlanMenuCategory.Base, PressureDoorConfig.ID)
				.AddToTech(Technology.Exosuits.TransitTubes)
				.AddModFrom(SourceModInfo.DupesEngineering);

			BuildingManager.CreateEntry<AIO_WoodenDoorConfig>()
				.AddToCategory(PlanMenuCategory.Base, DoorConfig.ID)
				.AddToTech(Technology.Decor.InteriorDecor)
				.AddModFrom(SourceModInfo.DupesEngineering);

			BuildingManager.CreateEntry<GlassDoorSimpleConfig>()
				.AddToCategory(PlanMenuCategory.Base, DoorConfig.ID)
				.AddToTech(Technology.Exosuits.TransitTubes)
				.AddModFrom(SourceModInfo.DupesEngineering);

			BuildingManager.CreateEntry<FacilityDoorConfig>()
				.AddToCategory(PlanMenuCategory.Base, DoorConfig.ID)
				.AddToTech(Technology.Gases.Ventilation)
				.AddModFrom(SourceModInfo.DupesEngineering)
				.MigrateFrom(["FacilityDoorRed", "FacilityDoorWhite", "FacilityDoorYellow"]);
			/// Trimmings

			BuildingManager.CreateEntry<WoodenCeilingConfig>()
				.AddToCategory(PlanMenuCategory.Furniture, CrownMouldingConfig.ID)
				.AddToTech(Technology.Decor.HomeLuxuries)
				.AddModFrom(SourceModInfo.DupesEngineering);

			BuildingManager.CreateEntry<WoodenCornerArchConfig>()
				.AddToCategory(PlanMenuCategory.Furniture, CornerMouldingConfig.ID)
				.AddToTech(Technology.Decor.HomeLuxuries)
				.AddModFrom(SourceModInfo.DupesEngineering);

			///Warning LED

			BuildingManager.CreateEntry<LogicAlertLightConfig>()
				.AddToCategory(PlanMenuCategory.Automation, LogicAlarmConfig.ID)
				.AddToTech(Technology.Computers.SmartHome)
				.AddModFrom(SourceModInfo.DupesEngineering)
				.MigrateFrom(["AlertLightGreen", "AlertLightRed", "AlertLightYellow"]);

			///Cement Mixer

			BuildingManager.CreateEntry<CementMixerConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, MilkPressConfig.ID)
				.AddToTech(Technology.SolidMaterial.BruteForceRefinement)
				.AddModFrom(SourceModInfo.DupesEngineering);

			///Tiles

			BuildingManager.CreateEntry<MouldingTileConfig>()
				.AddToCategory(PlanMenuCategory.Base, CarpetTileConfig.ID).ForceCategory()
				.AddToTech(Technology.Decor.RenaissanceArt)
				.AddModFrom(SourceModInfo.DupesEngineering)
				.MigrateFrom("MarbleTile");

			BuildingManager.CreateEntry<MosaicTileConfig>()
				.AddToCategory(PlanMenuCategory.Base, CarpetTileConfig.ID).ForceCategory()
				.AddToTech(Technology.Decor.RenaissanceArt)
				.AddModFrom(SourceModInfo.DupesEngineering);

			BuildingManager.CreateEntry<MonoElementTileConfig>()
				.AddToCategory(PlanMenuCategory.Base, TileConfig.ID)
				.AddToTech(Technology.Decor.HighCulture)
				.AddModFrom(SourceModInfo.DupesEngineering);

			BuildingManager.CreateEntry<ReinforcedConcreteTileConfig>()
				.AddToCategory(PlanMenuCategory.Base, BunkerTileConfig.ID)
				.AddToTech(Technology.SolidMaterial.SuperheatedForging)
				.AddModFrom(SourceModInfo.DupesEngineering)
				.MigrateFrom("ConcreteTile");

			BuildingManager.CreateEntry<SpacerTileSolidConfig>()
				.AddToCategory(PlanMenuCategory.Base, TileConfig.ID)
				.AddToTech(Technology.Exosuits.HazardProtection)
				.AddModFrom(SourceModInfo.DupesEngineering)
				.AddModFrom(SourceModInfo.AddedBySgt_Imalas);

			BuildingManager.CreateEntry<SpacerTileWindowConfig>()
				.AddToCategory(PlanMenuCategory.Base, GlassTileConfig.ID)
				.AddToTech(Technology.Decor.GlassBlowing)
				.AddModFrom(SourceModInfo.DupesEngineering)
				.AddModFrom(SourceModInfo.AddedBySgt_Imalas);

			BuildingManager.CreateEntry<StructureTileConfig>()
				.AddToCategory(PlanMenuCategory.Base, MeshTileConfig.ID)
				.AddToTech(Technology.SolidMaterial.Smelting)
				.AddModFrom(SourceModInfo.DupesEngineering);

			BuildingManager.CreateEntry<WoodCompositionTileConfig>()
				.AddToCategory(PlanMenuCategory.Base, WoodTileConfig.ID)
				.AddToTech(Technology.Decor.HomeLuxuries)
				.AddModFrom(SourceModInfo.DupesEngineering)
				.MigrateFrom("WoodenTile");

			BuildingManager.CreateEntry<WoodAirflowTileConfig>()
				.AddToCategory(PlanMenuCategory.Base, GasPermeableMembraneConfig.ID)
				.AddToTech(Technology.Gases.PressureManagement)
				.AddModFrom(SourceModInfo.DupesEngineering);

			BuildingManager.CreateEntry<WoodMeshTileConfig>()
				.AddToCategory(PlanMenuCategory.Base, MeshTileConfig.ID)
				.AddToTech(Technology.Liquids.Sanitation)
				.AddModFrom(SourceModInfo.DupesEngineering);

			BuildingManager.CreateEntry<InsulationCompositionTileConfig>()
				.AddToCategory(PlanMenuCategory.Base, InsulationTileConfig.ID)
				.AddToTech(Technology.Gases.TemperatureModulation)
				.AddModFrom(SourceModInfo.DupesEngineering)
				.MigrateFrom("InsulatingTile");

			///walls

			BuildingManager.CreateEntry<StructureFrameLargeConfig>()
				.AddToCategory(PlanMenuCategory.Base, ExteriorWallConfig.ID)
				.AddToTech(Technology.SolidMaterial.Smelting)
				.AddModFrom(SourceModInfo.DupesEngineering);

			BuildingManager.CreateEntry<StructureFrameSmallConfig>()
				.AddToCategory(PlanMenuCategory.Base, ExteriorWallConfig.ID)
				.AddToTech(Technology.SolidMaterial.Smelting)
				.AddModFrom(SourceModInfo.DupesEngineering);

			BuildingManager.CreateEntry<SpacerWallLargeConfig>()
				.AddToCategory(PlanMenuCategory.Base, ExteriorWallConfig.ID)
				.AddToTech(Technology.Decor.HighCulture)
				.AddModFrom(SourceModInfo.DupesEngineering);

			BuildingManager.CreateEntry<SpacerWallConfig>()
				.AddToCategory(PlanMenuCategory.Base, ExteriorWallConfig.ID)
				.AddToTech(Technology.Decor.HighCulture)
				.AddModFrom(SourceModInfo.DupesEngineering)
				.MigrateFrom(["SpacerWallSmall", "SpacerDanger", "SpacerDangerCorner", "SpacerPanel"]);


			BuildingManager.CreateEntry<SpacerWindowLargeConfig>()
				.AddToCategory(PlanMenuCategory.Base, ExteriorWallConfig.ID)
				.AddToTech(Technology.Decor.GlassBlowing)
				.AddModFrom(SourceModInfo.DupesEngineering);

			BuildingManager.CreateEntry<SpacerWindowSmallConfig>()
				.AddToCategory(PlanMenuCategory.Base, ExteriorWallConfig.ID)
				.AddToTech(Technology.Decor.GlassBlowing)
				.AddModFrom(SourceModInfo.DupesEngineering)
				.MigrateFrom(["SpacerWindow_A", "SpacerWindow_B"]);

			//Metal Ladder
			BuildingManager.CreateEntry<MetalLadderConfig>()
				.AddToCategory(PlanMenuCategory.Base, LadderFastConfig.ID)
				.AddToTech(Technology.SolidMaterial.Smelting)
				.AddModFrom(SourceModInfo.DupesEngineering)
				.MigrateFrom("MetalLadder");
		}
		private static void RegisterBuildings_CustomReservoirs()
		{
			string menuCategoryGas = Config.Instance.ReservoirsInConduitCategory ? PlanMenuCategory.Ventilation : PlanMenuCategory.Base;
			string menuCategoryLiquid = Config.Instance.ReservoirsInConduitCategory ? PlanMenuCategory.Plumbing : PlanMenuCategory.Base;


			BuildingManager.CreateEntry<SmallGasReservoirWallConfig>()
				.AddToCategory(menuCategoryGas, GasReservoirConfig.ID, ModUtil.BuildingOrdering.Before)
				.AddToTech(Technology.Gases.ImprovedVentilation)
				.AddModFrom(SourceModInfo.CustomReservoirs);

			BuildingManager.CreateEntry<SmallGasReservoirDefaultConfig>()
				.AddToCategory(menuCategoryGas, GasReservoirConfig.ID, ModUtil.BuildingOrdering.Before)
				.AddToTech(Technology.Gases.Ventilation)
				.AddModFrom(SourceModInfo.CustomReservoirs);

			BuildingManager.CreateEntry<MedGasReservoirConfig>()
				.AddToCategory(menuCategoryGas, GasReservoirConfig.ID, ModUtil.BuildingOrdering.Before)
				.AddToTech(Technology.Gases.ImprovedVentilation)
				.AddModFrom(SourceModInfo.CustomReservoirs);


			BuildingManager.CreateEntry<SmallLiquidReservoirWallConfig>()
				.AddToCategory(menuCategoryLiquid, LiquidReservoirConfig.ID, ModUtil.BuildingOrdering.Before)
				.AddToTech(Technology.Liquids.ImprovedPlumbing)
				.AddModFrom(SourceModInfo.CustomReservoirs);

			BuildingManager.CreateEntry<SmallLiquidReservoirDefaultConfig>()
				.AddToCategory(menuCategoryLiquid, LiquidReservoirConfig.ID, ModUtil.BuildingOrdering.Before)
				.AddToTech(Technology.Liquids.Plumbing)
				.AddModFrom(SourceModInfo.CustomReservoirs);

			BuildingManager.CreateEntry<MedLiquidReservoirConfig>()
				.AddToCategory(menuCategoryLiquid, LiquidReservoirConfig.ID, ModUtil.BuildingOrdering.Before)
				.AddToTech(Technology.Liquids.ImprovedPlumbing)
				.AddModFrom(SourceModInfo.CustomReservoirs);
		}
		private static void RegisterBuildings_DupesLogistics()
		{
			///storages
			BuildingManager.CreateEntry<CabinetFrozenConfig>()
				.AddToCategory(PlanMenuCategory.Shipping, SolidConduitOutboxConfig.ID)
				.AddToTech(Technology.SolidMaterial.SolidControl)
				.AddModFrom(SourceModInfo.DupesLogistics);

			BuildingManager.CreateEntry<CabinetNormalConfig>()
				.AddToCategory(PlanMenuCategory.Shipping, SolidConduitOutboxConfig.ID)
				.AddToTech(Technology.SolidMaterial.SolidControl)
				.AddModFrom(SourceModInfo.DupesLogistics);

			BuildingManager.CreateEntry<StoragePodConfig>()
				.AddToCategory(PlanMenuCategory.Base, StorageLockerConfig.ID)
				.AddToTech(Technology.SolidMaterial.BruteForceRefinement)
				.AddModFrom(SourceModInfo.DupesLogistics)
				.MigrateFrom(["StoragePod_A", "StoragePod_B", "StoragePod_C"]);

			///logistic rails
			BuildingManager.CreateEntry<LogisticTransferArmConfig>()
				.AddToCategory(PlanMenuCategory.Shipping, SolidTransferArmConfig.ID, ModUtil.BuildingOrdering.Before)
				.AddToTech(Technology.SolidMaterial.SmartStorage)
				.AddModFrom(SourceModInfo.DupesLogistics);

			BuildingManager.CreateEntry<LogisticRailConfig>()
				.AddToCategory(PlanMenuCategory.Shipping, SolidConduitConfig.ID, ModUtil.BuildingOrdering.Before)
				.AddToTech(Technology.SolidMaterial.SmartStorage)
				.AddModFrom(SourceModInfo.DupesLogistics);

			BuildingManager.CreateEntry<LogisticBridgeConfig>()
				.AddToCategory(PlanMenuCategory.Shipping, SolidConduitBridgeConfig.ID, ModUtil.BuildingOrdering.Before)
				.AddToTech(Technology.SolidMaterial.SmartStorage)
				.AddModFrom(SourceModInfo.DupesLogistics);

			BuildingManager.CreateEntry<LogisticLoaderConfig>()
				.AddToCategory(PlanMenuCategory.Shipping, SolidConduitInboxConfig.ID, ModUtil.BuildingOrdering.Before)
				.AddToTech(Technology.SolidMaterial.SmartStorage)
				.AddModFrom(SourceModInfo.DupesLogistics);

			BuildingManager.CreateEntry<LogisticOutBoxConfig>()
				.AddToCategory(PlanMenuCategory.Shipping, SolidConduitOutboxConfig.ID, ModUtil.BuildingOrdering.Before)
				.AddToTech(Technology.SolidMaterial.SmartStorage)
				.AddModFrom(SourceModInfo.DupesLogistics);

			BuildingManager.CreateEntry<LogisticFilterConfig>()
				.AddToCategory(PlanMenuCategory.Shipping, SolidFilterConfig.ID, ModUtil.BuildingOrdering.Before)
				.AddToTech(Technology.SolidMaterial.SolidTransport)
				.AddModFrom(SourceModInfo.DupesLogistics);

			BuildingManager.CreateEntry<LogisticVentConfig>()
				.AddToCategory(PlanMenuCategory.Shipping, SolidVentConfig.ID, ModUtil.BuildingOrdering.Before)
				.AddToTech(Technology.SolidMaterial.SmartStorage)
				.AddModFrom(SourceModInfo.DupesLogistics);


			BuildingManager.CreateEntry<LogisticRailValveConfig>()
				.AddToCategory(PlanMenuCategory.Shipping, SolidLogicValveConfig.ID)
				.AddToTech(Technology.SolidMaterial.SolidTransport)
				.AddModFrom(SourceModInfo.DupesLogistics)
				.AddModFrom(SourceModInfo.AddedBySgt_Imalas);
		}
		private static void RegisterBuildings_HighPressureApplications()
		{
			//gas
			BuildingManager.CreateEntry<HPAVentGasConfig>()
				.AddToCategory(PlanMenuCategory.Ventilation, GasVentHighPressureConfig.ID)
				.AddToTech(Technology.Gases.HVAC)
				.AddModFrom(SourceModInfo.HighPressureApplications);

			BuildingManager.CreateEntry<PressureGasPumpConfig>()
				.AddToCategory(PlanMenuCategory.Ventilation, GasPumpConfig.ID)
				.AddToTech(Technology.Power.ValveMiniaturization)
				.AddModFrom(SourceModInfo.HighPressureApplications);

			BuildingManager.CreateEntry<DecompressionGasValveConfig>()
				.AddToCategory(PlanMenuCategory.Ventilation, GasConduitBridgeConfig.ID)
				.AddToTech(Technology.Gases.HVAC)
				.AddModFrom(SourceModInfo.HighPressureApplications);

			BuildingManager.CreateEntry<HighPressureGasConduitConfig>()
				.AddToCategory(PlanMenuCategory.Ventilation, GasConduitRadiantConfig.ID)
				.AddToTech(Technology.Gases.ImprovedVentilation)
				.AddModFrom(SourceModInfo.HighPressureApplications);

			BuildingManager.CreateEntry<HighPressureGasConduitBridgeConfig>()
				.AddToCategory(PlanMenuCategory.Ventilation, GasConduitBridgeConfig.ID)
				.AddToTech(Technology.Gases.ImprovedVentilation)
				.AddModFrom(SourceModInfo.HighPressureApplications);

			BuildingManager.CreateEntry<HPAFilterGasConfig>()
				.AddToCategory(PlanMenuCategory.Ventilation, GasFilterConfig.ID)
				.AddToTech(Technology.Power.ValveMiniaturization)
				.AddModFrom(SourceModInfo.HighPressureApplications)
				.AddModFrom(SourceModInfo.AddedBySgt_Imalas);

			//liquid
			BuildingManager.CreateEntry<HPAVentLiquidConfig>()
				.AddToCategory(PlanMenuCategory.Plumbing, LiquidVentConfig.ID)
				.AddToTech(Technology.Liquids.ImprovedPlumbing)
				.AddModFrom(SourceModInfo.HighPressureApplications);

			BuildingManager.CreateEntry<PressureLiquidPumpConfig>()
				.AddToCategory(PlanMenuCategory.Plumbing, LiquidPumpConfig.ID)
				.AddToTech(Technology.Power.ValveMiniaturization)
				.AddModFrom(SourceModInfo.HighPressureApplications);

			BuildingManager.CreateEntry<DecompressionLiquidValveConfig>()
				.AddToCategory(PlanMenuCategory.Plumbing, LiquidConduitBridgeConfig.ID)
				.AddToTech(Technology.Liquids.LiquidTuning)
				.AddModFrom(SourceModInfo.HighPressureApplications);

			BuildingManager.CreateEntry<HighPressureLiquidConduitConfig>()
				.AddToCategory(PlanMenuCategory.Plumbing, LiquidConduitRadiantConfig.ID)
				.AddToTech(Technology.Liquids.ImprovedPlumbing)
				.AddModFrom(SourceModInfo.HighPressureApplications);

			BuildingManager.CreateEntry<HighPressureLiquidConduitBridgeConfig>()
				.AddToCategory(PlanMenuCategory.Plumbing, LiquidConduitBridgeConfig.ID)
				.AddToTech(Technology.Liquids.ImprovedPlumbing)
				.AddModFrom(SourceModInfo.HighPressureApplications);

			BuildingManager.CreateEntry<HPAFilterLiquidConfig>()
				.AddToCategory(PlanMenuCategory.Plumbing, LiquidFilterConfig.ID)
				.AddToTech(Technology.Power.ValveMiniaturization)
				.AddModFrom(SourceModInfo.HighPressureApplications)
				.AddModFrom(SourceModInfo.AddedBySgt_Imalas);

			//solid

			BuildingManager.CreateEntry<HPARailInsulatedConfig>()
				.AddToCategory(PlanMenuCategory.Shipping, SolidConduitConfig.ID)
				.AddToTech(ModTechs.HPA_Rails_Research_ID)
				.AddModFrom(SourceModInfo.HighPressureApplications)
				.AddModFrom(SourceModInfo.AddedBySgt_Imalas)
				.ConfigEnabled(Config.Instance.HPA_Rails_Insulation_Mod_Enabled && Config.Instance.HPA_Rails_Mod_Enabled);


			BuildingManager.CreateEntry<HPARailConfig>()
				.AddToCategory(PlanMenuCategory.Shipping, SolidConduitConfig.ID)
				.AddToTech(ModTechs.HPA_Rails_Research_ID)
				.AddModFrom(SourceModInfo.HighPressureApplications)
				.AddModFrom(SourceModInfo.AddedBySgt_Imalas)
				.ConfigEnabled(Config.Instance.HPA_Rails_Mod_Enabled);


			BuildingManager.CreateEntry<HPARailBridgeTileConfig>()
				.AddToCategory(PlanMenuCategory.Shipping, SolidConduitBridgeConfig.ID)
				.AddToTech(ModTechs.HPA_Rails_Research_ID)
				.AddModFrom(SourceModInfo.HighPressureApplications)
				.AddModFrom(SourceModInfo.AddedBySgt_Imalas)
				.ConfigEnabled(Config.Instance.HPA_Rails_Mod_Enabled);

			BuildingManager.CreateEntry<HPARailBridgeConfig>()
				.AddToCategory(PlanMenuCategory.Shipping, SolidConduitBridgeConfig.ID)
				.AddToTech(ModTechs.HPA_Rails_Research_ID)
				.AddModFrom(SourceModInfo.HighPressureApplications)
				.AddModFrom(SourceModInfo.AddedBySgt_Imalas)
				.ConfigEnabled(Config.Instance.HPA_Rails_Mod_Enabled);


			BuildingManager.CreateEntry<HPARailMergerConfig>()
				.AddToCategory(PlanMenuCategory.Shipping, SolidLogicValveConfig.ID)
				.AddToTech(ModTechs.HPA_Rails_Research_ID)
				.AddModFrom(SourceModInfo.HighPressureApplications)
				.AddModFrom(SourceModInfo.AddedBySgt_Imalas)
				.ConfigEnabled(Config.Instance.HPA_Rails_Mod_Enabled);

			BuildingManager.CreateEntry<HPARailValveConfig>()
				.AddToCategory(PlanMenuCategory.Shipping, SolidLogicValveConfig.ID)
				.AddToTech(ModTechs.HPA_Rails_Research_ID)
				.AddModFrom(SourceModInfo.HighPressureApplications)
				.AddModFrom(SourceModInfo.AddedBySgt_Imalas)
				.ConfigEnabled(Config.Instance.HPA_Rails_Mod_Enabled);

			BuildingManager.CreateEntry<HPAInBoxConfig>()
				.AddToCategory(PlanMenuCategory.Shipping, SolidConduitInboxConfig.ID)
				.AddToTech(ModTechs.HPA_Rails_Research_ID)
				.AddModFrom(SourceModInfo.HighPressureApplications)
				.AddModFrom(SourceModInfo.AddedBySgt_Imalas)
				.ConfigEnabled(Config.Instance.HPA_Rails_Mod_Enabled);

			BuildingManager.CreateEntry<HPATransferArmConfig>()
				.AddToCategory(PlanMenuCategory.Shipping, SolidTransferArmConfig.ID)
				.AddToTech(ModTechs.HPA_Rails_Research_ID)
				.AddModFrom(SourceModInfo.HighPressureApplications)
				.AddModFrom(SourceModInfo.AddedBySgt_Imalas)
				.ConfigEnabled(Config.Instance.HPA_Rails_Mod_Enabled);

			BuildingManager.CreateEntry<HPAFilterSolidConfig>()
				.AddToCategory(PlanMenuCategory.Shipping, SolidFilterConfig.ID)
				.AddToTech(Technology.SolidMaterial.SolidManagement)
				.AddModFrom(SourceModInfo.HighPressureApplications)
				.AddModFrom(SourceModInfo.AddedBySgt_Imalas)
				.ConfigEnabled(Config.Instance.HPA_Rails_Mod_Enabled);
		}
		private static void RegisterBuildings_DupesRefrigeration()
		{
			BuildingManager.CreateEntry<HightechBigFridgeConfig>()
				.AddToCategory(PlanMenuCategory.Food, RefrigeratorConfig.ID)
				.AddToTech(Technology.Food.GourmetMealPreparation)
				.AddModFrom(SourceModInfo.DupesRefrigeration);

			BuildingManager.CreateEntry<HightechSmallFridgeConfig>()
				.AddToCategory(PlanMenuCategory.Food, RefrigeratorConfig.ID)
				.AddToTech(Technology.Food.GourmetMealPreparation)
				.AddModFrom(SourceModInfo.DupesRefrigeration);

			BuildingManager.CreateEntry<FridgeLargeConfig>()
				.AddToCategory(PlanMenuCategory.Food, RefrigeratorConfig.ID)
				.AddToTech(Technology.Food.FoodRepurposing)
				.AddModFrom(SourceModInfo.DupesRefrigeration)
				.MigrateFrom(["FridgeAdvanced", "FridgeBlue", "FridgeRed", "FridgeYellow"]);

			BuildingManager.CreateEntry<FridgeSmallConfig>()
				.AddToCategory(PlanMenuCategory.Food, RefrigeratorConfig.ID)
				.AddToTech(Technology.Food.FoodRepurposing)
				.AddModFrom(SourceModInfo.DupesRefrigeration);

			BuildingManager.CreateEntry<FridgePodConfig>()
				.AddToCategory(PlanMenuCategory.Food, RefrigeratorConfig.ID)
				.AddToTech(Technology.Food.FoodRepurposing)
				.AddModFrom(SourceModInfo.DupesRefrigeration);

			BuildingManager.CreateEntry<SpaceBoxConfig>()
				.AddToCategory(PlanMenuCategory.Food, RationBoxConfig.ID)
				.AddToTech(Technology.Food.Agriculture)
				.AddModFrom(SourceModInfo.DupesRefrigeration);
		}
		private static void RegisterBuildings_CustomGenerators()
		{
			BuildingManager.CreateEntry<CustomDieselGeneratorConfig>()
				.AddToCategory(PlanMenuCategory.Power, PetroleumGeneratorConfig.ID, ModUtil.BuildingOrdering.Before)
				.AddToTech(Technology.Power.FossilFuels)
				.AddModFrom(SourceModInfo.CustomGenerators);

			BuildingManager.CreateEntry<CustomGasGeneratorConfig>()
				.AddToCategory(PlanMenuCategory.Power, MethaneGeneratorConfig.ID, ModUtil.BuildingOrdering.Before)
				.AddToTech(Technology.Power.FossilFuels)
				.AddModFrom(SourceModInfo.CustomGenerators);

			BuildingManager.CreateEntry<CustomSolarPanelConfig>()
				.AddToCategory(PlanMenuCategory.Power, SolarPanelConfig.ID, ModUtil.BuildingOrdering.Before)
				.AddToTech(Technology.Power.RenewableEnergy)
				.AddModFrom(SourceModInfo.CustomGenerators)
				.AddModFrom(SourceModInfo.AddedBySgt_Imalas);

			BuildingManager.CreateEntry<CustomSteamGeneratorConfig>()
				.AddToCategory(PlanMenuCategory.Power, SteamTurbineConfig2.ID, ModUtil.BuildingOrdering.Before)
				.AddToTech(Technology.Power.RenewableEnergy)
				.AddModFrom(SourceModInfo.CustomGenerators);

			BuildingManager.CreateEntry<CustomSolidGeneratorConfig>()
				.AddToCategory(PlanMenuCategory.Power, GeneratorConfig.ID)
				.AddToTech(Technology.Power.InternalCombustion)
				.AddModFrom(SourceModInfo.CustomGenerators);
		}

		internal static void RegisterAdditionalMigrations()
		{
			var savemng = SaveLoader.Instance.saveManager;
			if (savemng == null)
				return;
			MonoElementTileIgneousRockConfig.RegisterLegacyMigration();

			TryMigrate("WoodenLadder", LadderConfig.ID);
			TryMigrate("BrickWall", ExteriorWallConfig.ID);
			TryMigrate("WoodenDryWall", ExteriorWallConfig.ID);
			TryMigrate("WoodenDryWall_B", ExteriorWallConfig.ID);

			TryMigrate("Custom_MetalRefinery", MetalRefineryConfig.ID);
			TryMigrate("Custom_OilWellCap", OilWellCapConfig.ID);
		}
		static void TryMigrate(string oldId, string newId)
		{
			var savemng = SaveLoader.Instance.saveManager;
			if (savemng == null)
				return;
			var prefab = Assets.TryGetPrefab(newId);
			if (prefab != null && !savemng.prefabMap.ContainsKey(oldId))
				savemng.prefabMap.Add(oldId, prefab);
		}

		internal static void MoveVanillaReservoirs()
		{
			if (!Config.Instance.ReservoirsInConduitCategory)
			{ return; }

			InjectionMethods.MoveExistingBuildingToNewCategory(PlanMenuCategory.Ventilation, GasReservoirConfig.ID, subcategoryID: PlanMenuSubcategory.Storage);
			InjectionMethods.MoveExistingBuildingToNewCategory(PlanMenuCategory.Plumbing, LiquidReservoirConfig.ID, subcategoryID: PlanMenuSubcategory.Storage);
		}
	}
}
