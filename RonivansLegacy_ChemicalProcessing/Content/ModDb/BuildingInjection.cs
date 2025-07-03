using UtilLibs;
using Dupes_Industrial_Overhaul.Chemical_Processing.Buildings;
using Biochemistry.Buildings;
using Mineral_Processing_Mining.Buildings;
using Metallurgy.Buildings;
using HarmonyLib;
using Dupes_Machinery.Biological_Vats;
using Dupes_Machinery.Ethanol_Still;
namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	class BuildingInjection
	{
		/// <summary>
		/// Registers these as building tags, otherwise the name strings of those elements break
		/// </summary>
		internal static void RegisterAdditionalBuildingElements()
		{
			GameTags.MaterialBuildingElements.Add(SimHashes.Ceramic.CreateTag());
		}

		static void RegisterOilWellCapCustomPiping()
		{
			var oilWell = Assets.GetBuildingDef(OilWellCapConfig.ID);
			Custom_OilWellCapConfig.AttachPorts(oilWell.BuildingPreview);
			Custom_OilWellCapConfig.AttachPorts(oilWell.BuildingUnderConstruction);
			Custom_OilWellCapConfig.AttachPorts(oilWell.BuildingComplete);
		}

		public static void AddBuildingsToPlanscreen()
		{
			if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
			{
				AddBuildingsToPlanscreen_ChemicalProcessingIndustrialOverhaul();
				RegisterOilWellCapCustomPiping();
			}
			if (Config.Instance.ChemicalProcessing_BioChemistry_Enabled)
				AddBuildingsToPlanscreen_ChemicalProcessingBioChemistry();
			if(Config.Instance.MineralProcessing_Mining_Enabled)
				AddBuildingsToPlanscreen_MineralProcessingMining();
			if (Config.Instance.MineralProcessing_Metallurgy_Enabled)
				AddBuildingsToPlanscreen_MineralProcessingMetallurgy();
			if(Config.Instance.DupesMachinery_Enabled)
				AddBuildingsToPlanscreen_DupesMachinery();
		}

		private static void AddBuildingsToPlanscreen_ChemicalProcessingIndustrialOverhaul()
		{
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_AdvancedKilnConfig.ID, KilnConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_AdvancedMetalRefineryConfig.ID, MetalRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_BallCrusherMillConfig.ID, RockCrusherConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Oxygen, Chemical_Co2PumpConfig.ID, CO2ScrubberConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_Co2RecyclerConfig.ID, OxyliteRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_Coal_BoilerConfig.ID, OilRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_CrudeOilRefineryConfig.ID, OilRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_ElectricBoilerConfig.ID, Chemical_Coal_BoilerConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Utilities, Chemical_EndothermicUnitConfig.ID, LiquidConditionerConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_Gas_BoilerConfig.ID, Chemical_ElectricBoilerConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_GlassFoundryConfig.ID, GlassForgeConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_SmallCrusherMillConfig.ID, RockCrusherConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_NaphthaReformerConfig.ID, OilRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_PropaneReformerConfig.ID, Chemical_NaphthaReformerConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_RawGasRefineryConfig.ID, OilRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_RayonLoomConfig.ID, EthanolDistilleryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_SelectiveArcFurnaceConfig.ID, SupermaterialRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_SoilMixerConfig.ID, CompostConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_SourWaterStripperConfig.ID, WaterPurifierConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_SyngasRefineryConfig.ID, OilRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_SynthesizerNitricConfig.ID, ChemicalRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_SynthesizerSaltWaterConfig.ID, DesalinatorConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_SynthesizerSulfuricConfig.ID, ChemicalRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_ThermalDesalinatorConfig.ID, DesalinatorConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Chemical_Wooden_BoilerConfig.ID, Chemical_Coal_BoilerConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Custom_PolymerizerConfig.ID, PolymerizerConfig.ID);
		}
		private static void AddBuildingsToPlanscreen_ChemicalProcessingBioChemistry()
		{
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Oxygen, Biochemistry_AlgaeGrowingBasinConfig.ID, AlgaeHabitatConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Biochemistry_AnaerobicDigesterConfig.ID, FertilizerMakerConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Power, Biochemistry_BiodieselGeneratorConfig.ID, PetroleumGeneratorConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Biochemistry_BiodieselRefineryConfig.ID, OilRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Biochemistry_BioplasticPrinterConfig.ID, PolymerizerConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Biochemistry_ExpellerPressConfig.ID, Biochemistry_AnaerobicDigesterConfig.ID);
		}
		private static void AddBuildingsToPlanscreen_MineralProcessingMining()
		{
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Mining_CNCMachineConfig.ID, SupermaterialRefineryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Utilities, Mining_AugerDrillConfig.ID, OilWellCapConfig.ID);
		}

		private static void AddBuildingsToPlanscreen_MineralProcessingMetallurgy()
		{
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Metallurgy_PlasmaFurnaceConfig.ID, GlassForgeConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Metallurgy_PyrolysisKilnConfig.ID, KilnConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Metallurgy_BasicOilRefineryConfig.ID, OilRefineryConfig.ID,ordering:ModUtil.BuildingOrdering.Before);
		}
		private static void AddBuildingsToPlanscreen_DupesMachinery()
		{
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Machinery_FlocculationSieveConfig.ID, WaterPurifierConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Oxygen, Machinery_AlgaeVatConfig.ID, AlgaeHabitatConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Machinery_CoralVatConfig.ID, ChlorinatorConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Machinery_SlimeVatConfig.ID, AlgaeDistilleryConfig.ID);
			InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, Machinery_EthanolStillConfig.ID, EthanolDistilleryConfig.ID);
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
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.Smelting, Chemical_AdvancedKilnConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.SuperheatedForging, Chemical_AdvancedMetalRefineryConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.SuperheatedForging, Chemical_BallCrusherMillConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Food.Agriculture, Chemical_Co2PumpConfig.ID);

			///consolidate the two buildings into one bc its only an element change in the converter
			//if (DlcManager.IsExpansion1Active())
			//	InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Gases.PortableGasses, Chemical_Co2RecyclerDLC1Config.ID);
			//else
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Gases.PortableGasses, Chemical_Co2RecyclerConfig.ID);

			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.FossilFuels, Chemical_Coal_BoilerConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.FossilFuels, Chemical_CrudeOilRefineryConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.FossilFuels, Chemical_ElectricBoilerConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Liquids.LiquidTuning, Chemical_EndothermicUnitConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.FossilFuels, Chemical_Gas_BoilerConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.SuperheatedForging, Chemical_GlassFoundryConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.BruteForceRefinement, Chemical_SmallCrusherMillConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.FossilFuels, Chemical_NaphthaReformerConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.FossilFuels, Chemical_PropaneReformerConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.FossilFuels, Chemical_RawGasRefineryConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Decor.TextileProduction, Chemical_RayonLoomConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.Smelting, Chemical_SelectiveArcFurnaceConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Food.Agriculture, Chemical_SoilMixerConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Liquids.LiquidBasedRefinementProcess, Chemical_SourWaterStripperConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Liquids.Distillation, Chemical_SyngasRefineryConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Liquids.Emulsification, Chemical_SynthesizerNitricConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Liquids.Distillation, Chemical_SynthesizerSaltWaterConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Liquids.Emulsification, Chemical_SynthesizerSulfuricConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Liquids.LiquidBasedRefinementProcess, Chemical_ThermalDesalinatorConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.FossilFuels, Chemical_Wooden_BoilerConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.PlasticManufacturing, Custom_PolymerizerConfig.ID);
		}
		private static void AddBuildingsToTech_MineralProcessingMetallurgy()
		{
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Gases.Catalytics, Metallurgy_PlasmaFurnaceConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.BruteForceRefinement, Metallurgy_PyrolysisKilnConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.FossilFuels, Metallurgy_BasicOilRefineryConfig.ID);
		}
		private static void AddBuildingsToTech_ChemicalProcessingBioChemistry()
		{
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Food.Agriculture, Biochemistry_AlgaeGrowingBasinConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Food.FoodRepurposing, Biochemistry_AnaerobicDigesterConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.FossilFuels, Biochemistry_BiodieselGeneratorConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.FossilFuels, Biochemistry_BiodieselRefineryConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Power.PlasticManufacturing, Biochemistry_BioplasticPrinterConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.BruteForceRefinement, Biochemistry_ExpellerPressConfig.ID);
		}
		private static void AddBuildingsToTech_MineralProcessingMining()
		{
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.SolidManagement, Mining_CNCMachineConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.SolidManagement, Mining_AugerDrillConfig.ID);
		}
		private static void AddBuildingsToTech_DupesMachinery()
		{
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Liquids.LiquidBasedRefinementProcess, Machinery_FlocculationSieveConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Liquids.AirSystems, Machinery_AlgaeVatConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Liquids.LiquidBasedRefinementProcess, Machinery_CoralVatConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Liquids.LiquidBasedRefinementProcess, Machinery_SlimeVatConfig.ID);
			InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Liquids.Distillation, EthanolDistilleryConfig.ID);
		}
	}
}
