using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.Buildings.CargoBays;
using Rockets_TinyYetBig.Buildings.Utility;
using Rockets_TinyYetBig.Content.Scripts.UI.Sidescreens;
using Rockets_TinyYetBig.NonRocketBuildings;
using Rockets_TinyYetBig.RocketFueling;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TUNING;
using UnityEngine;
using UtilLibs;
using static PeterHan.PLib.UI.PTextField;
using static Rockets_TinyYetBig.RocketFueling.FuelLoaderComponent;
using static Rockets_TinyYetBig.STRINGS.BUILDING.STATUSITEMS;

namespace Rockets_TinyYetBig
{
    public class ModAssets
	{
		public static readonly HashedString LOGICPORT_ROCKETPORTLOADER_ACTIVE = "ROCKETPORTLOADER_ACTIVE";
		public class Hashes
		{
			public static ModHashes DockingConnectionChanged = new ModHashes("RTB_DockingConnectionChanged");
			public static ModHashes DockingConnectionConnected = new ModHashes("RTB_DockingConnectionConnected");
			public static ModHashes DockingConnectionDisconnected = new ModHashes("RTB_DockingConnectionDisconnected");
			public static ModHashes DockableAdded = new ModHashes("RTB_DockableAdded");
			public static ModHashes DockableRemoved = new ModHashes("RTB_DockableRemoved");
			public static ModHashes OnStationPartConstructionStarted = new ModHashes("RTB_OnStationPartConstructionStarted");
			public static ModHashes OnStationPartConstructionFinished = new ModHashes("RTB_OnStationPartConstructionFinished");
			public static ModHashes OnRocketModuleMoved = new ModHashes("RTB_OnRocketModuleMoved");
			public static ModHashes OnStationMove = new ModHashes("RTB_OnStationMove");
		}

		public static GameObject ModuleSettingsWindowPrefab;
		public static GameObject DockingSideScreenWindowPrefab;

		//This is required to keep the GO of DupeTransferSecondarySideScreen from getting GCed!
		public static GameObject DupeTransferSecondarySideScreenWindowPrefab;
		public static KScreen DupeTransferSecondarySideScreen;

		public static GameObject SpaceConstructionSideScreenWindowPrefab;

		//This is required to keep the GO of SpaceConstructionTargetSecondarySideScreen from getting GCed!
		public static GameObject SpaceConstructionTargetScreenWindowPrefab;
		public static KScreen SpaceConstructionTargetSecondarySideScreen;



		static bool tooltipsInitialized = false;
		public static string GetCategoryTooltip(int category)
		{
			if (!tooltipsInitialized)
				InitializeCategoryTooltipDictionary();

			if(Tooltips.TryGetValue(category, out var tooltip))
				return tooltip;

			SgtLogger.warning("RE category tooltip not found for category " + category);

			return "Category Tooltip not found!";
		}
		private static Dictionary<int, string> Tooltips = new Dictionary<int, string>();
		private static void InitializeCategoryTooltipDictionary()
		{
			if(tooltipsInitialized)
				return;

            tooltipsInitialized = true;

            Tooltips.Add(0, STRINGS.ROCKETBUILDMENUCATEGORIES.CATEGORYTOOLTIPS.ENGINES);
			Tooltips.Add(1, STRINGS.ROCKETBUILDMENUCATEGORIES.CATEGORYTOOLTIPS.HABITATS);
			Tooltips.Add(2, STRINGS.ROCKETBUILDMENUCATEGORIES.CATEGORYTOOLTIPS.NOSECONES);
			Tooltips.Add(3, STRINGS.ROCKETBUILDMENUCATEGORIES.CATEGORYTOOLTIPS.DEPLOYABLES);
			Tooltips.Add(4, STRINGS.ROCKETBUILDMENUCATEGORIES.CATEGORYTOOLTIPS.FUEL);
			Tooltips.Add(5, STRINGS.ROCKETBUILDMENUCATEGORIES.CATEGORYTOOLTIPS.CARGO);
			Tooltips.Add(6, STRINGS.ROCKETBUILDMENUCATEGORIES.CATEGORYTOOLTIPS.POWER);
			Tooltips.Add(7, STRINGS.ROCKETBUILDMENUCATEGORIES.CATEGORYTOOLTIPS.PRODUCTION);
			Tooltips.Add(8, STRINGS.ROCKETBUILDMENUCATEGORIES.CATEGORYTOOLTIPS.UTILITY);
			Tooltips.Add(-1, STRINGS.ROCKETBUILDMENUCATEGORIES.CATEGORYTOOLTIPS.UNCATEGORIZED);

			//engines = 0,
			//    habitats = 1,
			//nosecones = 2,
			//deployables = 3,
			//fuel = 4,
			//cargo = 5,
			//power = 6,
			//production = 7,
			//utility = 8,
			//uncategorized = -1
		}

		public static void LoadAssets()
		{
			AssetBundle bundle = AssetUtils.LoadAssetBundle("rocketryexpanded_ui_assets", platformSpecific: true);
			ModuleSettingsWindowPrefab = bundle.LoadAsset<GameObject>("Assets/UIs/ModuleSettings.prefab");
			DupeTransferSecondarySideScreenWindowPrefab = bundle.LoadAsset<GameObject>("Assets/UIs/DockingTransferScreen.prefab");
			DockingSideScreenWindowPrefab = bundle.LoadAsset<GameObject>("Assets/UIs/DockingScreen.prefab");
			SpaceConstructionSideScreenWindowPrefab = bundle.LoadAsset<GameObject>("Assets/UIs/SpaceAssembleMenu_Sidescreen.prefab");
			SpaceConstructionTargetScreenWindowPrefab = bundle.LoadAsset<GameObject>("Assets/UIs/ConstructionSelector_SecondarySidescreen.prefab");


			SgtLogger.Assert("ModuleSettingsWindowPrefab", ModuleSettingsWindowPrefab);
			SgtLogger.Assert("DockingSideScreenWindowPrefab", DockingSideScreenWindowPrefab);
			SgtLogger.Assert("DupeTransferSecondarySideScreenWindowPrefab", DupeTransferSecondarySideScreenWindowPrefab);
			SgtLogger.Assert("SpaceConstructionSideScreenWindowPrefab", SpaceConstructionSideScreenWindowPrefab);
			SgtLogger.Assert("SpaceConstructionTargetScreenWindowPrefab", SpaceConstructionTargetScreenWindowPrefab);

			var TMPConverter = new TMPConverter();
			TMPConverter.ReplaceAllText(ModuleSettingsWindowPrefab);
			TMPConverter.ReplaceAllText(DockingSideScreenWindowPrefab);
			TMPConverter.ReplaceAllText(DupeTransferSecondarySideScreenWindowPrefab);
			TMPConverter.ReplaceAllText(SpaceConstructionSideScreenWindowPrefab);
			TMPConverter.ReplaceAllText(SpaceConstructionTargetScreenWindowPrefab);

			DupeTransferSecondarySideScreen = DupeTransferSecondarySideScreenWindowPrefab.AddComponent<CrewAssignmentSidescreen>();
			SpaceConstructionTargetSecondarySideScreen = SpaceConstructionTargetScreenWindowPrefab.AddComponent<SpaceConstructionTargetScreen>();
		}

		public static float DefaultDrillconeHarvestSpeed = ROCKETRY.SOLID_CARGO_BAY_CLUSTER_CAPACITY * ROCKETRY.CARGO_CAPACITY_SCALE / 3600f;

		

		public static float GetMiningPilotSkillMultiplier(Clustercraft clustercraft)
		{
			float multiplier = 1f;
			if (Config.Instance.PilotSkillAffectsDrillSpeed)
			{
				float flyingSpeedMultiplier = clustercraft.PilotSkillMultiplier;
				float miningSpeedMultiplier = clustercraft.GetComponent<Clustercraft_AdditionalComponent>().Drillcone_MiningSkillMultiplier;

				multiplier = Mathf.Max(0.75f, (flyingSpeedMultiplier * 0.5f + miningSpeedMultiplier * 0.5f));
				//SgtLogger.l($"Total MiningMultiplier: {multiplier}, flying: {flyingSpeedMultiplier}, mining: {miningSpeedMultiplier}.");
				multiplier = (multiplier - 1) * 2 + 1;
				//SgtLogger.l($"Total MiningMultiplier adjusted: {multiplier}.");
			}
			return multiplier;
		}

		internal static void FreeGridSpace_Fixed(Vector2I size, Vector2I offset)
		{
			int cell = Grid.XYToCell(offset.x, offset.y), width = size.x, stride =
				Grid.WidthInCells - width;
			for (int y = size.y; y > 0; y--)
			{
				for (int x = width; x > 0; x--)
				{
					if (Grid.IsValidCell(cell))
						SimMessages.ReplaceElement(cell, SimHashes.Vacuum, null, 0.0f);
					cell++;
				}
				cell += stride;
			}
		}


		public static string DeepSpaceScienceID = "rtb_deepspace";
		public class Techs
		{
			public static string FuelLoaderTechID = "RTB_FuelLoadersTech";
			public static Tech FuelLoaderTech;
			public static string DockingTechID = "RTB_DockingTech";
			public static Tech DockingTech;
			public static string LargerRocketLivingSpaceTechID = "RTB_LargerRocketLivingSpaceTech";
			public static Tech LargerRocketLivingSpaceTech;
			public static string SpaceScienceTechID = "RTB_SpaceScienceTech";
			public static Tech SpaceScienceTech;
			public static string SpaceStationTechID = "RTB_SpaceStationTech"; 
			public static Tech SpaceStationTech;
			public static string SpaceStationTechMediumID = "RTB_MediumSpaceStationTech";
			public static Tech SpaceStationTechMedium;
			public static string SpaceStationTechLargeID = "RTB_LargeSpaceStationTech";
			public static Tech SpaceStationTechLarge;
			public static string HugeCargoBayTechID = "RTB_HugeCargoBayTech";
			public static Tech HugeCargoBayTech;
		}
		public class Tags
		{
			public static Tag VerticalPortAttachementPoint = TagManager.Create("RTB_verticalPortAttachmentPoint");

			public static Tag AttachmentSlotDockingDoor = TagManager.Create("RTB_DockingTubeAttachmentSlot");
			public static Tag IsSpaceStation = TagManager.Create("RTB_isSpaceStationInteriorWorld");
			public static Tag IsDerelict = TagManager.Create("RTB_isDerelictInterior");

			public static Tag NoBuildingAllowed = TagManager.Create("RTB_NoBuildingAllowed");


			public static Tag SpaceStationOnlyInteriorBuilding = TagManager.Create("RTB_SpaceStationInteriorOnly");
			public static Tag RocketInteriorOnlyBuilding = TagManager.Create("RTB_RocketInteriorOnly");

			public static Tag RocketPlatformTag = TagManager.Create("RTB_RocketPlatformTag");
			public static Tag NeutroniumAlloy = TagManager.Create("RTB_NeutroniumAlloyMaterial");

			/// <summary>
			///Use this tag to add the radiation shielding tag to a material, making it available for constructing the plated nosecone.
			///By default it is attached to Lead, depleted uranium, thermium, tungsten and neutronium alloy
			/// </summary>
			public static Tag RadiationShieldingRocketConstructionMaterial = TagManager.Create("RTB_RadiationShieldingRocketConstructionMaterial");


			/// <summary>
			/// add this tag to any liquid material that is a rocket fuel, by default it is attached to every material with the "Combustible Liquid" Tag and Hydrogen
			/// </summary>
			public static Tag RocketFuelTag = TagManager.Create("RTB_RocketFuelMaterial");


			/// <summary>
			/// add this tag to any solid material that should act as a solid rocket oxidizer, by default it is attached to oxylite and fertilizer
			/// </summary>
			public static Tag RocketSolidOxidizerTag = TagManager.Create("RTB_RocketSolidOxidizerTag");

			/// <summary>
			/// Liquid Oxidizer that is corrosive, requires storing in Plated Liquid Oxidizer Tank
			/// </summary>
			public static Tag CorrosiveOxidizer = TagManager.Create("RTB_OxidizerCorrosiveRequirement");

			/// <summary>
			/// Liquid Oxidizer that is non corrosive, thus stored in a normal Liquid oxygen tank
			/// </summary>
			public static Tag LOXTankOxidizer = TagManager.Create("RTB_OxidizerLOXTank");

			/// <summary>
			/// has Oxidizer efficiency of 1
			/// </summary>
			public static Tag OxidizerEfficiency_1 = TagManager.Create("RTB_OxidizerEfficiency_1");
			/// <summary>
			/// has Oxidizer efficiency of 2
			/// </summary>
			public static Tag OxidizerEfficiency_2 = TagManager.Create("RTB_OxidizerEfficiency_2");
			/// <summary>
			/// has Oxidizer efficiency of 3
			/// </summary>
			public static Tag OxidizerEfficiency_3 = TagManager.Create("RTB_OxidizerEfficiency_3");

			//TODO: lock behind lox tank science
			/// <summary>
			/// has Oxidizer efficiency of 4, is locked behind LOX tank science
			/// </summary>
			public static Tag OxidizerEfficiency_4 = TagManager.Create("RTB_OxidizerEfficiency_4");

			//TODO: lock behind space station material science

			/// <summary>
			/// has Oxidizer efficiency of 5, is locked behind a space station level science upgrade
			/// </summary>
			public static Tag OxidizerEfficiency_5 = TagManager.Create("RTB_OxidizerEfficiency_5");
			/// <summary>
			/// has Oxidizer efficiency of 6, is locked behind a space station level science upgrade
			/// </summary>
			public static Tag OxidizerEfficiency_6 = TagManager.Create("RTB_OxidizerEfficiency_6");
			/// <summary>
			/// Drillcone, Cargobay, Artifact module, any module that harvests resources in space should have this tag
			/// </summary>
			public static Tag SpaceHarvestModule = TagManager.Create("RTB_SpaceHarvestModule");

		}


		public enum SpaceStationType
		{
			small = 0,
			medium = 1,
			large = 2,
			jumpBeacon = 3,
			jumpGate = 4,
		}


		public static Dictionary<int, SpaceStationWithStats> SpaceStationTypes = new Dictionary<int, SpaceStationWithStats>()
		{
			{
				(int)SpaceStationType.small,

				new SpaceStationWithStats(
				"RTB_SpaceStationSmall",
				"Space Station Construction",
				"A tiny habitat in the vast emptyness of space",
				new Vector2I (30,30),
				new Dictionary<string,float> { [SimHashes.Steel.CreateTag().ToString()]= 500f },
				"space_station_small_kanim",
				20f,//150f,
                Techs.SpaceStationTechID
				)
			},
			{
				(int)SpaceStationType.medium,

				new SpaceStationWithStats(
					"RTB_SpaceStationMedium",
					"Medium Space Station Upgrade",
					"Increase the maximum size of your space stations",
					new Vector2I (45,45),
					new Dictionary<string,float> { [SimHashes.Steel.CreateTag().ToString()]= 750f,
												   [SimHashes.Niobium.CreateTag().ToString()]= 500f },
					"space_station_medium_kanim",
					20f,//300f
                    Techs.SpaceStationTechMediumID
				)
			},
			{
				(int)SpaceStationType.large,

			new SpaceStationWithStats(
				"RTB_SpaceStationLarge",
				"Large Space Station Upgrade",
				"Increase the maximum size of your space stations even further",
				new Vector2I (60,60),
				new Dictionary<string,float> { [SimHashes.Steel.CreateTag().ToString()]= 1000f,
											   [SimHashes.TempConductorSolid.CreateTag().ToString()]= 500f,
											   [SimHashes.Isoresin.CreateTag().ToString()]= 300f ,
											   [SimHashes.Graphite.CreateTag().ToString()]= 200f },
				"space_station_large_kanim",
				20f,//600f
                Techs.SpaceStationTechLargeID
				)
			}
		};

		public static Components.Cmps<FridgeModuleHatchGrabber> FridgeModuleGrabbers = new Components.Cmps<FridgeModuleHatchGrabber>();
		//public static Components.Cmps<FridgeModuleItemDistributor> FridgeModuleDistributors = new Components.Cmps<FridgeModuleItemDistributor>();
		//public static Components.Cmps<DockingManager> Dockables = new Components.Cmps<DockingManager>();

		public static Dictionary<Tuple<BuildingDef, int>, GameObject> CategorizedButtons = new Dictionary<Tuple<BuildingDef, int>, GameObject>();

		public static readonly CellOffset PLUG_OFFSET_SMALL = new CellOffset(-1, 0);
		public static readonly CellOffset PLUG_OFFSET_MEDIUM = new CellOffset(-2, 0);

		public static int InnerLimit = 0;
		public static int Rings = 0;


		public struct SpaceStationWithStats
		{
			public string ID;
			public string Name;
			public string Description;
			public Vector2I InteriorSize;
			public Dictionary<string, float> materials;
			public float constructionTime;
			public float demolishingTime;
			public string Kanim;
			public string requiredTechID;
			public bool HasInterior;
			public SpaceStationWithStats(string _id, string _name, string _description, Vector2I _size, Dictionary<string, float> _mats, string _kanim, float _constructionTime, string _reqTech = "", bool _hasInterior = true)
			{
				ID = _id;
				Name = _name;
				Description = _description;
				InteriorSize = _size;
				materials = _mats;
				Kanim = _kanim;
				requiredTechID = _reqTech == "" ? Techs.SpaceStationTechID : _reqTech;
				constructionTime = _constructionTime;
				demolishingTime = _constructionTime / 4;
				HasInterior = _hasInterior;
			}

		}
	}
}
