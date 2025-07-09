using UtilLibs;
using Dupes_Industrial_Overhaul.Chemical_Processing.Buildings;
using Biochemistry.Buildings;
using Mineral_Processing_Mining.Buildings;
using Metallurgy.Buildings;
using HarmonyLib;
using Dupes_Machinery.Biological_Vats;
using Dupes_Machinery.Ethanol_Still;
using System;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.MineralProcessing_Metallurgy;
using static UtilLibs.GameStrings;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.NuclearProcessing;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesEngineering;
using Mineral_Processing;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesEngineering.Tiles;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesEngineering.Walls;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.CustomReservoirs;
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
			GameTags.MaterialBuildingElements.Add(SimHashes.Tungsten.CreateTag());
		}
		internal static void RegisterExtraStrings()
		{
			//registering element names as tags
			Strings.Add("STRINGS.MISC.TAGS.TUNGSTEN", global::STRINGS.ELEMENTS.TUNGSTEN.NAME);
			Strings.Add("STRINGS.MISC.TAGS.SANDSTONE", global::STRINGS.ELEMENTS.SANDSTONE.NAME);
			Strings.Add("STRINGS.MISC.TAGS.GRANITE", global::STRINGS.ELEMENTS.GRANITE.NAME);
			Strings.Add("STRINGS.MISC.TAGS.CEMENT", global::STRINGS.ELEMENTS.CEMENT.NAME);
			Strings.Add("STRINGS.MISC.TAGS.IGNEOUSROCK", global::STRINGS.ELEMENTS.IGNEOUSROCK.NAME);
			Strings.Add("STRINGS.MISC.TAGS.CONCRETEBLOCK", STRINGS.ELEMENTS.CONCRETEBLOCK.NAME);
			Strings.Add("STRINGS.MISC.TAGS.BRICK", global::STRINGS.ELEMENTS.BRICK.NAME);

			global::STRINGS.BUILDINGS.PREFABS.TILEPOI.NAME = STRINGS.BUILDINGS.PREFABS.MOSAICTILESTRINGS.NAME;
			global::STRINGS.BUILDINGS.PREFABS.TILEPOI.DESC = STRINGS.BUILDINGS.PREFABS.MOSAICTILESTRINGS.DESC;
			global::STRINGS.BUILDINGS.PREFABS.TILEPOI.EFFECT = STRINGS.BUILDINGS.PREFABS.MOSAICTILESTRINGS.EFFECT;

			global::STRINGS.BUILDINGS.PREFABS.MOULDINGTILE.NAME = STRINGS.BUILDINGS.PREFABS.MARBLETILESTRINGS.NAME;
			global::STRINGS.BUILDINGS.PREFABS.MOULDINGTILE.DESC = STRINGS.BUILDINGS.PREFABS.MARBLETILESTRINGS.DESC;
			global::STRINGS.BUILDINGS.PREFABS.MOULDINGTILE.EFFECT = STRINGS.BUILDINGS.PREFABS.MARBLETILESTRINGS.EFFECT;
		}

		public static void RegisterOilWellCapCustomPiping()
		{
			if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
			{
				var oilWell = Assets.GetBuildingDef(OilWellCapConfig.ID);
				Custom_OilWellCapConfig.AttachPorts(oilWell.BuildingPreview);
				Custom_OilWellCapConfig.AttachPorts(oilWell.BuildingUnderConstruction);
				Custom_OilWellCapConfig.AttachPorts(oilWell.BuildingComplete);
			}
		}

		public static void RegisterBuildings()
		{
			BuildingManager.LoadConfigFile();
			if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
				RegisterBuildings_ChemicalProcessingIndustrialOverhaul();
			if (Config.Instance.MineralProcessing_Metallurgy_Enabled)
				RegisterBuildings_MineralProcessingMetallurgy();
			if (Config.Instance.ChemicalProcessing_BioChemistry_Enabled)
				RegisterBuildings_ChemicalProcessingBioChemistry();
			if (Config.Instance.MineralProcessing_Mining_Enabled)
				RegisterBuildings_MineralProcessingMining();
			if (Config.Instance.DupesMachinery_Enabled)
				RegisterBuildings_DupesMachinery();
			if(Config.Instance.NuclearProcessing_Enabled)
				RegisterBuildings_NuclearProcessing();
			if(Config.Instance.DupesEngineering_Enabled)
				RegisterBuildings_DupesEngineering();
			if (Config.Instance.CustomReservoirs)
				RegisterBuildings_CustomReservoirs();
		}
		private static void RegisterBuildings_ChemicalProcessingIndustrialOverhaul()
		{
			BuildingManager.CreateEntry<Chemical_AdvancedKilnConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, KilnConfig.ID)
				.AddToTech(Technology.SolidMaterial.Smelting)
				.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
				.AddModFrom(SourceModInfo.MineralProcessing_Metallurgy);
			BuildingManager.CreateEntry<Chemical_AdvancedMetalRefineryConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, MetalRefineryConfig.ID)
				.AddToTech(Technology.SolidMaterial.SuperheatedForging)
				.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
				.AddModFrom(SourceModInfo.MineralProcessing_Metallurgy);
			BuildingManager.CreateEntry<Chemical_AmmoniaBreakerConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, WaterPurifierConfig.ID)
				.AddToTech(Technology.Liquids.LiquidBasedRefinementProcess)
				.AddModFrom(SourceModInfo.ChemicalProcessing_IO);
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
				.AddModFrom(SourceModInfo.ChemicalProcessing_IO);
			BuildingManager.CreateEntry<Chemical_Co2PumpConfig>()
				.AddToCategory(PlanMenuCategory.Oxygen, CO2ScrubberConfig.ID)
				.AddToTech(Technology.Food.Agriculture)
				.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
				.AddModFrom(SourceModInfo.DupesMachinery);
			BuildingManager.CreateEntry<Chemical_Co2RecyclerConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, OxyliteRefineryConfig.ID)
				.AddToTech(Technology.Gases.PortableGasses)
				.AddModFrom(SourceModInfo.ChemicalProcessing_IO);
			BuildingManager.CreateEntry<Chemical_Coal_BoilerConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, OilRefineryConfig.ID)
				.AddToTech(Technology.Power.FossilFuels)
				.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
				.AddModFrom(SourceModInfo.DupesMachinery);
			BuildingManager.CreateEntry<Chemical_CrudeOilRefineryStagedConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, OilRefineryConfig.ID)
				.AddToTech(Technology.Power.FossilFuels)
				.AddModFrom(SourceModInfo.ChemicalProcessing_IO);
			BuildingManager.CreateEntry<Chemical_CrudeOilRefineryConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, OilRefineryConfig.ID)
				.AddToTech(Technology.Power.FossilFuels)
				.AddModFrom(SourceModInfo.ChemicalProcessing_IO);
			BuildingManager.CreateEntry<Chemical_ElectricBoilerConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, OilRefineryConfig.ID)
				.AddToTech(Technology.Power.FossilFuels)
				.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
				.AddModFrom(SourceModInfo.DupesMachinery);
			BuildingManager.CreateEntry<Chemical_EndothermicUnitConfig>()
				.AddToCategory(PlanMenuCategory.Utilities, LiquidConditionerConfig.ID)
				.AddToTech(Technology.Liquids.LiquidTuning)
				.AddModFrom(SourceModInfo.ChemicalProcessing_IO);
			BuildingManager.CreateEntry<Chemical_Gas_BoilerConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, OilRefineryConfig.ID)
				.AddToTech(Technology.Power.FossilFuels)
				.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
				.AddModFrom(SourceModInfo.DupesMachinery);
			BuildingManager.CreateEntry<Chemical_GlassFoundryConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, GlassForgeConfig.ID)
				.AddToTech(Technology.SolidMaterial.SuperheatedForging)
				.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
				.AddModFrom(SourceModInfo.MineralProcessing_Metallurgy);
			BuildingManager.CreateEntry<Chemical_SmallCrusherMillConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, RockCrusherConfig.ID)
				.AddToTech(Technology.SolidMaterial.BruteForceRefinement)
				.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
				.AddModFrom(SourceModInfo.MineralProcessing_Metallurgy);
			BuildingManager.CreateEntry<Chemical_MixingUnitConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, ChemicalRefineryConfig.ID)
				.AddToTech(Technology.Liquids.Distillation)
				.AddModFrom(SourceModInfo.ChemicalProcessing_IO);
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
				.AddToTech(Technology.Power.FossilFuels)
				.AddModFrom(SourceModInfo.ChemicalProcessing_IO); BuildingManager.CreateEntry<Chemical_RawGasRefineryConfig>()
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
				.AddModFrom(SourceModInfo.MineralProcessing_Metallurgy);
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
				.AddModFrom(SourceModInfo.DupesMachinery);
			BuildingManager.CreateEntry<Chemical_SynthesizerSulfuricConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, ChemicalRefineryConfig.ID)
				.AddToTech(Technology.Liquids.Emulsification)
				.AddModFrom(SourceModInfo.ChemicalProcessing_IO);
			BuildingManager.CreateEntry<Chemical_ThermalDesalinatorConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, DesalinatorConfig.ID)
				.AddToTech(Technology.Liquids.LiquidBasedRefinementProcess)
				.AddModFrom(SourceModInfo.ChemicalProcessing_IO);
			BuildingManager.CreateEntry<Chemical_Wooden_BoilerConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, OilRefineryConfig.ID)
				.AddToTech(Technology.Power.FossilFuels)
				.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
				.AddModFrom(SourceModInfo.DupesMachinery);
			BuildingManager.CreateEntry<Custom_PolymerizerConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, PolymerizerConfig.ID)
				.AddToTech(Technology.Power.PlasticManufacturing)
				.AddModFrom(SourceModInfo.ChemicalProcessing_IO)
				.AddModFrom(SourceModInfo.DupesMachinery);
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
				.AddToTech(Technology.Power.FossilFuels)
				.AddModFrom(SourceModInfo.ChemicalProcessing_BioChemistry);
			BuildingManager.CreateEntry<Biochemistry_BiodieselRefineryConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, OilRefineryConfig.ID)
				.AddToTech(Technology.Power.FossilFuels)
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
				.AddToTech(Technology.SolidMaterial.SolidManagement)
				.AddModFrom(SourceModInfo.MineralProcessing_Mining);
			BuildingManager.CreateEntry<Mining_AugerDrillConfig>()
				.AddToCategory(PlanMenuCategory.Utilities, OilWellCapConfig.ID)
				.AddToTech(Technology.SolidMaterial.SolidManagement)
				.AddModFrom(SourceModInfo.MineralProcessing_Mining);
			BuildingManager.CreateEntry<Mining_MineralDrillConfig>()
				.AddToCategory(PlanMenuCategory.Utilities, OilWellCapConfig.ID)
				.AddToTech(Technology.SolidMaterial.SolidManagement)
				.AddModFrom(SourceModInfo.MineralProcessing_Mining);
		}
		private static void RegisterBuildings_MineralProcessingMetallurgy()
		{
			BuildingManager.CreateEntry<Metallurgy_PlasmaFurnaceConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, GlassForgeConfig.ID)
				.AddToTech(Technology.Gases.Catalytics)
				.AddModFrom(SourceModInfo.MineralProcessing_Metallurgy)
				.AddModFrom(SourceModInfo.ChemicalProcessing_IO);
			BuildingManager.CreateEntry<Metallurgy_PyrolysisKilnConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, KilnConfig.ID)
				.AddToTech(Technology.SolidMaterial.BruteForceRefinement)
				.AddModFrom(SourceModInfo.MineralProcessing_Metallurgy)
				.AddModFrom(SourceModInfo.ChemicalProcessing_IO);
			BuildingManager.CreateEntry<Metallurgy_BasicOilRefineryConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, OilRefineryConfig.ID)
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
				.AddModFrom(SourceModInfo.DupesMachinery);
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
				.AddToCategory(PlanMenuCategory.Radiation, NuclearReactorConfig.ID)
				.AddToTech(Technology.ColonyDevelopment.RadiationRefinement)
				.AddModFrom(SourceModInfo.NuclearProcessing);
		}
		private static void RegisterBuildings_DupesEngineering()
		{
			///Doors

			BuildingManager.CreateEntry<GlassDoorComplexConfig>()
				.AddToCategory(PlanMenuCategory.Base, PressureDoorConfig.ID)
				.AddToTech(Technology.Exosuits.TransitTubes)
				.AddModFrom(SourceModInfo.DupesEngineering);

			BuildingManager.CreateEntry<WoodenDoorConfig>()
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
				.AddModFrom(SourceModInfo.DupesEngineering);
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
				.AddModFrom(SourceModInfo.DupesEngineering);

			///Cement Mixer

			BuildingManager.CreateEntry<CementMixerConfig>()
				.AddToCategory(PlanMenuCategory.Refinement, MilkPressConfig.ID)
				.AddToTech(Technology.SolidMaterial.BruteForceRefinement)
				.AddModFrom(SourceModInfo.DupesEngineering);

			///Tiles

			BuildingManager.CreateEntry<MouldingTileConfig>()
				.AddToCategory(PlanMenuCategory.Base, CarpetTileConfig.ID).ForceCategory()
				.AddToTech(Technology.Decor.RenaissanceArt)
				.AddModFrom(SourceModInfo.DupesEngineering);

			BuildingManager.CreateEntry<TilePOIConfig>()
				.AddToCategory(PlanMenuCategory.Base, CarpetTileConfig.ID).ForceCategory()
				.AddToTech(Technology.Decor.HomeLuxuries)
				.AddModFrom(SourceModInfo.DupesEngineering);

			BuildingManager.CreateEntry<MonoElementTileConfig>()
				.AddToCategory(PlanMenuCategory.Base, TileConfig.ID)
				.AddToTech(Technology.Decor.HighCulture)
				.AddModFrom(SourceModInfo.DupesEngineering);

			BuildingManager.CreateEntry<ReinforcedConcreteTileConfig>()
				.AddToCategory(PlanMenuCategory.Base, BunkerTileConfig.ID)
				.AddToTech(Technology.SolidMaterial.SuperheatedForging)
				.AddModFrom(SourceModInfo.DupesEngineering);

			BuildingManager.CreateEntry<SpacerTileSolidConfig>()
				.AddToCategory(PlanMenuCategory.Base, TileConfig.ID)
				.AddToTech(Technology.Exosuits.HazardProtection)
				.AddModFrom(SourceModInfo.DupesEngineering);

			BuildingManager.CreateEntry<SpacerTileWindowConfig>()
				.AddToCategory(PlanMenuCategory.Base, GlassTileConfig.ID)
				.AddToTech(Technology.Decor.GlassBlowing)
				.AddModFrom(SourceModInfo.DupesEngineering);

			BuildingManager.CreateEntry<StructureTileConfig>()
				.AddToCategory(PlanMenuCategory.Base, MeshTileConfig.ID)
				.AddToTech(Technology.SolidMaterial.Smelting)
				.AddModFrom(SourceModInfo.DupesEngineering);

			BuildingManager.CreateEntry<WoodCompositionTileConfig>()
				.AddToCategory(PlanMenuCategory.Base, WoodTileConfig.ID)
				.AddToTech(Technology.Decor.HomeLuxuries)
				.AddModFrom(SourceModInfo.DupesEngineering);

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
				.AddModFrom(SourceModInfo.DupesEngineering);

			///walls

			BuildingManager.CreateEntry<SpacerWallLargeConfig>()
				.AddToCategory(PlanMenuCategory.Base, ExteriorWallConfig.ID)
				.AddToTech(Technology.Decor.HighCulture)
				.AddModFrom(SourceModInfo.DupesEngineering);

			BuildingManager.CreateEntry<SpacerWallConfig>()
				.AddToCategory(PlanMenuCategory.Base, ExteriorWallConfig.ID)
				.AddToTech(Technology.Decor.HighCulture)
				.AddModFrom(SourceModInfo.DupesEngineering);

			BuildingManager.CreateEntry<SpacerWindowLargeConfig>()
				.AddToCategory(PlanMenuCategory.Base, ExteriorWallConfig.ID)
				.AddToTech(Technology.Decor.GlassBlowing)
				.AddModFrom(SourceModInfo.DupesEngineering);

			BuildingManager.CreateEntry<SpacerWindowSmallConfig>()
				.AddToCategory(PlanMenuCategory.Base, ExteriorWallConfig.ID)
				.AddToTech(Technology.Decor.GlassBlowing)
				.AddModFrom(SourceModInfo.DupesEngineering);

		}
		private static void RegisterBuildings_CustomReservoirs()
		{
			BuildingManager.CreateEntry<SmallGasReservoirDefaultConfig>()
				.AddToCategory(PlanMenuCategory.Base, GasReservoirConfig.ID)
				.AddToTech(Technology.Gases.Ventilation)
				.AddModFrom(SourceModInfo.CustomReservoirs);
		}


		#region oldBuildingRegistration
		public static void AddBuildingsToPlanscreen()
		{
			if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
			{
				AddBuildingsToPlanscreen_ChemicalProcessingIndustrialOverhaul();
				RegisterOilWellCapCustomPiping();
			}
			if (Config.Instance.MineralProcessing_Metallurgy_Enabled)
				AddBuildingsToPlanscreen_MineralProcessingMetallurgy();
			if (Config.Instance.ChemicalProcessing_BioChemistry_Enabled)
				AddBuildingsToPlanscreen_ChemicalProcessingBioChemistry();
			if (Config.Instance.MineralProcessing_Mining_Enabled)
				AddBuildingsToPlanscreen_MineralProcessingMining();
			if (Config.Instance.DupesMachinery_Enabled)
				AddBuildingsToPlanscreen_DupesMachinery();
		}
		private static void AddBuildingsToPlanscreen_ChemicalProcessingIndustrialOverhaul()
		{
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Chemical_AdvancedKilnConfig.ID, KilnConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Chemical_AdvancedMetalRefineryConfig.ID, MetalRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Chemical_BallCrusherMillConfig.ID, RockCrusherConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Oxygen, Chemical_Co2PumpConfig.ID, CO2ScrubberConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Chemical_Co2RecyclerConfig.ID, OxyliteRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Chemical_Coal_BoilerConfig.ID, OilRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Chemical_CrudeOilRefineryConfig.ID, OilRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Chemical_ElectricBoilerConfig.ID, Chemical_Coal_BoilerConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Utilities, Chemical_EndothermicUnitConfig.ID, LiquidConditionerConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Chemical_Gas_BoilerConfig.ID, Chemical_ElectricBoilerConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Chemical_GlassFoundryConfig.ID, GlassForgeConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Chemical_SmallCrusherMillConfig.ID, RockCrusherConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Chemical_NaphthaReformerConfig.ID, OilRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Chemical_PropaneReformerConfig.ID, Chemical_NaphthaReformerConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Chemical_RawGasRefineryConfig.ID, OilRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Chemical_RayonLoomConfig.ID, EthanolDistilleryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Chemical_SelectiveArcFurnaceConfig.ID, SupermaterialRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Chemical_SoilMixerConfig.ID, CompostConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Chemical_SourWaterStripperConfig.ID, WaterPurifierConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Chemical_SyngasRefineryConfig.ID, OilRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Chemical_SynthesizerNitricConfig.ID, ChemicalRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Chemical_SynthesizerSaltWaterConfig.ID, DesalinatorConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Chemical_SynthesizerSulfuricConfig.ID, ChemicalRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Chemical_ThermalDesalinatorConfig.ID, DesalinatorConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Chemical_Wooden_BoilerConfig.ID, Chemical_Coal_BoilerConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Custom_PolymerizerConfig.ID, PolymerizerConfig.ID);
		}
		private static void AddBuildingsToPlanscreen_ChemicalProcessingBioChemistry()
		{
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Oxygen, Biochemistry_AlgaeGrowingBasinConfig.ID, AlgaeHabitatConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Biochemistry_AnaerobicDigesterConfig.ID, FertilizerMakerConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Power, Biochemistry_BiodieselGeneratorConfig.ID, PetroleumGeneratorConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Biochemistry_BiodieselRefineryConfig.ID, OilRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Biochemistry_BioplasticPrinterConfig.ID, PolymerizerConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Biochemistry_ExpellerPressConfig.ID, Biochemistry_AnaerobicDigesterConfig.ID);
		}
		private static void AddBuildingsToPlanscreen_MineralProcessingMining()
		{
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Mining_CNCMachineConfig.ID, SupermaterialRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Utilities, Mining_AugerDrillConfig.ID, OilWellCapConfig.ID);
		}

		private static void AddBuildingsToPlanscreen_MineralProcessingMetallurgy()
		{
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Metallurgy_PlasmaFurnaceConfig.ID, GlassForgeConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Metallurgy_PyrolysisKilnConfig.ID, KilnConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Metallurgy_BasicOilRefineryConfig.ID, OilRefineryConfig.ID, ordering: ModUtil.BuildingOrdering.Before);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Metallurgy_BallCrusherMillConfig.ID, RockCrusherConfig.ID);
		}
		private static void AddBuildingsToPlanscreen_DupesMachinery()
		{
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Machinery_FlocculationSieveConfig.ID, WaterPurifierConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Oxygen, Machinery_AlgaeVatConfig.ID, AlgaeHabitatConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Machinery_CoralVatConfig.ID, ChlorinatorConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Machinery_SlimeVatConfig.ID, AlgaeDistilleryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(PlanMenuCategory.Refinement, Machinery_EthanolStillConfig.ID, EthanolDistilleryConfig.ID);
		}
		public static void AddBuildingsToTech()
		{
			if (Config.Instance.ChemicalProcessing_BioChemistry_Enabled)
				AddBuildingsToTech_ChemicalProcessingBioChemistry();
			if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
				AddBuildingsToTech_ChemicalProcessingIndustrialOverhaul();
			if (Config.Instance.MineralProcessing_Mining_Enabled)
				AddBuildingsToTech_MineralProcessingMining();
			if (Config.Instance.MineralProcessing_Metallurgy_Enabled)
				AddBuildingsToTech_MineralProcessingMetallurgy();
			if (Config.Instance.DupesMachinery_Enabled)
				AddBuildingsToTech_DupesMachinery();
		}
		private static void AddBuildingsToTech_ChemicalProcessingIndustrialOverhaul()
		{
			InjectionMethods.AddBuildingToTechnology(Technology.SolidMaterial.Smelting, Chemical_AdvancedKilnConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.SolidMaterial.SuperheatedForging, Chemical_AdvancedMetalRefineryConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.SolidMaterial.BruteForceRefinement, Chemical_SmallCrusherMillConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.SolidMaterial.SuperheatedForging, Chemical_BallCrusherMillConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.Food.Agriculture, Chemical_Co2PumpConfig.ID);

			///consolidate the two buildings into one bc its only an element change in the converter
			//if (DlcManager.IsExpansion1Active())
			//	InjectionMethods.AddBuildingToTechnology(Technology.Gases.PortableGasses, Chemical_Co2RecyclerDLC1Config.ID);
			//else
			InjectionMethods.AddBuildingToTechnology(Technology.Gases.PortableGasses, Chemical_Co2RecyclerConfig.ID);

			InjectionMethods.AddBuildingToTechnology(Technology.Power.FossilFuels, Chemical_Coal_BoilerConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.Power.FossilFuels, Chemical_CrudeOilRefineryConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.Power.FossilFuels, Chemical_ElectricBoilerConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.Liquids.LiquidTuning, Chemical_EndothermicUnitConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.Power.FossilFuels, Chemical_Gas_BoilerConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.SolidMaterial.SuperheatedForging, Chemical_GlassFoundryConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.Power.FossilFuels, Chemical_NaphthaReformerConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.Power.FossilFuels, Chemical_PropaneReformerConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.Power.FossilFuels, Chemical_RawGasRefineryConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.Decor.TextileProduction, Chemical_RayonLoomConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.SolidMaterial.Smelting, Chemical_SelectiveArcFurnaceConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.Food.Agriculture, Chemical_SoilMixerConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.Liquids.LiquidBasedRefinementProcess, Chemical_SourWaterStripperConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.Liquids.Distillation, Chemical_SyngasRefineryConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.Liquids.Emulsification, Chemical_SynthesizerNitricConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.Liquids.Distillation, Chemical_SynthesizerSaltWaterConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.Liquids.Emulsification, Chemical_SynthesizerSulfuricConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.Liquids.LiquidBasedRefinementProcess, Chemical_ThermalDesalinatorConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.Power.FossilFuels, Chemical_Wooden_BoilerConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.Power.PlasticManufacturing, Custom_PolymerizerConfig.ID);
		}
		private static void AddBuildingsToTech_MineralProcessingMetallurgy()
		{
			InjectionMethods.AddBuildingToTechnology(Technology.Gases.Catalytics, Metallurgy_PlasmaFurnaceConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.SolidMaterial.BruteForceRefinement, Metallurgy_PyrolysisKilnConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.Power.FossilFuels, Metallurgy_BasicOilRefineryConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.SolidMaterial.SuperheatedForging, Metallurgy_BallCrusherMillConfig.ID);
		}
		private static void AddBuildingsToTech_ChemicalProcessingBioChemistry()
		{
			InjectionMethods.AddBuildingToTechnology(Technology.Food.Agriculture, Biochemistry_AlgaeGrowingBasinConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.Food.FoodRepurposing, Biochemistry_AnaerobicDigesterConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.Power.FossilFuels, Biochemistry_BiodieselGeneratorConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.Power.FossilFuels, Biochemistry_BiodieselRefineryConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.Power.PlasticManufacturing, Biochemistry_BioplasticPrinterConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.SolidMaterial.BruteForceRefinement, Biochemistry_ExpellerPressConfig.ID);
		}
		private static void AddBuildingsToTech_MineralProcessingMining()
		{
			InjectionMethods.AddBuildingToTechnology(Technology.SolidMaterial.SolidManagement, Mining_CNCMachineConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.SolidMaterial.SolidManagement, Mining_AugerDrillConfig.ID);
		}
		private static void AddBuildingsToTech_DupesMachinery()
		{
			InjectionMethods.AddBuildingToTechnology(Technology.Liquids.LiquidBasedRefinementProcess, Machinery_FlocculationSieveConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.Liquids.AirSystems, Machinery_AlgaeVatConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.Liquids.LiquidBasedRefinementProcess, Machinery_CoralVatConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.Liquids.LiquidBasedRefinementProcess, Machinery_SlimeVatConfig.ID);
			InjectionMethods.AddBuildingToTechnology(Technology.Liquids.Distillation, EthanolDistilleryConfig.ID);
		}
		#endregion
	}
}
