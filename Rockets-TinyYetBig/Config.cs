using Newtonsoft.Json;
using PeterHan.PLib.Options;
using Rockets_TinyYetBig.Content.Scripts.UI;
using System;
using System.Collections.Generic;
using UtilLibs;

namespace Rockets_TinyYetBig
{
	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	[ModInfo("https://github.com/Sgt-Imalas/Sgt_Imalas-Oni-Mods", "preview.png")]
	public class Config : SingletonOptions<Config>, PeterHan.PLib.Options.IOptions
	{
		public static bool SpaceStationsPossible =>
			   Instance.SpaceStationsAndTech
			&& Instance.CompressInteriors
			&& Instance.EnableAdvWorldSelector
			&& Instance.NeutroniumMaterial
			&& Instance.RocketDocking
			;
		public static bool Derelicts =>
			   SpaceStationsPossible && false;


		//[Option("Vanilla+ Preset", "Load a settings preset: All the bugfixes and qol improvements, everything else disabled")]
		//public Action<object> LoadVanillaPlusSettings { get 
		//    {
		//        return i =>
		//        {
		//            EnableAdvWorldSelector = true;
		//            CompressInteriors = true;
		//            ScannerModuleRangeRadius = 4;
		//            ScannerModuleScanSpeed = 0.33f;
		//            HabitatPowerPlug = true;
		//            EnableExtendedHabs = false;
		//            HabitatInteriorRadiation = false;
		//            HabitatInteriorPortImprovements = true;
		//            SlimLargeEngines = false;

		//            ///Drilling&Shipping
		//            EnableCritterStorage = false;
		//            CritterStorageCapacity = 5;
		//            EnableLaserDrill = false;
		//            LaserDrillconeSpeed = 3.75f;
		//            EnableFridge = false;
		//            InfinitePOI = false;
		//            EnableLargeCargoBays = false;
		//            InsulatedCargoBays = true;
		//            EnableRadboltStorage = false;
		//            EnableDrillSupport = false;
		//            LaserDrillconeSpeed = 25;
		//            DrillconeSupportDiamondMass = 1500;
		//            PilotSkillAffectsDrillSpeed = false;
		//            RefillDrillSupport = false;
		//            EnablePOISensor = false;

		//            RebalancedCargoCapacity = false;
		//            GasCargoBayKgPerUnit = 500;
		//            LiquidCargoBayKgPerUnit = 1250;
		//            SolidCargoBayKgPerUnit = 2000;
		//            SmallCargoBayUnits = 9;
		//            MediumCargoBayUnits = 27;
		//            CollossalCargoBayUnits = 64;


		//            /// Fuel&Logistics
		//            BuffLargeOxidizer = true;
		//            EthanolEngines = true;
		//            Boosters = false;
		//            EnableNatGasEngine = false;
		//            EnableNatGasEngineRange = 15;
		//            EnableEarlyGameFuelTanks = false;
		//            EnableFuelLoaders = false;
		//            EnableWallAdapter = false;
		//            EnableBunkerPlatform = false;

		//            /// Power&Utility
		//            EnableSolarNosecone = false;
		//            EnableGenerators = false;
		//            EnableSmolBattery = false;
		//            IsotopeDecayTime = 50;

		//            /// SpaceStations
		//            SpaceStationsAndTech = false;
		//            RocketDocking =  false;
		//            NeutroniumMaterial = false;

		//            /// EasterEggs
		//            SpiceEyes = false;
		//        };
		//    }  
		//}


		#region vanillaplus


		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.OPENBUILDINGEDITOR.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.OPENBUILDINGEDITOR.TOOLTIP")]
		[JsonIgnore]
		public System.Action<object> Button_OpenBuildingConfigEditor => (_) => BuildingEditor_MainScreen.ShowBuildingEditor(null, null);

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLEADVWORLDSELECTOR.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLEADVWORLDSELECTOR.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.A_ROCKETRYPLUS")]
		[JsonProperty]
		public bool EnableAdvWorldSelector { get; set; } = true;

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.COMPRESSINTERIORS.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.COMPRESSINTERIORS.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.A_ROCKETRYPLUS")]
		[JsonProperty]
		public bool CompressInteriors { get; set; } = true;
		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.CEMTEREDINTERIORS.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CEMTEREDINTERIORS.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.A_ROCKETRYPLUS")]
		[JsonProperty]
		public bool CenteredInteriors { get; set; } = true;

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLEEXTENDEDHABS.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.TOGGLEMULTI", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.A_ROCKETRYPLUS")]
		[JsonProperty]
		public bool EnableExtendedHabs { get; set; } = true;

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.SCANNERMODULERANGERADIUS.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.SCANNERMODULERANGERADIUS.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.A_ROCKETRYPLUS")]
		[Limit(0, 6)]
		[JsonProperty]
		public int ScannerModuleRangeRadius { get; set; } = 4;

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.SCANNERMODULESCANSPEED.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.SCANNERMODULESCANSPEED.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.A_ROCKETRYPLUS")]
		[Limit(0.1f, 1f)]
		[JsonProperty]
		public float ScannerModuleScanSpeed { get; set; } = 0.4f;

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.HABITATPOWERPLUG.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.HABITATPOWERPLUG.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.A_ROCKETRYPLUS")]
		[JsonProperty]
		public bool HabitatPowerPlug { get; set; } = true;

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.HABITATINTERIORRADIATION.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.HABITATINTERIORRADIATION.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.A_ROCKETRYPLUS")]
		[JsonProperty]
		public bool HabitatInteriorRadiation { get; set; } = true;

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.HABITATINTERIORPORTIMPROVEMENTS.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.HABITATINTERIORPORTIMPROVEMENTS.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.A_ROCKETRYPLUS")]
		[JsonProperty]
		public bool HabitatInteriorPortImprovements { get; set; } = true;


		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.SLIMLARGEENGINES.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.SLIMLARGEENGINES.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.A_ROCKETRYPLUS")]
		[JsonProperty]
		public bool SlimLargeEngines { get; set; } = false;

		#endregion

		#region mining&shipping

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.LASERDRILLCONESPEED.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.LASERDRILLCONESPEED.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
		[Limit(1f, 15f)]
		[JsonProperty]
		public float LaserDrillconeSpeed { get; set; } = 4.875f;


		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.DRILLCONESUPPORTBOOST.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.DRILLCONESUPPORTBOOST.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
		[JsonProperty]
		[Limit(0, 100)]
		public int DrillconeSupportSpeedBoost { get; set; } = 20;


		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.REFILLDRILLSUPPORT.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.REFILLDRILLSUPPORT.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
		[JsonProperty]
		public bool RefillDrillSupport { get; set; } = false;


		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.PILOTSKILLAFFECTSDRILLSPEED.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.PILOTSKILLAFFECTSDRILLSPEED.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
		[JsonProperty]
		public bool PilotSkillAffectsDrillSpeed { get; set; } = true;


		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.INFINITEPOI.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.INFINITEPOI.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
		[JsonProperty]
		public bool InfinitePOI { get; set; } = false;

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLELARGECARGOBAYS.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.TOGGLEMULTI", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
		[JsonProperty]
		public bool EnableLargeCargoBays { get; set; } = true;

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.INSULATEDCARGOBAYS.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.INSULATEDCARGOBAYS.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
		[JsonProperty]
		public bool InsulatedCargoBays { get; set; } = true;

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.CARGOBAY_LOGICPORTS.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CARGOBAY_LOGICPORTS.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
		[JsonProperty]
		public bool CargoBayLogicPorts { get; set; } = true;
				
		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ROCKETMODULEUPGRADES.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.ROCKETMODULEUPGRADES.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
		[JsonProperty]
		public bool EnableRocketModuleUpgrades { get; set; } = true;
				

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.REBALANCEDCARGOCAPACITY.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.REBALANCEDCARGOCAPACITY.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
		[JsonProperty]
		public bool RebalancedCargoCapacity { get; set; } = true;

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.CARGOBAYUNITS.GASCARGOBAYKGPERUNIT", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CARGOBAYUNITS.UNITDESCRIPTION", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
		[Limit(200, 1500)]
		[JsonProperty]
		public int GasCargoBayKgPerUnit { get; set; } = 500;


		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.CARGOBAYUNITS.LIQUIDCARGOBAYKGPERUNIT", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CARGOBAYUNITS.UNITDESCRIPTION", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
		[Limit(500, 2000)]
		[JsonProperty]
		public int LiquidCargoBayKgPerUnit { get; set; } = 1250;

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.CARGOBAYUNITS.SOLIDCARGOBAYKGPERUNIT", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CARGOBAYUNITS.UNITDESCRIPTION", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
		[Limit(800, 6000)]
		[JsonProperty]
		public int SolidCargoBayKgPerUnit { get; set; } = 2000;


		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.CARGOBAYUNITS.SMALLCARGOBAYUNITS", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CARGOBAYUNITS.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
		[Limit(3, 32)]
		[JsonProperty]
		public float SmallCargoBayUnits { get; set; } = 9;


		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.CARGOBAYUNITS.MEDIUMCARGOBAYUNITS", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CARGOBAYUNITS.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
		[Limit(6, 64)]
		[JsonProperty]
		public float MediumCargoBayUnits { get; set; } = 27;

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.CARGOBAYUNITS.COLLOSSALCARGOBAYUNITS", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CARGOBAYUNITS.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
		[Limit(9, 128)]
		[JsonProperty]
		public float CollossalCargoBayUnits { get; set; } = 64;

		#endregion

		#region Fuel&Logistics

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.BUFFLARGEOXIDIZER.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.BUFFLARGEOXIDIZER.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.C_FUELLOGISTICS")]
		[JsonProperty]
		public bool BuffLargeOxidizer { get; set; } = true;

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ETHANOLENGINES.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.ETHANOLENGINES.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.C_FUELLOGISTICS")]
		[JsonProperty]
		public bool EthanolEngines { get; set; } = true;

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLEBOOSTERS.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.TOGGLEMULTI", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.C_FUELLOGISTICS")]
		[JsonProperty]
#if DEBUG
        public bool EnableBoosters { get; set; }
#else
		private bool EnableBoosters { get; set; }
#endif

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLEELECTRICENGINE.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.TOGGLESINGLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.C_FUELLOGISTICS")]
		[JsonProperty]
		private bool EnableElectricEngine { get; set; }

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLEFUELLOADERS.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.TOGGLEMULTI", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.C_FUELLOGISTICS")]
		[JsonProperty]
		public bool EnableFuelLoaders { get; set; } = true;

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLEROCKETLOADERLOGICOUTPUTS.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLEROCKETLOADERLOGICOUTPUTS.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.C_FUELLOGISTICS")]
		[JsonProperty]
		public bool EnableRocketLoaderLogicOutputs { get; set; } = true;

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLEWALLADAPTER.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.TOGGLEMULTI", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.C_FUELLOGISTICS")]
		[JsonProperty]
		public bool EnableWallAdapter { get; set; } = true; 
		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.VERTICALPORT_LADDER.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.VERTICALPORT_LADDER.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.C_FUELLOGISTICS")]
		[JsonProperty]
		public bool VerticalPortAdapterLadder { get; set; } = false;
		#endregion

		#region Power&Utility

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLEGENERATORS.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.TOGGLEMULTI", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.D_POWERUTILITY")]
		[JsonProperty]
		public bool EnableGenerators { get; set; } = true;

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ISOTOPEDECAYTIME.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.ISOTOPEDECAYTIME.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.D_POWERUTILITY")]
		[Limit(10f, 200f)]
		[JsonProperty]
		public float IsotopeDecayTime { get; set; } = 50;
		#endregion
		#region SpaceStations

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.SPACESTATIONSANDTECH.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.SPACESTATIONSANDTECH.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.E_SPACEEXPANSION")]
		[JsonProperty]
		public bool SpaceStationsAndTech { get; set; } = false;

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ROCKETDOCKING.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.ROCKETDOCKING.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.E_SPACEEXPANSION")]
		[JsonProperty]
		public bool RocketDocking { get; set; } = true;

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.NEUTRONIUMMATERIAL.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.NEUTRONIUMMATERIAL.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.E_SPACEEXPANSION")]
		[JsonProperty]
		public bool NeutroniumMaterial { get; set; } = true;

		#endregion

		#region EasterEggs

		[Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.SPICEEYES.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.SPICEEYES.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.F_EASTEREGGS")]
		[JsonProperty]
		public bool SpiceEyes { get; set; } = true;

		#endregion

		public IEnumerable<IOptionsEntry> CreateOptions()
		{
			return new List<IOptionsEntry>();
		}

		public void OnOptionsChanged()
		{
			if (SpaceStationsAndTech)
			{
				CompressInteriors = true;
				EnableAdvWorldSelector = true;
				NeutroniumMaterial = true;
			}
			Instance = this;
		}
	}
}
